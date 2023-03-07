namespace Enemy.FiniteStateMachine.SuperState
{
    public class EnemyAbilityState : EnemyState
    {
        protected bool isAbilityDone;

        public EnemyAbilityState(EnemyStateController stateController, EnemyStateMachine stateMachine, EnemyStatistic enemyStatistic, string animBoolName) : base(stateController, stateMachine, enemyStatistic, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            isAbilityDone = false;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (!isAbilityDone)
                return;
            
            if (EnemyStatistic.IsDead)
            {
                StateMachine.ChangeState(StateController.DeathState);
            }
            else
            {
                StateMachine.ChangeState(StateController.IdleState);
            }
        }
    }
}
