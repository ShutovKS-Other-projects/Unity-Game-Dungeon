using UnityEngine;
namespace Player.FiniteStateMachine.SubState
{
    public class PlayerCrouchMoveState : SuperState.PlayerGroundedState
    {
        public PlayerCrouchMoveState(PlayerStateController stateController, PlayerStateMachine stateMachine, PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            StateController.SetColliderHeight(playerStatistic.CrouchColliderHeight, playerStatistic.CrouchColliderCenter);
        }

        public override void Exit()
        {
            base.Exit();

            StateController.SetColliderHeight(playerStatistic.StandColliderHeight, playerStatistic.StandColliderCenter);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (!IsExitingState)
            {
                if (MovementInput == Vector2.zero)
                {
                    StateMachine.ChangeState(StateController.CrouchIdleState);
                }
                else if (!CrouchInput && !IsTouchingCelling)
                {
                    StateMachine.ChangeState(StateController.MoveState);
                }
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            StateController.Movement(MovementInput, playerStatistic.CrouchMovementSpeedMax);

        }
    }
}
