using UnityEngine;
namespace Player.FiniteStateMachine.SubState
{
    public class PlayerInAirState : PlayerState
    {
        private bool _isGrounded;

        public PlayerInAirState(PlayerStateController stateController, PlayerStateMachine stateMachine, PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic, animBoolName)
        {
        }

        protected override void DoChecks()
        {
            base.DoChecks();

            _isGrounded = StateController.CheckIfGrounded();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            if(_isGrounded && StateController.Rb.velocity.y < 0.01f)
            {
                StateMachine.ChangeState(StateController.LandState);
            }
        }
    }
}
