using UnityEngine;

public class AnimatedEnemy : Enemy
{
    private Animator _animator;

    protected override void Awake()
    {
        base.Awake();

        _animator = GetComponent<Animator>();
    }

    protected override void StopWalk()
    {
        ChangeAnimation("IsWalk", false);
    }

    protected override void StartWalk()
    {
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
        ChangeAnimation("IsRun", true);
    }

    protected override void StopRun()
    {
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
