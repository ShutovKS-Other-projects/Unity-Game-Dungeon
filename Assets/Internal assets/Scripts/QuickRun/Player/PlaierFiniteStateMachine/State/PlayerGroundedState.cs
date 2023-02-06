using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    protected Vector2 movementInput;

    protected bool crouchInput;
    protected bool interactInput;
    private bool attackInput;
    private bool blockInput;
    private bool jumpInput;
    private bool isInteractable;
    
    protected bool isTouchingCelling;
    private bool isTouchingGrounded;

    public PlayerGroundedState(PlayerS player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isTouchingGrounded = player.CheckIfGrounded();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        movementInput = player.InputManager.GetPlayerMovementInput();
        crouchInput = player.InputManager.GetPlayerCrouchInput();
        jumpInput = player.InputManager.GetPlayerJumpInput();
        attackInput = player.InputManager.GetPlayerAttackInput();
        blockInput = player.InputManager.GetPlayerBlockInput();
        interactInput = player.InputManager.GetPlayerInteractInput();
        isInteractable = CheckVisibleIfInteractable();

        if (attackInput) // && !isTouchingCelling)
        {
            stateMachine.ChangeState(player.AttackState);
        }
        else if (blockInput) // && !isTouchingCelling)
        {
            stateMachine.ChangeState(player.BlockState);
        }
        else if (jumpInput && !isTouchingCelling)
        {
            stateMachine.ChangeState(player.JumpState);
        }
        else if(isInteractable && interactInput)
        {
            stateMachine.ChangeState(player.InteractState);
        }
        else if (!isTouchingGrounded)
        {
            stateMachine.ChangeState(player.InAirState);
        } 
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        player.Rotation();
    }

    #region Check Methods
    private bool CheckVisibleIfInteractable()
    {
        Ray ray = new(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hitInfo;

        if (Physics.SphereCast(ray, playerData.interCheckSphereRadius, out hitInfo, playerData.interCheckRadius))
        {
            InteractableBase _interactable = hitInfo.collider.GetComponent<InteractableBase>();

            if (_interactable != null)
            {
                if (playerData.interactionData.IsEmpy())
                {
                    playerData.interactionData.Interactable = _interactable;
                    player.uiInteractionBare.SetTooltipText(_interactable.TooltipText);
                }
                else if (!playerData.interactionData.IsSameInteractable(_interactable))
                {
                    playerData.interactionData.Interactable = _interactable;
                    player.uiInteractionBare.SetTooltipText(_interactable.TooltipText);
                }

                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            playerData.interactionData.ResetData();
            player.uiInteractionBare.SetTooltipText("");
            return false;
        }
    }
    #endregion
}
