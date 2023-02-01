using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [Space, Header("Data")]
    [SerializeField] private InteractionInputData _interactionInputData = null;
    [SerializeField] private InteractionData _interactionData = null;

    [Space, Header("UI")]
    [SerializeField] private UIInteractionBare _uiInteractionBare = null;

    [Space, Header("Ray Settings")]
    [SerializeField] private float _rayDistance;
    [SerializeField] private float _raySphereRadius;
    [SerializeField] private LayerMask _interactibleLayer;

    private float _holdTimer = 0f;
    private bool _isInteracting = false;

    private void Update()
    {
        CheckForInteractable();
        CheckForInteractableInput();
    }

    void CheckForInteractable()
    {
        Ray ray = new(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hitInfo;

        if (Physics.SphereCast(ray, _raySphereRadius, out hitInfo, _rayDistance, _interactibleLayer))
        {
            InteractableBase _interactable = hitInfo.collider.GetComponent<InteractableBase>();

            if (_interactable != null)
            {
                if (_interactionData.IsEmpy())
                {
                    _interactionData.Interactable = _interactable;
                    _uiInteractionBare.SetTooltipText(_interactable.TooltipText);
                }
                else if (!_interactionData.IsSameInteractable(_interactable))
                {
                    _interactionData.Interactable = _interactable;
                    _uiInteractionBare.SetTooltipText(_interactable.TooltipText);
                }
            }
        }
        else
        {
            _interactionData.ResetData();
            _uiInteractionBare.Reset();
        }

        Debug.DrawRay(ray.origin, ray.direction * _rayDistance, Physics.SphereCast(ray, _raySphereRadius, out hitInfo, _rayDistance, _interactibleLayer) ? Color.green : Color.red);
    }

    void CheckForInteractableInput()
    {
        if (_interactionData.IsEmpy()) return;

        if (_interactionInputData.InteractedClicked)
        {
            _isInteracting = true;
            _holdTimer = 0f;
        }

        if (_interactionInputData.InteractedReleased)
        {
            _isInteracting = false;
            _holdTimer = 0f;
            _uiInteractionBare.SetProgress(_holdTimer);
        }

        if (_isInteracting)
        {
            if (!_interactionData.Interactable.IsInteractable) return;

            if (!_interactionData.Interactable.HoldInteract)
            {
                _holdTimer += Time.deltaTime;

                float progress = _holdTimer / _interactionData.Interactable.HoldDuration;
                _uiInteractionBare.SetProgress(progress);

                if (_holdTimer >= _interactionData.Interactable.HoldDuration)
                {
                    _interactionData.Interact();
                    _isInteracting = false;
                }
            }
            else
            {
                _interactionData.Interact();
                _isInteracting = false;
            }
        }
    }
}
