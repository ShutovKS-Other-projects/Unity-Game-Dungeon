using System;
using UnityEngine;
namespace Item
{
    public class ItemSurveillancePlayer : MonoBehaviour
    {
        static Transform cameraTransform;

        void Start()
        {
            cameraTransform = UnityEngine.Camera.main!.transform;
        }

        private void LateUpdate()
        {
            transform.forward = cameraTransform.forward;
        }
    }
}
