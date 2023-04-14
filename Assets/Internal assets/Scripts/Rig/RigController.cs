using Input;
using Player;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Rig
{
    public class RigController : MonoBehaviour
    {
        private InputReader _inputReader;
        private static Transform _transform;

        [Space] public static RigBuilder rigBuilder;

        [Space] [Header("Rig")] public static UnityEngine.Animations.Rigging.Rig rigGrabs;
        public static UnityEngine.Animations.Rigging.Rig rigArms;

        [Space] [Header("Arm")] public static TwoBoneIKConstraint lArm;
        public static TwoBoneIKConstraint rArm;

        [Space] [Header("Arm Target")] public static Transform lArmTargetTransform;
        public static Transform rArmTargetTransform;
        
        

        [Space] [Header("Grab")] public static MultiParentConstraint lGrab;
        public static MultiParentConstraint rGrab;

        private void Start()
        {
            _transform = transform;
            FindRig();
            
            _inputReader = Resources.Load<InputReader>($"ScriptableObject/Input/InputReader");

            _inputReader.XRTrackingArmLeftEvent += OnEnableLeftArm;
            _inputReader.XRTrackingArmLeftCancelledEvent += OnDisableLeftArm;
            _inputReader.XRTrackingArmRightEvent += OnEnableRightArm;
            _inputReader.XRTrackingArmRightCancelledEvent += OnDisableRightArm;
        }

        public static void SetTransformTargetZero(Transform transform)
        {
            transform.localPosition = new Vector3(0, 0, 0);
            transform.localRotation = new Quaternion(0, 0, 0, 0);
        }

        private static void FindRig()
        {
            rigBuilder = _transform.GetComponentInParent<RigBuilder>();

            rigArms = _transform.Find("Arms").GetComponent<UnityEngine.Animations.Rigging.Rig>();
            lArm = rigArms.transform.Find("L_Arm").GetComponent<TwoBoneIKConstraint>();
            rArm = rigArms.transform.Find("R_Arm").GetComponent<TwoBoneIKConstraint>();

            lArmTargetTransform = lArm.transform.Find("Target");
            rArmTargetTransform = rArm.transform.Find("Target");

            rigGrabs = _transform.Find("Grabs").GetComponent<UnityEngine.Animations.Rigging.Rig>();
            lGrab = rigGrabs.transform.Find("L_Grab").GetComponent<MultiParentConstraint>();
            rGrab = rigGrabs.transform.Find("R_Grab").GetComponent<MultiParentConstraint>();
        }

        private static void OnEnableLeftArm() => lArm.weight = 1;
        private static void OnDisableLeftArm() => lArm.weight = 0;
        private static void OnEnableRightArm() => rArm.weight = 1;
        private static void OnDisableRightArm() => rArm.weight = 0;
    }
}