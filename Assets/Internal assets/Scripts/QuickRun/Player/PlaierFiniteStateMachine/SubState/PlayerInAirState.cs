using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInAirState : PlayerState
{
    private Vector2 movementInput;
    private bool isGrounded;

    public PlayerInAirState(PlayerStateController playerStateController, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(playerStateController, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isGrounded = playerStateController.CheckIfGrounded();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        movementInput = playerStateController.InputManager.GetPlayerMovementInput();

        if(isGrounded && playerStateController.RB.velocity.y < 0.01f)
        {
            stateMachine.ChangeState(playerStateController.LandState);
        }
    }
}
