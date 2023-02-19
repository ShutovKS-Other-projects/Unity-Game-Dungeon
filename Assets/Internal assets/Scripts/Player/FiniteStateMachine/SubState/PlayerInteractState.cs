using Player.FiniteStateMachine.SuperState;
namespace Player.FiniteStateMachine.SubState
{
    public class PlayerInteractState : PlayerAbilityState
    {
        public PlayerInteractState(PlayerStateController stateController, PlayerStateMachine stateMachine, PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            StateController.SetVelocityX(0f);
            StateController.SetVelocityZ(0f);

            playerStatistic.InteractionData.Interact();
        }

        public override void Exit()
        {
            base.Exit();

            playerStatistic.InteractionData.ResetData();
        }

        public override void AnimationFinishTrigger()
        {
            base.AnimationFinishTrigger();

            isAbilityDone = true;
        }
    }
}
