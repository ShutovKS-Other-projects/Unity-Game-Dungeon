using UnityEngine;

namespace Player.Game.FiniteStateMachine
{
    public class PlayerStateMachine : MonoBehaviour
    {
        public PlayerState CurrentState { get; private set; }

        public void Initialize(PlayerState startingState)
        {
            CurrentState = startingState;
            CurrentState.Enter();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void ChangeState(PlayerState newState)
        {
            CurrentState.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }
    }
}