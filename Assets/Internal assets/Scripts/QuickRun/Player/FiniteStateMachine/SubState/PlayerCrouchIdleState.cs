using Internal_assets.Scripts.QuickRun.Player.FiniteStateMachine.SuperState;
using UnityEngine;
using PlayerGroundedState = Internal_assets.Scripts.QuickRun.Player.FiniteStateMachine.SuperState.PlayerGroundedState;
namespace Internal_assets.Scripts.QuickRun.Player.FiniteStateMachine.SubState
{
    public class PlayerCrouchIdleState : PlayerGroundedState
    {
        public PlayerCrouchIdleState(PlayerStateController stateController, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(stateController, stateMachine, playerData, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            StateController.SetVelocityZero();
            StateController.SetColliderHeight(playerData.crouchColliderHeight, playerData.crouchColliderCenter);
        }

        public override void Exit()
        {
            base.Exit();

            StateController.SetColliderHeight(playerData.standColliderHeight, playerData.standColliderCenter);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (!IsExitingState)
            {
                if (MovementInput != Vector2.zero)
                {
                    StateMachine.ChangeState(StateController.CrouchMoveState);
                }
                else if (!CrouchInput && !IsTouchingCelling)
                {
                    StateMachine.ChangeState(StateController.IdleState);
                }
            }
        }
    }
}
