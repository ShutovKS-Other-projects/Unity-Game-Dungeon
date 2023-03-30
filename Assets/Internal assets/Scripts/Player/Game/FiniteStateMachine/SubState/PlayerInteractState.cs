namespace Player.Game.FiniteStateMachine.SubState
{
    public class PlayerInteractState : SuperState.PlayerAbilityState
    {
        public PlayerInteractState(PlayerStateController stateController, PlayerStateMachine stateMachine,
            PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic,
            animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            StateController.SetVelocityZero();
            PlayerStatistic.interactionObject.Interact();
        }

        public override void Exit()
        {
            base.Exit();

            PlayerStatistic.interactionObject.ResetData();
        }

        public override void AnimationFinishTrigger()
        {
            base.AnimationFinishTrigger();

            IsAbilityDone = true;
        }
    }
}