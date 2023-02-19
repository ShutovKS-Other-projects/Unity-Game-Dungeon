using UnityEngine;
namespace Player.FiniteStateMachine.SubState
{
    public class PlayerInAirState : PlayerState
    {
        private Vector2 movementInput;
        private bool isGrounded;

        public PlayerInAirState(PlayerStateController stateController, PlayerStateMachine stateMachine, PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic, animBoolName)
        {
        }

        protected override void DoChecks()
        {
            base.DoChecks();

            isGrounded = StateController.CheckIfGrounded();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            movementInput = StateController.InputManager.GetPlayerMovementInput();

            if(isGrounded && StateController.Rb.velocity.y < 0.01f)
            {
                StateMachine.ChangeState(StateController.LandState);
            }
        }
    }
}
