//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect.Graphs;

namespace DungeonArchitect.SpatialConstraints
{
    [System.Serializable]
    public class SpatialConstraintAsset : ScriptableObject
    {
        [SerializeField]
        public bool rotateToFit = true;

        [SerializeField]
        public bool applyFitRotation = false;

        [SerializeField]
        public bool applyMarkerRotation = true;

        [SerializeField]
        public bool checkRelativeToMarkerRotation = true;

        [SerializeField]
        public GraphNode hostThemeNode;
        
        [HideInInspector]
        [SerializeField]
        private SpatialConstraintGraph graph;

        public SpatialConstraintGraph Graph
        {
            get { return graph; }
        }

        public void Init(GraphNode hostThemeNode)
        {
            this.hostThemeNode = hostThemeNode;

            graph = CreateInstance<SpatialConstraintGraph>();
            graph.asset = this;
            graph.hideFlags = HideFlags.HideInHierarchy;
        }

        public void OnEnable()
        {
            hideFlags = HideFlags.HideInHierarchy;
        }
        
    }
}

