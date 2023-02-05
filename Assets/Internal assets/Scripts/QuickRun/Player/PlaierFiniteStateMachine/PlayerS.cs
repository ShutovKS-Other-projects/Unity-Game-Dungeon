using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerS : MonoBehaviour
{
    public PlayerStateMachine StateMachine { get; private set; }

    public PlayerIdleState IdleState { get; private set; }
    public PlayerMovementState MovementState { get; private set; }

    [SerializeField] private PlayerData playerData;

    public Animator Animator { get; private set; }
    public InputManager InputManager { get; private set; }
    public Rigidbody RB { get; private set; }

    private void Awake()
    {
        StateMachine = new PlayerStateMachine();

        IdleState = new PlayerIdleState(this, StateMachine, playerData, "Idle");
        MovementState = new PlayerMovementState(this, StateMachine, playerData, "Movement");
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
}
