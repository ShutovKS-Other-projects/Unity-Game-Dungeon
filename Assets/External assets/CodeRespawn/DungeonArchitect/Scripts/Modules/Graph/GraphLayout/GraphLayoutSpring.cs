//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Graphs.Layouts.Spring
{
    [System.Serializable]
    public class GraphLayoutSpringConfig
    {
        [SerializeField]
        public float interNodeDistance = 120;

        [SerializeField]
        public float interNodeTension = 0.5f;

        [SerializeField]
        public float springDistance = 30;

        [SerializeField]
        public float springTension = 0.1f;

        [SerializeField]
        public int iterations = 200;

        [SerializeField]
        public float timeStep = 1.0f;
    }

    class SpatialGrid<T>
    {
        Dictionary<IntVector2, List<GraphLayoutNode<T>>> grid = new Dictionary<IntVector2, List<GraphLayoutNode<T>>>();
        float cellSize;
        public SpatialGrid(float cellSize)
        {
            this.cellSize = cellSize;
        }

        public void Refresh(GraphLayoutNode<T>[] nodes)
        {
            grid.Clear();
            foreach (var node in nodes)
            {
                var key = GetKey(node);
                if (!grid.ContainsKey(key))
                {
                    grid.Add(key, new List<GraphLayoutNode<T>>());
                }
                grid[key].Add(node);
            }
        }

        IntVector2 GetKey(GraphLayoutNode<T> node)
        {
            int sx = Mathf.FloorToInt(node.Position.x / cellSize);
            int sy = Mathf.FloorToInt(node.Position.y / cellSize);
            return new IntVector2(sx, sy);
        }

        public GraphLayoutNode<T>[] GetNearbyNodes(GraphLayoutNode<T> node)
        {
            var key = GetKey(node);
            var nearbyNodes = new List<GraphLayoutNode<T>>();
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    var nkey = key + new IntVector2(dx, dy);
                    if (grid.ContainsKey(nkey))
                    {
                        nearbyNodes.AddRange(grid[nkey]);
                    }
                }
            }

            nearbyNodes.Remove(node);
            return nearbyNodes.ToArray();
        }
    }

    public class GraphLayoutSpring<T> : GraphLayoutBase<T>
    {
        GraphLayoutSpringConfig config;
        public GraphLayoutSpring(GraphLayoutSpringConfig config)
        {
            this.config = config;
        }

        protected override void LayoutImpl(GraphLayoutNode<T>[] nodes)
        {
            var random = new System.Random(0);
            int initialScatterRadius = 30;
            foreach (var node in nodes)
            {
                float angle = random.NextFloat() * Mathf.PI * 2;
                float distance = random.NextFloat();
                distance = 1 - distance * distance * distance;
                node.Position = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * initialScatterRadius * distance;
            }

            var spatialGrid = new SpatialGrid<T>(config.interNodeDistance);

            // separate
            for (int i = 0; i < config.iterations; i++)
            {
                spatialGrid.Refresh(nodes);
                var nearbyNodes = new Dictionary<GraphLayoutNode<T>, GraphLayoutNode<T>[]>();
                foreach (var node in nodes)
                {
                    nearbyNodes.Add(node, spatialGrid.GetNearbyNodes(node));
                }

                float interNodeDistanceSq = config.interNodeDistance * config.interNodeDistance;
                foreach (var node in nodes)
                {
                    foreach (var nearbyNode in nearbyNodes[node])
                    {
                        // Separate by pushing them away
                        var distanceSq = (nearbyNode.Position - node.Position).sqrMagnitude;
                        if (distanceSq < interNodeDistanceSq)
                        {
                            // Needs to be pushed away
                            float distance = Mathf.Sqrt(distanceSq);
                            var direction = (nearbyNode.Position - node.Position) / distance;
                            var pushVector = direction * distance * config.interNodeTension * config.timeStep;
                            nearbyNode.Position += pushVector;
                            node.Position -= pushVector;
                        }
                    }
                }

                // Apply spring tension
                foreach (var node in nodes)
                {
                    foreach (var outgoingNode in node.Outgoing)
                    {
                        var distance = (outgoingNode.Position - node.Position).magnitude;
                        var direction = (outgoingNode.Position - node.Position) / distance;

                        float pushDistance = config.springDistance - distance;
                        var pushThisFrame = direction * pushDistance * config.springTension * config.timeStep;
                        outgoingNode.Position += pushThisFrame;
                        node.Position -= pushThisFrame;
                    }
                }
            }
        }
    }
}
