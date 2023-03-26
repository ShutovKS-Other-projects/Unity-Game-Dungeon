using UnityEngine;

namespace Old.Item
{
    public class ItemSurveillancePlayer : MonoBehaviour
    {
        private static Transform cameraTransform;

        private void Start()
        {
            cameraTransform = UnityEngine.Camera.main!.transform;
        }

        private void LateUpdate()
        {
            transform.forward = cameraTransform.forward;
        }
    }
}