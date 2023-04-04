namespace Player.FiniteStateMachine.SubState
{
    public class PlayerJumpState : SuperState.PlayerAbilityState
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
            IsAbilityDone = true;
        }
    }
}