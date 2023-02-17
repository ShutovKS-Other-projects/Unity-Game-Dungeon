namespace Internal_assets.Scripts.QuickRun.Enemy.FiniteStateMachine
{
    public class EnemyStateMachine
    {
        public EnemyState CurrentState { get; private set; }

        public void Initialize(EnemyState startingState) //инициализация
        {
            CurrentState = startingState; 
            CurrentState.Enter();   
        }

        public void ChangeState(EnemyState newState) //смена состояния
        {
            CurrentState.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }
    }
}
