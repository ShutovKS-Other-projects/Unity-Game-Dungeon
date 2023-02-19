using Player.FiniteStateMachine.SuperState;
namespace Player.FiniteStateMachine.SubState
{
    public class PlayerAttackState : PlayerAbilityState
    {
        public PlayerAttackState(PlayerStateController stateController, PlayerStateMachine stateMachine, PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic, animBoolName)
        {
        }

        Delegate.SwitchCollider SwitchCollider
        {
            get
            {
                return StateController.SwitchCollider;
            }
        }

        public override void Enter()
        {
            base.Enter();

            SwitchCollider(true);
            playerStatistic.Stamina -= 10;
        }

        public override void Exit()
        {
            base.Exit();

            SwitchCollider(false);
        }

        public override void AnimationFinishTrigger()
        {
            base.AnimationFinishTrigger();

            isAbilityDone = true;
        }
    }
}
