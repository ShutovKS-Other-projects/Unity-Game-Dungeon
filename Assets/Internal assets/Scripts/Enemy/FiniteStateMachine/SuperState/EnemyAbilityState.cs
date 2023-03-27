namespace Enemy.FiniteStateMachine.SuperState
{
    public class EnemyAbilityState : EnemyState
    {
        public EnemyAbilityState(EnemyStateController stateController, EnemyStateMachine stateMachine,
            EnemyStatistic enemyStatistic, string animBoolName) : base(stateController, stateMachine, enemyStatistic,
            animBoolName)
        {
        }

        protected bool IsAbilityDone;

        public override void Enter()
        {
            base.Enter();
            IsAbilityDone = false;
            StateController.NavMeshAgent.isStopped = true;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (!IsAbilityDone)
                return;

            if (StateController.Rb.velocity.y < 0.01f)
            {
                StateMachine.ChangeState(StateController.IdleState);
            }
        }

        public override void Exit()
        {
            base.Exit();
            StateController.NavMeshAgent.isStopped = false;
        }
    }
}