using Internal_assets.Scripts.QuickRun.Player.FiniteStateMachine.SuperState;
namespace Internal_assets.Scripts.QuickRun.Player.FiniteStateMachine.SubState
{
    public class PlayerAttackState : PlayerAbilityState
    {
        public PlayerAttackState(PlayerStateController stateController, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(stateController, stateMachine, playerData, animBoolName)
        {
        }

        Delegate.DelegateSwitchCollider SwitchCollider
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
            playerData.stamina -= 10;
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
