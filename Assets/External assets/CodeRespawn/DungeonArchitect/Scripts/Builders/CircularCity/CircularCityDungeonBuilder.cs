//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;
using DungeonArchitect.RoadNetworks;
using DungeonArchitect.Splatmap;

namespace DungeonArchitect.Builders.CircularCity
{
    public static class CircularCityDungeonMarkerNames
    {
        public static readonly string House = "House";
        public static readonly string WallMarkerName = "CityWall";
        public static readonly string DoorMarkerName = "CityDoor";
        public static readonly string GroundMarkerName = "CityGround";
        public static readonly string CornerTowerMarkerName = "CornerTower";
        public static readonly string WallPaddingMarkerName = "CityWallPadding";
    }

    public class CircularCityDungeonBuilder : DungeonBuilder
    {
        CircularCityDungeonConfig cityConfig;
        CircularCityDungeonModel cityModel;

        new System.Random random;
        /// <summary>
        /// Builds the dungeon layout.  In this method, you should build your dungeon layout and save it in your model file
        /// No markers should be emitted here.   (EmitMarkers function will be called later by the engine to do that)
        /// </summary>
        /// <param name="config">The builder configuration</param>
        /// <param name="model">The dungeon model that the builder will populate</param>
        public override void BuildDungeon(DungeonConfig config, DungeonModel model)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            base.BuildDungeon(config, model);

            random = new System.Random((int)config.Seed);

            // We know that the dungeon prefab would have the appropriate config and models attached to it
            // Cast and save it for future reference
            cityConfig = config as CircularCityDungeonConfig;
            cityModel = model as CircularCityDungeonModel;
            cityModel.Config = cityConfig;

            // Generate the city layout and save it in a model.   No markers are emitted here. 
            GenerateCityLayout();

            sw.Stop();
            Debug.Log("Time elapsed: " + (sw.ElapsedMilliseconds / 1000.0f) + " s");
        }
        
        public override void OnDestroyed()
        {
            base.OnDestroyed();

            cityModel = model as CircularCityDungeonModel;
            if (cityModel.roadGraph != null)
            {
                //cityModel.roadGraph.nodes = new RoadGraphNode[0];
            }
        }
        

        /// <summary>
        /// Override the builder's emit marker function to emit our own markers based on the layout that we built
        /// You should emit your markers based on the layout you have saved in the model generated previously
        /// When the user is designing the theme interactively, this function will be called whenever the graph state changes,
        /// so the theme engine can populate the scene (BuildDungeon will not be called if there is no need to rebuild the layout again)
        /// </summary>
        public override void EmitMarkers()
        {
            base.EmitMarkers();
            EmitCityMarkers();
            ProcessMarkerOverrideVolumes();
        }

        delegate void InsertHouseDelegate();

        /// <summary>
        /// Grabs the texture stored in the splat asset.  The index is based on the texture info array you define in the splat component
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        Texture2D GetSplatTexture(int index)
        {
            var splatComponent = GetComponent<DungeonSplatmap>();
            if (splatComponent == null) return null;
            if (splatComponent.splatmap == null) return null;

            var splatmap = splatComponent.splatmap;
            if (index >= splatmap.splatTextures.Length) return null;

            return splatmap.splatTextures[index];
        }

        Texture2D GetRoadmap()
        {
            return GetSplatTexture(0);  
        }
        
