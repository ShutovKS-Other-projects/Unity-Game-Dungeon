using UnityEngine;

namespace Enemy.FiniteStateMachine.SubState
{
    public class EnemyDamageState : SuperState.EnemyAbilityState
    {
        public EnemyDamageState(EnemyStateController stateController, EnemyStateMachine stateMachine,
            EnemyStatistic enemyStatistic, string animBoolName) : base(stateController, stateMachine, enemyStatistic,
            animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            Debug.Log($"Damage State {EnemyStatistic.Health}");
            IsAbilityDone = true;
        }
    }
}