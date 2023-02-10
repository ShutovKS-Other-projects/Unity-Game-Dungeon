using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
