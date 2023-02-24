using Cinemachine;
using UnityEngine;
namespace Camera
{
    public class CineMachinePovExtension : CinemachineExtension
    {
        private float _clampAngle = 72.5f;
        [Space]
        private float _verticalSpeedMouse = 7.5f;
        private float _horizontalSpeedMouse = 7.5f;

        private Input.InputManager _inputManager;
        private Vector2 _startingRotation;

        private void Start()
        {
            _inputManager = Input.InputManager.Instance;
            transform.GetComponent<CinemachineVirtualCameraBase>().Follow = GameObject.Find("PositionCamera").transform;
        }

        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (stage == CinemachineCore.Stage.Aim)
            {
                RotateCamera(vcam, ref state, deltaTime);
            }
        }

        private void RotateCamera(ICinemachineCamera vcam, ref CameraState state, float deltaTime)
        {
            if (!vcam.Follow)
            {
                return;
            }
            var deltaInput = _inputManager.GetLookInput();
            _startingRotation.x += deltaInput.x * _verticalSpeedMouse * deltaTime;
            _startingRotation.y += deltaInput.y * _horizontalSpeedMouse * deltaTime;
            _startingRotation.y = Mathf.Clamp(_startingRotation.y, -_clampAngle, _clampAngle);
            state.RawOrientation = Quaternion.Euler(-_startingRotation.y, _startingRotation.x, 0f);
        }
    }
}