using UnityEngine;
using PlayerGroundedState = Internal_assets.Scripts.QuickRun.Player.FiniteStateMachine.SuperState.PlayerGroundedState;
namespace Internal_assets.Scripts.QuickRun.Player.FiniteStateMachine.SubState
{
    public class PlayerMoveState : PlayerGroundedState
    {
        public PlayerMoveState(PlayerStateController stateController, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(stateController, stateMachine, playerData, animBoolName)
        {
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (!IsExitingState)
            {
                if (MovementInput == Vector2.zero)
                {
                    StateMachine.ChangeState(StateController.IdleState);
                }
                else if (CrouchInput)
                {
                    StateMachine.ChangeState(StateController.CrouchMoveState);
                }
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            StateController.Movement(MovementInput, playerData.movementSpeed);
        }
    }
}
