using Internal_assets.Scripts.QuickRun.Player.FiniteStateMachine.SuperState;
namespace Internal_assets.Scripts.QuickRun.Player.FiniteStateMachine.SubState
{
    public class PlayerJumpState : PlayerAbilityState
    {
        public PlayerJumpState(PlayerStateController stateController, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(stateController, stateMachine, playerData, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            StateController.SetVelocityY(playerData.jumpSpeed);
            playerData.stamina -= 10;
            isAbilityDone = true;
        }
    }
}
