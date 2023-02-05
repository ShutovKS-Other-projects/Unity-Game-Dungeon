using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    protected PlayerS player;
    protected PlayerStateMachine stateMachine;
    protected PlayerData playerData;

    protected Transform playerTransform => player.transform;
    protected float startTime; //время начала анимации
    private string animBoolName; //логическое имя анимации
    private float animFloatName; //вещественное имя анимации


    public PlayerState(PlayerS player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.playerData = playerData;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter() //вход в состояние
    {
        DoChecks();
        player.Animator.SetBool(animBoolName, true);
        startTime = Time.time;
    }

    public virtual void Exit() //выход из состояния 
    {
        player.Animator.SetBool(animBoolName, false);
    }

    public virtual void LogicUpdate() //логическое обновление (Update)
    {
        DoChecks();
    }

    public virtual void PhysicsUpdate() //физическое обновление (FixedUpdate)
    {
        DoChecks();
    }

    public virtual void DoChecks() //проверка
    {

    }
}