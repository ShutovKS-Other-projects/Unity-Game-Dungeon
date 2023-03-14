//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;
using DungeonArchitect.Utils;
using DungeonArchitect.Graphs;
using DungeonArchitect.SpatialConstraints;

namespace DungeonArchitect.Themeing
{
    using PropBySocketType_t = Dictionary<string, List<DungeonThemeItem>>;
    using PropBySocketTypeByTheme_t = Dictionary<DungeonThemeData, Dictionary<string, List<DungeonThemeItem>>>;

    public struct DungeonThemeExecutionContext
    {
        public DungeonBuilder builder;
        public DungeonConfig config;
        public DungeonModel model;
        public SpatialConstraintProcessor spatialConstraintProcessor;
        public ThemeOverrideVolume[] themeOverrideVolumes;
        public DungeonSceneProvider sceneProvider;
        public DungeonSceneObjectSpawner objectSpawner;
        public DungeonItemSpawnListener[] spawnListeners;
        public IDungeonSceneObjectInstantiator objectInstantiator;
    }

    public class DungeonThemeEngine
    {
        DungeonThemeExecutionContext context;
        PMRandom random;

        public DungeonThemeEngine(DungeonThemeExecutionContext context)
        {
            this.context = context;
            random = new PMRandom(context.config.Seed);
        }

        public void ApplyTheme(LevelMarkerList markers, List<DungeonThemeData> Themes)
        {
            var instanceCache = new InstanceCache();
            var constraintProcessor = context.spatialConstraintProcessor;

            if (random == null)
            {
                random = new PMRandom(context.config.Seed);
            }

            PropBySocketTypeByTheme_t PropBySocketTypeByTheme = new PropBySocketTypeByTheme_t();
            foreach (DungeonThemeData Theme in Themes)
            {
                CreatePropLookup(Theme, PropBySocketTypeByTheme);
            }

            // Collect all the theme override volumes and prepare their theme lookup
            var overrideVolumes = new List<ThemeOverrideVolume>();
            Dictionary<Graph, DungeonThemeData> GraphToThemeMapping = new Dictionary<Graph, DungeonThemeData>();

            // Process the theme override volumes
            var themeOverrides = context.themeOverrideVolumes;
            foreach (var volume in themeOverrides)
            {
                overrideVolumes.Add(volume);
                var graph = volume.overrideTheme;
                if (graph != null && !GraphToThemeMapping.ContainsKey(graph))
                {
                    DungeonThemeData theme = new DungeonThemeData();
                    theme.BuildFromGraph(volume.overrideTheme);
                    GraphToThemeMapping.Add(volume.overrideTheme, theme);

                    CreatePropLookup(theme, PropBySocketTypeByTheme);
                }
            }

            var srandom = new PMRandom(context.config.Seed);

            var nodesExecutionContext = new NodeListExecutionContext();
            nodesExecutionContext.instanceCache = instanceCache;
            nodesExecutionContext.constraintProcessor = constraintProcessor;
            nodesExecutionContext.srandom = srandom;
            nodesExecutionContext.SceneProvider = context.sceneProvider;
            nodesExecutionContext.objectInstantiator = context.objectInstantiator;

            var spawnDataList = new List<DungeonNodeSpawnData>();

            var delayedExecutionList = new Queue<NodeListExecutionData>();
            // Fill up the markers with the defined mesh data 
            for (int i = 0; i < markers.Count; i++)
            {
                PropSocket socket = markers[i];
                if (!socket.markForDeletion)
                {
                    DungeonThemeData themeToUse = GetBestMatchedTheme(Themes, socket, PropBySocketTypeByTheme); // PropAsset;
                    DungeonThemeData fallbackThemeToUse = themeToUse;

                    // Check if this socket resides within a override volume
                    {
                        var socketPosition = Matrix.GetTranslation(ref socket.Transform);
                        foreach (var volume in overrideVolumes)
                        {
                            if (volume.GetBounds().Contains(socketPosition))
                            {
                                var graph = volume.overrideTheme;
                                if (graph != null && GraphToThemeMapping.ContainsKey(graph))
                                {
                                    themeToUse = GraphToThemeMapping[volume.overrideTheme];
                                    if (!volume.useBaseThemeForMissingMarkers)
                                    {
                                        fallbackThemeToUse = themeToUse;
                                    }
                                    break;
                                }
                            }
                        }
                    }

                    if (themeToUse != null)
                    {
                        PropBySocketType_t PropBySocketType = PropBySocketTypeByTheme[themeToUse];
                        if (!PropBySocketType.ContainsKey(socket.SocketType) && fallbackThemeToUse != null && fallbackThemeToUse != themeToUse)
                        {
                            PropBySocketType = PropBySocketTypeByTheme[fallbackThemeToUse];
                        }

                        if (PropBySocketType.ContainsKey(socket.SocketType))
                        {
                            var data = new NodeListExecutionData();
                            data.socket = socket;
                            data.nodeDataList = PropBySocketType[socket.SocketType];

                            if (ShouldDelayExecution(data.nodeDataList))
                            {
                                delayedExecutionList.Enqueue(data);
                            }
                            else
                            {
                                ExecuteNodesUnderMarker(data, nodesExecutionContext, markers, spawnDataList);
                            }
                        }
                    }
                }

                // We execute the delayed node list (that have spatial constraints) at the very end of the list
                // Each execution of the delayed node however, can add more items to this list
                bool isLastIndex = (i == markers.Count - 1);
                while (isLastIndex && delayedExecutionList.Count > 0)
                {
                    var data = delayedExecutionList.Dequeue();
                    if (!data.socket.markForDeletion)
                    {
                        ExecuteNodesUnderMarker(data, nodesExecutionContext, markers, spawnDataList);
                    }

                    isLastIndex = (i == markers.Count - 1);
                }
            }

            RecursivelyTagMarkersForDeletion(markers);

            if (context.objectSpawner != null)
            {
                context.objectSpawner.Destroy();
            }

            
            context.objectSpawner.Spawn(spawnDataList.ToArray(), nodesExecutionContext.SceneProvider,
                    random, nodesExecutionContext.objectInstantiator, context.spawnListeners);
        }
        
