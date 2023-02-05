using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    protected Vector2 movementInput;

    public PlayerGroundedState(PlayerS player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
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
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        Rotation();
    }

    public virtual void Rotation()
    {
        playerTransform.rotation = new Quaternion(0f, Camera.main.transform.rotation.y, 0f, Camera.main.transform.rotation.w);
    }
}
