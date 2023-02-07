using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLandState : PlayerGroundedState
{
    public PlayerLandState(PlayerStateController playerStateController, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(playerStateController, stateMachine, playerData, animBoolName)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        playerStateController.Movement(movementInput, playerData.movementSpeed);

        if (!isExitingState)
        {
            if (movementInput != Vector2.zero)
            {
                stateMachine.ChangeState(playerStateController.MoveState);
            }
            else if (isAnimationFinished)
            {
                stateMachine.ChangeState(playerStateController.IdleState);
            }
        }
    }
}