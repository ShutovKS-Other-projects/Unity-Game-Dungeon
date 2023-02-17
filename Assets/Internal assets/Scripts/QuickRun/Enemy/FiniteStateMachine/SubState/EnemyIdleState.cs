using Internal_assets.Scripts.QuickRun.Enemy.FiniteStateMachine.SuperState;
using UnityEngine;
namespace Internal_assets.Scripts.QuickRun.Enemy.FiniteStateMachine.SubState
{
    public class EnemyIdleState : EnemyGroundedState
    {
        private float attackTimer;

        public EnemyIdleState(EnemyStateController stateController, EnemyStateMachine stateMachine, EnemyData enemyData, string animBoolName) : base(stateController, stateMachine, enemyData, animBoolName)
        {
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (!isVisiblePlayer)
                return;
            
            if (playerDistance <= enemyData.attackDistance)
            {
                isAttack = Attack();

                if (isAttack)
                {
                    StateMachine.ChangeState(StateController.AttackState);
                }
            }
            else if (playerDistance > enemyData.attackDistance)
            {
                StateMachine.ChangeState(StateController.MoveState);
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
}
