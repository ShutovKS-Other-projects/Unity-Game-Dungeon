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

        [Space] [Header("Rig")] public static UnityEngine.Animations.Rigging.Rig rigGrips;
        public static UnityEngine.Animations.Rigging.Rig rigArms;

        [Space] [Header("Arm")] public static TwoBoneIKConstraint lArm;
        public static TwoBoneIKConstraint rArm;

        [Space] [Header("Arm Target")] public static Transform lArmTargetTransform;
        public static Transform rArmTargetTransform;
        
        

        [Space] [Header("Grip")] public static MultiParentConstraint lArmGrip;
        public static MultiParentConstraint rArmGrip;

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

            rigGrips = _transform.Find("Grips").GetComponent<UnityEngine.Animations.Rigging.Rig>();
            lArmGrip = rigGrips.transform.Find("L_Grip").GetComponent<MultiParentConstraint>();
            rArmGrip = rigGrips.transform.Find("R_Grip").GetComponent<MultiParentConstraint>();
        }

        private static void OnEnableLeftArm() => lArm.weight = 1;
        private static void OnDisableLeftArm() => lArm.weight = 0;
        private static void OnEnableRightArm() => rArm.weight = 1;
        private static void OnDisableRightArm() => rArm.weight = 0;
    }
}