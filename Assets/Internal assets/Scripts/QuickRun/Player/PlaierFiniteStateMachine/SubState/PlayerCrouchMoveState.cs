using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouchMoveState : PlayerGroundedState
{
    public PlayerCrouchMoveState(PlayerS player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.SetColliderHeight(playerData.crouchColliderHeight, playerData.crouchColliderCenter);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState)
        {
            if (movementInput == Vector2.zero)
            {
                stateMachine.ChangeState(player.CrouchIdleState);
            }
            else if (!crouchInput && !isTouchingCelling)
            {
                stateMachine.ChangeState(player.MoveState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        player.Movement(movementInput, playerData.crouchMovementSpeed);

    }
}
