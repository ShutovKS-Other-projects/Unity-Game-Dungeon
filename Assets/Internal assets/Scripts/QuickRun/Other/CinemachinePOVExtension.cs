using UnityEngine;
using Cinemachine;

public class CinemachinePOVExtension : CinemachineExtension
{
    [SerializeField] private float verticalSpeed = 10f;
    [SerializeField] private float horisontalSpeed = 10f;
    [SerializeField] private float clampAngle = 72.5f;

    private InputManager _inputManager;
    private Vector3 staringRotation;

    private void Start()
    {
        _inputManager = InputManager.Instance;
        transform.GetComponent<CinemachineVirtualCameraBase>().Follow = GameObject.Find("PositionCamera").transform;
    }

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime) => RotationCamera(vcam, stage, ref state, deltaTime);

    private void RotationCamera(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (vcam.Follow)
            if (stage == CinemachineCore.Stage.Aim)
                if (staringRotation == null)
                {
                    staringRotation = transform.localRotation.eulerAngles;
                    Vector2 daltaImput = _inputManager.GetLookInput();
                    staringRotation.x += daltaImput.x * verticalSpeed * Time.deltaTime;
                    staringRotation.y += daltaImput.y * horisontalSpeed * Time.deltaTime;
                    staringRotation.y = Mathf.Clamp(staringRotation.y, -clampAngle, clampAngle);
                    state.RawOrientation = Quaternion.Euler(staringRotation.y, staringRotation.x, 0f);
                }
    }
}