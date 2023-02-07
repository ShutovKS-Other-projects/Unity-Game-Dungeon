using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(PlayerStateController playerStateController, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(playerStateController, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        playerStateController.SetVelocityZero();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState)
        {
            if (movementInput != Vector2.zero)
            {
                stateMachine.ChangeState(playerStateController.MoveState);
            }
            else if (crouchInput)
            {
                stateMachine.ChangeState(playerStateController.CrouchIdleState);
            }
        }
    }
}
