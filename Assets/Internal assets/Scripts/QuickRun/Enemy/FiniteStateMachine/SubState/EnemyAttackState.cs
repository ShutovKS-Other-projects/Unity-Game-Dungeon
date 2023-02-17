using Internal_assets.Scripts.QuickRun.Enemy.FiniteStateMachine.SuperState;
namespace Internal_assets.Scripts.QuickRun.Enemy.FiniteStateMachine.SubState
{
    public class EnemyAttackState : EnemyAbilityState
    {
        Delegate.DelegateSwitchCollider SwitchCollider
        {
            get
            {
                return StateController.SwitchCollider;
            }
        }

        public EnemyAttackState(EnemyStateController stateController, EnemyStateMachine stateMachine, EnemyData enemyData, string animBoolName) : base(stateController, stateMachine, enemyData, animBoolName)
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
