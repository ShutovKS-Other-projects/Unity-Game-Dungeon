using Scene;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Manager
{
    public class ManagerRig : MonoBehaviour
    {
        public static ManagerRig Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        [Space] public RigBuilder rigBuilder;

        [Space] [Header("Hand Rig")] public Rig grab;
        public Rig lHandRig;
        public Rig rHandRig;

        [Space] [Header("Hand")] public TwoBoneIKConstraint lHand;
        public TwoBoneIKConstraint rHand;

        [Space] [Header("Target")] public Transform lHandTargetTransform;
        public Transform rHandTargetTransform;

        [Space] [Header("Capture Weapon")] public MultiParentConstraint lHandCapture;
        public MultiParentConstraint rHandCapture;


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

        private void FindRig()
        {
            rigBuilder = ManagerPlayer.Instance.playerTransform!.GetComponentInChildren<RigBuilder>();

            var collectionRig = rigBuilder.transform.Find("Collection Rig");

            grab = collectionRig.transform.Find("Grab").GetComponent<Rig>();
            lHandRig = collectionRig.transform.Find("L_Hand").GetComponent<Rig>();
            rHandRig = collectionRig.transform.Find("R_Hand").GetComponent<Rig>();

            lHand = lHandRig.transform.Find("Hand").GetComponent<TwoBoneIKConstraint>();
            rHand = rHandRig.transform.Find("Hand").GetComponent<TwoBoneIKConstraint>();

            lHandTargetTransform = lHand.transform.Find("Target");
            rHandTargetTransform = rHand.transform.Find("Target");

            lHandCapture = grab.transform.Find("L_Grab").GetComponent<MultiParentConstraint>();
            rHandCapture = grab.transform.Find("R_Grab").GetComponent<MultiParentConstraint>();
        }
    }
}