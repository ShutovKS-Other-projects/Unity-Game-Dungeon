using Internal_assets.Scripts.QuickRun.Player.FiniteStateMachine;
using UnityEngine;
namespace Internal_assets.Scripts.QuickRun.Player.FiniteStateMachine.SubState
{
    public class PlayerInAirState : PlayerState
    {
        private Vector2 movementInput;
        private bool isGrounded;

        public PlayerInAirState(PlayerStateController stateController, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(stateController, stateMachine, playerData, animBoolName)
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
