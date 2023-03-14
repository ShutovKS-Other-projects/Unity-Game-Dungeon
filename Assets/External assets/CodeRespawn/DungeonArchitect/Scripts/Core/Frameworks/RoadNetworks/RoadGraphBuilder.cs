//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.RoadNetworks
{
    public class RoadGraphBuilderSettings
    {
        public float interNodeDistance = 10;
    }


    public class RoadGraphBuilder
    {
        public RoadGraphBuilder()
        {
            this.settings = new RoadGraphBuilderSettings();
        }

        public RoadGraphBuilder(RoadGraphBuilderSettings settings)
        {
            this.settings = settings;
        }

        public void Initialize(RoadGraph graph)
        {
            nodes.Clear();

            // Create the node instances
            foreach (var nodeInfo in graph.nodes)
            {
                var node = new RoadGraphBuildNode(nodeInfo);
                nodes.Add(node.nodeId, node);
                _nodeIdCounter = Mathf.Max(_nodeIdCounter, node.nodeId);
            }

            // Generate the node adjacency list
            foreach (var nodeInfo in graph.nodes)
            {
                var node = nodes[nodeInfo.nodeId];
                foreach (var edge in nodeInfo.adjacentEdges)
                {
                    var adjacentNode = nodes[edge.otherNodeId];
                    node.connectedNodes.Add(adjacentNode);
                }
            }
        }

        RoadGraphBuildNode FindNearestNode(Vector3 position, float searchRadius)
        {
            // TODO: Optimize me with a spatial grid
            RoadGraphBuildNode bestMatch = null;
            float bestDistanceSq = float.MaxValue;
            foreach (var node in nodes.Values)
            {
                var distanceSq = (node.position - position).sqrMagnitude;
                if (distanceSq < bestDistanceSq)
                {
                    bestMatch = node;
                    bestDistanceSq = distanceSq;
                }
            }

            return bestMatch;
        }

        public void CreateLine(Vector3 start, Vector3 end, float thickness)
        {
            float searchDistance = settings.interNodeDistance * 2;
            var startNode = FindNearestNode(start, searchDistance);
            var endNode = FindNearestNode(end, searchDistance);

            if (startNode == null)
            {
                startNode = CreateNode(start);
            }
            if (endNode == null)
            {
                endNode = CreateNode(end);
            }

            var lineLength = (endNode.position - startNode.position).magnitude;
            int numSegments = Mathf.RoundToInt(lineLength / settings.interNodeDistance);
            numSegments = Mathf.Max(numSegments, 1);
            var segmentLength = lineLength / numSegments;

            // we want the start / end positions to match the node positions
            startNode.position = start;
            endNode.position = end;

            var direction = (end - start).normalized;
            var previousNode = startNode;
            for (int i = 1; i < numSegments; i++)
            {
                var position = start + direction * segmentLength * i;
                var segmentNode = CreateNode(position);
                ConnectNodes(previousNode, segmentNode, thickness);
                previousNode = segmentNode;
            }
            // Connect the last node
            ConnectNodes(previousNode, endNode, thickness);
        }

        public void CreateCircle(Vector3 center, float radius, float thickness)
        {
            float circumference = 2 * Mathf.PI * radius;
            if (settings.interNodeDistance <= 0)
            {
                settings.interNodeDistance = 10;
            }
            int numSegments = Mathf.RoundToInt(circumference / settings.interNodeDistance);

            RoadGraphBuildNode firstNode = null;
            RoadGraphBuildNode previousNode = null;
            for (int i = 0; i < numSegments; i++)
            {
                float angle = i / (float)numSegments * (2 * Mathf.PI);
                var offset = Vector3.zero;
                offset.x = Mathf.Cos(angle) * radius;
                offset.z = Mathf.Sin(angle) * radius;
                var position = center + offset;
                var node = CreateNode(position);
                if (i == 0)
                {
                    firstNode = node;
                }
                else
                {
                    ConnectNodes(previousNode, node, thickness);
                }
                previousNode = node;
            }

            // Connect the last node with the first node
            ConnectNodes(previousNode, firstNode, thickness);
        }

        public RoadGraph BakeRoadGraph()
        {
            return RoadGraphBuilderUtils.BakeRoadGraph(nodes.Values);
        }

        public RoadGraphBuildNode CreateNode(Vector3 position)
        {
            int nodeId = ++_nodeIdCounter;
            var node = new RoadGraphBuildNode(nodeId, position);
            nodes.Add(node.nodeId, node);
            return node;
        }

        public void ConnectNodes(RoadGraphBuildNode a, RoadGraphBuildNode b, float thickness)
        {
            if (!a.connectedNodes.Contains(b))
            {
                a.connectedNodes.Add(b);
                a.edgeThickness.Add(thickness);
            }

            if (!b.connectedNodes.Contains(a))
            {
                b.connectedNodes.Add(a);
                b.edgeThickness.Add(thickness);
            }
        }

        RoadGraphBuilderSettings settings;
        Dictionary<int, RoadGraphBuildNode> nodes = new Dictionary<int, RoadGraphBuildNode>();
        int _nodeIdCounter = 0;
    }


    // This node structure contains more data for easier and faster graph traversal, as opposed to the serializable RoadGraphNode structure
    public class RoadGraphBuildNode
    {
        public RoadGraphBuildNode(int nodeId, Vector3 position)
        {
            this.nodeId = nodeId;
            this.position = position;
        }

        public RoadGraphBuildNode(RoadGraphNode graphNode)
        {
            nodeId = graphNode.nodeId;
            position = graphNode.position;
        }


        public int nodeId;
        public Vector3 position;
        public List<RoadGraphBuildNode> connectedNodes = new List<RoadGraphBuildNode>();
        public List<float> edgeThickness = new List<float>();
    }



    class RoadGraphBuilderUtils
    {
        public static RoadGraph BakeRoadGraph(IEnumerable<RoadGraphBuildNode> buildNodes)
        {
            var bakedNodes = new List<RoadGraphNode>();
            int _edgeIdCounter = 0;

            foreach (var node in buildNodes)
            {
                var bakedNode = new RoadGraphNode();
                bakedNode.nodeId = node.nodeId;
                bakedNode.position = node.position;

                var edges = new List<RoadGraphEdge>();
                for (int i = 0; i < node.connectedNodes.Count; i++)
                {
                    var connectedNode = node.connectedNodes[i];
                    var edgeThickness = node.edgeThickness[i];
                    var edge = new RoadGraphEdge();
                    edge.edgeId = ++_edgeIdCounter;
                    edge.ownerNodeId = node.nodeId;
                    edge.otherNodeId = connectedNode.nodeId;
                    edge.thickness = edgeThickness;

                    // Find the dot product with the x axis
                    Vector3 direction = (connectedNode.position - node.position).normalized;
                    var dot = Vector3.Dot(new Vector3(1, 0, 0), direction);
                    float angle = Mathf.Acos(dot);
                    if (direction.z < 0)
                    {
                        angle = 2 * Mathf.PI - angle;
                    }

                    edge.angleToXAxis = angle;

                    edges.Add(edge);
                }

                bakedNode.adjacentEdges = edges.ToArray();
                System.Array.Sort(bakedNode.adjacentEdges, SortEdgesByAngle);
                bakedNodes.Add(bakedNode);
            }

            var graph = new RoadGraph();
            graph.nodes = bakedNodes.ToArray();
            return graph;
        }

        private static int SortEdgesByAngle(RoadGraphEdge a, RoadGraphEdge b)
        {
            if (a.angleToXAxis == b.angleToXAxis) return 0;
            return a.angleToXAxis < b.angleToXAxis ? -1 : 1;
        }

    }

}
