using UnityEngine;

public class AnimatedEnemy : Enemy
{
    private Animator _animator;
    private bool _isWalk;
    private bool _isRun;

    protected override void Awake()
    {
        base.Awake();

        _animator = GetComponent<Animator>();
    }

    protected override void Update()
    {
        if (!IsServer)
            return;

        if (_agent.velocity.sqrMagnitude > 0.1f)
        {
            if (_isWalk && !_animator.GetBool("IsWalk"))
                _animator.SetBool("IsWalk", true);

            if (_isRun && !_animator.GetBool("IsRun"))
                _animator.SetBool("IsRun", true);
        }
        else
        {
            if (_isWalk && _animator.GetBool("IsWalk"))
                _animator.SetBool("IsWalk", false);

            if (_isRun && _animator.GetBool("IsRun"))
                _animator.SetBool("IsRun", false);
        }

        base.Update();
    }

    protected override void StopWalk()
    {
        _isWalk = false;

        ChangeAnimation("IsWalk", false);
    }

    protected override void StartWalk()
    {
        _isRun = false;
        _isWalk = true;

        ChangeAnimation("IsWalk", true);
    }

    protected override void StartWarning()
    {
        ChangeAnimation("IsWarning", true);
    }

    protected override void StopWarning()
    {
        ChangeAnimation("IsWarning", false);
    }

    protected override void StartRun()
    {
        _isWalk = false;
        _isRun = true;

        ChangeAnimation("IsRun", true);
    }

    protected override void StopRun()
    {
        _isRun = false;

        ChangeAnimation("IsRun", false);
    }

    protected override void Attack()
    {
        ChangeAnimation("IsAttack", true);

        base.Attack();
    }

    public override void StopAttack()
    {
        ChangeAnimation("IsRun", false);
        ChangeAnimation("IsAttack", false);

        base.StopAttack();
    }

    private void ChangeAnimation(string name, bool value)
    {
        if (!IsServer)
            return;

        _animator.SetBool(name, value);
    }
}
