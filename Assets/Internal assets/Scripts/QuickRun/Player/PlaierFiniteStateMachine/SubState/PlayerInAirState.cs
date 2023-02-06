using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInAirState : PlayerState
{
    private Vector2 movementInput;
    private bool isGrounded;

    public PlayerInAirState(PlayerS player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isGrounded = player.CheckIfGrounded();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        movementInput = player.InputManager.GetPlayerMovementInput();

        if(isGrounded && player.RB.velocity.y < 0.01f)
        {
            stateMachine.ChangeState(player.LandState);
        }
    }
}
