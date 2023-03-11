//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;

namespace DungeonArchitect.Graphs
{
    /// <summary>
    /// Theme graph utility functions
    /// </summary>
    public class GraphUtils
    {
        private static GraphNode[] GetDirectionalNodes(GraphPin pin, bool isIncoming)
        {
            var result = new List<GraphNode>();
            var hostNode = pin.Node;
            if (hostNode && hostNode.Graph)
            {
                var graph = hostNode.Graph;
                foreach (var link in graph.Links)
                {
                    if (isIncoming && link.Input == pin)
                    {
                        var otherNode = link.Output.Node;
                        result.Add(otherNode);
                    }
                    else if (!isIncoming && link.Output == pin)
                    {
                        var otherNode = link.Input.Node;
                        result.Add(otherNode);
                    }
                }
            }
            return result.ToArray();
        }

        private static GraphNode[] GetDirectionalNodes(GraphNode hostNode, bool isIncoming)
        {
            var result = new List<GraphNode>();
            if (hostNode && hostNode.Graph)
            {
                var graph = hostNode.Graph;
                foreach (var link in graph.Links)
                {
                    if (isIncoming && link.Input.Node == hostNode)
                    {
                        var otherNode = link.Output.Node;
                        result.Add(otherNode);
                    }
                    else if (!isIncoming && link.Output.Node == hostNode)
                    {
                        var otherNode = link.Input.Node;
                        result.Add(otherNode);
                    }
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// Test if the graph link lies within the rectangle
        /// </summary>
        /// <param name="outer">The rect to test against</param>
        /// <param name="link">The link to test the intersection</param>
        /// <returns>True if intersects, false otherwise</returns>
        public static bool Intersects(Rect outer, GraphLink link) {
            if (link == null || link.Input == null || link.Output == null) return false;

            var p0 = link.Input.WorldPosition;
            var p1 = link.Output.WorldPosition;

            if (outer.Contains(p0) || outer.Contains(p1)) {
                return true;
            }
			
            var x0 = outer.position.x;
            var x1 = outer.position.x + outer.size.x;
            var y0 = outer.position.y;
            var y1 = outer.position.y + outer.size.y;

            var outside = 
                (p0.x < x0 && p1.x < x0) ||
                (p0.x > x1 && p1.x > x1) ||
                (p0.y < y0 && p1.y < y0) ||
                (p0.y > y1 && p1.y > y1);

            return !outside;
        }

        public static GraphNode[] GetIncomingNodes(GraphPin pin)
        {
            return GetDirectionalNodes(pin, true);
        }

        public static GraphNode[] GetOutgoingNodes(GraphPin pin)
        {
            return GetDirectionalNodes(pin, false);
        }

        public static GraphNode[] GetIncomingNodes(GraphNode node)
        {
            return GetDirectionalNodes(node, true);
        }

        public static GraphNode[] GetOutgoingNodes(GraphNode node)
        {
            return GetDirectionalNodes(node, false);
        }


    }
}
