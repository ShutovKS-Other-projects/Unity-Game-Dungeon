//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect
{
    public class PlaceableMarker : MonoBehaviour
    {
        public string[] supportedMarkers;

        public Color debugColor = Color.red;
        public string debugText = "Marker Description";

        [HideInInspector]
        public bool drawDebugVisuals = true;
        
        void OnDrawGizmos()
        {
            if (drawDebugVisuals && transform != null)
            {
                Gizmos.color = debugColor;
                
                var center = transform.position;
                var radius = 0.2f;
                Gizmos.DrawSphere(center, radius);

                var start = center;
                var end = start + transform.forward;
                Gizmos.DrawLine(start, end);
            }
        }
    }
}
