using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    protected Vector2 movementInput;

    protected bool crouchInput;
    protected bool interactInput;
    protected bool isTouchingCelling;

    private bool attackInput;
    private bool blockInput;
    private bool jumpInput;
    private bool isInteractable;
    private bool isGrounded;

    public PlayerGroundedState(PlayerStateController playerStateController, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(playerStateController, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isGrounded = playerStateController.CheckIfGrounded();
        isTouchingCelling = playerStateController.CheckIfTouchingCelling();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        movementInput = playerStateController.InputManager.GetPlayerMovementInput();
        crouchInput = playerStateController.InputManager.GetPlayerCrouchInput();
        jumpInput = playerStateController.InputManager.GetPlayerJumpInput();
        attackInput = playerStateController.InputManager.GetPlayerAttackInput();
        blockInput = playerStateController.InputManager.GetPlayerBlockInput();
        interactInput = playerStateController.InputManager.GetPlayerInteractInput();
        isInteractable = CheckVisibleIfInteractable();

        if (attackInput && !isTouchingCelling)
        {
            stateMachine.ChangeState(playerStateController.AttackState);
        }
        else if (blockInput && !isTouchingCelling)
        {
            stateMachine.ChangeState(playerStateController.BlockState);
        }
        else if (jumpInput && !isTouchingCelling)
        {
            stateMachine.ChangeState(playerStateController.JumpState);
        }
        else if (isInteractable && interactInput)
        {
            stateMachine.ChangeState(playerStateController.InteractState);
        }
        else if (!isGrounded)
        {
            stateMachine.ChangeState(playerStateController.InAirState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!playerStateController.InputManager.GetPlayerSprintInput())
            RecoveryStamina();
            
        playerStateController.Rotation();
    }

    #region Check Methods
    private bool CheckVisibleIfInteractable()
    {
        Ray ray = new(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hitInfo;

        if (Physics.SphereCast(ray, playerData.interCheckSphereRadius, out hitInfo, playerData.interCheckDistance, playerData.interactableLayer))
        {
            InteractableBase _interactable = hitInfo.collider.GetComponent<InteractableBase>();

            if (_interactable != null)
            {
                if (playerData.interactionData.IsEmpy() || playerData.interactionData.IsSameInteractable(_interactable))
                {
                    playerData.interactionData.Interactable = _interactable;
                    playerStateController.uiInteractionBare.SetTooltipText(_interactable.TooltipText);

                    return true;
                }
                else
                {
                    playerData.interactionData.ResetData();
                    playerStateController.uiInteractionBare.SetTooltipText("");

                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            playerData.interactionData.ResetData();
            playerStateController.uiInteractionBare.SetTooltipText("");
            return false;
        }
    }
    #endregion

    #region Other Methods
    private void RecoveryStamina()
    {
        if (playerData.stamina >= playerData.maxStamina)
        {
            playerData.stamina = playerData.maxStamina;
        }
        else
        {
            if (playerData.isFatigue)
            {
                playerData.stamina += playerData.staminaRecoverySpeedIsFatigue * Time.deltaTime;
                if (playerData.stamina >= playerData.maxStamina / 4)
                {
                    playerData.isFatigue = false;
                }
            }
            else
            {
                playerData.stamina += playerData.staminaRecoverySpeed * Time.deltaTime;
            }
        }
    }

    private void RecoveryHealth()
    {
        if (playerData.health >= playerData.maxHealth)
        {
            playerData.health = playerData.maxHealth;
        }
        else
        {
            playerData.health += playerData.healthRecoverySpeed * Time.deltaTime;
        }
    }
    #endregion
}
