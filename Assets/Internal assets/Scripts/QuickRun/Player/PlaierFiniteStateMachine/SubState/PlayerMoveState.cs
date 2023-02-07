using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(PlayerStateController playerStateController, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(playerStateController, stateMachine, playerData, animBoolName)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState)
        {
            if (movementInput == Vector2.zero)
            {
                stateMachine.ChangeState(playerStateController.IdleState);
            }
            else if (crouchInput)
            {
                stateMachine.ChangeState(playerStateController.CrouchMoveState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        playerStateController.Movement(movementInput, playerData.movementSpeed);
    }
}
