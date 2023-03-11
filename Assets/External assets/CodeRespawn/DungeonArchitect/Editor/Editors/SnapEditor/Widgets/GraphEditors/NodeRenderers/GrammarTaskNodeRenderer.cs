//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Grammar;
using DungeonArchitect.Graphs;
using UnityEngine;
using DMathUtils = DungeonArchitect.Utils.MathUtils;

namespace DungeonArchitect.Editors.SnapFlow
{
    public class GrammarTaskNodeRenderer : GrammarNodeRendererBase
    {
        protected override Color GetPinColor(GraphNode node)
        {
            var taskNode = node as GrammarTaskNode;

            var nodeColor = (taskNode != null && taskNode.NodeType != null)
                ? taskNode.NodeType.nodeColor : new Color(0.2f, 0.3f, 0.3f);
            return nodeColor;
        }

        protected override Color GetBodyColor(GraphNode node)
        {
            var taskNode = node as GrammarTaskNode;
            var nodeType = (taskNode != null) ? taskNode.NodeType : null;
            return (nodeType != null) ? new Color(0.1f, 0.1f, 0.1f, 1) : Color.red;
        }

        protected override string GetCaption(GraphNode node)
        {
            var taskNode = node as GrammarTaskNode;
            var nodeType = (taskNode != null) ? taskNode.NodeType : null;

            if (nodeType == null)
            {
                return "[DELETED]";
            }

            var nodeMessage = nodeType.nodeName;
            if (taskNode.DisplayExecutionIndex)
            {
                nodeMessage = string.Format("{0}:{1}", nodeMessage, taskNode.executionIndex);
            }
            return nodeMessage;
        }
    }
}
