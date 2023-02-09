//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System;
using System.Collections.Generic;
using DungeonArchitect.Flow.Items;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Flow.Domains.Layout
{
    public class FlowLayoutGraphUtils
    {
        
        public static FlowLayoutGraphNode[] FindNodesOnPath(FlowLayoutGraph graph, string pathName)
        {
            var result = new List<FlowLayoutGraphNode>();

            if (pathName.Length > 0)
            {
                foreach (var node in graph.Nodes)
                {
                    if (node != null && node.pathName == pathName)
                    {
                        result.Add(node);
                    }
                }
            }
            
            return result.ToArray();
        }

        public static FlowLayoutGraphNode[] FindNodesWithItemType(FlowLayoutGraph graph, FlowGraphItemType itemType)
        {
            var result = new List<FlowLayoutGraphNode>();
            foreach (var node in graph.Nodes)
            {
                foreach (var item in node.items)
                {
                    if (item.type == itemType)
                    {
                        result.Add(node);
                        break;
                    }
                }
            }

            return result.ToArray();
        }

        public static FlowLayoutGraphNode FindNodeWithItemType(FlowLayoutGraph graph, FlowGraphItemType itemType)
        {
            foreach (var node in graph.Nodes)
            {
                foreach (var item in node.items)
                {
                    if (item.type == itemType)
                    {
                        return node;
                    }
                }
            }

            return null;
        }

        public static bool ContainsItem(List<FlowItem> items, FlowGraphItemType itemType)
        {
            foreach (var item in items)
            {
                if (item.type == itemType)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool CanReachNode(FlowLayoutGraphQuery graphQuery, DungeonUID startNode, DungeonUID endNode, bool ignoreDirection,
                bool ignoreOneWayDoors, bool traverseTeleporters, Func<FlowLayoutGraphTraversal.FNodeInfo, bool> canTraverse)
        {
            var traversal = graphQuery.Traversal;
            var visitedNodes = new HashSet<DungeonUID>();
            var stack = new Stack<DungeonUID>();
            stack.Push(startNode);
            while (stack.Count > 0) {
                var nodeId = stack.Pop();
                if (nodeId == endNode) {
                    return true;
                }

                visitedNodes.Add(nodeId);

                // Grab the connected nodes
                var connectedNodes = new List<FlowLayoutGraphTraversal.FNodeInfo>();
                connectedNodes.AddRange(traversal.GetOutgoingNodes(nodeId));
                if (ignoreDirection) {
                    connectedNodes.AddRange(traversal.GetIncomingNodes(nodeId));
                }
                
                if (traverseTeleporters) {
                    // TODO: Implement me
                }

                // Traverse through them
                foreach (var connectedNode in connectedNodes) {
                    if (!canTraverse(connectedNode)) continue;
                    if (visitedNodes.Contains(connectedNode.NodeId)) continue;
                    if (!ignoreOneWayDoors) {
                        var link = graphQuery.GetLink(connectedNode.LinkId);
                        if (link != null && link.state.type == FlowLayoutGraphLinkType.OneWay) {
                            // Make sure we can pass through the one-way door
                            if (link.source != nodeId) {
                                // Cannot pass through the one-way link
                                continue;
                            }
                        }
                    }

                    stack.Push(connectedNode.NodeId);
                }
            }

            return false;
        }
        
        protected struct NodeWeightAssignInfo
        {
            public FlowLayoutGraphNode node;
            public int weight;

            public NodeWeightAssignInfo(FlowLayoutGraphNode node, int weight)
            {
                this.node = node;
                this.weight = weight;
            }
        }

        public static Dictionary<FlowLayoutGraphNode, int> CalculateWeights(FlowLayoutGraph graph, int lockedWeight)
        {
            var weights = new Dictionary<FlowLayoutGraphNode, int>();

            // Find the start node
            FlowLayoutGraphNode[] startNodes = FindNodesWithItemType(graph, FlowGraphItemType.Entrance);
            var visited = new HashSet<FlowLayoutGraphNode>();
            var queue = new Queue<NodeWeightAssignInfo>();
            foreach (var startNode in startNodes)
            {
                queue.Enqueue(new NodeWeightAssignInfo(startNode, 0));
                visited.Add(startNode);
            }

            while (queue.Count > 0)
            {
                var front = queue.Dequeue();
                visited.Add(front.node);
                if (weights.ContainsKey(front.node))
                {
                    weights[front.node] = Mathf.Min(weights[front.node], front.weight);
                }
                else
                {
                    weights.Add(front.node, front.weight);
                }

                // Traverse the children
                foreach (var outgoingLink in graph.GetOutgoingLinks(front.node))
                {
                    if (outgoingLink.state.type == FlowLayoutGraphLinkType.Unconnected) continue;
                    var outgoingNode = graph.GetNode(outgoingLink.destination);
                    if (!outgoingNode.active) continue;
                    bool traverseChild = true;
                    if (visited.Contains(outgoingNode))
                    {
                        // The child node has already been traversed.  Do not traverse if the child's weight
                        // is less than the current weight
                        var currentWeight = front.weight;
                        var childWeight = weights[outgoingNode];
                        if (currentWeight > childWeight)
                        {
                            traverseChild = false;
                        }
                    }
                    if (traverseChild)
                    {
                        var nodeWeight = 1;
                        if (ContainsItem(outgoingLink.state.items, FlowGraphItemType.Lock))
                        {
                            nodeWeight = lockedWeight;
                        }

                        queue.Enqueue(new NodeWeightAssignInfo(outgoingNode, front.weight + nodeWeight));
                    }
                }
            }
            return weights;
        }
        
        public static FlowLayoutGraphNode[] FilterNodes(FlowLayoutGraphNode[] nodes, int minWeight, int maxWeight, Dictionary<FlowLayoutGraphNode, int> weights)
        {
            var validNodes = new List<FlowLayoutGraphNode>();
            foreach (var node in nodes)
            {
                var weight = weights[node];
                if (weight >= minWeight && weight <= maxWeight)
                {
                    validNodes.Add(node);
                }
            }
            return validNodes.ToArray();
        }

    }
}