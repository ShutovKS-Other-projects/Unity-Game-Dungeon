using Internal_assets.Scripts.QuickRun.Enemy.FiniteStateMachine.SuperState;
using UnityEngine;
namespace Internal_assets.Scripts.QuickRun.Enemy.FiniteStateMachine.SubState
{
    public class EnemyIdleState : EnemyGroundedState
    {
        private float attackTimer;

        public EnemyIdleState(EnemyStateController stateController, EnemyStateMachine stateMachine, EnemyStatistic enemyStatistic, string animBoolName) : base(stateController, stateMachine, enemyStatistic, animBoolName)
        {
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (!isVisiblePlayer)
                return;
            
            if (playerDistance <= enemyStatistic.attackDistance)
            {
                isAttack = Attack();

                if (isAttack)
                {
                    StateMachine.ChangeState(StateController.AttackState);
                }
            }
            else if (playerDistance > enemyStatistic.attackDistance)
            {
                StateMachine.ChangeState(StateController.MoveState);
            }
        }

        private bool Attack()
        {
            if (isVisiblePlayer && playerDistance <= enemyStatistic.attackDistance && !isAttack)
            {
                attackTimer += Time.deltaTime;
                if (attackTimer >= enemyStatistic.attackRetryTime)
                {
                    attackTimer = 0f;
                    return true;
                }
            }
            return false;
        }
    }
}