        /// <summary>
        /// Generate a layout and save it in the model
        /// </summary>
        void GenerateCityLayout()
        {
            var roadGraphSettings = new RoadGraphBuilderSettings();
            roadGraphSettings.interNodeDistance = cityConfig.interNodeDistance;
            var roadGraphBuilder = new RoadGraphBuilder(roadGraphSettings);

            float startRadius = cityConfig.startRadius;
            float endRadius = cityConfig.endRadius;
            var center = Vector3.zero;

            float mainRoadStrength = cityConfig.mainRoadStrength;
            float sideRoadStrength = cityConfig.sideRoadStrength;
            float interRingDistance = (endRadius - startRadius) / (cityConfig.numRings - 1);

            // Draw the city rings
            {
                float ringRadius = startRadius;
                for (int r = 0; r < cityConfig.numRings; r++)
                {
                    roadGraphBuilder.CreateCircle(center, ringRadius, mainRoadStrength);
                    if (r < cityConfig.numRings - 1)
                    {
                        float ringLaneRadius = ringRadius + interRingDistance / 2.0f;
                        roadGraphBuilder.CreateCircle(center, ringLaneRadius, sideRoadStrength);
                    }
                    ringRadius += interRingDistance;
                }
            }

            // Draw the ray lanes ejecting out from the center
            float interRayAngle = Mathf.PI * 2 / cityConfig.numRays;
            for (int i = 0; i < cityConfig.numRays; i++)
            {
                float ringRadius = startRadius;
                int mainSegmentToRemove = random.Next() % (cityConfig.numRings - 1);
                for (int r = 0; r < cityConfig.numRings - 1; r++)
                {
                    float nextRingRadius = ringRadius + interRingDistance;

                    bool bDrawMainSegment = (r != mainSegmentToRemove);

                    if (bDrawMainSegment)
                    {
                        float angle = i * interRayAngle;
                        var direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
                        float midRingRadius = ringRadius + (nextRingRadius - ringRadius) / 2.0f;

                        var start = center + direction * ringRadius;
                        var end = center + direction * midRingRadius;
                        roadGraphBuilder.CreateLine(start, end, mainRoadStrength);

                        start = center + direction * midRingRadius;
                        end = center + direction * nextRingRadius;
                        roadGraphBuilder.CreateLine(start, end, mainRoadStrength);
                    }

                    // Draw the side segments
                    {
                        float interSideRayAngle = interRayAngle / (r + 2);
                        float sideRayAngle = i * interRayAngle;
                        float interSideRingDistance = interRingDistance / 2.0f;
                        for (int si = 0; si <= r; si++)
                        {
                            sideRayAngle += interSideRayAngle;
                            for (int t = 0; t < 2; t++)
                            {
                                bool bRemoveSideSegment = (random.NextFloat() < cityConfig.sideRoadRemovalProbability);
                                if (!bRemoveSideSegment)
                                {
                                    float perimeter = Mathf.PI * 2 * ringRadius;
                                    float angleRandomization = cityConfig.randomSideLaneOffsetAngle * Mathf.Deg2Rad / perimeter;
                                    float randomValue = random.NextFloat() * 2 - 1;
                                    float randomizedAngle = sideRayAngle + angleRandomization * randomValue;
                                    float laneStartRadius = ringRadius + interSideRingDistance * t;
                                    float laneEndRadius = ringRadius + interSideRingDistance * (t + 1);

                                    var direction = new Vector3(Mathf.Cos(randomizedAngle), 0, Mathf.Sin(randomizedAngle));
                                    var start = center + direction * laneStartRadius;
                                    var end = center + direction * laneEndRadius;
                                    roadGraphBuilder.CreateLine(start, end, sideRoadStrength);
                                }
                            }
                        }
                    }

                    ringRadius = nextRingRadius;
                }
            }

            cityModel.roadGraph = roadGraphBuilder.BakeRoadGraph();

            var layoutBuilder = new RoadLayoutBuilder(cityModel.roadGraph, cityConfig.roadMesh);
            layoutBuilder.RoadBlockLayoutBuilt += LayoutBuilder_RoadBlockLayoutBuilt;

            cityModel.layoutGraph = layoutBuilder.BakeLayoutGraph();
        }

        private void LayoutBuilder_RoadBlockLayoutBuilt(ref Vector3[] layout)
        {
            
        }

        void DebugDrawGraphGizmo(RoadGraph graph, Color edgeColor, Color nodeColor)
        {
            if (graph == null || graph.nodes == null)
            {
                return;
            }

            // Create a hash table of the nodes for fast lookup
            var nodes = new Dictionary<int, RoadGraphNode>();
            foreach (var node in graph.nodes)
            {
                nodes.Add(node.nodeId, node);
            }

            // Draw the edges
            Gizmos.color = edgeColor;
            foreach (var node in graph.nodes)
            {
                foreach (var edge in node.adjacentEdges)
                {
                    var otherNode = nodes[edge.otherNodeId];
                    Gizmos.DrawLine(node.position, otherNode.position);
                }
            }

            // Draw the nodes
            Gizmos.color = nodeColor;
            foreach (var node in graph.nodes)
            {
                Gizmos.DrawSphere(node.position, .25f);
            }
        }

