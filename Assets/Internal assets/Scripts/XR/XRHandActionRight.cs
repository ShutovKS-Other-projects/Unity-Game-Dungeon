using System;
using Input;
using Interactive;
using JetBrains.Annotations;
using Player;
using Unity.VisualScripting;
using UnityEngine;

namespace XR
{
    public class XRHandActionRight : MonoBehaviour
    {
        private CapsuleCollider _sphereCollider;
        private InputReader _inputReader;
        private SideType _sideType = SideType.Right;
        private Animator _animator;

        private bool _isGrab;
        private bool _isAction;

        private void Start()
        {
            _sphereCollider = transform.AddComponent<CapsuleCollider>();
            _sphereCollider.isTrigger = true;
            _sphereCollider.enabled = false;
            _sphereCollider.radius = 0.3f;
            _sphereCollider.height = 1.25f;
            _sphereCollider.center = new Vector3(0, 0.5f, 0);

            _animator = PlayerController.player.GetComponent<Animator>();

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
            _animator.SetFloat("GripRight", 1);
            _isGrab = true;
        }

        private void OnGrabCancelled()
        {
            Debug.Log("OnGrabCancelled");
            _animator.SetFloat("GripRight", 0);
            _isGrab = false;
            GrabsController.LetGoRightGrab();
        }

        private void OnAction()
        {
            Debug.Log("OnAction");
            _animator.SetFloat("TriggerRight", 1);
            _isAction = true;
        }

        private void OnActionCancelled()
        {
            Debug.Log("OnActionCancelled");
            _animator.SetFloat("TriggerRight", 0);
            _isAction = false;
        }

        private void OnEnableCollider() => _sphereCollider.enabled = true;
        private void OnDisableCollider() => _sphereCollider.enabled = false;
    }
}