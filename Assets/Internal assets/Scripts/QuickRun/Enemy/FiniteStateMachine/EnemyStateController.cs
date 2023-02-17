using Internal_assets.Scripts.QuickRun.Enemy.Delegate;
using Internal_assets.Scripts.QuickRun.Enemy.FiniteStateMachine.SubState;
using Internal_assets.Scripts.QuickRun.Other;
using UnityEngine;
namespace Internal_assets.Scripts.QuickRun.Enemy.FiniteStateMachine
{
    public class EnemyStateController : MonoBehaviour
    {
        [SerializeField] private EnemyData enemyDataStatic;
        [System.NonSerialized] public EnemyData enemyData;

        #region State Machine

        public EnemyStateMachine StateMachine { get; private set; }

        public EnemyAttackState AttackState { get; private set; }
        public EnemyDeathState DeathState { get; private set; }
        public EnemyIdleState IdleState { get; private set; }
        public EnemyMoveState MoveState { get; private set; }
        public EnemyDamageState DamageState { get; private set; }

        #endregion

        #region Components

        public Animator Animator { get; private set; }
        GameObject PlayerGameObject { get; set; }
        Rigidbody Rb { get; set; }
        static readonly int zVelocity = Animator.StringToHash("zVelocity");

        #endregion

        #region Delegate

        public DelegateSwitchCollider SwitchCollider;

        #endregion

        #region Unity Callbacks Functions

        private void Awake()
        {
            enemyData = enemyDataStatic;
            StateMachine = new EnemyStateMachine();

            AttackState = new EnemyAttackState(this, StateMachine, enemyData, "Attack");
            DeathState = new EnemyDeathState(this, StateMachine, enemyData, "Death");
            IdleState = new EnemyIdleState(this, StateMachine, enemyData, "Idle");
            MoveState = new EnemyMoveState(this, StateMachine, enemyData, "Move");
            DamageState = new EnemyDamageState(this, StateMachine, enemyData, "Damage");
        }

        private void Start()
        {
            Animator = GetComponent<Animator>();
            Animator.avatar = transform.GetChild(1).GetComponent<Animator>().avatar;
            //Animator.runtimeAnimatorController = enemyData.AnimatorController;
            PlayerGameObject = GameObject.FindGameObjectWithTag("Player");
            Rb = GetComponent<Rigidbody>();
            GetComponent<CapsuleCollider>();

            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent<GameObjectTriggerEnable>(out var component))
                {
                    SwitchCollider += component.EnableCollider;
                }
            }

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

        void OnTriggerEnter(Collider other)
        {
            StateMachine.CurrentState.TriggerEnter(other);
        }

        void OnTriggerExit(Collider other)
        {
            StateMachine.CurrentState.TriggerExit(other);
            Debug.Log($"Trigger {other.GetType().Name}");
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
            Rb.AddRelativeForce(0f, 0f, enemyData.movementSpeed * Time.deltaTime, ForceMode.VelocityChange);
            Animator.SetFloat(zVelocity, Rb.velocity.z);
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
}
