using System.Collections;
using Input;
using Interactable;
using Other;
using Player.FiniteStateMachine.SubState;
using UnityEngine;
namespace Player.FiniteStateMachine
{
    public class PlayerStateController : MonoBehaviour
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

        #endregion

        #region Transforms

        [SerializeField] private Transform groundCheckTransform;
        [SerializeField] private Transform cellingCheckTransform;
        [SerializeField] public Transform damageObjectWeaponTransform;
        [SerializeField] public Transform damageObjectNoWeaponTransform;

        #endregion

        #region Components

        public Animator Animator { get; private set; }
        public InputManager InputManager { get; private set; }
        public Rigidbody Rb { get; private set; }
        public UIInteractionBare uiInteractionBare;

        private CapsuleCollider _collider;

        static readonly int XSpeed = Animator.StringToHash("xSpeed");
        static readonly int YSpeed = Animator.StringToHash("ySpeed");
        static readonly int ZSpeed = Animator.StringToHash("zSpeed");

        #endregion

        #region Delegate Functions

        public Delegate.SwitchCollider? SwitchCollider;

        public Delegate.StrengthAttackFloat? StrengthAttackFloat;
        public void RegisterDelegateStrengthAttackFloat(Delegate.StrengthAttackFloat del) => StrengthAttackFloat = del;

        #endregion

        #region Unity Callbacks Functions

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
        }

        private void Start()
        {
            Animator = GetComponentInChildren<Animator>();
            InputManager = InputManager.Instance;
            Rb = GetComponent<Rigidbody>();
            _collider = GetComponent<CapsuleCollider>();

            groundCheckTransform = transform.Find("GroundCheck");
            cellingCheckTransform = transform.Find("CellingCheck");

            //Scene Test
            // uiInteractionBare = GameObject.Find("UIInteractionBare").GetComponent<UIInteractionBare>();

            //Scene QuickRun
            uiInteractionBare = GameObject.Find("ManagerScene").transform.Find("Canvas").transform.Find("UIPanelGame").transform.Find("UIInteractionBare").GetComponent<UIInteractionBare>();

            SwitchCollider += gameObject.transform.Find("DamageNoWeapon").GetComponent<GameObjectTriggerEnable>().EnableCollider;
            SwitchCollider += gameObject.transform.Find("DamageWeapon").GetComponent<GameObjectTriggerEnable>().EnableCollider;

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

            //move = transform.TransformDirection(move);
            //Rb.AddForce(move * _playerStatistic.MovementForce * Time.deltaTime, ForceMode.VelocityChange);
            Rb.AddRelativeForce(move * _playerStatistic.MovementForce * Time.deltaTime, ForceMode.VelocityChange);

            if (Rb.velocity.magnitude > speedMax)
                Rb.velocity = Rb.velocity.normalized * speedMax;

            Animator.SetFloat(ZSpeed, move.z);
            Animator.SetFloat(XSpeed, move.x);
        }

        #endregion

        #region Rotation

        public virtual void Rotation()
        {
            var cameraRotation = UnityEngine.Camera.main!.transform.rotation;
            transform.rotation = new Quaternion(0f, cameraRotation.y, 0f, cameraRotation.w);
        }

        #endregion

        #region Check Functions

        public bool CheckIfGrounded()
        {
            return Physics.CheckSphere(groundCheckTransform.position, _playerStatistic.GroundCheckRadius, LayerMask.GetMask("Ground"));
        }

        public bool CheckIfTouchingCelling()
        {
            return Physics.CheckSphere(cellingCheckTransform.position, _playerStatistic.GroundCheckRadius, LayerMask.GetMask("Ground"));
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

        private void Acceleration(out float acceleration)
        {
            if (!_playerStatistic.IsFatigue)
            {
                if (InputManager.GetPlayerSprintInput() && InputManager.GetPlayerMovementInput().y > 0)
                {
                    _playerStatistic.Stamina -= 3f * Time.deltaTime;
                    acceleration = 1.25f;
                    if (_playerStatistic.Stamina <= 0)
                    {
                        _playerStatistic.IsFatigue = true;
                    }
                }
                else
                {
                    _playerStatistic.Stamina += 5f * Time.deltaTime;
                    acceleration = 1f;
                }
            }
            else
            {
                if (InputManager.GetPlayerSprintInput() && InputManager.GetPlayerMovementInput().y > 0)
                {
                    _playerStatistic.Stamina += 5f * Time.deltaTime;
                    acceleration = 1f;
                    if (_playerStatistic.Stamina >= 100)
                    {
                        _playerStatistic.IsFatigue = false;
                    }
                }
                else
                {
                    _playerStatistic.Stamina += 5f * Time.deltaTime;
                    acceleration = 1f;
                }
            }
        }

        private void AnimationTrigger() => _stateMachine.CurrentState.AnimationTrigger();
        private void AnimationFinishTrigger() => _stateMachine.CurrentState.AnimationFinishTrigger();

        #endregion
    }
}
