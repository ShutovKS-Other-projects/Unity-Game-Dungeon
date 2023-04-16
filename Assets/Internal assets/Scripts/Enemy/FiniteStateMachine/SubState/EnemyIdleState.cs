using UnityEngine;

namespace Enemy.FiniteStateMachine.SubState
{
    public class EnemyIdleState : SuperState.EnemyGroundedState
    {
        public EnemyIdleState(EnemyStateController stateController, EnemyStateMachine stateMachine,
            EnemyStatistic enemyStatistic, string animBoolName) : base(stateController, stateMachine, enemyStatistic,
            animBoolName)
        {
        }

        private float _attackTimer;

        public override void Enter()
        {
            base.Enter();
            Debug.Log($"Idle");
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (!EnemyStatistic.isVisiblePlayer) return;
            if (PlayerDistance > EnemyStatistic.AttackDistance)
                StateMachine.ChangeState(StateController.MoveState);
            else if (IsAttack())
                StateMachine.ChangeState(StateController.AttackState);
        }

        private bool IsAttack()
        {
            _attackTimer += Time.deltaTime;
            if (!(_attackTimer >= EnemyStatistic.AttackRetryTime)) return false;
            _attackTimer = 0;
            return true;
        }
    }
}