        // Picks a theme from the list that has a definition for the defined socket
        DungeonThemeData GetBestMatchedTheme(List<DungeonThemeData> Themes, PropSocket socket, PropBySocketTypeByTheme_t PropBySocketTypeByTheme)
        {
            var ValidThemes = new List<DungeonThemeData>();
            foreach (DungeonThemeData Theme in Themes)
            {
                if (PropBySocketTypeByTheme.ContainsKey(Theme))
                {
                    PropBySocketType_t PropBySocketType = PropBySocketTypeByTheme[Theme];
                    if (PropBySocketType.Count > 0)
                    {
                        if (PropBySocketType.ContainsKey(socket.SocketType) && PropBySocketType[socket.SocketType].Count > 0)
                        {
                            ValidThemes.Add(Theme);
                        }
                    }
                }
            }
            if (ValidThemes.Count == 0)
            {
                return null;
            }

            int index = Mathf.FloorToInt(random.GetNextUniformFloat() * ValidThemes.Count) % ValidThemes.Count;
            return ValidThemes[index];
        }

        bool ProcessSpatialConstraint(SpatialConstraintProcessor constraintProcessor, SpatialConstraintAsset constraint, PropSocket marker, LevelMarkerList markers, out Matrix4x4 OutOffset, out PropSocket[] outMarkersToRemove)
        {
            if (constraintProcessor == null)
            {
                OutOffset = Matrix4x4.identity;
                outMarkersToRemove = new PropSocket[0];
                return false;
            }
            var spatialContext = new SpatialConstraintProcessorContext();
            spatialContext.constraintAsset = constraint;
            spatialContext.marker = marker;
            spatialContext.model = context.model;
            spatialContext.config = context.config;
            spatialContext.builder = context.builder;
            spatialContext.levelMarkers = markers;
            return constraintProcessor.ProcessSpatialConstraint(spatialContext, out OutOffset, out outMarkersToRemove);
        }


        void RecursivelyTagMarkerForDeletion(PropSocket marker, HashSet<int> visited)
        {
            visited.Add(marker.Id);
            marker.markForDeletion = true;
            foreach (var childMarker in marker.childMarkers)
            {
                if (!visited.Contains(childMarker.Id))
                {
                    RecursivelyTagMarkerForDeletion(childMarker, visited);
                }
            }
        }

        void RecursivelyTagMarkersForDeletion(LevelMarkerList markers)
        {
            var visited = new HashSet<int>();
            foreach (var marker in markers)
            {
                if (marker.markForDeletion && !visited.Contains(marker.Id))
                {
                    RecursivelyTagMarkerForDeletion(marker, visited);
                }
            }
        }

        // The data for executing all the nodes attached under a marker
        struct NodeListExecutionData
        {
            public List<DungeonThemeItem> nodeDataList;
            public PropSocket socket;
        }

        // The context required for executing all the nodes attached under a marker
        struct NodeListExecutionContext
        {
            public InstanceCache instanceCache;
            public SpatialConstraintProcessor constraintProcessor;
            public PMRandom srandom;
            public DungeonSceneProvider SceneProvider;
            public IDungeonSceneObjectInstantiator objectInstantiator;
        };
        
