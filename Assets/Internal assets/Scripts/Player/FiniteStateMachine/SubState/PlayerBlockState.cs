using Player.FiniteStateMachine.SuperState;
namespace Player.FiniteStateMachine.SubState
{
    public class PlayerBlockState : PlayerAbilityState
    {
        public PlayerBlockState(PlayerStateController stateController, PlayerStateMachine stateMachine, PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            //OnEnable trigger weapon collider
        }

        public override void Exit()
        {
            base.Exit();

            //OnDisable trigger weapon collider
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (!StateController.InputManager.GetPlayerBlockInput())
            {
                IsAbilityDone = true;
            }
            else if (StateController.InputManager.GetPlayerAttackInput())
            {
                StateMachine.ChangeState(StateController.AttackState);
            }
        }
    }
}
