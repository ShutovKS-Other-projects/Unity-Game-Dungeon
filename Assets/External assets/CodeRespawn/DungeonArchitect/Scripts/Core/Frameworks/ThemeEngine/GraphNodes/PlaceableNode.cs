//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Graphs
{
    public class PlaceableNode : GraphNode
    {
        public Matrix4x4 offset = Matrix4x4.identity;
        public bool consumeOnAttach = true;
        public float attachmentProbability = 1.0f;


        public override void CopyFrom(GraphNode node)
        {
            base.CopyFrom(node);

            var otherNode = node as PlaceableNode;
            if (otherNode == null) return;

            this.offset = otherNode.offset;
            this.consumeOnAttach = otherNode.consumeOnAttach;
            this.attachmentProbability = otherNode.attachmentProbability;
        }

    }
}
