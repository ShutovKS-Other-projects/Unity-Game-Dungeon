using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractState : PlayerAbilityState
{
    public PlayerInteractState(PlayerS player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.SetVelocityX(0f);
        player.SetVelocityZ(0f);

        playerData.interactionData.Interact();
        isAbilityDone = true;
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
