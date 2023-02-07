using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerAbilityState
{
    public PlayerJumpState(PlayerStateController playerStateController, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(playerStateController, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        playerStateController.SetVelocityY(playerData.jumpSpeed);
        playerData.stamina -= 10;
        isAbilityDone = true;
    }
}
