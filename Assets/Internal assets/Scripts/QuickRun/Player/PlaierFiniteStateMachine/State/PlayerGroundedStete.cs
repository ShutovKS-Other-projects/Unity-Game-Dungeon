using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    protected Vector2 movementInput;

    protected bool crouchInput;
    private bool attackInput;
    private bool blockInput;
    private bool jumpInput;
    
    protected bool isTouchingCelling;
    private bool isTouchingGrounded;

    public PlayerGroundedState(PlayerS player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isTouchingGrounded = player.CheckIfGrounded();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        movementInput = player.InputManager.GetPlayerMovementInput();
        crouchInput = player.InputManager.GetPlayerCrouchInput();
        jumpInput = player.InputManager.GetPlayerJumpInput();
        attackInput = player.InputManager.GetPlayerAttackInput();
        blockInput = player.InputManager.GetPlayerBlockInput();

        if (attackInput) // && !isTouchingCelling)
        {
            stateMachine.ChangeState(player.AttackState);
        }
        else if (blockInput) // && !isTouchingCelling)
        {
            stateMachine.ChangeState(player.BlockState);
        }
        else if (jumpInput && !isTouchingCelling)
        {
            stateMachine.ChangeState(player.JumpState);
        }
        else if (!isTouchingGrounded)
        {
            stateMachine.ChangeState(player.InAirState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        player.Rotation();
    }
}
