using Enemy.FiniteStateMachine.SuperState;
namespace Enemy.FiniteStateMachine.SubState
{
    public class EnemyMoveState : EnemyGroundedState
    {
        public EnemyMoveState(EnemyStateController stateController, EnemyStateMachine stateMachine, EnemyStatistic enemyStatistic, string animBoolName) : base(stateController, stateMachine, enemyStatistic, animBoolName)
        {
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (IsVisiblePlayer && PlayerDistance <= EnemyStatistic.attackDistance)
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
