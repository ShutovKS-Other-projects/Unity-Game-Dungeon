using Old.Player.FiniteStateMachine.SuperState;

namespace Old.Player.FiniteStateMachine.SubState
{
    public class PlayerInteractState : PlayerAbilityState
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
            PlayerStatistic.interactionData.Interact();
        }

        public override void Exit()
        {
            base.Exit();

            PlayerStatistic.interactionData.ResetData();
        }

        public override void AnimationFinishTrigger()
        {
            base.AnimationFinishTrigger();

            IsAbilityDone = true;
        }
    }
}