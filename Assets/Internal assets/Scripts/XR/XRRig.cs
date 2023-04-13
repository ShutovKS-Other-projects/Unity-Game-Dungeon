using Input;
using UnityEngine;

namespace XR
{
    public class XRRig : MonoBehaviour
    {
        private InputReader _inputReader;
        private bool _isTriggeredLeftHand, _isTriggeredRightHand;

        public XRMap leftHand;
        public XRMap rightHand;

        private void Start()
        {
            _inputReader = Resources.Load<InputReader>($"ScriptableObject/Input/InputReader");

            _inputReader.XRTriggeredHandLeftEvent += OnEnableLeftHand;
            _inputReader.XRTriggeredHandLeftCancelledEvent += OnDisableLeftHand;
            _inputReader.XRTriggeredHandRightEvent += OnEnableRightHand;
            _inputReader.XRTriggeredHandRightCancelledEvent += OnDisableRightHand;
        }

        private void LateUpdate()
        {
            if (_isTriggeredLeftHand) leftHand.Map();
            if (_isTriggeredRightHand) rightHand.Map();
        }

        private void OnEnableLeftHand() => _isTriggeredLeftHand = true;
        private void OnDisableLeftHand() => _isTriggeredLeftHand = false;
        private void OnEnableRightHand() => _isTriggeredRightHand = true;
        private void OnDisableRightHand() => _isTriggeredRightHand = false;
    }
}