//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Graphs
{
    public class CommentNode : GraphNode
    {
        public string message = "Comment";
        public Color background = new Color(1.0f, 0.714f, 0.992f, 0.175f);
        public int fontSize = 22;

        public override void CopyFrom(GraphNode node)
        {
            base.CopyFrom(node);

            var otherNode = node as CommentNode;
            if (otherNode == null) return;

            message = otherNode.message;
            background = otherNode.background;
        }

    }
}
