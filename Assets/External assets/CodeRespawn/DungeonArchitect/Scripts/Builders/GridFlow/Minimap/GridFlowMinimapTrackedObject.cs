//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;


namespace DungeonArchitect.Builders.GridFlow
{
    public class GridFlowMinimapTrackedObject : MonoBehaviour
    {
        public Texture2D icon;
        public float iconScale = 1.0f;
        public bool rotateIcon = false;
        public Color tint = Color.white;
        public bool exploresFogOfWar = false;
        public float fogOfWarNumTileRadius = 5;
        public float fogOfWarLightFalloffStart = 0.5f;

        // Start is called before the first frame update
        void Start()
        {
            var minimap = FindObjectOfType<GridFlowMinimap>();
            if (minimap != null)
            {
                minimap.AddTrackedObject(this);
            }
        }

    }
}
