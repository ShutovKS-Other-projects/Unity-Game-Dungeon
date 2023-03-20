using Old.Enemy.FiniteStateMachine.SuperState;
using UnityEngine;

namespace Old.Enemy.FiniteStateMachine.SubState
{
    public class EnemyIdleState : EnemyGroundedState
    {
        private float _attackTimer;

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
            
            _attackTimer += Time.deltaTime;
            if (!(_attackTimer >= EnemyStatistic.AttackRetryTime))
                return false;
            
            _attackTimer = 0f;
            return true;
        }
    }
}
