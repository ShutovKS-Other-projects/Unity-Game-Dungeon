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
    public GameObject PlayerGameObject { get; private set; }
    public Rigidbody RB { get; private set; }
    public CapsuleCollider Collider { get; private set; }
    [SerializeField] public EnemyData enemyData;
    #endregion

    #region Unity Callbacks Functions
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
        Animator = GetComponent<Animator>();
        PlayerGameObject = GameObject.FindGameObjectWithTag("Player");
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
        if (Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), PlayerGameObject.transform.position - transform.position, out RaycastHit hit, enemyData.playerCheckDistance))
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
        return Vector3.Distance(transform.position, PlayerGameObject.transform.position);
    }
    #endregion

    #region Movement
    public void Move()
    {
        RB.AddRelativeForce(0f, 0f, enemyData.movementSpeed * Time.deltaTime, ForceMode.VelocityChange);
        Animator.SetFloat("zVelocity", RB.velocity.z);
    }
    #endregion

    #region Rotation
    public void LookAtPlayer()
    {
        transform.LookAt(PlayerGameObject.transform.position);
    }
    #endregion

    #region Other Functions
    private void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();
    private void AnimationFinishTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();
    #endregion
}
