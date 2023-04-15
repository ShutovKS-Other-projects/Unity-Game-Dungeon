using System;
using Input;
using Interactive;
using Unity.VisualScripting;
using UnityEngine;

namespace XR
{
    public class XRHandActionLeft : MonoBehaviour
    {
        private SphereCollider _sphereCollider;
        private InputReader _inputReader;
        private SideType _sideType = SideType.Left;

        private bool _isGrab;
        private bool _isAction;
 
        private void Start()
        {
            _sphereCollider = transform.AddComponent<SphereCollider>();
            _sphereCollider.isTrigger = true;
            _sphereCollider.enabled = false;
            _sphereCollider.radius = 0.2f;

            _inputReader = Resources.Load<InputReader>($"ScriptableObject/Input/InputReader");

            _inputReader.XRGripLeftEvent += OnGrab;
            _inputReader.XRGripLeftCancelledEvent += OnGrabCancelled;

            _inputReader.XRActionLeftEvent += OnAction;
            _inputReader.XRActionLeftCancelledEvent += OnActionCancelled;

            _inputReader.XRTrackingArmLeftEvent += OnEnableCollider;
            _inputReader.XRTrackingArmLeftCancelledEvent += OnDisableCollider;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Interactive")) return;

            if (_isAction)
            {
                var interactive = other.GetComponent<IInteractive>();
                interactive?.OnInteractXR(_sideType);
            }
            if (_isGrab)
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
            GrabsController.LetGoLeftGrab();
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