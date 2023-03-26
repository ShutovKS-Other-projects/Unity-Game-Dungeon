using Old.Enemy.FiniteStateMachine.SuperState;

namespace Old.Enemy.FiniteStateMachine.SubState
{
    public class EnemyMoveState : EnemyGroundedState
    {
        public EnemyMoveState(EnemyStateController stateController, EnemyStateMachine stateMachine,
            EnemyStatistic enemyStatistic, string animBoolName) : base(stateController, stateMachine, enemyStatistic,
            animBoolName)
        {
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (IsVisiblePlayer && PlayerDistance <= EnemyStatistic.AttackDistance)
            {
                StateMachine.ChangeState(StateController.IdleState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            StateController.Move();
        }
    }
}