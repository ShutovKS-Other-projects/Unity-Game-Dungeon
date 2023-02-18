using Internal_assets.Scripts.QuickRun.Player.FiniteStateMachine.SuperState;
namespace Internal_assets.Scripts.QuickRun.Player.FiniteStateMachine.SubState
{
    public class PlayerJumpState : PlayerAbilityState
    {
        public PlayerJumpState(PlayerStateController stateController, PlayerStateMachine stateMachine, PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            StateController.SetVelocityY(playerStatistic.JumpSpeed);
            playerStatistic.Stamina -= 10;
            isAbilityDone = true;
        }
    }
}
