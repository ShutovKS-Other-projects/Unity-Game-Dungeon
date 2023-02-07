using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractState : PlayerAbilityState
{
    public PlayerInteractState(PlayerStateController playerStateController, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(playerStateController, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        playerStateController.SetVelocityX(0f);
        playerStateController.SetVelocityZ(0f);

        playerData.interactionData.Interact();
    }

    public override void Exit()
    {
        base.Exit();

        playerData.interactionData.ResetData();
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();

        isAbilityDone = true;
    }
}
