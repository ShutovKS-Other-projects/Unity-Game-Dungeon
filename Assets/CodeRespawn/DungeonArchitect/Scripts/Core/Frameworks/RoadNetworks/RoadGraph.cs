//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.RoadNetworks
{
    //[System.Serializable]
    public class RoadGraphEdge
    {
        //[SerializeField]
        public int edgeId;

        //[SerializeField]
        public int ownerNodeId;

        //[SerializeField]
        public int otherNodeId;

        //[SerializeField]
        public float thickness;

        //[SerializeField]
        public float angleToXAxis;
        
    }

    //[System.Serializable]
    public class RoadGraphNode
    {
        //[SerializeField]
        public int nodeId;

        //[SerializeField]
        public Vector3 position;

        // The edges will be sorted based on their angle
        //[SerializeField]
        public RoadGraphEdge[] adjacentEdges;
    }

    //[System.Serializable]
    public class RoadGraph
    {
        //[SerializeField]
        public RoadGraphNode[] nodes = new RoadGraphNode[0];
    }
}
