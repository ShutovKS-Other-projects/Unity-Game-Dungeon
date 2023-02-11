using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateController : MonoBehaviour
{
    #region State Machine
    public EnemyStateMachine StateMachine { get; private set; }

    public EnemyAttackState AttackState { get; private set; }
    public EnemyDeathState DeathState { get; private set; }
    public EnemyIdleState IdleState { get; private set; }
    public EnemyMoveState MoveState { get; private set; }
    #endregion

    #region Components
    public Animator Animator { get; private set; }
    public Rigidbody RB { get; private set; }
    public CapsuleCollider Collider { get; private set; }
    [SerializeField] private EnemyData enemyData;
    #endregion

    #region Unity Callbacks Functions
    private void Awake()
    {
        StateMachine = new EnemyStateMachine();

        AttackState = new EnemyAttackState(this, StateMachine, enemyData, "Attack");
        DeathState = new EnemyDeathState(this, StateMachine, enemyData, "Dead");
        IdleState = new EnemyIdleState(this, StateMachine, enemyData, "Idle");
        MoveState = new EnemyMoveState(this, StateMachine, enemyData, "Move");
    }

    private void Start()
    {
        Animator = GetComponentInChildren<Animator>();
        RB = GetComponent<Rigidbody>();
        Collider = GetComponent<CapsuleCollider>();

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
    #endregion

    #region Check Functions
    public bool CheckIfPlayer()
    {
        Ray ray = new Ray(transform.position, enemyData.playerGameObject.transform.position - transform.position);
        if (Physics.Raycast(ray, out RaycastHit hit, enemyData.playerCheckDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    public float CheckPlayerDistance()
    {
        return Vector3.Distance(transform.position, enemyData.playerGameObject.transform.position);
    }
    #endregion

    #region Movement
    public void Move()
    {

    }
    #endregion

    #region Rotation
    public void LookAtPlayer()
    {
        transform.LookAt(enemyData.playerGameObject.transform.position);
    }
    #endregion

    #region Other Functions
    private void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();
    private void AnimationFinishTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();
    #endregion
}
