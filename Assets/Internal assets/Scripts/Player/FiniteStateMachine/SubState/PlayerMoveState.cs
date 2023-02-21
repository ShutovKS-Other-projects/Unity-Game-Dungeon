using UnityEngine;
namespace Player.FiniteStateMachine.SubState
{
    public class PlayerMoveState : SuperState.PlayerGroundedState
    {
        public PlayerMoveState(PlayerStateController stateController, PlayerStateMachine stateMachine, PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic, animBoolName)
        {
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (IsExitingState)
                return;
            
            if (MovementInput == Vector2.zero)
            {
                StateMachine.ChangeState(StateController.IdleState);
            }
            else if (CrouchInput)
            {
                StateMachine.ChangeState(StateController.CrouchMoveState);
            }
            else if (RunInput)
            {
                StateMachine.ChangeState(StateController.RunState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            StateController.Movement(MovementInput, playerStatistic.MovementSpeedMax);
        }
    }
}
