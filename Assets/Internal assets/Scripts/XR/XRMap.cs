using System;
using UnityEngine;

namespace XR
{
    [Serializable]
    public class XRMap
    {
        public Transform xrTarget;
        public Transform rigTarget;
        public Vector3 trackingPositionOffset;
        public Vector3 trackingRotationOffset;

        public void Map()
        {
            rigTarget.position = xrTarget.TransformPoint(trackingPositionOffset);
            rigTarget.rotation = xrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
        }
    }
}