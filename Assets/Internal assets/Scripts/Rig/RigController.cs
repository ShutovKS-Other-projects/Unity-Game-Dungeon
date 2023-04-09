using Player;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Rig
{
    public class RigController : MonoBehaviour
    {
        [Space] public static RigBuilder rigBuilder;

        [Space] [Header("Hand Rig")] public static UnityEngine.Animations.Rigging.Rig grab;
        public static UnityEngine.Animations.Rigging.Rig lHandRig;
        public static UnityEngine.Animations.Rigging.Rig rHandRig;

        [Space] [Header("Hand")] public static TwoBoneIKConstraint lHand;
        public static TwoBoneIKConstraint rHand;

        [Space] [Header("Target")] public static Transform lHandTargetTransform;
        public static Transform rHandTargetTransform;

        [Space] [Header("Capture Weapon")] public static MultiParentConstraint lHandCapture;
        public static MultiParentConstraint rHandCapture;

        //[Space] [Header("")]

        //[Space] [Header("")]

        //[Space] [Header("")]

        private void Start()
        {
            FindRig();
            // SceneController.OnNewSceneLoaded += FindRig;
        }

        public static void SetTransformTargetZero(Transform transform)
        {
            transform.localPosition = new Vector3(0, 0, 0);
            transform.localRotation = new Quaternion(0, 0, 0, 0);
        }

        private static void FindRig()
        {
            rigBuilder = PlayerController.playerTransform!.GetComponentInChildren<RigBuilder>();

            var collectionRig = rigBuilder.transform.Find("Collection Rig");

            grab = collectionRig.transform.Find("Grab").GetComponent<UnityEngine.Animations.Rigging.Rig>();
            lHandRig = collectionRig.transform.Find("L_Hand").GetComponent<UnityEngine.Animations.Rigging.Rig>();
            rHandRig = collectionRig.transform.Find("R_Hand").GetComponent<UnityEngine.Animations.Rigging.Rig>();

            lHand = lHandRig.transform.Find("Hand").GetComponent<TwoBoneIKConstraint>();
            rHand = rHandRig.transform.Find("Hand").GetComponent<TwoBoneIKConstraint>();

            lHandTargetTransform = lHand.transform.Find("Target");
            rHandTargetTransform = rHand.transform.Find("Target");

            lHandCapture = grab.transform.Find("L_Grab").GetComponent<MultiParentConstraint>();
            rHandCapture = grab.transform.Find("R_Grab").GetComponent<MultiParentConstraint>();
        }
    }
}