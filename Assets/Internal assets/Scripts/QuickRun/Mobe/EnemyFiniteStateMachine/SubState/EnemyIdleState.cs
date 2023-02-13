using UnityEngine;

public class EnemyIdleState : EnemyGroundedState
{
    private float attackTimer;

    public EnemyIdleState(EnemyStateController enemyStateController, EnemyStateMachine stateMachine, EnemyData enemyData, string animBoolName) : base(enemyStateController, stateMachine, enemyData, animBoolName)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isVisiblePlayer)
        {
            if (playerDistance <= enemyData.attackDistance)
            {
                isAttack = Attack();

                if (isAttack)
                {
                    stateMachine.ChangeState(enemyStateController.AttackState);
                }
            }
            else if (playerDistance > enemyData.attackDistance)
            {
                stateMachine.ChangeState(enemyStateController.MoveState);
            }
        }
    }

    private bool Attack()
    {
        if (isVisiblePlayer && playerDistance <= enemyData.attackDistance && !isAttack)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= Random.Range(enemyData.attackRetryTime[0], enemyData.attackRetryTime[1]))
            {
                attackTimer = 0f;
                return true;
            }
        }
        return false;
    }
}
