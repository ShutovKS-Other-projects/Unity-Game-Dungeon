using JetBrains.Annotations;
using Player.Game.FiniteStateMachine.SubState;
using UnityEngine;
using Weapon;

namespace Player.Game.FiniteStateMachine
{
    public sealed class PlayerStateController : MonoBehaviour
    {
        private PlayerStatistic _playerStatistic;

        #region State Machine

        private PlayerStateMachine _stateMachine;

        public PlayerIdleState IdleState { get; private set; }
        public PlayerMoveState MoveState { get; private set; }
        public PlayerJumpState JumpState { get; private set; }
        public PlayerInAirState InAirState { get; private set; }
        public PlayerLandState LandState { get; private set; }
        public PlayerCrouchIdleState CrouchIdleState { get; private set; }
        public PlayerCrouchMoveState CrouchMoveState { get; private set; }
        public PlayerAttackState AttackState { get; private set; }
        public PlayerBlockState BlockState { get; private set; }
        public PlayerInteractState InteractState { get; private set; }
        public PlayerDamageState DamageState { get; private set; }
        public PlayerDeathState DeathState { get; private set; }
        public PlayerRunState RunState { get; private set; }
        public PlayerMagicAttackState MagicAttackState { get; private set; }

        #endregion

        #region Transforms

        private Transform _groundCheckTransform;
        [CanBeNull] public Transform damageObjectWeaponTransform;

        #endregion

        #region Components

        public Animator Animator { get; private set; }
        public Rigidbody Rb { get; private set; }

        private CapsuleCollider _collider;

        private static readonly int XSpeed = Animator.StringToHash("xSpeed");
        private static readonly int YSpeed = Animator.StringToHash("ySpeed");
        private static readonly int ZSpeed = Animator.StringToHash("zSpeed");

        #endregion

        #region Delegate Functions

        [CanBeNull] public Delegate.SwitchCollider SwitchCollider;
        [CanBeNull] public Delegate.OnMagicAttackDelegate MagicAttackDelegate;
        [CanBeNull] public Delegate.StrengthAttackFloat StrengthAttackFloat;

        public void RegisterDelegateSwitchCollider(Delegate.SwitchCollider del) => SwitchCollider = del;

        public void RegisterDelegateMagicAttackDelegate(Delegate.OnMagicAttackDelegate del) =>
            MagicAttackDelegate = del;

        public void RegisterDelegateStrengthAttackFloat(Delegate.StrengthAttackFloat del) => StrengthAttackFloat = del;

        #endregion

        #region Unity Functions

        private void Awake()
        {
            _playerStatistic = GetComponent<PlayerStatistic>();
            _stateMachine = new PlayerStateMachine();

            IdleState = new PlayerIdleState(this, _stateMachine, _playerStatistic, "Idle");
            MoveState = new PlayerMoveState(this, _stateMachine, _playerStatistic, "Move");
            JumpState = new PlayerJumpState(this, _stateMachine, _playerStatistic, "InAir");
            InAirState = new PlayerInAirState(this, _stateMachine, _playerStatistic, "InAir");
            LandState = new PlayerLandState(this, _stateMachine, _playerStatistic, "Land");
            CrouchIdleState = new PlayerCrouchIdleState(this, _stateMachine, _playerStatistic, "CrouchIdle");
            CrouchMoveState = new PlayerCrouchMoveState(this, _stateMachine, _playerStatistic, "CrouchMove");
            AttackState = new PlayerAttackState(this, _stateMachine, _playerStatistic, "Attack");
            BlockState = new PlayerBlockState(this, _stateMachine, _playerStatistic, "Block");
            InteractState = new PlayerInteractState(this, _stateMachine, _playerStatistic, "Interact");
            DamageState = new PlayerDamageState(this, _stateMachine, _playerStatistic, "Damage");
            DeathState = new PlayerDeathState(this, _stateMachine, _playerStatistic, "Death");
            RunState = new PlayerRunState(this, _stateMachine, _playerStatistic, "Run");
            MagicAttackState = new PlayerMagicAttackState(this, _stateMachine, _playerStatistic, "MagicAttack");
        }

        private void Start()
        {
            Animator = GetComponentInChildren<Animator>();
            Rb = GetComponent<Rigidbody>();
            _collider = GetComponent<CapsuleCollider>();

            _groundCheckTransform = transform.Find("GroundCheck").transform;
            if (FindObjectOfType<WeaponColliderEnable>())
                SwitchCollider += FindObjectOfType<WeaponColliderEnable>().EnableCollider;


            _stateMachine.Initialize(IdleState);
        }

        private void Update() => _stateMachine.CurrentState.LogicUpdate();
        private void FixedUpdate() => _stateMachine.CurrentState.PhysicsUpdate();
        private void OnTriggerEnter(Collider other) => _stateMachine.CurrentState.TriggerEnter(other);
        private void OnTriggerExit(Collider other) => _stateMachine.CurrentState.TriggerExit(other);

        #endregion

        #region Velocity

        public void SetVelocityZero()
        {
            Rb.velocity = Vector3.zero;
            Animator.SetFloat(YSpeed, 0);
            Animator.SetFloat(XSpeed, 0);
            Animator.SetFloat(ZSpeed, 0);
        }

        public void SetVelocityY(float velocityY)
        {
            var velocity = Rb.velocity;
            velocity = new Vector3(velocity.x, velocityY, velocity.z);
            Rb.velocity = velocity;
            Animator.SetFloat(YSpeed, velocityY);
        }

        public void SetVelocityX(float velocityX)
        {
            var velocity = Rb.velocity;
            velocity = new Vector3(velocityX, velocity.y, velocity.z);
            Rb.velocity = velocity;
            Animator.SetFloat(XSpeed, velocityX);
        }

        public void SetVelocityZ(float velocityZ)
        {
            var velocity = Rb.velocity;
            velocity = new Vector3(velocity.x, velocity.y, velocityZ);
            Rb.velocity = velocity;
            Animator.SetFloat(ZSpeed, velocityZ);
        }

        #endregion

        #region Movement

        public void Movement(Vector2 movementInput, float speedMax)
        {
            var move = new Vector3(movementInput.x, 0, movementInput.y);

            Rb.AddRelativeForce(move * _playerStatistic.MovementForce * Time.deltaTime, ForceMode.VelocityChange);

            if (Rb.velocity.magnitude > speedMax)
                Rb.velocity = Rb.velocity.normalized * speedMax;

            Animator.SetFloat(ZSpeed, move.z);
            Animator.SetFloat(XSpeed, move.x);
        }

        #endregion

        #region Rotation

        public void Rotation()
        {
            var cameraRotation = UnityEngine.Camera.main!.transform.rotation;
            transform.rotation = new Quaternion(0f, cameraRotation.y, 0f, cameraRotation.w);
        }

        #endregion

        #region Check Functions

        public bool CheckIfGrounded()
        {
            return Physics.CheckSphere(_groundCheckTransform.position, _playerStatistic.GroundCheckRadius,
                LayerMask.GetMask("Ground"));
        }

        #endregion

        #region Other Functions

        public void SetColliderHeight(float height, float center)
        {
            _collider.height = height;
            var colliderCenter = _collider.center;
            colliderCenter = new Vector3(colliderCenter.x, center, colliderCenter.z);
            _collider.center = colliderCenter;
        }

        private void AnimationTrigger() => _stateMachine.CurrentState.AnimationTrigger();
        private void AnimationFinishTrigger() => _stateMachine.CurrentState.AnimationFinishTrigger();

        #endregion
    }
}