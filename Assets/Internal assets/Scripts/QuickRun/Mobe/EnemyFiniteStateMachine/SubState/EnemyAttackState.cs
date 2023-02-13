public class EnemyAttackState : EnemyAbilityState
{
    public EnemyAttackState(EnemyStateController enemyStateController, EnemyStateMachine stateMachine, EnemyData enemyData, string animBoolName) : base(enemyStateController, stateMachine, enemyData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // Eneble the attack collider
    }

    public override void Exit()
    {
        base.Exit();

        // Disable the attack collider
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();

        isAbilityDone = true;
    }
}
