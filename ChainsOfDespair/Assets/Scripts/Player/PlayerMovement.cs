using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _speed;
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _jumpForce;

    [Header("Ground check")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _groundCheckRadius;
    [SerializeField] private LayerMask _groundCheckLayer;

    [Header("Sounds")]
    [SerializeField] private AudioSource _stepsSound;
    [SerializeField] private AudioSource _startJumpSound;
    [SerializeField] private AudioSource _endJumpSound;

    private Animator _animator;
    private Rigidbody _rb;
    private bool _isOnGround;
    private Vector3 _move;
    private Stamina _staminaManager;
    private bool _isPlayEndJumpSound;
    private PlayerInitialize _playerInitialize;

    private NetworkVariable<Vector3> _localMove = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<bool> _isRun = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _staminaManager = GetComponent<Stamina>();
        _playerInitialize = GetComponent<PlayerInitialize>();
    }

    private void Update()
    {
        if (!_playerInitialize.isMove)
            return;

        MoveAnimation();

        if (!IsOwner)
            return;

        if (!Input.GetKey(KeyCode.LeftShift) || !Input.GetKey(KeyCode.W))
        {
            _staminaManager.StopUse();
            _isRun.Value = false;
        }

        Jump();
    }

    private void FixedUpdate()
    {
        if (PlayersManager.Instance.ownerPlayer != null)
            if (!PlayersManager.Instance.ownerPlayer.isMove)
                return;

        _isOnGround = Physics.CheckSphere(_groundCheck.position, _groundCheckRadius, _groundCheckLayer);

        if (_isOnGround)
        {
            if (_animator.GetBool("IsJump") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                _animator.SetBool("IsJump", false);

            if (_isPlayEndJumpSound)
                _endJumpSound.Play();

            _isPlayEndJumpSound = false;
        }
        else
        {
            _isPlayEndJumpSound = true;
        }

        if (!IsOwner)
            return;

        Move();
    }

    private void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        _move = transform.right * horizontal + transform.forward * vertical;
        _move.Normalize();
     
        if (_move.magnitude > .1f)
        {
            float speed = _speed;
            
            if (Input.GetKey(KeyCode.LeftShift) && vertical > 0)
            {
                if (_staminaManager.isCanRun)
                {
                    speed = _runSpeed;
                    _isRun.Value = true;
                    _staminaManager.StartUse();
                }
            }

            _rb.linearVelocity = new Vector3(_move.x * speed, _rb.linearVelocity.y, _move.z * speed);
        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_isOnGround)
            {
                if (!_animator.GetBool("IsJump"))
                {
                    StartJumpAnimationServerRpc();
                    _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
                }
            }
        }
    }

    private void MoveAnimation()
    {
        if (IsOwner)
            _localMove.Value = transform.InverseTransformDirection(_move);

        if (_localMove.Value.magnitude > 0)
        {
            if (!_stepsSound.isPlaying)
                _stepsSound.Play();
        }
        else
        {
            if (_stepsSound.isPlaying)
                _stepsSound.Stop();
        }

        if (_localMove.Value.x > .5f && !_animator.GetBool("IsWalkRight"))
            _animator.SetBool("IsWalkRight", true);

        else if (_localMove.Value.x <= 0 && _animator.GetBool("IsWalkRight"))
            _animator.SetBool("IsWalkRight", false);

        if (_localMove.Value.x < -.5f && !_animator.GetBool("IsWalkLeft"))
            _animator.SetBool("IsWalkLeft", true);
        
        else if (_localMove.Value.x >= 0 && _animator.GetBool("IsWalkLeft"))
            _animator.SetBool("IsWalkLeft", false);

        if (_localMove.Value.z < -.5f && !_animator.GetBool("IsWalkBack"))
            _animator.SetBool("IsWalkBack", true);
        
        else if (_localMove.Value.z >= 0 && _animator.GetBool("IsWalkBack"))
            _animator.SetBool("IsWalkBack", false);

        if (_localMove.Value.z > .5f && !_animator.GetBool("IsWalk"))
            _animator.SetBool("IsWalk", true);
        
        else if (_localMove.Value.z <= 0 && _animator.GetBool("IsWalk"))
            _animator.SetBool("IsWalk", false);

        if (!_animator.GetBool("IsRun") && _isRun.Value)
        {
            _stepsSound.pitch = 1.4f;
            _animator.SetBool("IsRun", true);
        }

        if (_animator.GetBool("IsRun") && !_isRun.Value || !_staminaManager.isCanRun)
        {
            _stepsSound.pitch = 1f;
            _animator.SetBool("IsRun", false);
        }
    }

    public void CheckIfFall()
    {
        if (!_isOnGround)
        {
            _animator.SetBool("IsJump", false);
        }
    }

    [ServerRpc]
    private void StartJumpAnimationServerRpc()
    {
        StartJumpAnimationClientRpc();
    }

    [ClientRpc]
    private void StartJumpAnimationClientRpc() 
    {
        _startJumpSound.Play();
        _animator.SetBool("IsJump", true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void StopAllAnimationsServerRpc()
    {
        StopAllAnimationsClientRpc();
    }

    [ClientRpc]
    private void StopAllAnimationsClientRpc()
    {
        foreach (AnimatorControllerParameter param in _animator.parameters)
        {
            _animator.SetBool(param.name, false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _isOnGround ? Color.green : Color.red;

        Gizmos.DrawWireSphere(_groundCheck.position, _groundCheckRadius);
    }
}
