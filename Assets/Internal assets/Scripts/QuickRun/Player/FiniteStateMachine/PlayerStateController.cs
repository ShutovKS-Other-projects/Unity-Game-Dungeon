using Internal_assets.Scripts.QuickRun.Input;
using Internal_assets.Scripts.QuickRun.Interactable;
using Internal_assets.Scripts.QuickRun.Other;
using Internal_assets.Scripts.QuickRun.Player.Delegate;
using Internal_assets.Scripts.QuickRun.Player.FiniteStateMachine.SubState;
using UnityEngine;

namespace Internal_assets.Scripts.QuickRun.Player.FiniteStateMachine
{
    public class PlayerStateController : MonoBehaviour
    {
        [SerializeField] private PlayerData playerData;

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

        static readonly int xSpeed = Animator.StringToHash("xSpeed");
        static readonly int ySpeed = Animator.StringToHash("ySpeed");
        static readonly int zSpeed = Animator.StringToHash("zSpeed");

        #endregion

        #region Unity Callbacks Functions

        private void Awake()
        {
            _stateMachine = new PlayerStateMachine();

            IdleState = new PlayerIdleState(this, _stateMachine, playerData, "Idle");
            MoveState = new PlayerMoveState(this, _stateMachine, playerData, "Move");
            JumpState = new PlayerJumpState(this, _stateMachine, playerData, "InAir");
            InAirState = new PlayerInAirState(this, _stateMachine, playerData, "InAir");
            LandState = new PlayerLandState(this, _stateMachine, playerData, "Land");
            CrouchIdleState = new PlayerCrouchIdleState(this, _stateMachine, playerData, "CrouchIdle");
            CrouchMoveState = new PlayerCrouchMoveState(this, _stateMachine, playerData, "CrouchMove");
            AttackState = new PlayerAttackState(this, _stateMachine, playerData, "Attack");
            BlockState = new PlayerBlockState(this, _stateMachine, playerData, "Block");
            InteractState = new PlayerInteractState(this, _stateMachine, playerData, "Interact");
            DamageState = new PlayerDamageState(this, _stateMachine, playerData, "Damage");
            DeathState = new PlayerDeathState(this, _stateMachine, playerData, "Death");
            RunState = new PlayerRunState(this, _stateMachine, playerData, "Run");
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
            //uiInteractionBare = GameObject.Find("UIInteractionBare").GetComponent<UIInteractionBare>();
            
            //Scene QuickRun
            uiInteractionBare = GameObject.Find("ManagerScene").transform.Find("Canvas").transform.Find("UIPanelGame").transform.Find("UIInteractionBare").GetComponent<UIInteractionBare>();
        
            SwitchCollider += gameObject.transform.Find("DamageNoWeapon").GetComponent<GameObjectTriggerEnable>().EnableCollider;
            SwitchCollider += gameObject.transform.Find("DamageWeapon").GetComponent<GameObjectTriggerEnable>().EnableCollider;
        
            _stateMachine.Initialize(IdleState);
        }

        private void Update()
        {
            _stateMachine.CurrentState.LogicUpdate();
        }

        private void FixedUpdate()
        {
            _stateMachine.CurrentState.PhysicsUpdate();
        }

        void OnTriggerEnter(Collider other)
        {
            _stateMachine.CurrentState.TriggerEnter(other);
        }

        void OnTriggerExit(Collider other)
        {
            _stateMachine.CurrentState.TriggerExit(other);
        }

        #endregion

        #region Velocity

        public void SetVelocityZero()
        {
            Rb.velocity = Vector3.zero;
            Animator.SetFloat(ySpeed, 0);
            Animator.SetFloat(xSpeed, 0);
            Animator.SetFloat(zSpeed, 0);
        }

        public void SetVelocityY(float velocityY)
        {
            var velocity = Rb.velocity;
            velocity = new Vector3(velocity.x, velocityY, velocity.z);
            Rb.velocity = velocity;
            Animator.SetFloat(ySpeed, velocityY);
        }

        public void SetVelocityX(float velocityX)
        {
            var velocity = Rb.velocity;
            velocity = new Vector3(velocityX, velocity.y, velocity.z);
            Rb.velocity = velocity;
            Animator.SetFloat(xSpeed, velocityX);
        }

        public void SetVelocityZ(float velocityZ)
        {
            var velocity = Rb.velocity;
            velocity = new Vector3(velocity.x, velocity.y, velocityZ);
            Rb.velocity = velocity;
            Animator.SetFloat(zSpeed, velocityZ);
        }

        #endregion

        #region Movement

        public void Movement(Vector2 movementInput, float speed)
        {
            Acceleration(out float acceleration);
        
            var move = new Vector3(movementInput.x, 0, movementInput.y);
            Rb.AddRelativeForce(move * speed * acceleration * Time.deltaTime, ForceMode.VelocityChange);

            Animator.SetFloat(zSpeed, move.z * acceleration);
            Animator.SetFloat(xSpeed, move.x * acceleration);
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
            return Physics.CheckSphere(groundCheckTransform.position, playerData.groundCheckRadius, LayerMask.GetMask("Ground"));
        }

        public bool CheckIfTouchingCelling()
        {
            return Physics.CheckSphere(cellingCheckTransform.position, playerData.groundCheckRadius, LayerMask.GetMask("Ground"));
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
            if (!playerData.isFatigue)
            {
                if (InputManager.GetPlayerSprintInput() && InputManager.GetPlayerMovementInput().y > 0)
                {
                    playerData.stamina -= 3f * Time.deltaTime;
                    acceleration = 1.25f;
                    if (playerData.stamina <= 0)
                    {
                        playerData.isFatigue = true;
                    }
                }
                else
                {
                    playerData.stamina += 5f * Time.deltaTime;
                    acceleration = 1f;
                }
            }
            else
            {
                if (InputManager.GetPlayerSprintInput() && InputManager.GetPlayerMovementInput().y > 0)
                {
                    playerData.stamina += 5f * Time.deltaTime;
                    acceleration = 1f;
                    if (playerData.stamina >= 100)
                    {
                        playerData.isFatigue = false;
                    }
                }
                else
                {
                    playerData.stamina += 5f * Time.deltaTime;
                    acceleration = 1f;
                }
            }
        }

        private void AnimationTrigger() => _stateMachine.CurrentState.AnimationTrigger();
        private void AnimationFinishTrigger() => _stateMachine.CurrentState.AnimationFinishTrigger();

        #endregion
    
        #region Delegate Functions
        public DelegateSwitchCollider SwitchCollider;
        #endregion
    }
}
