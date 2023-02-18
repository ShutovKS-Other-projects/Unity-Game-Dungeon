using Internal_assets.Scripts.QuickRun.Player.FiniteStateMachine;
namespace Internal_assets.Scripts.QuickRun.Player.FiniteStateMachine.SuperState
{
    public class PlayerAbilityState : PlayerState
    {
        protected bool isAbilityDone;
        protected bool isGrounded;

        public PlayerAbilityState(PlayerStateController stateController, PlayerStateMachine stateMachine, PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic, animBoolName)
        {
        }

        protected override void DoChecks()
        {
            base.DoChecks();

            isGrounded = StateController.CheckIfGrounded();
        }

        public override void Enter()
        {
            base.Enter();

            isAbilityDone = false;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if(isAbilityDone)
            {
                if(isGrounded && StateController.Rb.velocity.y < 0.01f)
                {
                    StateMachine.ChangeState(StateController.IdleState);
                }
                else
                {
                    StateMachine.ChangeState(StateController.InAirState);
                }
            }
        }
    }
}
