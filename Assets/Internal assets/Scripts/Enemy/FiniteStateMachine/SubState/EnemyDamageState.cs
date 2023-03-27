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
            Debug.Log("Enter Damage State");
            if (EnemyStatistic.IsDead)
                StateMachine.ChangeState(StateController.DeathState);
            IsAbilityDone = true;
        }
    }
}