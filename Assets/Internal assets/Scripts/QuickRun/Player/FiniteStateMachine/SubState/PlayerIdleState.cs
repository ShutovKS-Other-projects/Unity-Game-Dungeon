using Internal_assets.Scripts.QuickRun.Player.FiniteStateMachine.SuperState;
using UnityEngine;
using PlayerGroundedState = Internal_assets.Scripts.QuickRun.Player.FiniteStateMachine.SuperState.PlayerGroundedState;
namespace Internal_assets.Scripts.QuickRun.Player.FiniteStateMachine.SubState
{
    public class PlayerIdleState : PlayerGroundedState
    {
        public PlayerIdleState(PlayerStateController stateController, PlayerStateMachine stateMachine, PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            StateController.SetVelocityZero();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (!IsExitingState)
            {
                if (MovementInput != Vector2.zero)
                {
                    StateMachine.ChangeState(StateController.MoveState);
                }
                else if (CrouchInput)
                {
                    StateMachine.ChangeState(StateController.CrouchIdleState);
                }
            }
        }
    }
}
