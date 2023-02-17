using Internal_assets.Scripts.QuickRun.Player.FiniteStateMachine.SuperState;
namespace Internal_assets.Scripts.QuickRun.Player.FiniteStateMachine.SubState
{
    public class PlayerBlockState : PlayerAbilityState
    {
        public PlayerBlockState(PlayerStateController stateController, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(stateController, stateMachine, playerData, animBoolName)
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
                isAbilityDone = true;
            }
            else if (StateController.InputManager.GetPlayerAttackInput())
            {
                StateMachine.ChangeState(StateController.AttackState);
            }
        }
    }
}
