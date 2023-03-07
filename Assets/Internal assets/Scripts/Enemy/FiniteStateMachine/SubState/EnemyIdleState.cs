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
            
            if (PlayerDistance <= EnemyStatistic.AttackDistance)
            {
                IsAttack = Attack();

                if (IsAttack)
                {
                    StateMachine.ChangeState(StateController.AttackState);
                }
            }
            else if (PlayerDistance > EnemyStatistic.AttackDistance)
            {
                StateMachine.ChangeState(StateController.MoveState);
            }
        }

        private bool Attack()
        {
            if (!IsVisiblePlayer || !(PlayerDistance <= EnemyStatistic.AttackDistance) || IsAttack)
                return false;
            
            attackTimer += Time.deltaTime;
            if (!(attackTimer >= EnemyStatistic.AttackRetryTime))
                return false;
            
            attackTimer = 0f;
            return true;
        }
    }
}
