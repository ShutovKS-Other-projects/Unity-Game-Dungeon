//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.RoadNetworks
{
    public delegate void RoadBlockLayoutBuiltDelegate(ref Vector3[] layout);

    public class RoadLayoutBuilder
    {
        public event RoadBlockLayoutBuiltDelegate RoadBlockLayoutBuilt;

        Dictionary<int, RoadGraphNode> graphNodes = new Dictionary<int, RoadGraphNode>();
        MeshFilter meshFilter;
        public RoadLayoutBuilder(RoadGraph roadGraph, MeshFilter meshFilter)
        {
            this.meshFilter = meshFilter;
            meshFilter.mesh = new Mesh();
            
            foreach (var node in roadGraph.nodes)
            {
                graphNodes.Add(node.nodeId, node);
            }
        }

        void GenerateBoundaryMesh(RoadGraphEdge[] edges, Vector3[] boundaryPoints, List<Vector3> vertices, List<Vector2> uv)
        {
            for (int i = 0; i < edges.Length; i++)
            {
                var nextIndex = (i + 1) % edges.Length;
                var edge0 = edges[i];
                var edge1 = edges[nextIndex];

                var node0 = graphNodes[edge0.ownerNodeId];
                var node1 = graphNodes[edge1.ownerNodeId];

                var p0 = node0.position;
                var p1 = node1.position;

                var b0 = boundaryPoints[i];
                var b1 = boundaryPoints[nextIndex];

                vertices.Add(p0);
                vertices.Add(p1);
                vertices.Add(b1);

                vertices.Add(p0);
                vertices.Add(b1);
                vertices.Add(b0);

                var uvP0 = new Vector2(0, 0);
                var uvP1 = new Vector2(0, 1);
                var uvB0 = new Vector2(1, 0);
                var uvB1 = new Vector2(1, 1);

                uv.Add(uvP0);
                uv.Add(uvP1);
                uv.Add(uvB1);

                uv.Add(uvP0);
                uv.Add(uvB1);
                uv.Add(uvB0);
            }
        }
        Vector3[] GenerateBlockBoundary(RoadGraphEdge[] edges)
        {
            if (edges.Length == 0)
            {
                return new Vector3[0];
            }

            var boundary = new List<Vector3>();
            for (int i = 0; i < edges.Length; i++)
            {
                int previousIndex = (i == 0 ? edges.Length - 1 : i - 1);
                var edge = edges[i];
                var previousEdge = edges[previousIndex];

                var node0 = graphNodes[previousEdge.ownerNodeId];
                var node1 = graphNodes[edge.ownerNodeId];
                var node2 = graphNodes[edge.otherNodeId];

                if (previousEdge.otherNodeId != edge.ownerNodeId)
                {
                    Debug.Log("invalid node edge config");
                }
                var dir0 = (node0.position - node1.position).normalized;
                var dir1 = (node1.position - node2.position).normalized;

                var normalRotator = Quaternion.Euler(0, -90, 0);
                var normal0 = normalRotator * dir0;
                var normal1 = normalRotator * dir1;

                var thickness0 = previousEdge.thickness;
                var thickness1 = edge.thickness;

                float curvature = 1.0f;
                {
                    float cos = Vector3.Dot(normal0, normal1);
                    curvature = 1.0f / (1 + cos);
                }
                
                var owningNode = graphNodes[edge.ownerNodeId];
                var offset = (normal0 * thickness0 + normal1 * thickness1);
                var position = owningNode.position + offset * curvature;
                
                boundary.Add(position);
            }

            // Apply offset to the boundary
            return boundary.ToArray();
        }

        public RoadGraph BakeLayoutGraph()
        {
            var vertices = new List<Vector3>();
            var uvs = new List<Vector2>();

            var graphBuilder = new RoadGraphBuilder();
            var edgeVisited = new HashSet<int>();
            foreach (var node in graphNodes.Values)
            {
                foreach (var edge in node.adjacentEdges)
                {
                    if (!edgeVisited.Contains(edge.edgeId))
                    {
                        var blockEdges = TraverseEdgeBlock(edge, edgeVisited);
                        var boundary = GenerateBlockBoundary(blockEdges);

                        if (RoadBlockLayoutBuilt != null)
                        {
                            RoadBlockLayoutBuilt(ref boundary);
                        }

                        GenerateBoundaryMesh(blockEdges, boundary, vertices, uvs);

                        var boundaryBuildNodes = new List<RoadGraphBuildNode>();
                        foreach (var point in boundary)
                        {
                            var buildNode = graphBuilder.CreateNode(point);
                            boundaryBuildNodes.Add(buildNode);
                        }

                        // Connect the build nodes
                        for (int i = 0; i < boundaryBuildNodes.Count; i++)
                        {
                            RoadGraphBuildNode start = boundaryBuildNodes[i];
                            RoadGraphBuildNode end = boundaryBuildNodes[(i + 1) % boundaryBuildNodes.Count];
                            graphBuilder.ConnectNodes(start, end, 0);
                        }
                    }
                }

            }

            //var vertexMap = new Dictionary<Vector3, int>();
            //var vertexBuffer = new List<Vector3>();
            //var uvBuffer = new List<Vector2>();
            var indexBuffer = new List<int>();

            // Populate the vertex buffer
            for (int i = 0; i < vertices.Count; i++)
            {
                indexBuffer.Add(i);
            }
            
            var mesh = meshFilter.sharedMesh;
            mesh.vertices = vertices.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.triangles = indexBuffer.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return graphBuilder.BakeRoadGraph();
        }

        RoadGraphEdge[] TraverseEdgeBlock(RoadGraphEdge startEdge, HashSet<int> edgeVisited)
        {
            var edges = new List<RoadGraphEdge>();
            RoadGraphEdge edge = startEdge;
            while (edge != null && !edgeVisited.Contains(edge.edgeId))
            {
                edges.Add(edge);
                edgeVisited.Add(edge.edgeId);

                // Move to the next edge
                var owningNode = graphNodes[edge.ownerNodeId];
                var otherNode = graphNodes[edge.otherNodeId];

                // Find the opposite edge
                RoadGraphEdge nextEdge = null;
                int numOtherNodeEdges = otherNode.adjacentEdges.Length;
                for (int i = 0; i < numOtherNodeEdges; i++) {
                    var otherNodeEdge = otherNode.adjacentEdges[i];
                    if (otherNodeEdge.otherNodeId == owningNode.nodeId)
                    {
                        // This is the opposite edge.  Grab the adjacent one
                        nextEdge = otherNode.adjacentEdges[(i + 1) % numOtherNodeEdges];
                    }
                }

                edge = nextEdge;
            }

            return edges.ToArray();
        }


    }
}