        public override void DebugDrawGizmos()
        {
            if (cityConfig == null)
            {
                cityConfig = GetComponent<CircularCityDungeonConfig>();
                if (cityConfig == null) return;
            }

            if (model == null)
            {
                model = GetComponent<CircularCityDungeonModel>();
            }

            if (cityModel == null) {
                cityModel = model as CircularCityDungeonModel;
            }

            if (cityModel == null)
            {
                return;
            }

            DebugDrawGraphGizmo(cityModel.roadGraph, Color.green, Color.red);
            DebugDrawGraphGizmo(cityModel.layoutGraph, Color.yellow, Color.blue);
        }

        class SpatialPartitionCache
        {
            private int gridSize;
            Dictionary<IntVector2, List<Vector3>> occupancyGrid = new Dictionary<IntVector2, List<Vector3>>();

            public SpatialPartitionCache(int gridSize)
            {
                this.gridSize = gridSize;
            }

            public void RegisterAsOccupied(Vector3 position)
            {
                var cell = GetCell(position);
                if (!occupancyGrid.ContainsKey(cell))
                {
                    occupancyGrid.Add(cell, new List<Vector3>());
                }
                occupancyGrid[cell].Add(position);
            }

            public bool IsFree(Vector3 position, float distanceSearch)
            {
                var baseCell = GetCell(position);
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        var cell = baseCell + new IntVector2(dx, dy);
                        if (!IsFree(position, cell, distanceSearch))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

            bool IsFree(Vector3 sourcePosition, IntVector2 cell, float distanceSearch)
            {
                if (!occupancyGrid.ContainsKey(cell))
                {
                    return true;
                }

                float distanceSearchSq = distanceSearch * distanceSearch;
                foreach (var position in occupancyGrid[cell])
                {
                    var distanceSq = (sourcePosition - position).sqrMagnitude;
                    if (distanceSq < distanceSearchSq)
                    {
                        return false;
                    }
                }
                return true;
            }

            IntVector2 GetCell(Vector3 position)
            {
                var cell = new IntVector2();
                cell.x = Mathf.RoundToInt(position.x / gridSize);
                cell.y = Mathf.RoundToInt(position.z / gridSize);
                return cell;
            }

        }

        /// <summary>
        /// Emit marker points so that the theme can decorate the scene layout that we just built
        /// </summary>
        void EmitCityMarkers()
        {
            var basePosition = transform.position;

            float occupancySearchDistance = cityConfig.buildingSize * 1.0f;
            int cacheGridResolution = Mathf.RoundToInt(occupancySearchDistance * 2.0f);
            var spatialOccupancy = new SpatialPartitionCache(cacheGridResolution);
            foreach (var node in cityModel.roadGraph.nodes)
            {
                spatialOccupancy.RegisterAsOccupied(node.position);
            }
            foreach (var node in cityModel.layoutGraph.nodes)
            {
                spatialOccupancy.RegisterAsOccupied(node.position);
            }

            for (float r = cityConfig.startRadius; r < cityConfig.endRadius; r += cityConfig.buildingSize)
            {
                float circumference = 2 * Mathf.PI * r;
                int numBuildings = Mathf.RoundToInt(circumference / cityConfig.buildingSize);
                for (int i = 0; i < numBuildings; i++)
                {
                    float angle = 2 * Mathf.PI * i / (float)numBuildings;
                    var offset = Vector3.zero;
                    offset.x = Mathf.Cos(angle) * r;
                    offset.z = Mathf.Sin(angle) * r;
                    var position = basePosition + offset;
                    if (!spatialOccupancy.IsFree(offset, occupancySearchDistance))
                    {
                        continue;
                    }
                    var rotation = Quaternion.Euler(0, -angle * Mathf.Rad2Deg, 0);
                    EmitMarkerAt(CircularCityDungeonMarkerNames.House, position, rotation);
                }
            }

        }
                

        void EmitMarkerAt(string markerName, Vector3 worldPosition, Quaternion rotation)
        {
            var transformation = Matrix4x4.TRS(worldPosition, rotation, Vector3.one);
            var gridPosition = IntVector.Zero;
            EmitMarker(markerName, transformation, gridPosition, -1);
        }
        
    }
}