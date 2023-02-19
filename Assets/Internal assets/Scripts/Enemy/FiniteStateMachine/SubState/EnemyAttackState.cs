using Enemy.FiniteStateMachine.SuperState;
namespace Enemy.FiniteStateMachine.SubState
{
    public class EnemyAttackState : EnemyAbilityState
    {
        Delegate.SwitchCollider SwitchCollider
        {
            get
            {
                return StateController.SwitchCollider;
            }
        }

        public EnemyAttackState(EnemyStateController stateController, EnemyStateMachine stateMachine, EnemyStatistic enemyStatistic, string animBoolName) : base(stateController, stateMachine, enemyStatistic, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            SwitchCollider(true);
        }

        public override void Exit()
        {
            base.Exit();

            SwitchCollider(false);
        }

        public override void AnimationFinishTrigger()
        {
            base.AnimationFinishTrigger();

            isAbilityDone = true;
        }
    }
}
