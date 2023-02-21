using UnityEngine;
namespace Player.FiniteStateMachine.SubState
{
    public class PlayerCrouchIdleState : SuperState.PlayerGroundedState
    {
        public PlayerCrouchIdleState(PlayerStateController stateController, PlayerStateMachine stateMachine, PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            StateController.SetVelocityZero();
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

            if (IsExitingState)
                return;
            
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
