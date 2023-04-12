using UnityEngine;

namespace Player.FiniteStateMachine.SubState
{
    public class PlayerRunState : SuperState.PlayerGroundedState
    {
        public PlayerRunState(PlayerStateController stateController, PlayerStateMachine stateMachine,
            PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic,
            animBoolName)
        {
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            if (IsExitingState)
                return;

            if (StateController.MovementInput == Vector2.zero)
            {
                StateMachine.ChangeState(StateController.IdleState);
            }
            else if (!StateController.RunInput)
            {
                StateMachine.ChangeState(StateController.MoveState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            StateController.Movement(PlayerStatistic.RunMovementSpeedMax);
        }
    }
}