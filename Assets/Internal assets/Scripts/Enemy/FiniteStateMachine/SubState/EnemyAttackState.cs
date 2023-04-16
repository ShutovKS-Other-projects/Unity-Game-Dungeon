using Player;
using UnityEngine;

namespace Enemy.FiniteStateMachine.SubState
{
    public class EnemyAttackState : SuperState.EnemyAbilityState
    {
        public EnemyAttackState(EnemyStateController stateController, EnemyStateMachine stateMachine,
            EnemyStatistic enemyStatistic, string animBoolName) : base(stateController, stateMachine, enemyStatistic,
            animBoolName)
        {
        }
        
        public override void Enter()
        {
            base.Enter();
            PlayerController.player.GetComponent<PlayerStatistic>().CharacteristicHealth.AddValueMax(-1);
            IsAbilityDone = true;
        }
    }
}