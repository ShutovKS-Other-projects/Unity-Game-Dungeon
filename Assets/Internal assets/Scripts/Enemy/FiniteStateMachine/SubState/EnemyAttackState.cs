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
            Debug.Log("Enter Attack State");
            IsAbilityDone = true;
        }
    }
}