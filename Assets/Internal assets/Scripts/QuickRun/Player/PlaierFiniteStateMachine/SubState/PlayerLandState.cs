using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLandState : PlayerGroundedState
{
    public PlayerLandState(PlayerS player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (movementInput != Vector2.zero)
        {
            stateMachine.ChangeState(player.MoveState);
        }
        else if(isAnimationFinished)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }
}