using Cinemachine;
using Input;
using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Camera
{
    public class CineMachinePovExtension : CinemachineExtension
    {
        private InputReader _inputReader;
        private const float CLAMP_ANGLE = 72.5f;
        [Space] private const float VERTICAL_SPEED_MOUSE = 7.5f;
        private const float HORIZONTAL_SPEED_MOUSE = 7.5f;

        private Vector2 _startingRotation;
        private Vector2 _deltaInput;

        protected override void Awake()
        {
            base.Awake();
            _inputReader = Resources.Load<InputReader>($"ScriptableObject/Input/InputReader");
        }
        
        private void Start()
        {
            FindPositionCamera();
            _inputReader.LookEvent += HandlerLook;
        }

        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (stage == CinemachineCore.Stage.Aim)
            {
                RotateCamera(vcam, ref state, deltaTime);
            }
        }

        private void RotateCamera(ICinemachineCamera vcam, ref CameraState state, float deltaTime)
        {
            if (!vcam.Follow)
                return;

            _startingRotation.x += _deltaInput.x * VERTICAL_SPEED_MOUSE * deltaTime;
            _startingRotation.y += _deltaInput.y * HORIZONTAL_SPEED_MOUSE * deltaTime;
            _startingRotation.y = Mathf.Clamp(_startingRotation.y, -CLAMP_ANGLE, CLAMP_ANGLE);
            state.RawOrientation = Quaternion.Euler(-_startingRotation.y, _startingRotation.x, 0f);
        }

        private void FindPositionCamera()
        {
            transform.GetComponent<CinemachineVirtualCameraBase>().Follow = GameObject.Find("PositionCamera").transform;
        }
        
        private void HandlerLook(Vector2 delta)
        {
            _deltaInput = delta;
        }
    }
}