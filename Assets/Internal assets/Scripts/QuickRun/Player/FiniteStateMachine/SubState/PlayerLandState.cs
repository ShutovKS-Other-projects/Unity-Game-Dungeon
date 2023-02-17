using Internal_assets.Scripts.QuickRun.Player.FiniteStateMachine.SuperState;
using UnityEngine;
using PlayerGroundedState = Internal_assets.Scripts.QuickRun.Player.FiniteStateMachine.SuperState.PlayerGroundedState;
namespace Internal_assets.Scripts.QuickRun.Player.FiniteStateMachine.SubState
{
    public class PlayerLandState : PlayerGroundedState
    {
        public PlayerLandState(PlayerStateController stateController, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(stateController, stateMachine, playerData, animBoolName)
        {
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            StateController.Movement(MovementInput, playerData.movementSpeed);

            if (IsExitingState)
                return;
            
            if (MovementInput != Vector2.zero)
            {
                StateMachine.ChangeState(StateController.MoveState);
            }
            else if (IsAnimationFinished)
            {
                StateMachine.ChangeState(StateController.IdleState);
            }
        }
        
        public override void AnimationFinishTrigger()
        {
            base.AnimationFinishTrigger();

            IsAnimationFinished = true;
        }
    }
}