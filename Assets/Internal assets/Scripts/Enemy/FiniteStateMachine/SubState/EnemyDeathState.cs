using Enemy.FiniteStateMachine.SuperState;
using UnityEngine;
namespace Enemy.FiniteStateMachine.SubState
{
    public class EnemyDeathState : EnemyAbilityState
    {
        public EnemyDeathState(EnemyStateController stateController, EnemyStateMachine stateMachine, EnemyStatistic enemyStatistic, string animBoolName) : base(stateController, stateMachine, enemyStatistic, animBoolName)
        {
        }

        public override void AnimationFinishTrigger()
        {
            StateController.gameObject.layer = LayerMask.NameToLayer("Interactable");
        }
    }
}
