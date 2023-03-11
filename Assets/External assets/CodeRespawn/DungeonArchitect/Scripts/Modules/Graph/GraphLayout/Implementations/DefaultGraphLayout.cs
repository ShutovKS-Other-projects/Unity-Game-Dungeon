//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Graphs.Layouts
{
    public class DefaultGraphLayoutNodeActions : IGraphLayoutNodeActions<GraphNode>
    {
        Dictionary<GraphNode, List<GraphNode>> outgoingNodes = new Dictionary<GraphNode, List<GraphNode>>();

        public DefaultGraphLayoutNodeActions(Graph graph) {
            // Cache the outgoing nodes to avoid O(N^2) with GetOutgoingNodes function
            if (graph != null)
            {
                foreach (var link in graph.Links)
                {
                    if (link != null && link.Output != null && link.Input != null)
                    {
                        var startNode = link.Output.Node;
                        var endNode = link.Input.Node;
                        if (startNode != null && endNode != null)
                        {
                            if (!outgoingNodes.ContainsKey(startNode))
                            {
                                outgoingNodes.Add(startNode, new List<GraphNode>());
                            }
                            outgoingNodes[startNode].Add(endNode);
                        }
                    }
                }
            }
        }

        public void SetNodePosition(GraphNode node, Vector2 position)
        {
            node.Position = position;
        }

        public Vector2 GetNodePosition(GraphNode node)
        {
            return node.Position;
        }

        public GraphNode[] GetOutgoingNodes(GraphNode node)
        {
            if (outgoingNodes.ContainsKey(node))
            {
                return outgoingNodes[node].ToArray();
            }
            return new GraphNode[0];
        }
    }
}


