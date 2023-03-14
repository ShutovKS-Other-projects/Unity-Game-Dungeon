//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using DungeonArchitect.Flow.Domains;
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Exec;
using DungeonArchitect.Flow.Impl.SnapGridFlow;
using DungeonArchitect.Flow.Impl.SnapGridFlow.Components;
using DungeonArchitect.Flow.Items;
using DungeonArchitect.Frameworks.Snap;
using DungeonArchitect.Themeing;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Builders.SnapGridFlow
{
    public class SnapGridFlowBuilder : DungeonBuilder
    {
        new System.Random random;
        
        public override bool IsThemingSupported()
        {
            return false;
        }

        public override bool DestroyDungeonOnRebuild()
        {
            return true;
        }
        
        public override void BuildNonThemedDungeon(DungeonSceneProvider sceneProvider, IDungeonSceneObjectInstantiator objectInstantiator)
        {
            base.BuildNonThemedDungeon(sceneProvider, objectInstantiator);
            
            
            random = new System.Random((int)config.Seed);
            markers.Clear();

            // We know that the dungeon prefab would have the appropriate config and models attached to it
            // Cast and save it for future reference
            var sgfConfig = config as SnapGridFlowConfig;
            var sgfModel = model as SnapGridFlowModel;

            if (sgfConfig == null)
            {
                Debug.LogError("No snap config script found in dungeon game object");
                return;
            }

            if (sgfModel == null)
            {
                Debug.LogError("No snap model script found in dungeon game object");
                return;
            }

            {
                string errorMessage = "";
                if (!sgfConfig.HasValidConfig(ref errorMessage))
                {
                    Debug.LogError(errorMessage);
                    return;
                }
            }

            SgfModuleNode[] snapModules = null;
            FlowLayoutGraph layoutGraph = null;
            var numRetriesLeft = sgfConfig.numGraphRetries;
            bool buildSuccess = false;
            while (!buildSuccess && numRetriesLeft > 0) {
                var domainExtensions = new FlowDomainExtensions();
                var snapDomainExtension = domainExtensions.GetExtension<SnapGridFlowDomainExtension>();
                snapDomainExtension.ModuleDatabase = sgfConfig.moduleDatabase;

                var execGraph = sgfConfig.flowGraph.execGraph;
                if (execGraph == null || execGraph.resultNode == null)
                {
                    Debug.LogError("Invalid flow exec graph");
                    return;
                }
                
                FlowExecutor executor = new FlowExecutor();
                FlowExecNodeOutputRegistry nodeOutputRegistry;
                
                //var sw = System.Diagnostics.Stopwatch.StartNew();
                if (!executor.Execute(execGraph, random, domainExtensions, numRetriesLeft, out nodeOutputRegistry))
                {
                    Debug.LogError("Failed to produce graph");
                    return;
                }
                //Debug.Log("Time elapsed: " + (sw.ElapsedMilliseconds / 1000.0f) + " s");
                
                numRetriesLeft = Mathf.Max(0, numRetriesLeft - executor.RetriesUsed);

                var execResult = nodeOutputRegistry.Get(execGraph.resultNode.Id);
                if (execResult == null || execResult.State == null)
                {
                    Debug.LogError("Invalid flow exec result");
                    return;
                }

                var execState = execResult.State;
                layoutGraph = execState.GetState<FlowLayoutGraph>();
                
                if (layoutGraph == null)
                {
                    Debug.LogError("Invalid layout graph state");
                    return;
                }

                var boundsAsset = sgfConfig.moduleDatabase.ModuleBoundsAsset;
                var chunkSize = boundsAsset.chunkSize;
                var baseYOffset = chunkSize.y * 0.5f - boundsAsset.doorOffsetY;

                var settings = new SgfLayoutModuleResolverSettings();
                settings.Seed = (int)config.Seed;
                settings.BaseTransform = transform.localToWorldMatrix * Matrix4x4.Translate(new Vector3(0, baseYOffset, 0));
                settings.LayoutGraph = layoutGraph;
                settings.ModuleDatabase = sgfConfig.moduleDatabase;
                settings.MaxResolveFrames = sgfConfig.maxResolverFrames;
                settings.NonRepeatingRooms = sgfConfig.nonRepeatingRooms;

                sgfModel.layoutGraph = layoutGraph;
                sgfModel.snapModules = new SgfModuleNode[0];

                buildSuccess = SgfLayoutModuleResolver.Resolve(settings, out snapModules);
            }

            if (buildSuccess && snapModules != null)
            {
                // Spawn the module prefabs
                sceneProvider.OnDungeonBuildStart();

                // Spawn the modules and register them in the model
                foreach (var moduleInfo in snapModules)
                {
                    if (moduleInfo.ModuleDBItem == null || moduleInfo.ModuleDBItem.ModulePrefab == null)
                    {
                        continue;
                    }

                    var templateInfo = new GameObjectDungeonThemeItem();
                    templateInfo.Template = moduleInfo.ModuleDBItem.ModulePrefab.gameObject;
                    templateInfo.NodeId = moduleInfo.ModuleInstanceId.ToString();
                    templateInfo.Offset = Matrix4x4.identity;
                    templateInfo.StaticState = DungeonThemeItemStaticMode.Unchanged;
                    templateInfo.externallyManaged = true;

                    var moduleGameObject = sceneProvider.AddGameObject(templateInfo, moduleInfo.WorldTransform, objectInstantiator);
                    moduleInfo.SpawnedModule = moduleGameObject.GetComponent<SnapGridFlowModule>();

                    var spawnedConnections = moduleInfo.SpawnedModule.GetComponentsInChildren<SnapConnection>();

                    var doorInfoValid = spawnedConnections.Length == moduleInfo.Doors.Length;
                    Debug.Assert(doorInfoValid);
                    if (doorInfoValid)
                    {
                        for (var doorIdx = 0; doorIdx < moduleInfo.Doors.Length; doorIdx++)
                        {
                            moduleInfo.Doors[doorIdx].SpawnedDoor = spawnedConnections[doorIdx];
                        }
                    }
                }

                sceneProvider.OnDungeonBuildStop();

                FixupDoorStates(snapModules, layoutGraph);

                SpawnItems(snapModules, sceneProvider, objectInstantiator);

                sgfModel.snapModules = snapModules;
            }
            else
            {
                Debug.LogError("Cannot build snap graph. Retries exhausted. Try adjusting your flow graph or increasing the num retries parameter");
            }

            Cleanup(snapModules);
        }

        public override void OnDestroyed()
        {
            base.OnDestroyed();
            
            var sgfModel = GetComponent<SnapGridFlowModel>();
            if (sgfModel != null)
            {
                sgfModel.layoutGraph = new FlowLayoutGraph();
                sgfModel.snapModules = new SgfModuleNode[0];
            }
        }

        private void SpawnItems(SgfModuleNode[] modules, DungeonSceneProvider sceneProvider, IDungeonSceneObjectInstantiator objectInstantiator)
        {
            var levelMarkers = new LevelMarkerList();
            var sgfConfig = config as SnapGridFlowConfig;
            if (sgfConfig == null)
            {
                Debug.LogError("No snap config script found in dungeon game object");
                return;
            }
            
            foreach (var module in modules)
            {
                if (module == null || module.SpawnedModule == null) continue;
                
                var markers = new List<PlaceableMarker>(module.SpawnedModule.GetComponentsInChildren<PlaceableMarker>());
                MathUtils.Shuffle(markers, random);
                
                foreach (var item in module.LayoutNode.items)
                {
                    if (item == null) continue;
                    
                    PlaceableMarker bestMarker = null;
                    foreach (var markerInfo in markers)
                    {
                        if (markerInfo.supportedMarkers == null) continue;
                        
                        var supportedMarkers = new List<string>(markerInfo.supportedMarkers);
                        if (supportedMarkers.Contains(item.markerName))
                        {
                            bestMarker = markerInfo;
                            break;
                        }
                    }

                    if (bestMarker != null)
                    {
                        markers.Remove(bestMarker);

                        var flowItemMetadata = new FlowItemMetadata();
                        flowItemMetadata.itemId = item.itemId;
                        flowItemMetadata.itemType = item.type;
                        flowItemMetadata.referencedItems = item.referencedItemIds.ToArray();
                        flowItemMetadata.parentTransform = sgfConfig.spawnItemsUnderRoomPrefabs ? bestMarker.transform.parent : null;
                        
                        var themeMarkerEntry = new PropSocket();
                        themeMarkerEntry.SocketType = item.markerName;
                        themeMarkerEntry.Transform = bestMarker.transform.localToWorldMatrix;
                        themeMarkerEntry.metadata = flowItemMetadata;
                        levelMarkers.Add(themeMarkerEntry);
                    }
                    else
                    {
                        Debug.LogWarning(string.Format("Cannot spawn item: {0}. Make sure you have a placeable marker in the module prefab", item.markerName));
                    }
                }
            }
            
            // Run the theme engine
            if (levelMarkers.Count > 0)
            {
                var dungeon = GetComponent<Dungeon>();
                if (dungeon != null)
                {
                    var itemSpawnListeners = new List<DungeonItemSpawnListener>();
                    itemSpawnListeners.Add(GetComponent<FlowItemMetadataHandler>());
                    itemSpawnListeners.AddRange(GetComponents<DungeonItemSpawnListener>());
                    
                    var context = new DungeonThemeExecutionContext();
                    context.builder = this;
                    context.config = config;
                    context.model = model;
                    context.spatialConstraintProcessor = null;
                    context.themeOverrideVolumes = new ThemeOverrideVolume[0];
                    context.sceneProvider = sceneProvider;
                    context.objectSpawner = new SyncDungeonSceneObjectSpawner();
                    context.objectInstantiator = objectInstantiator;
                    context.spawnListeners = itemSpawnListeners.ToArray();

                    var themeEngine = new DungeonThemeEngine(context);
                    themeEngine.ApplyTheme(levelMarkers, dungeon.GetThemeAssets());
                }
                else
                {
                    Debug.LogError("Invalid dungeon reference");
                }
            }
        }
        
        private void Cleanup(SgfModuleNode[] modules)
        {
            // Disable Bounds rendering
            foreach (var module in modules)
            {
                if (module.SpawnedModule != null)
                {
                    module.SpawnedModule.drawBounds = false;
                    
                    // Hide the debug data of the placeable markers
                    var placeableMarkers = module.SpawnedModule.GetComponentsInChildren<PlaceableMarker>();
                    foreach (var placeableMarker in placeableMarkers)
                    {
                        if (placeableMarker != null)
                        {
                            placeableMarker.drawDebugVisuals = false;
                        }
                    }
                }
            }
        }

        private void FixupDoorStates(SgfModuleNode[] snapModules, FlowLayoutGraph layoutGraph)
        {
            var graphQuery = new FlowLayoutGraphQuery(layoutGraph);

            foreach (var moduleInfo in snapModules)
            { 
                var moduleComponent = moduleInfo.SpawnedModule;
                if (moduleComponent == null) continue;
                var connectionComponents = moduleComponent.gameObject.GetComponentsInChildren<SnapConnection>();
                
                for (var doorIdx = 0; doorIdx < moduleInfo.Doors.Length; doorIdx++)
                {
                    var doorInfo = moduleInfo.Doors[doorIdx];
                    doorInfo.SpawnedDoor = connectionComponents[doorIdx];

                    bool foundDoor = false;
                    GameObject spawnedObject = null;
                    
                    bool containsLock = false;
                    FlowItem lockItem = null;
                    
                    if (doorInfo.CellInfo.connectionIdx != -1)
                    {
                        var link = graphQuery.GetLink(doorInfo.CellInfo.linkId);
                        if (link != null)
                        {
                            if (link.state.type != FlowLayoutGraphLinkType.Unconnected)
                            {
                                if (link.source == moduleInfo.ModuleInstanceId)
                                {
                                    // Check if we have a lock here
                                    if (link.state.items != null)
                                    {
                                        foreach (var item in link.state.items)
                                        {
                                            if (item.type == FlowGraphItemType.Lock)
                                            {
                                                containsLock = true;
                                                lockItem = item;
                                                // TODO: Setup Key-Lock metadata
                                            }
                                        }
                                    }

                                    if (containsLock)
                                    {
                                        spawnedObject = doorInfo.SpawnedDoor.UpdateDoorState(SnapConnectionState.DoorLocked, lockItem.markerName);
                                    }
                                    else if (link.state.type == FlowLayoutGraphLinkType.OneWay)
                                    {
                                        spawnedObject = doorInfo.SpawnedDoor.UpdateDoorState(SnapConnectionState.DoorOneWay);
                                    }
                                    else
                                    {
                                        spawnedObject = doorInfo.SpawnedDoor.UpdateDoorState(SnapConnectionState.Door);
                                    }
                                }
                                else
                                {
                                    // Hide the other door so we don't have duplicates.
                                    spawnedObject = doorInfo.SpawnedDoor.UpdateDoorState(SnapConnectionState.None);
                                }
                                
                                foundDoor = true;
                            }
                        }
                    }

                    if (!foundDoor)
                    {
                        spawnedObject = doorInfo.SpawnedDoor.UpdateDoorState(SnapConnectionState.Wall);
                    }

                    if (spawnedObject != null)
                    {
                        if (containsLock && lockItem != null)
                        {
                            var metaDataComponent = spawnedObject.GetComponent<FlowItemMetadataComponent>();
                            if (metaDataComponent == null)
                            {
                                metaDataComponent = spawnedObject.AddComponent<FlowItemMetadataComponent>();
                            }

                            metaDataComponent.itemType = FlowGraphItemType.Lock;
                            metaDataComponent.itemId = lockItem.itemId.ToString();
                            var referencesIds = new List<string>();
                            foreach (var lockRefId in lockItem.referencedItemIds)
                            {
                                referencesIds.Add(lockRefId.ToString());
                            }
                            metaDataComponent.referencedItemIds = referencesIds.ToArray();
                        }
                        else
                        {
                            var metaDataComponent = spawnedObject.GetComponent<FlowItemMetadataComponent>();
                            if (metaDataComponent != null)
                            {
                                DungeonUtils.DestroyObject(metaDataComponent);
                            }
                        }
                    }
                }
            }
        }
    }
}