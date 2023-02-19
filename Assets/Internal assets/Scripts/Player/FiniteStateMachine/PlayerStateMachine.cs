namespace Player.FiniteStateMachine
{
    public class PlayerStateMachine
    {
        public PlayerState CurrentState { get; private set; }

        public void Initialize(PlayerState startingState) //инициализацияx
        {
            CurrentState = startingState; 
            CurrentState.Enter();   
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void ChangeState(PlayerState newState) //смена состояния
        {
            CurrentState.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }
    }
}