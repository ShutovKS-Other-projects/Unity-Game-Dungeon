using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerS : MonoBehaviour
{
    #region State Machine
    public PlayerStateMachine StateMachine { get; private set; }

    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerInAirState InAirState { get; private set; }
    public PlayerLandState LandState { get; private set; }
    public PlayerCrouchIdleState CrouchIdleState { get; private set; }
    public PlayerCrouchMoveState CrouchMoveState { get; private set; }

    [SerializeField] private PlayerData playerData;
    #endregion

    #region Check Transforms
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform cellingCheck;
    #endregion

    #region Components
    public Animator Animator { get; private set; }
    public InputManager InputManager { get; private set; }
    public Rigidbody RB { get; private set; }
    public CapsuleCollider MovementCollider { get; private set; }
    #endregion

    #region Unity Callbacks Functions
    private void Awake()
    {
        StateMachine = new PlayerStateMachine();

        IdleState = new PlayerIdleState(this, StateMachine, playerData, "Idle");
        MoveState = new PlayerMoveState(this, StateMachine, playerData, "Move");
        JumpState = new PlayerJumpState(this, StateMachine, playerData, "InAir");
        InAirState = new PlayerInAirState(this, StateMachine, playerData, "InAir");
        LandState = new PlayerLandState(this, StateMachine, playerData, "Land");
        CrouchIdleState = new PlayerCrouchIdleState(this, StateMachine, playerData, "CrouchIdle");
        CrouchMoveState = new PlayerCrouchMoveState(this, StateMachine, playerData, "CrouchMove");
    }

    private void Start()
    {
        Animator = GetComponentInChildren<Animator>();
        InputManager = InputManager.Instance;
        RB = GetComponent<Rigidbody>();
        MovementCollider = GetComponent<CapsuleCollider>();

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

    #region Velocity    
    public void SetVelocityY(float velocityY)
    {
        RB.velocity = new Vector3(RB.velocity.x, velocityY, RB.velocity.z);
        Animator.SetFloat("ySpeed", velocityY);
    }
    #endregion

    #region Movement
    public void Movement(Vector2 movementInput, float speed)
    {
        Acceleration(out float acceleration);
        Vector3 move = new Vector3(movementInput.x, 0, movementInput.y);

        RB.AddRelativeForce(move * speed * acceleration * Time.deltaTime, ForceMode.VelocityChange);

        Animator.SetFloat("zSpeed", move.z * acceleration);
        Animator.SetFloat("xSpeed", move.x * acceleration);
    }
    #endregion

    #region Rotation
    public virtual void Rotation()
    {
        transform.rotation = new Quaternion(0f, Camera.main.transform.rotation.y, 0f, Camera.main.transform.rotation.w);
    }
    #endregion

    #region Check Functions
    public bool CheckIfGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, playerData.groundCheckRadius, playerData.whatIsGround);
    }

    public bool CheckIfTouchingCeiling()
    {
        return Physics.CheckSphere(cellingCheck.position, playerData.groundCheckRadius, playerData.whatIsGround);
    }
    #endregion

    #region Other Functions
    public void SetColliderHeight(float height, float center)
    {
        MovementCollider.height = height;
        MovementCollider.center = new Vector3(MovementCollider.center.x, center, MovementCollider.center.z);
    }

    public void SpeedZero()
    {
        RB.velocity = Vector3.zero;
        Animator.SetFloat("zSpeed", 0);
        Animator.SetFloat("xSpeed", 0);
        Animator.SetFloat("ySpeed", 0);
    }

    private void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();

    private void AnimationFinishTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();

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

    #endregion
}
