using System.Collections;
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
    [SerializeField] private Transform _minMapPos;
    [SerializeField] private Transform _maxMapPos;
    [Header("Sounds")]
    [SerializeField] private AudioSource _stepsSound;
    [SerializeField] private AudioSource _warningSound;
    [SerializeField] private AudioSource[] _growlSounds;

    private NavMeshAgent _agent;
    private Animator _animator;
    private Transform _attackedPlayer;
    private bool _isWarning;
    private bool _isAttack;
    private bool _isSeePlayer;
    private Coroutine _currentCoroutine;

    protected virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {   
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
                    _isAttack = false;
                    _isSeePlayer = false;
                    _agent.speed = 0;
                    ChangeAnimation("IsAttack", true);

                    PlayerInitialize player = _attackedPlayer.GetComponent<PlayerInitialize>();
                    player.isMove = false;
                    player.isAlive = false;
                    
                    _attackedPlayer.LookAt(transform.position);
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
        if (!IsServer)
            StopCoroutine(Walk());

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
                ChangeAnimation("IsWalk", false);
                _agent.speed = 0;

                yield return new WaitForSeconds(3);
            }

            else if (randomAction == 1)
            {
                ChangeAnimation("IsWalk", true);

                Vector3 randomPos = new Vector3(Random.Range(_minMapPos.position.x, _maxMapPos.position.x),
                    _minMapPos.position.y, Random.Range(_minMapPos.position.z, _maxMapPos.position.z));

                _agent.speed = _speed;
                _agent.destination = randomPos;

                yield return new WaitForSeconds(10);
            }
        }
    }

    private IEnumerator Warning()
    {
        if (!_isAttack)
            _warningSound.Play();

        _agent.speed = 0;
        transform.LookAt(_attackedPlayer);
        ChangeAnimation("IsWarning", true);

        yield return new WaitForSeconds(_warningTime);

        _isWarning = false;
        ChangeAnimation("IsWarning", false);

        if (_isSeePlayer)
        {
            _isAttack = true;
            ChangeAnimation("IsRun", true);
        }
        else
        {
            _isAttack = false;
            ChangeAnimation("IsRun", false);
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
                            _attackedPlayer = player.transform;
                            _isSeePlayer = true;
                            break;
                        }
                        else
                        {
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

    private void ChangeAnimation(string name, bool value)
    {
        if (!IsServer)
            return;

        _animator.SetBool(name, value);
    }

    public void StopAttack()
    {
        ChangeAnimation("IsRun", false);
        ChangeAnimation("IsAttack", false);

        AttackPlayerClientRpc();

        ChangeState(Walk());
    }

    [ClientRpc]
    private void AttackPlayerClientRpc()
    {
        if (_attackedPlayer != null)
        {
            _attackedPlayer.GetComponent<PlayerDied>().Died();
            _attackedPlayer = null;
        }
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
            _growlSounds[randomGrowlIndex].Play();
        }
    }
}
