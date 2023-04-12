using System;
using Input;
using Interactable;
using Manager;
using Player.FiniteStateMachine.SubState;
using Scene;
using UnityEngine;

namespace Player.FiniteStateMachine
{
    public sealed class PlayerStateController : MonoBehaviour
    {
        #region State Machine

        private PlayerStateMachine _stateMachine;

        public PlayerIdleState IdleState { get; private set; }
        public PlayerMoveState MoveState { get; private set; }
        public PlayerAttackState AttackState { get; private set; }
        public PlayerAttackSuperState AttackSuperState { get; private set; }
        public PlayerInteractState InteractState { get; private set; }
        public PlayerDamageState DamageState { get; private set; }
        public PlayerDeathState DeathState { get; private set; }
        public PlayerRunState RunState { get; private set; }
        public PlayerAttackMagicState AttackMagicState { get; private set; }

        #endregion

        #region Components

        private InputReader _inputReader;
        private PlayerStatistic _playerStatistic;

        public Animator Animator { get; private set; }
        public Rigidbody Rb { get; private set; }

        private CapsuleCollider _collider;

        private static readonly int XSpeed = Animator.StringToHash("xSpeed");
        private static readonly int YSpeed = Animator.StringToHash("ySpeed");
        private static readonly int ZSpeed = Animator.StringToHash("zSpeed");

        #endregion

        #region Unity Functions

        private void Awake()
        {
            if (TryGetComponent<PlayerStatistic>(out var playerStatistic))
            {
                _playerStatistic = playerStatistic;
            }
            else
            {
                _playerStatistic = gameObject.AddComponent<PlayerStatistic>();
                _playerStatistic.playerData = Resources.Load<PlayerData>($"ScriptableObject/Player/PlayerData");
                _playerStatistic.interactionObject = ScriptableObject.CreateInstance<InteractionObject>();
            }

            _stateMachine = new PlayerStateMachine();

            IdleState = new PlayerIdleState(this, _stateMachine, _playerStatistic, "Idle");
            MoveState = new PlayerMoveState(this, _stateMachine, _playerStatistic, "Move");
            AttackState = new PlayerAttackState(this, _stateMachine, _playerStatistic, "Attack");
            AttackMagicState = new PlayerAttackMagicState(this, _stateMachine, _playerStatistic, "AttackMagic");
            AttackSuperState = new PlayerAttackSuperState(this, _stateMachine, _playerStatistic, "AttackSuper");
            InteractState = new PlayerInteractState(this, _stateMachine, _playerStatistic, "Interact");
            DamageState = new PlayerDamageState(this, _stateMachine, _playerStatistic, "Damage");
            DeathState = new PlayerDeathState(this, _stateMachine, _playerStatistic, "Death");
            RunState = new PlayerRunState(this, _stateMachine, _playerStatistic, "Run");

            _inputReader = Resources.Load<InputReader>($"ScriptableObject/Input/InputReader");
            _inputReader.MoveEvent += HandlerMovement;
            _inputReader.AttackEvent += HandlerAttack;
            _inputReader.AttackCancelledEvent += HandlerAttackCancelled;
            _inputReader.AttackSuperEvent += HandlerAttackSuper;
            _inputReader.AttackSuperCancelledEvent += HandlerAttackSuperCancelled;
            _inputReader.AttackMagicEvent += HandlerAttackMagic;
            _inputReader.AttackMagicCancelledEvent += HandlerAttackMagicCancelled;
            _inputReader.SprintEvent += HandlerRun;
            _inputReader.SprintCancelledEvent += HandlerRunCancelled;
            _inputReader.InteractEvent += HandlerInteract;
            _inputReader.InteractCancelledEvent += HandlerInteractCancelled;
        }

        private void Start()
        {
            Animator = TryGetComponent<Animator>(out var animator) ? animator : gameObject.AddComponent<Animator>();

            if (TryGetComponent<CapsuleCollider>(out var capsuleCollider))
            {
                _collider = capsuleCollider;
            }
            else
            {
                _collider = gameObject.AddComponent<CapsuleCollider>();
                _collider.radius = 0.2f;
                _collider.height = 1.8f;
                _collider.center = new Vector3(0, 0.9f, 0f);
            }

            if (TryGetComponent<Rigidbody>(out var rb))
            {
                Rb = rb;
            }
            else
            {
                Rb = gameObject.AddComponent<Rigidbody>();
                Rb.freezeRotation = true;
            }

            _stateMachine.Initialize(IdleState);
        }

        private void Update() => _stateMachine.CurrentState.LogicUpdate();
        private void FixedUpdate() => _stateMachine.CurrentState.PhysicsUpdate();
        private void OnTriggerEnter(Collider other) => _stateMachine.CurrentState.TriggerEnter(other);
        private void OnTriggerExit(Collider other) => _stateMachine.CurrentState.TriggerExit(other);

        #endregion
        
        #region Movement

        public void Movement(float speedMax)
        {
            var move = new Vector3(MovementInput.x, 0, MovementInput.y);

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
            return Physics.CheckSphere(new Vector3(0, 0.12f, 0), _playerStatistic.GroundCheckRadius,
                LayerMask.GetMask("Ground"));
        }

        #endregion

        #region Other Functions

        private void AnimationTrigger() => _stateMachine.CurrentState.AnimationTrigger();
        private void AnimationFinishTrigger() => _stateMachine.CurrentState.AnimationFinishTrigger();

        #endregion

        #region Handler Input

        public Vector2 MovementInput { get; private set; }
        public bool RunInput { get; private set; }
        public bool AttackInput { get; private set; }
        public bool AttackSuperInput { get; private set; }
        public bool AttackMagicInput { get; private set; }
        public bool InteractInput { get; private set; }

        private void HandlerMovement(Vector2 value) => MovementInput = value;
        private void HandlerRun() => RunInput = true;
        private void HandlerRunCancelled() => RunInput = false;
        private void HandlerAttack() => AttackInput = true;
        private void HandlerAttackCancelled() => AttackInput = false;
        private void HandlerAttackSuper() => AttackSuperInput = true;
        private void HandlerAttackSuperCancelled() => AttackSuperInput = false;
        private void HandlerAttackMagic() => AttackMagicInput = true;
        private void HandlerAttackMagicCancelled() => AttackMagicInput = false;
        private void HandlerInteract() => InteractInput = true;
        private void HandlerInteractCancelled() => InteractInput = false;

        #endregion
    }
}