using Old.Enemy.FiniteStateMachine.SuperState;

namespace Old.Enemy.FiniteStateMachine.SubState
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

            IsAbilityDone = true;
        }

        private Delegate.SwitchCollider SwitchCollider => StateController.SwitchCollider;

        private float AttackStart() => EnemyStatistic.AttackDamage;
        private static float AttackEnd() => 0;
    }
}
