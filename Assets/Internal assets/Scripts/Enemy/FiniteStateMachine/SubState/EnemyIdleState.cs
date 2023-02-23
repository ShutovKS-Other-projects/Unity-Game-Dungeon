using Enemy.FiniteStateMachine.SuperState;
using UnityEngine;
namespace Enemy.FiniteStateMachine.SubState
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

            if (!IsVisiblePlayer)
                return;
            
            if (PlayerDistance <= EnemyStatistic.attackDistance)
            {
                IsAttack = Attack();

                if (IsAttack)
                {
                    StateMachine.ChangeState(StateController.AttackState);
                }
            }
            else if (PlayerDistance > EnemyStatistic.attackDistance)
            {
                StateMachine.ChangeState(StateController.MoveState);
            }
        }

        private bool Attack()
        {
            if (!IsVisiblePlayer || !(PlayerDistance <= EnemyStatistic.attackDistance) || IsAttack)
                return false;
            
            attackTimer += Time.deltaTime;
            if (!(attackTimer >= EnemyStatistic.attackRetryTime))
                return false;
            
            attackTimer = 0f;
            return true;
        }
    }
}