        bool ShouldDelayExecution(List<DungeonThemeItem> nodeDataList)
        {
            // If we use a spatial constraint, delay the execution
            foreach (DungeonThemeItem nodeData in nodeDataList)
            {
                if (nodeData.useSpatialConstraint && nodeData.spatialConstraint != null)
                {
                    return true;
                }
            }
            return false;
        }

        void CreatePropLookup(DungeonThemeData theme, PropBySocketTypeByTheme_t PropBySocketTypeByTheme)
        {
            if (theme == null || PropBySocketTypeByTheme.ContainsKey(theme))
            {
                // Lookup for this theme has already been built
                return;
            }

            PropBySocketType_t PropBySocketType = new PropBySocketType_t();
            PropBySocketTypeByTheme.Add(theme, PropBySocketType);

            foreach (DungeonThemeItem Prop in theme.Props)
            {
                if (!PropBySocketType.ContainsKey(Prop.AttachToSocket))
                {
                    PropBySocketType.Add(Prop.AttachToSocket, new List<DungeonThemeItem>());
                }
                PropBySocketType[Prop.AttachToSocket].Add(Prop);
            }
        }

        void ExecuteNodesUnderMarker(NodeListExecutionData data, NodeListExecutionContext nodeContext, LevelMarkerList markers, List<DungeonNodeSpawnData> spawnDataList)
        {
            var marker = data.socket;
            var nodeDataList = data.nodeDataList;
            foreach (DungeonThemeItem nodeData in nodeDataList)
            {
                bool insertMesh = false;
                Matrix4x4 transform = marker.Transform * nodeData.Offset;

                if (nodeData.UseSelectionRule && nodeData.SelectorRuleClassName != null)
                {
                    var selectorRule = nodeContext.instanceCache.GetInstance(nodeData.SelectorRuleClassName) as SelectorRule;
                    if (selectorRule != null)
                    {
                        // Run the selection rule logic to determine if we need to insert this mesh in the scene
                        insertMesh = selectorRule.CanSelect(marker, transform, context.model, random.UniformRandom);
                    }
                }
                else
                {
                    // Perform probability based selection logic
                    float probability = nodeContext.srandom.GetNextUniformFloat();
                    insertMesh = (probability < nodeData.Affinity);
                }

                if (insertMesh && nodeData.useSpatialConstraint && nodeData.spatialConstraint != null)
                {
                    Matrix4x4 spatialOffset;
                    PropSocket[] markersToRemove;
                    if (!ProcessSpatialConstraint(nodeContext.constraintProcessor, nodeData.spatialConstraint, marker, markers, out spatialOffset, out markersToRemove))
                    {
                        // Fails spatial constraint
                        insertMesh = false;
                    }
                    else
                    {
                        // Apply the offset
                        var markerOffset = marker.Transform;
                        if (nodeData.spatialConstraint != null && !nodeData.spatialConstraint.applyMarkerRotation)
                        {
                            var markerPosition = Matrix.GetTranslation(ref markerOffset);
                            var markerScale = Matrix.GetScale(ref markerOffset);
                            markerOffset = Matrix4x4.TRS(markerPosition, Quaternion.identity, markerScale);
                        }
                        transform = markerOffset * spatialOffset * nodeData.Offset;

                        foreach (var markerToRemove in markersToRemove)
                        {
                            markerToRemove.markForDeletion = true;
                        }
                    }
                }

                if (insertMesh)
                {
                    // Attach this prop to the socket
                    // Apply transformation logic, if specified
                    if (nodeData.UseTransformRule && nodeData.TransformRuleClassName != null && nodeData.TransformRuleClassName.Length > 0)
                    {
                        var transformer = nodeContext.instanceCache.GetInstance(nodeData.TransformRuleClassName) as TransformationRule;
                        if (transformer != null)
                        {
                            Vector3 _position, _scale;
                            Quaternion _rotation;
                            transformer.GetTransform(marker, context.model, transform, random.UniformRandom, out _position, out _rotation, out _scale);
                            var offset = Matrix4x4.TRS(_position, _rotation, _scale);
                            transform = transform * offset;
                        }
                    }

                    // Create a spawn request
                    var spawnData = new DungeonNodeSpawnData();
                    spawnData.nodeData = nodeData;
                    spawnData.transform = transform;
                    spawnData.socket = marker;
                    spawnDataList.Add(spawnData);

                    // Add child markers if any
                    foreach (PropChildSocketData ChildSocket in nodeData.ChildSockets)
                    {
                        Matrix4x4 childTransform = transform * ChildSocket.Offset;
                        var childMarker = markers.EmitMarker(ChildSocket.SocketType, childTransform, marker.gridPosition, marker.cellId);
                        data.socket.childMarkers.Add(childMarker);
                    }

                    if (nodeData.ConsumeOnAttach)
                    {
                        // Attach no more on this socket
                        break;
                    }
                }
            }
        }
    }
}
