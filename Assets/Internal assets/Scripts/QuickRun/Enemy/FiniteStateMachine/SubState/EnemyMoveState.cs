using Internal_assets.Scripts.QuickRun.Enemy.FiniteStateMachine.SuperState;
namespace Internal_assets.Scripts.QuickRun.Enemy.FiniteStateMachine.SubState
{
    public class EnemyMoveState : EnemyGroundedState
    {
        public EnemyMoveState(EnemyStateController stateController, EnemyStateMachine stateMachine, EnemyData enemyData, string animBoolName) : base(stateController, stateMachine, enemyData, animBoolName)
        {
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (isVisiblePlayer && playerDistance <= enemyData.attackDistance)
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
