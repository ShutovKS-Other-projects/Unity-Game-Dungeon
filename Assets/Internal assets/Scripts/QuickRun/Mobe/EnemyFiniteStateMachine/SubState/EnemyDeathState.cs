using UnityEngine;

public class EnemyDeathState : EnemyAbilityState
{
    public EnemyDeathState(EnemyStateController enemyStateController, EnemyStateMachine stateMachine, EnemyData enemyData, string animBoolName) : base(enemyStateController, stateMachine, enemyData, animBoolName)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();

        enemyStateController.gameObject.layer = LayerMask.NameToLayer("Interactable");
    }
}
