using UnityEngine;
namespace Player.FiniteStateMachine.SubState
{
    public class PlayerLandState : SuperState.PlayerGroundedState
    {
        public PlayerLandState(PlayerStateController stateController, PlayerStateMachine stateMachine, PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic, animBoolName)
        {
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
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
        
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            
            StateController.Movement(MovementInput, playerStatistic.MovementSpeedMax);
        }
        
        public override void AnimationFinishTrigger()
        {
            base.AnimationFinishTrigger();

            IsAnimationFinished = true;
        }
    }
}