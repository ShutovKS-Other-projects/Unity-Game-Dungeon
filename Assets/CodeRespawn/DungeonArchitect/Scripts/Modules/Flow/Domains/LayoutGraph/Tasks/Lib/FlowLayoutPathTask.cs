//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using DungeonArchitect.Flow.Items;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Flow.Domains.Layout.Pathing
{
    class FlowLayoutPathStackGrowthTask
    {
        public static void Execute(FlowLayoutPathStackFrame frameState, FlowLayoutStaticGrowthState staticState, FlowLayoutSharedGrowthState sharedState,
                StackSystem<FlowLayoutPathStackFrame, FlowLayoutStaticGrowthState, FlowLayoutSharedGrowthState, FFlowLayoutPathingSystemResult> stackSystem)
        {
            Debug.Assert(staticState.MinPathSize > 0 && staticState.MaxPathSize > 0);
            Debug.Assert(staticState.GraphQuery != null);

            var state = frameState.State;
            var currentNode = frameState.CurrentNode;
            var incomingNode = frameState.IncomingNode;

            var pathIndex = state.Path.Count;
            var pathLength = Mathf.Clamp(pathIndex + 1, staticState.MinPathSize, staticState.MaxPathSize);
            if (pathIndex == 0 && staticState.HeadNode != null) {
                // Check if we can connect from the head node to this node
                if (!staticState.GraphConstraint.IsValid(staticState.GraphQuery, staticState.HeadNode, new FlowLayoutGraphNode[]{currentNode}))
                {
                    stackSystem.SharedState.LastError = EFlowLayoutGrowthErrorType.GraphConstraint;
                    return;
                }
            }

            if (staticState.NodeCreationConstraint != null) {
                if (!staticState.NodeCreationConstraint.CanCreateNodeAt(currentNode, pathLength, pathIndex)) {
                    stackSystem.SharedState.LastError = EFlowLayoutGrowthErrorType.NodeConstraint;
                    return;
                }
            }

            bool bFirstNodeInPath = (pathIndex == 0);

            var baseIncomingConstraintLinks = new List<FFAGConstraintsLink>();
            if (bFirstNodeInPath && staticState.HeadNode != null) {
                var headSubNode = staticState.HeadNode;
                if (staticState.HeadNode.MergedCompositeNodes.Count > 1) {
                    foreach (var graphLink in staticState.Graph.Links) {
                        if (graphLink.state.type != FlowLayoutGraphLinkType.Unconnected) continue;
                        if (graphLink.source == currentNode.nodeId && graphLink.destination == staticState.HeadNode.nodeId) {
                            headSubNode = staticState.GraphQuery.GetSubNode(graphLink.destinationSubNode);
                            Debug.Assert(headSubNode != null);
                            break;
                        }
                        else if (graphLink.source == staticState.HeadNode.nodeId && graphLink.destination == currentNode.nodeId) {
                            headSubNode = staticState.GraphQuery.GetSubNode(graphLink.sourceSubNode);
                            Debug.Assert(headSubNode != null);
                            break;
                        }
                    }
                }

                var headConnectedLink = staticState.GraphQuery.GetConnectedLink(currentNode.nodeId, headSubNode.nodeId);
                Debug.Assert(headConnectedLink != null, "Cannot find link to head node");
                
                sharedState.LinkFromHead = new FFAGConstraintsLink(currentNode, headSubNode, headConnectedLink);
                baseIncomingConstraintLinks.Add(sharedState.LinkFromHead);
            }
            if (incomingNode != null) {
                baseIncomingConstraintLinks.Add(new FFAGConstraintsLink(currentNode, incomingNode, frameState.IncomingLink));
            }

            Debug.Assert(staticState.NodeGroupGenerator != null);
            
            var sortedNodeGroups = new List<FlowLayoutPathNodeGroup>();
            {
                FlowLayoutPathNodeGroup[] possibleNodeGroupsArray = staticState.NodeGroupGenerator.Generate(staticState.GraphQuery, currentNode, pathIndex,
                    pathLength, staticState.Random, state.Visited);
                if (possibleNodeGroupsArray.Length == 0)
                {
                    stackSystem.SharedState.LastError = EFlowLayoutGrowthErrorType.EmptyNodeGroup;
                }

                var possibleNodeGroups = new List<FlowLayoutPathNodeGroup>(possibleNodeGroupsArray);
                MathUtils.Shuffle(possibleNodeGroups, staticState.Random);

                while (possibleNodeGroups.Count > 0)
                {
                    int indexToProcess = 0;
                    {
                        float maxWeight = 0;
                        foreach (var group in possibleNodeGroups)
                        {
                            maxWeight = Mathf.Max(maxWeight, group.Weight);
                        }

                        float frameSelectionWeight = staticState.Random.NextFloat() * maxWeight;
                        for (int i = 0; i < possibleNodeGroups.Count; i++)
                        {
                            if (frameSelectionWeight <= possibleNodeGroups[i].Weight)
                            {
                                indexToProcess = i;
                                break;
                            }
                        }
                    }
                    sortedNodeGroups.Add(possibleNodeGroups[indexToProcess]);
                    possibleNodeGroups.RemoveAt(indexToProcess);
                }
            }

            var framesToPush = new List<FlowLayoutPathStackFrame>();
            foreach (var growthNodeGroup in sortedNodeGroups)
            {   
                // Check if we can use this newly created group node by connecting in to it
                if (!staticState.GraphConstraint.IsValid(staticState.GraphQuery, growthNodeGroup, pathIndex, pathLength, baseIncomingConstraintLinks.ToArray()))
                {
                    stackSystem.SharedState.LastError = EFlowLayoutGrowthErrorType.GraphConstraint;
                    continue;
                }

                FlowLayoutGrowthState nextState = state.Clone();

                // Update the frame path and visited state
                foreach (var groupNode in growthNodeGroup.GroupNodes)
                {
                    nextState.Visited.Add(groupNode);
                }
                
                var pathFrame = new FlowLayoutGrowthStatePathItem();
                pathFrame.NodeId = currentNode.nodeId;
                pathFrame.PreviousNodeId = incomingNode != null ? incomingNode.nodeId : DungeonUID.Empty;
                pathFrame.userdata = growthNodeGroup.userdata;
                nextState.Path.Add(pathFrame);

                // Add path node group info
                if (growthNodeGroup.IsGroup) {
                    var nodeGroup = new FlowLayoutGraphNodeGroup();
                    nodeGroup.GroupId = DungeonUID.NewUID();
                    nodeGroup.GroupNodes = growthNodeGroup.GroupNodes;
                    nextState.NodeGroups.Add(nodeGroup);
                }

                // Check if we reached the desired path size
                if (nextState.Path.Count >= staticState.MinPathSize) {
                    // Check if we are near the sink node, if any
                    if (staticState.SinkNodes.Count == 0) {
                        // No sink nodes defined.
                        var result = new FFlowLayoutPathingSystemResult(nextState, staticState, sharedState);
                        stackSystem.FinalizeResult(result);
                        return;
                    }

                    {
                        var sinkNodeIndices = MathUtils.GetShuffledIndices(staticState.SinkNodes.Count, staticState.Random);
                        var groupEdgeNodeIndices = MathUtils.GetShuffledIndices(growthNodeGroup.GroupEdgeNodes.Count, staticState.Random);
                        foreach (var groupEdgeNodeIndex in groupEdgeNodeIndices) {
                            var groupEdgeNodeId = growthNodeGroup.GroupEdgeNodes[groupEdgeNodeIndex];
                            DungeonUID[] connectedNodeIds;
                            FlowLayoutGraphLink[] connectedLinks;
                            staticState.GraphQuery.GetConnectedNodes(groupEdgeNodeId, out connectedNodeIds, out connectedLinks);
                            
                            var connectedNodeIndices = MathUtils.GetShuffledIndices(connectedNodeIds.Length, staticState.Random);
                            foreach (var connectedNodeIndex in connectedNodeIndices) {
                                var connectedNodeId = connectedNodeIds[connectedNodeIndex];
                                var connectedLink = connectedLinks[connectedNodeIndex];
                                var connectedNode = staticState.GraphQuery.GetNode(connectedNodeId);
                                foreach (var sinkNodeIndex in sinkNodeIndices) {
                                    var sinkNode = staticState.SinkNodes[sinkNodeIndex];
                                    if (sinkNode == null) continue;

                                    if (nextState.Path.Count == 1 && sinkNode == staticState.HeadNode) {
                                        // If the path node size is 1, we don't want to connect back to the head node
                                        continue;
                                    }
                                    
                                    if (connectedNode == sinkNode) {
                                        var groupEdgeNode = staticState.GraphQuery.GetNode(groupEdgeNodeId);
                                        // TODO: Iterate through the edge nodes and check if we can connect to the tail node
                                        var incomingConstraintLinks = new List<FFAGConstraintsLink>(baseIncomingConstraintLinks);
                                        var connectedSubNode = connectedNode;
                                        if (connectedNode.MergedCompositeNodes.Count > 1) {
                                            foreach (var graphLink in staticState.Graph.Links) {
                                                if (graphLink.state.type != FlowLayoutGraphLinkType.Unconnected) continue;
                                                if (graphLink.source == groupEdgeNodeId && graphLink.destination == connectedNodeId) {
                                                    connectedSubNode = staticState.GraphQuery.GetSubNode(graphLink.destinationSubNode);
                                                    Debug.Assert(connectedSubNode != null);
                                                    break;
                                                }
                                                else if (graphLink.source == connectedNodeId && graphLink.destination == groupEdgeNodeId) {
                                                    connectedSubNode = staticState.GraphQuery.GetSubNode(graphLink.sourceSubNode);
                                                    Debug.Assert(connectedSubNode != null);
                                                    break;
                                                }
                                            }
                                        }
                                        
                                        sharedState.LinkToTail = new FFAGConstraintsLink(connectedSubNode, groupEdgeNode, connectedLink);
                                        incomingConstraintLinks.Add(sharedState.LinkToTail);
                                        if (!staticState.GraphConstraint.IsValid(
                                            staticState.GraphQuery, growthNodeGroup, pathIndex, pathLength, incomingConstraintLinks.ToArray()))
                                        {
                                            continue;
                                        }

                                        var sinkIncomingNodes = new List<FlowLayoutGraphNode>() { groupEdgeNode };
                                        if (sinkNode == staticState.HeadNode) {
                                            // The sink and the head are the same. Add the first node to the connected list
                                            var firstNodeInPath = staticState.GraphQuery.GetNode(nextState.Path[0].NodeId);
                                            if (firstNodeInPath != null) {
                                                sinkIncomingNodes.Add(firstNodeInPath);
                                            }
                                        }

                                        if (!staticState.GraphConstraint.IsValid(staticState.GraphQuery, sinkNode, sinkIncomingNodes.ToArray()))
                                            continue;

                                        nextState.TailNode = sinkNode;
                                        stackSystem.FinalizeResult(new FFlowLayoutPathingSystemResult(nextState, staticState, sharedState));
                                        return;
                                    }
                                }
                            }
                        }
                    }

                    if (nextState.Path.Count == staticState.MaxPathSize) {
                        // no sink nodes nearby and we've reached the max path size
                        stackSystem.SharedState.LastError = EFlowLayoutGrowthErrorType.CannotMerge;
                        return;
                    }
                }


                // Try to grow from each outgoing node
                {
                    var groupEdgeNodeIndices = MathUtils.GetShuffledIndices(growthNodeGroup.GroupEdgeNodes.Count, staticState.Random);
                    foreach (var groupEdgeNodeIndex in groupEdgeNodeIndices) {
                        var groupEdgeNodeId = growthNodeGroup.GroupEdgeNodes[groupEdgeNodeIndex];
                        DungeonUID[] connectedNodeIds;
                        FlowLayoutGraphLink[] connectedLinks;
                        staticState.GraphQuery.GetConnectedNodes(groupEdgeNodeId, out connectedNodeIds, out connectedLinks);
                        var connectedNodeIndices = MathUtils.GetShuffledIndices(connectedNodeIds.Length, staticState.Random);
                        foreach (var connectedNodeIndex in connectedNodeIndices) {
                            var connectedNodeId = connectedNodeIds[connectedNodeIndex];
                            if (nextState.Visited.Contains(connectedNodeId)) continue;

                            var connectedNode = staticState.GraphQuery.GetNode(connectedNodeId);
                            if (connectedNode == null) continue;
                            if (connectedNode.active) continue;
                            
                            var connectedLink = connectedLinks[connectedNodeIndex];
                            var groupEdgeNode = staticState.GraphQuery.GetNode(groupEdgeNodeId);
                            var incomingConstraintLinks = new List<FFAGConstraintsLink>(baseIncomingConstraintLinks);
                            incomingConstraintLinks.Add(new FFAGConstraintsLink(groupEdgeNode, connectedNode, connectedLink));
                            if (!staticState.GraphConstraint.IsValid(staticState.GraphQuery, growthNodeGroup, pathIndex, pathLength, incomingConstraintLinks.ToArray())) {
                                continue;
                            }

                            var nextFrame = new FlowLayoutPathStackFrame();
                            nextFrame.State = nextState;
                            nextFrame.CurrentNode = connectedNode;
                            nextFrame.IncomingNode = groupEdgeNode;
                            nextFrame.IncomingLink = connectedLink;
                            framesToPush.Add(nextFrame);
                        }
                    }
                }
            }
            
            // Push in reverse order since this is a stack, so that our sort order works correctly
            framesToPush.Reverse();
            foreach (var frame in framesToPush)
            {
                stackSystem.PushFrame(frame);
            }
        }
    }
}
