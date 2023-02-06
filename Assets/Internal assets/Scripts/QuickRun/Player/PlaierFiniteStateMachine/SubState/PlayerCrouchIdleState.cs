using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouchIdleState : PlayerGroundedState
{
    public PlayerCrouchIdleState(PlayerS player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.SetVelocityZero();
        player.SetColliderHeight(playerData.crouchColliderHeight, playerData.crouchColliderCenter);
    }

    public override void Exit()
    {
        base.Exit();

        player.SetColliderHeight(playerData.standColliderHeight, playerData.standColliderCenter);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState)
        {
            if (movementInput != Vector2.zero)
            {
                stateMachine.ChangeState(player.CrouchMoveState);
            }
            else if (!crouchInput && !isTouchingCelling)
            {
                stateMachine.ChangeState(player.IdleState);
            }
        }
    }
}
