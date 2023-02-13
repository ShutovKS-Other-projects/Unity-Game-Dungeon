public class EnemyMoveState : EnemyGroundedState
{
    public EnemyMoveState(EnemyStateController enemyStateController, EnemyStateMachine stateMachine, EnemyData enemyData, string animBoolName) : base(enemyStateController, stateMachine, enemyData, animBoolName)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isVisiblePlayer && playerDistance <= enemyData.attackDistance)
        {
            stateMachine.ChangeState(enemyStateController.IdleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        enemyStateController.Move();
    }
}
