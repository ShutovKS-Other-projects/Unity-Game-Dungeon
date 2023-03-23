using Cinemachine;
using Input;
using UnityEngine;

namespace Camera
{
    public class CineMachinePovExtension : CinemachineExtension
    {
        private float _clampAngle = 72.5f;
        [Space] private float _verticalSpeedMouse = 7.5f;
        private float _horizontalSpeedMouse = 7.5f;

        private InputManagerGame _inputManagerGame;
        private InputManagerHomeScene _inputManagerHomeScene;
        private Vector2 _startingRotation;

        private delegate Vector2 LookInput();

        private LookInput _getLookInput;

        private void Start()
        {
            transform.GetComponent<CinemachineVirtualCameraBase>().Follow = GameObject.Find("PositionCamera").transform;

            if (GameObject.Find("ManagerScene").GetComponent<ManagerScene.ManagerScene>().currentSceneType ==
                ManagerScene.SceneType.Home)
                _getLookInput = GetLookInputHome;
            else if (GameObject.Find("ManagerScene").GetComponent<ManagerScene.ManagerScene>().currentSceneType ==
                     ManagerScene.SceneType.Game)
                _getLookInput = GetLookInputGame;

            _inputManagerGame = InputManagerGame.Instance;
            _inputManagerHomeScene = InputManagerHomeScene.Instance;
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

            var deltaInput = _getLookInput();
            _startingRotation.x += deltaInput.x * _verticalSpeedMouse * deltaTime;
            _startingRotation.y += deltaInput.y * _horizontalSpeedMouse * deltaTime;
            _startingRotation.y = Mathf.Clamp(_startingRotation.y, -_clampAngle, _clampAngle);
            state.RawOrientation = Quaternion.Euler(-_startingRotation.y, _startingRotation.x, 0f);
        }


        private Vector2 GetLookInputHome() => _inputManagerHomeScene.GetLookInput();
        private Vector2 GetLookInputGame() => _inputManagerGame.GetLookInput();
    }
}