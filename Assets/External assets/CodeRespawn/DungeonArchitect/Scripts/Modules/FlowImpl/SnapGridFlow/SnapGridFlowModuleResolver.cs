//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using System;
using System.Collections.Generic;
using System.Linq;
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Domains.Layout.Pathing;
using DungeonArchitect.Flow.Items;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Flow.Impl.SnapGridFlow
{
    class SgfModuleItemFitnessCalculator {
        public SgfModuleItemFitnessCalculator(SgfModuleDatabasePlaceableMarkerInfo[] moduleMarkers) {
            foreach (var info in moduleMarkers)
            {
                ModuleMarkers[info.placeableMarkerTemplate] = info.count;
            }
        }

        public int Calculate(string[] markerNames) {
            var availableMarkers = new Dictionary<PlaceableMarker, int>(ModuleMarkers);
            return Solve(markerNames, availableMarkers);
        }

        private static int Solve(string[] markerNames, Dictionary<PlaceableMarker, int> availableMarkers) {
            int numFailed;
            if (availableMarkers.Count > 0) {
                numFailed = SolveImpl(markerNames, 0, availableMarkers);
                Debug.Assert(numFailed >= 0);
            }
            else
            {
                numFailed = markerNames.Length;
            }

            const int FAIL_WEIGHT = 1000000;
            return numFailed * FAIL_WEIGHT;
        }

        // Returns the no. of items failed in the processed sub tree
        private static int SolveImpl(string[] markerNames, int index, Dictionary<PlaceableMarker, int> availableMarkers) {
            if (index == markerNames.Length) {
                return 0;
            }
            
            Debug.Assert(index >= 0 || index < markerNames.Length);

            int bestFrameFailCount = markerNames.Length;
            var markerName = markerNames[index];
            var keys = availableMarkers.Keys.ToArray();
            foreach (var key in keys) {
                var availableMarkerAsset = key;
                int count = availableMarkers[key];
                
                bool canAttachHere = count > 0 && availableMarkerAsset.supportedMarkers.Contains(markerName);
                int frameFailCount = canAttachHere ? 0 : 1;
                if (canAttachHere) {
                    count--;
                }
                frameFailCount += SolveImpl(markerNames, index + 1, availableMarkers);
                if (canAttachHere) {
                    count++;
                }

                availableMarkers[availableMarkerAsset] = count;

                if (frameFailCount < bestFrameFailCount) {
                    bestFrameFailCount = frameFailCount;
                }
                
                if (availableMarkerAsset.supportedMarkers.Length == 1 && availableMarkerAsset.supportedMarkers[0] == markerName) {
                    // Faster bailout
                    break;
                }
            } 
            
            return bestFrameFailCount;
        }

        private Dictionary<PlaceableMarker, int> ModuleMarkers = new Dictionary<PlaceableMarker, int>();
    };
    
    public struct SgfLayoutModuleResolverSettings
    {
        public int Seed;
        public Matrix4x4 BaseTransform;
        public float ModulesWithMinimumDoorsProbability;
        public SnapGridFlowModuleDatabase ModuleDatabase;
        public FlowLayoutGraph LayoutGraph;
        public int MaxResolveFrames;
        public int NonRepeatingRooms;
    }
    
    public class SgfLayoutModuleResolver
    {
        class FModuleFitCandidate {
            public SgfModuleDatabaseItem ModuleItem;
            public Quaternion ModuleRotation;
            public int AssemblyIndex;
            public SgfModuleAssemblySideCell[] DoorIndices;
            public int ItemFitness = 0;
            public int ConnectionWeight = 0;
            public int ModuleLastUsedDepth = int.MaxValue;
            public float ModuleWeight = 0;
        };

        class NodeGroupData
        {
            public FlowLayoutPathNodeGroup Group;
            public FFAGConstraintsLink[] ConstraintLinks;
        }

        class ResolveState
        {
            public FlowLayoutGraphQuery graphQuery;
            public System.Random random;
            public Dictionary<DungeonUID, SgfModuleNode> moduleNodesById = new Dictionary<DungeonUID, SgfModuleNode>();
            public Dictionary<DungeonUID, SgfModuleAssemblySideCell[]> activeModuleDoorIndices = new Dictionary<DungeonUID, SgfModuleAssemblySideCell[]>();
            public Dictionary<FlowLayoutGraphNode, NodeGroupData> nodeGroups = new Dictionary<FlowLayoutGraphNode, NodeGroupData>();
            public Dictionary<SgfModuleDatabaseItem, Stack<int>> moduleLastUsedDepth = new Dictionary<SgfModuleDatabaseItem, Stack<int>>();
            public int frameIndex = 0;
        }
        
        public static bool Resolve(SgfLayoutModuleResolverSettings settings, out SgfModuleNode[] outModuleNodes)
        {
            if (settings.LayoutGraph == null || settings.ModuleDatabase == null || settings.ModuleDatabase.ModuleBoundsAsset == null)
            {
                outModuleNodes = new SgfModuleNode[]{};
                return false;
            }

            var graph = settings.LayoutGraph;
            var resolveState = new ResolveState
            {
                graphQuery = new FlowLayoutGraphQuery(graph),
                random = new System.Random(settings.Seed)
            };
            
            foreach (var node in graph.Nodes)
            {
                if (node.active)
                {
                    var nodeGroupData = new NodeGroupData();
                    SnapFlowLayoutGraphConstraints.BuildNodeGroup(resolveState.graphQuery, node, new FlowLayoutGraphNode[] { }, out nodeGroupData.Group, out nodeGroupData.ConstraintLinks);
                    resolveState.nodeGroups.Add(node, nodeGroupData);
                }
            }

            if (!ResolveNodes(settings, resolveState))
            {
                outModuleNodes = new SgfModuleNode[] { };
                return false;
            }
            
            foreach (var entry in resolveState.activeModuleDoorIndices) {
                var moduleId = entry.Key;
                var doorSideCells = entry.Value;
                for (var i = 0; i < doorSideCells.Length; i++)
                {
                    foreach (var graphLink in graph.Links) {
                        if (graphLink.state.type == FlowLayoutGraphLinkType.Unconnected) continue;
                        if ((graphLink.source == doorSideCells[i].nodeId || graphLink.sourceSubNode == doorSideCells[i].nodeId)
                            && (graphLink.destination == doorSideCells[i].linkedNodeId || graphLink.destinationSubNode == doorSideCells[i].linkedNodeId)) {
                            // Outgoing Node
                            doorSideCells[i].linkId = graphLink.linkId;
                            break;
                        }
                        else if ((graphLink.source == doorSideCells[i].linkedNodeId || graphLink.sourceSubNode == doorSideCells[i].linkedNodeId)
                                 && (graphLink.destination == doorSideCells[i].nodeId || graphLink.destinationSubNode == doorSideCells[i].nodeId)) {
                            // Incoming Node
                            doorSideCells[i].linkId = graphLink.linkId;
                            break;
                        }
                    }
                }
            }

            foreach (var entry in resolveState.activeModuleDoorIndices)
            {
                var nodeId = entry.Key;
                var moduleDoorCells = entry.Value;
                
                if (!resolveState.moduleNodesById.ContainsKey(nodeId))
                {
                    continue;
                }

                var moduleInfo = resolveState.moduleNodesById[nodeId];
                foreach (var doorInfo in moduleInfo.Doors)
                {
                    doorInfo.CellInfo = SgfModuleAssemblySideCell.Empty;
                }
                
                foreach (var doorCell in moduleDoorCells)
                {
                    var doorInfo = moduleInfo.Doors[doorCell.connectionIdx];
                    doorInfo.CellInfo = doorCell;
                }
            }
            
            // Link the module nodes together
            foreach (var graphLink in graph.Links) {
                if (graphLink == null || graphLink.state.type == FlowLayoutGraphLinkType.Unconnected) continue;
                
                var sourceId = graphLink.source;
                var destId = graphLink.destination;

                SgfModuleAssemblySideCell srcCell = SgfModuleAssemblySideCell.Empty;
                SgfModuleAssemblySideCell dstCell = SgfModuleAssemblySideCell.Empty;
                bool foundSrcCell = false;
                bool foundDstCell = false;
                
                {
                    if (resolveState.activeModuleDoorIndices.ContainsKey(sourceId))
                    {
                        var sourceDoorCells = resolveState.activeModuleDoorIndices[sourceId];
                        foreach (var sourceDoorCell in sourceDoorCells)
                        {
                            if (sourceDoorCell.linkId == graphLink.linkId)
                            {
                                srcCell = sourceDoorCell;
                                foundSrcCell = true;
                                break;
                            }
                        }
                    }

                    if (resolveState.activeModuleDoorIndices.ContainsKey(destId))
                    {
                        var destDoorCells = resolveState.activeModuleDoorIndices[destId];
                        foreach (var destDoorCell in destDoorCells)
                        {
                            if (destDoorCell.linkId == graphLink.linkId)
                            {
                                dstCell = destDoorCell;
                                foundDstCell = true;
                                break;
                            }
                        }
                    }
                }

                if (!foundSrcCell || !foundDstCell) {
                    outModuleNodes = new SgfModuleNode[]{};
                    return false;
                }
                
                if (!resolveState.moduleNodesById.ContainsKey(sourceId) || !resolveState.moduleNodesById.ContainsKey(destId)) {
                    outModuleNodes = new SgfModuleNode[]{};
                    return false;
                }

                var srcModule = resolveState.moduleNodesById[sourceId];
                var dstModule = resolveState.moduleNodesById[destId];
                var srcDoor = srcModule.Doors[srcCell.connectionIdx];
                var dstDoor = dstModule.Doors[dstCell.connectionIdx];

                srcDoor.ConnectedDoor = dstDoor;
                dstDoor.ConnectedDoor = srcDoor;

                srcModule.Outgoing.Add(srcDoor);
                dstModule.Incoming.Add(dstDoor);
            }

            outModuleNodes = resolveState.moduleNodesById.Values.ToArray();
            return true;
        }

        private static SgfModuleNode CreateModuleNode(FlowLayoutGraphNode layoutNode, SgfModuleDatabaseItem item)
        {
            var node = new SgfModuleNode();
            node.ModuleInstanceId = layoutNode.nodeId;
            node.ModuleDBItem = item;
            node.LayoutNode = layoutNode;

            var nodeDoors = new List<SgfModuleDoor>();
            foreach (var doorInfo in item.Connections) {
                var door = new SgfModuleDoor();
                door.LocalTransform = doorInfo.Transform;
                door.Owner = node;
                nodeDoors.Add(door);
            }

            node.Doors = nodeDoors.ToArray();

            return node;
        }
        
        private static bool ResolveNodes(SgfLayoutModuleResolverSettings settings, ResolveState resolveState)
        {
            return ResolveNodesRecursive(settings, resolveState);
            //return ResolveNodesLinear(settings, resolveState);
        }

        private static bool ResolveNodesRecursive(SgfLayoutModuleResolverSettings settings, ResolveState resolveState)
        {
            var graph = settings.LayoutGraph;
            
            // Find the spawn room node
            FlowLayoutGraphNode startNode = null;
            foreach (var node in graph.Nodes)
            {
                if (node.active)
                {
                    foreach (var nodeItem in node.items)
                    {
                        if (nodeItem.type == FlowGraphItemType.Entrance)
                        {
                            startNode = node;
                            break;
                        }
                    }
                }
            }

            if (startNode == null)
            {
                return false;
            }
            
            var visited = new HashSet<FlowLayoutGraphNode>();
            return ResolveNodeRecursive(startNode, 0, settings, resolveState, visited);
        }

        private static bool ResolveNodeRecursive(FlowLayoutGraphNode node, int depth, SgfLayoutModuleResolverSettings settings, 
            ResolveState resolveState, HashSet<FlowLayoutGraphNode> visited)
        {
            if (resolveState.frameIndex > settings.MaxResolveFrames)
            {
                return false;
            }
            resolveState.frameIndex++;
            
            if (visited.Contains(node))
            {
                // Already resolved
                return true;
            }

            var nodeGroupData = resolveState.nodeGroups[node];
            SgfModuleAssembly nodeAssembly;
            SGFModuleAssemblyBuilder.Build(resolveState.graphQuery, nodeGroupData.Group, nodeGroupData.ConstraintLinks, out nodeAssembly);
            
            var candidates = GetCandidates(node, depth, settings, resolveState, nodeAssembly);
            if (candidates.Length == 0) return false;
            
            //IncrementModuleLastUsedDepth(resolveState);
            
            visited.Add(node);
            bool success = false;
            foreach (var candidate in candidates)
            {
                // Find the best fit
                RegisterNodeModule(node, candidate, settings, resolveState);

                // Recursively resolve all outgoing nodes
                var outgoingNodes = new List<FlowLayoutGraphNode>();
                {
                    var outgoingNodeIds = resolveState.graphQuery.GetOutgoingNodes(node.nodeId);
                    foreach (var outgoingNodeId in outgoingNodeIds)
                    {
                        var outgoingNode = resolveState.graphQuery.GetNode(outgoingNodeId);
                        if (outgoingNode != null && outgoingNode.active)
                        {
                            outgoingNodes.Add(outgoingNode);
                        }
                    }
                }

                // Save the depth of this module so we don't repeat them next to each other
                PushModuleLastUsedDepth(resolveState, candidate.ModuleItem, depth);
                
                bool allBranchesSuccessful = true;
                // Resolve the child branches
                foreach (var outgoingNode in outgoingNodes)
                {
                    if (!ResolveNodeRecursive(outgoingNode, depth + 1, settings, resolveState, visited))
                    {
                        allBranchesSuccessful = false;
                        break;
                    }
                }

                PopModuleLastUsedDepth(resolveState, candidate.ModuleItem);
                
                if (allBranchesSuccessful)
                {
                    success = true;
                    break;
                }

                DeregisterNodeModule(node, resolveState);
            }
            
            //DecrementModuleLastUsedDepth(resolveState);
            
            // Cannot find a solution in this path. Backtrack and find another path
            visited.Remove(node);

            return success;
        }

        private static void PushModuleLastUsedDepth(ResolveState resolveState, SgfModuleDatabaseItem module, int depth)
        {
            if (!resolveState.moduleLastUsedDepth.ContainsKey(module))
            {
                resolveState.moduleLastUsedDepth.Add(module, new Stack<int>());
            }
            resolveState.moduleLastUsedDepth[module].Push(depth);
        }

        private static void PopModuleLastUsedDepth(ResolveState resolveState, SgfModuleDatabaseItem module)
        {
            resolveState.moduleLastUsedDepth[module].Pop();
        }
        
        private static int GetModuleLastUsedDepth(ResolveState resolveState, SgfModuleDatabaseItem module, int currentDepth, int maxNonRepeatingDepth)
        {
            if (!resolveState.moduleLastUsedDepth.ContainsKey(module) || resolveState.moduleLastUsedDepth[module].Count == 0)
            {
                return int.MaxValue;
            }

            var moduleDepthFromCurrentNode = currentDepth - resolveState.moduleLastUsedDepth[module].Peek();
            return moduleDepthFromCurrentNode < maxNonRepeatingDepth + 1 ? moduleDepthFromCurrentNode : int.MaxValue;
        }
        
        private static FModuleFitCandidate[] GetCandidates(FlowLayoutGraphNode node, int depth, SgfLayoutModuleResolverSettings settings, ResolveState state, SgfModuleAssembly nodeAssembly)
        {
            var categoryNames = new HashSet<string>();
            FlowLayoutNodeSnapDomainData nodeSnapData = node.GetDomainData<FlowLayoutNodeSnapDomainData>();
            if (nodeSnapData != null)
            {
                categoryNames = new HashSet<string>(nodeSnapData.Categories);
            }
            else
            {
                Debug.LogError("Snap Domain data missing in the abstract graph node");
            }

            var possibleModules = new List<SgfModuleDatabaseItem>();
            foreach (string categoryName in categoryNames)
            {
                possibleModules.AddRange(settings.ModuleDatabase.GetCategoryModules(categoryName));
            }

            var desiredNodeMarkers = new List<string>();
            foreach (var nodeItem in node.items)
            {
                if (nodeItem == null) continue;
                var markerName = nodeItem.markerName.Trim();
                if (markerName.Length > 0)
                {
                    desiredNodeMarkers.Add(markerName);
                }
            }

            bool bChooseModulesWithMinDoors = state.random.NextFloat() < settings.ModulesWithMinimumDoorsProbability;

            var candidates = new List<FModuleFitCandidate>();
            var moduleIndices = MathUtils.GetShuffledIndices(possibleModules.Count, state.random);
            foreach (var moduleIdx in moduleIndices)
            {
                var moduleInfo = possibleModules[moduleIdx];
                if (moduleInfo == null) continue;

                var numAssemblies = moduleInfo.RotatedAssemblies.Length;
                var shuffledAsmIndices = MathUtils.GetShuffledIndices(numAssemblies, state.random);
                foreach (var asmIdx in shuffledAsmIndices)
                {
                    var moduleAssembly = moduleInfo.RotatedAssemblies[asmIdx];
                    SgfModuleAssemblySideCell[] doorIndices;
                    if (moduleAssembly.CanFit(nodeAssembly, out doorIndices))
                    {
                        bool doorCategoriesValid = true;
                        // Make sure all the door indices are compatible with the category on the other connected end
                        foreach (var doorInfo in doorIndices)
                        {
                            var nodeConnection = moduleInfo.Connections[doorInfo.connectionIdx];
                            Debug.Assert(nodeConnection != null, "SGF Resolve: Invalid node connection");
                            DungeonUID doorNodeId;
                            if (!state.graphQuery.GetParentNode(doorInfo.nodeId, out doorNodeId))
                            {
                                doorNodeId = doorInfo.nodeId;
                            }
                            
                            Debug.Assert(doorNodeId == node.nodeId, "Invalid door link node data");
                            DungeonUID otherNodeId;
                            if (!state.graphQuery.GetParentNode(doorInfo.linkedNodeId, out otherNodeId))
                            {
                                otherNodeId = doorInfo.linkedNodeId;
                            }
                            
                            if (state.activeModuleDoorIndices.ContainsKey(otherNodeId))
                            {
                                var otherNodeDoors = state.activeModuleDoorIndices[otherNodeId];
                                foreach (var otherNodeDoor in otherNodeDoors)
                                {
                                    DungeonUID otherDoorLinkedNodeId;
                                    if (!state.graphQuery.GetParentNode(otherNodeDoor.linkedNodeId, out otherDoorLinkedNodeId))
                                    {
                                        otherDoorLinkedNodeId = otherNodeDoor.linkedNodeId;
                                    }

                                    if (otherDoorLinkedNodeId == node.nodeId)
                                    {
                                        var otherNodeModule = state.moduleNodesById[otherNodeId];
                                        var otherNodeConnection = otherNodeModule.ModuleDBItem.Connections[otherNodeDoor.connectionIdx];
                                        Debug.Assert(otherNodeConnection != null, "SGF Resolve: Invalid remote node connection");

                                        if (nodeConnection.Category != otherNodeConnection.Category)
                                        {
                                            doorCategoriesValid = false;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (!doorCategoriesValid)
                            {
                                break;
                            }
                        }

                        if (doorCategoriesValid)
                        {
                            var candidate = new FModuleFitCandidate();
                            candidates.Add(candidate);

                            candidate.ModuleItem = moduleInfo;
                            candidate.ModuleRotation = Quaternion.AngleAxis(asmIdx * -90, Vector3.up);
                            candidate.AssemblyIndex = asmIdx;
                            candidate.DoorIndices = doorIndices;
                        }
                    }
                }
            }
            
            // Update the weights
            float maxSelectionWeight = 0;
            foreach (var candidate in candidates)
            {
                maxSelectionWeight = Mathf.Max(maxSelectionWeight, candidate.ModuleItem.SelectionWeight);
            }

            if (maxSelectionWeight == 0)
            {
                maxSelectionWeight = 1;
            }
            
            foreach (var candidate in candidates)
            {
                var moduleInfo = candidate.ModuleItem;
                var itemFitnessCalculator = new SgfModuleItemFitnessCalculator(moduleInfo.AvailableMarkers);
                int itemFitness = itemFitnessCalculator.Calculate(desiredNodeMarkers.ToArray());

                int connectionWeight = bChooseModulesWithMinDoors ? moduleInfo.Connections.Length : 0;
                var moduleWeight = Mathf.Clamp(moduleInfo.SelectionWeight / maxSelectionWeight, 0.0f, 1.0f);

                candidate.ItemFitness = itemFitness;
                candidate.ConnectionWeight = connectionWeight;
                candidate.ModuleWeight = moduleWeight;
                candidate.ModuleLastUsedDepth = GetModuleLastUsedDepth(state, moduleInfo, depth, settings.NonRepeatingRooms);
            }
            
            candidates.Sort((a, b) =>
            {
                if (a.ItemFitness == b.ItemFitness)
                {
                    if (Mathf.Approximately(a.ModuleWeight, b.ModuleWeight))
                    {
                        if (a.ConnectionWeight == b.ConnectionWeight)
                        {
                            if (a.ModuleLastUsedDepth == b.ModuleLastUsedDepth)
                            {
                                return 0;
                            }
                            else
                            {
                                return a.ModuleLastUsedDepth < b.ModuleLastUsedDepth ? 1 : -1;
                            }
                        }
                        else
                        {
                            return a.ConnectionWeight < b.ConnectionWeight ? -1 : 1;
                        }
                    }
                    else
                    {
                        return a.ModuleWeight < b.ModuleWeight ? 1 : -1;
                    }
                }
                else
                {
                    return a.ItemFitness < b.ItemFitness ? -1 : 1;
                }
            });
            
            return candidates.ToArray();
        }
        
        private static void DeregisterNodeModule(FlowLayoutGraphNode node, ResolveState resolveState)
        {
            resolveState.activeModuleDoorIndices.Remove(node.nodeId);
            
            // Register in lookup
            resolveState.moduleNodesById.Remove(node.nodeId);
        }
        
        private static void RegisterNodeModule(FlowLayoutGraphNode node, FModuleFitCandidate candidate, SgfLayoutModuleResolverSettings settings, ResolveState resolveState)
        {
            var moduleNode = CreateModuleNode(node, candidate.ModuleItem);
            var moduleRotation = candidate.ModuleRotation;
            var moduleDBItem = candidate.ModuleItem;

            var chunkSize = settings.ModuleDatabase.ModuleBoundsAsset.chunkSize;
            var doorOffsetY = settings.ModuleDatabase.ModuleBoundsAsset.doorOffsetY;


            var halfChunkSize = Vector3.Scale(MathUtils.ToVector3(moduleDBItem.NumChunks), chunkSize) * 0.5f;
            {
                //var localCenter = new Vector3(halfChunkSize.x, doorOffsetY, halfChunkSize.z);
                var localCenter = halfChunkSize;
                localCenter = moduleRotation * localCenter;
                var desiredCenter = Vector3.Scale(node.coord, chunkSize);
                var position = desiredCenter - localCenter;
                moduleNode.WorldTransform = settings.BaseTransform * Matrix4x4.TRS(position, moduleRotation, Vector3.one);
            }

            // Add the doors
            resolveState.activeModuleDoorIndices[node.nodeId] = candidate.DoorIndices;
            
            // Register in lookup
            resolveState.moduleNodesById[node.nodeId] = moduleNode;
        }
    }
}