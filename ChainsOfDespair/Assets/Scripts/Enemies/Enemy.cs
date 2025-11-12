using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : NetworkBehaviour
{
    [SerializeField] private float _attackDistance;
    [SerializeField] private float _speed;
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _viewAngle;
    [SerializeField] private float _viewDistance;
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private float _warningTime;
    [Header("Sounds")]
    [SerializeField] private AudioSource _stepsSound;
    [SerializeField] private AudioSource _warningSound;
    [SerializeField] private AudioSource _attackSound;
    [SerializeField] private AudioSource[] _growlSounds;

    private Transform _minMapPos;
    private Transform _maxMapPos;
    protected Transform _attackedPlayer;
    private NavMeshAgent _agent;
    private bool _isWarning;
    private bool _isAttack;
    private bool _isSeePlayer;
    private bool _isCallAttack;
    private Coroutine _currentCoroutine;
    private List<AudioSource> _sounds = new List<AudioSource>();

    protected virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        
        _sounds.Add(_stepsSound);
        _sounds.Add(_warningSound);
        _sounds.Add(_attackSound);

        foreach (var sound in _growlSounds)
            _sounds.Add(sound);
    }

    private void Start()
    {
        if (!IsServer)
            return;

        _minMapPos = GameObject.FindWithTag("MinMapPos").transform;
        _maxMapPos = GameObject.FindWithTag("MaxMapPos").transform;

        ChangeState(Walk());
        StartCoroutine(IsSeePlayer());
        StartCoroutine(PlayGrowlSound());
    }

    private void Update()
    {
        if (_agent.speed > 0 && !_stepsSound.isPlaying)
        {
            _stepsSound.pitch = .7f;
            _stepsSound.Play();
        }

        if (_agent.speed > _speed)
            _stepsSound.pitch = .9f;

        if (_agent.speed == 0)
            _stepsSound.Stop();

        if (!IsServer)
            return;

        if (!_isWarning && !_isAttack)
        {
            if (_isSeePlayer)
            {
                ChangeState(Warning());
                _isWarning = true;
            }
        }

        if (_isAttack)
        {
            if (_isSeePlayer)
            {
                if (Vector3.Distance(transform.position, _attackedPlayer.position) < _attackDistance)
                {
                    if (!_isCallAttack)
                    {
                        Attack();
                        _isCallAttack = true;
                    }
                }
                else
                {
                    _agent.speed = _runSpeed;
                    _agent.destination = _attackedPlayer.position;
                }
            }
            else if (!_isWarning)
            {
                ChangeState(Warning());
                _isWarning = true;
            }
        }
    }

    private IEnumerator Walk()
    {
        while (true)
        {
            if (!_agent.isActiveAndEnabled)
            {
                yield return new WaitForSeconds(1);
                continue;
            }

            int randomAction = Random.Range(0, 2);
            
            if (randomAction == 0)
            {
                StopWalk();
                
                _agent.speed = 0;

                yield return new WaitForSeconds(3);
            }

            else if (randomAction == 1)
            {
                StartWalk();

                if (_minMapPos == null || _maxMapPos == null)
                {
                    yield return new WaitForSeconds(1);
                    continue;
                }

                Vector3 randomPos = new Vector3(Random.Range(_minMapPos.position.x, _maxMapPos.position.x),
                    _minMapPos.position.y, Random.Range(_minMapPos.position.z, _maxMapPos.position.z));

                _agent.speed = _speed;
                _agent.destination = randomPos;

                yield return new WaitForSeconds(15);
            }
        }
    }

    private IEnumerator Warning()
    {
        if (!_isAttack)
            PlaySoundClientRpc(_warningSound.name);

        _agent.speed = 0;
        transform.LookAt(_attackedPlayer);

        StartWarning();

        yield return new WaitForSeconds(_warningTime);

        _isWarning = false;
        
        StopWarning();

        if (_isSeePlayer)
        {
            _isAttack = true;
            
            StartRun();
        }
        else
        {
            _isAttack = false;

            StopRun();
            ChangeState(Walk());
        }
    }

    private IEnumerator IsSeePlayer()
    {
        while (true)
        {
            foreach (var player in PlayersManager.Instance.players)
            {
                if (!player.isAlive)
                    continue;

                Vector3 dirToPlayer = (player.transform.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, dirToPlayer) < _viewAngle / 2f)
                {
                    float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

                    if (distanceToPlayer <= _viewDistance)
                    {
                        if (!Physics.Raycast(transform.position + Vector3.up * 1.5f, dirToPlayer, distanceToPlayer, _obstacleMask))
                        {
                            if (_attackedPlayer == null)
                                _attackedPlayer = player.transform;
                            
                            _isSeePlayer = true;
                            break;
                        }
                        else
                        {
                            if (_attackedPlayer != null)
                                _attackedPlayer = null;

                            _isSeePlayer = false;
                        }
                    }
                }
            }

            yield return new WaitForSeconds(1);
        }
    }

    private void ChangeState(IEnumerator newState)
    {
        if (_currentCoroutine != null)
            StopCoroutine(_currentCoroutine);

        _currentCoroutine = StartCoroutine(newState);
    }

    public virtual void StopAttack()
    {
        if (_attackedPlayer == null)
            return;

        _attackedPlayer.GetComponent<PlayerDied>().Died();
        _attackedPlayer = null;

        _isAttack = false;
        _isSeePlayer = false;
        _isWarning = false;
        _isCallAttack = false;
        
        ChangeState(Walk());
    }

    private IEnumerator PlayGrowlSound()
    {
        while (true)
        {
            float growlVolume;

            if (_isAttack && _isSeePlayer)
            {
                growlVolume = 1.2f;
                yield return new WaitForSeconds(Random.Range(2f, 3f));
            }
            else
            {
                growlVolume = 1f;
                yield return new WaitForSeconds(Random.Range(3f, 15f));
            }

            int randomGrowlIndex = Random.Range(0, _growlSounds.Length);

            _growlSounds[randomGrowlIndex].volume *= growlVolume;
            PlaySoundClientRpc(_growlSounds[randomGrowlIndex].name);
        }
    }

    [ClientRpc]
    private void PlaySoundClientRpc(string soundName)
    {
        foreach (AudioSource sound in _sounds)
        {
            if (sound.name == soundName)
            {
                sound.Play();
            }
        }
    }

    protected virtual void Attack()
    {
        _agent.speed = 0;
        _attackSound.Play();
        
        PlayerInitialize attackedPlayer = _attackedPlayer.GetComponent<PlayerInitialize>();
        attackedPlayer.transform.LookAt(transform.position);
        
        AttackClientRpc(attackedPlayer.NetworkObject);
    }

    [ClientRpc]
    private void AttackClientRpc(NetworkObjectReference attackedPlayerRef)
    {
        if (!attackedPlayerRef.TryGet(out NetworkObject attackedPlayerObj))
            return;

        PlayerInitialize attackedPlayer = attackedPlayerObj.GetComponent<PlayerInitialize>();
        attackedPlayer.isMove = false;
        attackedPlayer.isAlive = false;
    }

    protected virtual void StartWalk() { }
    protected virtual void StopWalk() { }
    protected virtual void StartWarning() { }
    protected virtual void StopWarning() { }
    protected virtual void StartRun() { }
    protected virtual void StopRun() { }
}
