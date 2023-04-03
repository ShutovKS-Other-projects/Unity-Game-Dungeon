using System;
using Enemy.FiniteStateMachine.SubState;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.FiniteStateMachine
{
    public class EnemyStateController : MonoBehaviour
    {
        [SerializeField] private EnemyData enemyData;
        private EnemyStatistic _enemyStatistic;

        #region State Machine

        public EnemyStateMachine StateMachine { get; private set; }

        public EnemyAttackState AttackState { get; private set; }
        public EnemyDamageState DamageState { get; private set; }
        public EnemyDeathState DeathState { get; private set; }
        public EnemyIdleState IdleState { get; private set; }
        public EnemyMoveState MoveState { get; private set; }

        #endregion

        #region Components

        public Animator Animator { get; private set; }
        public NavMeshAgent NavMeshAgent { get; private set; }
        public Rigidbody Rb { get; set; }

        #endregion

        #region Unity Callbacks Functions

        private void Awake()
        {
            _enemyStatistic = new EnemyStatistic(enemyData, this);
            StateMachine = new EnemyStateMachine();

            AttackState = new EnemyAttackState(this, StateMachine, _enemyStatistic, "Attack");
            DamageState = new EnemyDamageState(this, StateMachine, _enemyStatistic, "Damage");
            DeathState = new EnemyDeathState(this, StateMachine, _enemyStatistic, "Death");
            IdleState = new EnemyIdleState(this, StateMachine, _enemyStatistic, "Idle");
            MoveState = new EnemyMoveState(this, StateMachine, _enemyStatistic, "Move");
            
            ManagerEnemies.ManagerEnemies.Instance.AddEnemy(gameObject);
        }

        private void Start()
        {
            Animator = TryGetComponent<Animator>(out var animator) ? animator : transform.AddComponent<Animator>();
            Animator.runtimeAnimatorController = _enemyStatistic.AnimatorController;

            NavMeshAgent = TryGetComponent<NavMeshAgent>(out var navMeshAgent)
                ? navMeshAgent
                : transform.AddComponent<NavMeshAgent>();

            Rb = TryGetComponent<Rigidbody>(out var rb) ? rb : transform.AddComponent<Rigidbody>();
            Rb.constraints = RigidbodyConstraints.FreezeRotation;

            StateMachine.Initialize(IdleState);
        }

        private void Update() => StateMachine.CurrentState.LogicUpdate();
        private void FixedUpdate() => StateMachine.CurrentState.PhysicsUpdate();

        private void OnTriggerEnter(Collider other) => StateMachine.CurrentState.TriggerEnter(other);
        private void OnTriggerExit(Collider other) => StateMachine.CurrentState.TriggerExit(other);

        #endregion

        #region Other Function

        private void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();
        private void AnimationFinishTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();

        #endregion
    }
}