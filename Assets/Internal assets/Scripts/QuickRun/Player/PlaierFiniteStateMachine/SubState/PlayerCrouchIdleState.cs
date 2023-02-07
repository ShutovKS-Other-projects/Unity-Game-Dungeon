using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouchIdleState : PlayerGroundedState
{
    public PlayerCrouchIdleState(PlayerStateController playerStateController, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(playerStateController, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        playerStateController.SetVelocityZero();
        playerStateController.SetColliderHeight(playerData.crouchColliderHeight, playerData.crouchColliderCenter);
    }

    public override void Exit()
    {
        base.Exit();

        playerStateController.SetColliderHeight(playerData.standColliderHeight, playerData.standColliderCenter);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState)
        {
            if (movementInput != Vector2.zero)
            {
                stateMachine.ChangeState(playerStateController.CrouchMoveState);
            }
            else if (!crouchInput && !isTouchingCelling)
            {
                stateMachine.ChangeState(playerStateController.IdleState);
            }
        }
    }
}
