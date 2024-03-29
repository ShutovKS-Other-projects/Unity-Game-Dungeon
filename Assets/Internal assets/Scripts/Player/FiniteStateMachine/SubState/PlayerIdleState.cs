using Manager;
using UnityEngine;

namespace Player.FiniteStateMachine.SubState
{
    public class PlayerIdleState : SuperState.PlayerGroundedState
    {
        public PlayerIdleState(PlayerStateController stateController, PlayerStateMachine stateMachine,
            PlayerStatistic playerStatistic, string animBoolName) : base(stateController, stateMachine, playerStatistic,
            animBoolName)
        {
        }
        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (IsExitingState)
                return;

            if (StateController.MovementInput != Vector2.zero)
            {
                StateMachine.ChangeState(StateController.MoveState);
            }
        }
    }
}