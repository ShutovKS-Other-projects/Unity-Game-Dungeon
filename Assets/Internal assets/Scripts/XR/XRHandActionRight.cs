using System;
using Input;
using Interactive;
using Unity.VisualScripting;
using UnityEngine;

namespace XR
{
    public class XRHandActionRight : MonoBehaviour
    {
        private InteractionObject _interactionObject = null;
        private SphereCollider _sphereCollider;
        private InputReader _inputReader;
        private SideType _sideType = SideType.Right;

        private bool _isGrab;
        private bool _isAction;

        private void Start()
        {
            _sphereCollider = transform.AddComponent<SphereCollider>();
            _sphereCollider.isTrigger = true;
            _sphereCollider.enabled = false;
            _sphereCollider.radius = 0.1f;

            _inputReader = Resources.Load<InputReader>($"ScriptableObject/Input/InputReader");

            _inputReader.XRGripRightEvent += OnGrab;
            _inputReader.XRGripRightCancelledEvent += OnGrabCancelled;

            _inputReader.XRActionRightEvent += OnAction;
            _inputReader.XRActionRightCancelledEvent += OnActionCancelled;
            
            _inputReader.XRTrackingArmRightEvent += OnEnableCollider;
            _inputReader.XRTrackingArmRightCancelledEvent += OnDisableCollider;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Interactive")) return;

            if (_isAction)
            {
                var interactive = other.GetComponent<IInteractive>();
                interactive?.OnInteractXR(_sideType);
            }
            else if (_isGrab)
            {
                var interactive = other.GetComponent<IInteractive>();
                interactive.OnGrabXR(_sideType);
            }
        }


        private void OnGrab()
        {
            Debug.Log("OnGrab");
            _isGrab = true;
        }

        private void OnGrabCancelled()
        {
            Debug.Log("OnGrabCancelled");
            _isGrab = false;
        }

        private void OnAction()
        {
            Debug.Log("OnAction");
            _isAction = true;
        }

        private void OnActionCancelled()
        {
            Debug.Log("OnActionCancelled");
            _isAction = false;
        }

        private void OnEnableCollider() => _sphereCollider.enabled = true;
        private void OnDisableCollider() => _sphereCollider.enabled = false;
    }
}