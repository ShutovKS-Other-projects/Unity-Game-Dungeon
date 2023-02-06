using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(PlayerS player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.SetVelocityZero();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState)
        {
            if (movementInput != Vector2.zero)
            {
                stateMachine.ChangeState(player.MoveState);
            }
            else if (crouchInput)
            {
                stateMachine.ChangeState(player.CrouchIdleState);
            }
        }
    }
}
