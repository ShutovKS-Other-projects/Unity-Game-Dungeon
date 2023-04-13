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

        [Space] [Header("Hand Rig")] public static UnityEngine.Animations.Rigging.Rig rigGrabs;
        public static UnityEngine.Animations.Rigging.Rig rigHands;

        [Space] [Header("Hand")] public static TwoBoneIKConstraint lHand;
        public static TwoBoneIKConstraint rHand;

        [Space] [Header("Target")] public static Transform lHandTargetTransform;
        public static Transform rHandTargetTransform;

        [Space] [Header("Capture Weapon")] public static MultiParentConstraint lHandGrab;
        public static MultiParentConstraint rHandGrab;

        private void Start()
        {
            _transform = transform;
            FindRig();
            
            _inputReader = Resources.Load<InputReader>($"ScriptableObject/Input/InputReader");

            _inputReader.XRTriggeredHandLeftEvent += OnEnableLeftHand;
            _inputReader.XRTriggeredHandLeftCancelledEvent += OnDisableLeftHand;
            _inputReader.XRTriggeredHandRightEvent += OnEnableRightHand;
            _inputReader.XRTriggeredHandRightCancelledEvent += OnDisableRightHand;
        }

        public static void SetTransformTargetZero(Transform transform)
        {
            transform.localPosition = new Vector3(0, 0, 0);
            transform.localRotation = new Quaternion(0, 0, 0, 0);
        }

        private static void FindRig()
        {
            rigBuilder = _transform.GetComponentInParent<RigBuilder>();

            rigHands = _transform.Find("Hands").GetComponent<UnityEngine.Animations.Rigging.Rig>();
            lHand = rigHands.transform.Find("L_Hand").GetComponent<TwoBoneIKConstraint>();
            rHand = rigHands.transform.Find("R_Hand").GetComponent<TwoBoneIKConstraint>();

            lHandTargetTransform = lHand.transform.Find("Target");
            rHandTargetTransform = rHand.transform.Find("Target");

            rigGrabs = _transform.Find("Grabs").GetComponent<UnityEngine.Animations.Rigging.Rig>();
            lHandGrab = rigGrabs.transform.Find("L_Grab").GetComponent<MultiParentConstraint>();
            rHandGrab = rigGrabs.transform.Find("R_Grab").GetComponent<MultiParentConstraint>();
        }

        private static void OnEnableLeftHand() => lHand.weight = 1;
        private static void OnDisableLeftHand() => lHand.weight = 0;
        private static void OnEnableRightHand() => rHand.weight = 1;
        private static void OnDisableRightHand() => rHand.weight = 0;
    }
}