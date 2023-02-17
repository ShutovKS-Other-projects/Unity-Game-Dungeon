using Internal_assets.Scripts.QuickRun.Enemy.FiniteStateMachine.SuperState;
namespace Internal_assets.Scripts.QuickRun.Enemy.FiniteStateMachine.SubState
{
    public class EnemyDamageState : EnemyAbilityState
    {
        public EnemyDamageState(EnemyStateController stateController, EnemyStateMachine stateMachine, EnemyData enemyData, string animBoolName) : base(stateController, stateMachine, enemyData, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            enemyData.health = 0;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (!IsAnimationFinished)
                return;

            if (enemyData.health > 0)
            {
                StateMachine.ChangeState(StateController.IdleState);
            }
            else
            {
                StateMachine.ChangeState(StateController.DeathState);
            }
        }

        public override void AnimationFinishTrigger()
        {
            base.AnimationFinishTrigger();

            IsAnimationFinished = true;
        }
    }
}
