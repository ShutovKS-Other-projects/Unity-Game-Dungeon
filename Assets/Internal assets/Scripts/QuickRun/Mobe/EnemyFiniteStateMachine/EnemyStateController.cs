using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateController : MonoBehaviour
{
    public EnemyStateMachine StateMachine { get; private set; }

    public EnemyAttackState AttackState { get; private set; }
    public EnemyDeathState DeathState { get; private set; }
    public EnemyIdleState IdleState { get; private set; }
    public EnemyMoveState MoveState { get; private set; }

    [SerializeField] private EnemyData enemyData;

    [SerializeField] private Transform groundCheck;

    public Animator Animator { get; private set; }
    public Rigidbody RB { get; private set; }
    public CapsuleCollider Collider { get; private set; }

    private void Awake()
    {
        StateMachine = new EnemyStateMachine();

        AttackState = new EnemyAttackState(this, StateMachine, enemyData, "Attack");
        DeathState = new EnemyDeathState(this, StateMachine, enemyData, "Death");
        IdleState = new EnemyIdleState(this, StateMachine, enemyData, "Idle");
        MoveState = new EnemyMoveState(this, StateMachine, enemyData, "Move");
    }

    private void Start()
    {
        Animator = GetComponentInChildren<Animator>();
        RB = GetComponent<Rigidbody>();
        Collider = GetComponent<CapsuleCollider>();
        groundCheck = transform.Find("GroundCheck");
        StateMachine.Initialize(IdleState);
    }

    private void Update()
    {
        StateMachine.CurrentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }

    public bool CheckIfGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, playerData.groundCheckRadius, playerData.groundLayer);
    }

    public bool ChecIfPlayer()
    {
        Ray ray = new Ray(transform.position, enemyData.playerGameObject.transform.position - transform.position);
        return true;
    }

    private void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();

    private void AnimationFinishTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();
}
