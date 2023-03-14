//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Graphs;

namespace DungeonArchitect.Editors.Utils
{
    public static class ThemeEditorUtils
    {
        public static MarkerNode GetMarkerNodeInHierarchy(GraphNode node)
        {
            MarkerNode markerNode = null;

            if (node is MarkerNode)
            {
                markerNode = node as MarkerNode;
            }
            else if (node is VisualNode)
            {
                // Check if the node is linked to a marker node
                var incomingNodes = GraphUtils.GetIncomingNodes(node);
                if (incomingNodes.Length == 1)
                {
                    markerNode = incomingNodes[0] as MarkerNode;
                }
            }

            return markerNode;
        }
    }
}