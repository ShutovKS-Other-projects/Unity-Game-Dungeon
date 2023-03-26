using Old.Player.FiniteStateMachine.SuperState;

namespace Old.Player.FiniteStateMachine.SubState
{
    public class PlayerJumpState : PlayerAbilityState
    {
        public PlayerJumpState(PlayerStateController stateController, PlayerStateMachine stateMachine,
            PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic,
            animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            StateController.SetVelocityY(PlayerStatistic.JumpSpeed);
            PlayerStatistic.Stamina -= 10;
            IsAbilityDone = true;
        }
    }
}