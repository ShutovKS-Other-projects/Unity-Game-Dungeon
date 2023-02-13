using UnityEngine;

public class EnemyGroundedState : EnemyState
{
    protected float playerDistance;
    protected bool isAttack;
    protected bool isVisiblePlayer;

    public EnemyGroundedState(EnemyStateController enemyStateController, EnemyStateMachine stateMachine, EnemyData enemyData, string animBoolName) : base(enemyStateController, stateMachine, enemyData, animBoolName)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        playerDistance = enemyStateController.CheckPlayerDistance();
        isVisiblePlayer = enemyStateController.CheckIfPlayer();

        if (enemyData.isDead)
        {
            stateMachine.ChangeState(enemyStateController.DeathState);
        }
        else if (!isVisiblePlayer)
        {
            stateMachine.ChangeState(enemyStateController.IdleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        enemyStateController.LookAtPlayer();
    }

}
