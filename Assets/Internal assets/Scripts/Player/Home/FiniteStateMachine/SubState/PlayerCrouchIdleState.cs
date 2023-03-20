using Player.Home.FiniteStateMachine.SuperState;
using UnityEngine;

namespace Player.Home.FiniteStateMachine.SubState
{
    public class PlayerCrouchIdleState : PlayerGroundedState
    {
        public PlayerCrouchIdleState(PlayerStateController stateController, PlayerStateMachine stateMachine,
            PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic,
            animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            StateController.SetVelocityZero();
            StateController.SetColliderHeight(PlayerStatistic.CrouchColliderHeight,
                PlayerStatistic.CrouchColliderCenter);
        }

        public override void Exit()
        {
            base.Exit();

            StateController.SetColliderHeight(PlayerStatistic.StandColliderHeight, 
                PlayerStatistic.StandColliderCenter);
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
            else if (!CrouchInput)
            {
                StateMachine.ChangeState(StateController.IdleState);
            }
        }
    }
}