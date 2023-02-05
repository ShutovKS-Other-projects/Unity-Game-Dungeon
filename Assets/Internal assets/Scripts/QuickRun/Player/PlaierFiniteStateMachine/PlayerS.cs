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
    
    [SerializeField] private PlayerData playerData;
    #endregion

    #region Check Transforms
    [SerializeField] private Transform groundCheck;
    #endregion

    #region Components
    public Animator Animator { get; private set; }
    public InputManager InputManager { get; private set; }
    public Rigidbody RB { get; private set; }
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
    }

    private void Start()
    {
        Animator = GetComponentInChildren<Animator>();
        InputManager = InputManager.Instance;
        RB = GetComponent<Rigidbody>();

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

    #region Force
    public void SetVelocityY(float velocityY)
    {
        RB.velocity = new Vector3(RB.velocity.x, velocityY, RB.velocity.z);
    }
    #endregion
    #region Movement
    public void Movement(Vector2 movementInput)
    {
        Acceleration(out float acceleration);
        Animator.SetFloat("Speed", movementInput.y * acceleration);
        Vector3 move = new Vector3(movementInput.x, 0, movementInput.y);

        RB.AddRelativeForce(move * playerData.movementVelocity * acceleration * Time.deltaTime);

        void Acceleration(out float acceleration)
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
    }
    #endregion

    #region Check Functions
    public bool CheckIfGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, playerData.groundCheckRadius, playerData.whatIsGround);
    }
    #endregion

    #region Other Functions
    private void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();
    private void AnimationFinishTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();
    #endregion
}
