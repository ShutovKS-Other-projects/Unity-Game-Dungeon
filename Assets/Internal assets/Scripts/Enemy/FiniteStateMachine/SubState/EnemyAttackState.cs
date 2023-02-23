using System.Collections;
using Enemy.FiniteStateMachine.SuperState;
using UnityEngine;
namespace Enemy.FiniteStateMachine.SubState
{
    public class EnemyAttackState : EnemyAbilityState
    {
        public EnemyAttackState(EnemyStateController stateController, EnemyStateMachine stateMachine, EnemyStatistic enemyStatistic, string animBoolName) : base(stateController, stateMachine, enemyStatistic, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            StateController.RegisterDelegateStrengthAttackFloat(AttackStart);
            SwitchCollider(true);
        }

        public override void Exit()
        {
            base.Exit();

            SwitchCollider(false);
            StateController.RegisterDelegateStrengthAttackFloat(AttackEnd);
        }

        public override void AnimationFinishTrigger()
        {
            base.AnimationFinishTrigger();

            isAbilityDone = true;
        }

        Delegate.SwitchCollider SwitchCollider { get { return StateController.SwitchCollider; } }
        
        private float AttackStart() => EnemyStatistic.attackDamage;
        private static float AttackEnd() => 0;
    }
}
