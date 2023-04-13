using Input;
using UnityEngine;
using UnityEngine.Serialization;

namespace XR
{
    public class XRRig : MonoBehaviour
    {
        private InputReader _inputReader;
        private bool _isTriggeredLeftArm, _isTriggeredRightArm;

        [FormerlySerializedAs("leftHand")] public XRMap leftArm;
        [FormerlySerializedAs("rightHand")] public XRMap rightArm;

        private void Start()
        {
            _inputReader = Resources.Load<InputReader>($"ScriptableObject/Input/InputReader");

            _inputReader.XRTrackingArmLeftEvent += OnEnableLeftArm;
            _inputReader.XRTrackingArmLeftCancelledEvent += OnDisableLeftArm;
            _inputReader.XRTrackingArmRightEvent += OnEnableRightArm;
            _inputReader.XRTrackingArmRightCancelledEvent += OnDisableRightArm;
        }

        private void LateUpdate()
        {
            if (_isTriggeredLeftArm) leftArm.Map();
            if (_isTriggeredRightArm) rightArm.Map();
        }

        private void OnEnableLeftArm() => _isTriggeredLeftArm = true;
        private void OnDisableLeftArm() => _isTriggeredLeftArm = false;
        private void OnEnableRightArm() => _isTriggeredRightArm = true;
        private void OnDisableRightArm() => _isTriggeredRightArm = false;
    }
}