//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System;
using System.Collections.Generic;
using System.Linq;
using DungeonArchitect.Flow.Exec;
using DungeonArchitect.Flow.Items;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Flow.Domains.Layout.Tasks
{
    public class LayoutBaseFlowTaskCreateKeyLock : FlowExecTask
    {
        public string keyBranch = "main";
        public string lockBranch = "main";
        public string keyMarkerName = "Key";
        public string lockMarkerName = "Lock";


        protected virtual bool Validate(FlowTaskExecContext context, FlowTaskExecInput input, ref string errorMessage, ref FlowTaskExecutionResult executionResult)
        {
            return true;
        }

        public override FlowTaskExecOutput Execute(FlowTaskExecContext context, FlowTaskExecInput input)
        {
            var output = new FlowTaskExecOutput();
            if (input.IncomingTaskOutputs.Length == 0)
            {
                output.ErrorMessage = "Missing Input";
                output.ExecutionResult = FlowTaskExecutionResult.FailHalt;
                return output;
            }

            if (input.IncomingTaskOutputs.Length > 1)
            {
                output.ErrorMessage = "Only one input allowed";
                output.ExecutionResult = FlowTaskExecutionResult.FailHalt;
                return output;
            }
            
            output.State = input.CloneInputState();
            if (!Validate(context, input, ref output.ErrorMessage, ref output.ExecutionResult))
            {
                return output;
            }

            var graph = output.State.GetState<FlowLayoutGraph>();

            FlowLayoutGraphNode keyNode;
            FlowLayoutGraphLink lockLink;
            
            var graphQuery = new FlowLayoutGraphQuery(graph);
            if (FindKeyLockSetup(graphQuery, context.Random, out keyNode, out lockLink, out output.ErrorMessage))
            {
                var keyItem = new FlowItem();
                keyItem.type = FlowGraphItemType.Key;
                keyItem.markerName = keyMarkerName;
                keyNode.AddItem(keyItem);
                
                ProcessKeyItem(keyItem, keyNode, lockLink);

                var lockItem = new FlowItem();
                lockItem.type = FlowGraphItemType.Lock;
                lockItem.markerName = lockMarkerName;
                lockLink.state.AddItem(lockItem);
                
                keyItem.referencedItemIds.Add(lockItem.itemId);
                lockItem.referencedItemIds.Add(keyItem.itemId);

                output.ExecutionResult = FlowTaskExecutionResult.Success;
                return output;
            }

            output.ExecutionResult = FlowTaskExecutionResult.FailRetry;
            return output;
        }

        protected virtual void ProcessKeyItem(FlowItem keyItem, FlowLayoutGraphNode keyNode, FlowLayoutGraphLink lockLink)
        {
        }

        DungeonUID[] GetLockedNodesInPath(FlowLayoutGraphQuery graphQuery, FlowLayoutGraphLink lockLink)
        {
            var sourceNode = graphQuery.GetNode(lockLink.source);
            var destNode = graphQuery.GetNode(lockLink.destination);
            
            var disallowedNodes = new List<DungeonUID>();
            disallowedNodes.Add(destNode.nodeId);
            
            // If the link belongs to the main path, we want to disallow all the nodes in the main path after this link
            bool mainPathLink = (sourceNode != null && destNode != null && sourceNode.mainPath && destNode.mainPath);
            if (mainPathLink)
            {
                var graph = graphQuery.Graph;
                // Grab all the main path nodes after this link
                var mainPathNodeId = destNode.nodeId;
                var visited = new HashSet<DungeonUID>() {mainPathNodeId};

                while (true)
                {
                    FlowLayoutGraphLink nextLink = null;
                    foreach (var link in graph.Links)
                    {
                        if (link.state.type == FlowLayoutGraphLinkType.Unconnected) continue;
                        if (link.source == mainPathNodeId)
                        {
                            // make sure the destination node is also a main path node
                            var dest = graphQuery.GetNode(link.destination);
                            if (dest.mainPath)
                            {
                                nextLink = link;
                                break;
                            }
                        }
                    }

                    if (nextLink == null)
                    {
                        break;
                    }

                    mainPathNodeId = nextLink.destination;
                    if (visited.Contains(mainPathNodeId))
                    {
                        break;
                    }

                    visited.Add(mainPathNodeId);
                    disallowedNodes.Add(mainPathNodeId);
                }
            }

            return disallowedNodes.ToArray();
        }
        
        private bool FindKeyLockSetup(FlowLayoutGraphQuery graphQuery, System.Random random, out FlowLayoutGraphNode outKeyNode,
                out FlowLayoutGraphLink outLockLink, out string errorMessage)
        {
            var graph = graphQuery.Graph;
            var entranceNode = FlowLayoutGraphUtils.FindNodeWithItemType(graph, FlowGraphItemType.Entrance);
            if (entranceNode == null)
            {
                errorMessage = "Missing Entrance Node";
                outKeyNode = null;
                outLockLink = null;
                return false;
            }

            var keyNodes = FlowLayoutGraphUtils.FindNodesOnPath(graph, keyBranch);
            var lockNodes = FlowLayoutGraphUtils.FindNodesOnPath(graph, lockBranch);

            MathUtils.Shuffle(keyNodes, random);
            MathUtils.Shuffle(lockNodes, random);

            var traversal = graphQuery.Traversal;
            
            foreach (var keyNode in keyNodes)
            {
                foreach (var lockNode in lockNodes)
                {
                    
                    // Lock link list creation criteria
                    //     1. Get all lock node links that connect to other nodes in the same lock path
                    //     2. grab the rest of the links connected to the lock node
                    //     3. Filter out the ones that already have a lock on them
                    // Lock link selection criteria 
                    //     1. Make sure the key node is accessible from the entrance, after blocking off the selected lock link
                    //     2. Make sure lock node is not accessible from the entrance after blocking off the selected lock link

                    // Generate the lock link array
                    var lockNodeLinks = new List<FlowLayoutGraphTraversal.FNodeInfo>();
                    {
                        var allLockLinks = traversal.GetConnectedNodes(lockNode.nodeId);

                        var resultPrimary = new List<FlowLayoutGraphTraversal.FNodeInfo>();
                        var resultSecondary = new List<FlowLayoutGraphTraversal.FNodeInfo>();
                        
                        foreach (var connectionInfo in allLockLinks)
                        {
                            // Make sure this link doesn't already have a lock
                            var lockLink = graphQuery.GetLink(connectionInfo.LinkId);
                            if (lockLink == null || FlowLayoutGraphUtils.ContainsItem(lockLink.state.items, FlowGraphItemType.Lock))
                            {
                                continue;
                            }

                            var connectedNode = graphQuery.GetNode(connectionInfo.NodeId);
                            if (connectedNode != null)
                            {
                                if (connectedNode.pathName == lockBranch)
                                {
                                    resultPrimary.Add(connectionInfo);
                                }
                                else
                                {
                                    resultSecondary.Add(connectionInfo);
                                }
                            }
                        }
                        
                        MathUtils.Shuffle(resultPrimary, random);
                        lockNodeLinks.AddRange(resultPrimary);

                        MathUtils.Shuffle(resultSecondary, random);
                        lockNodeLinks.AddRange(resultSecondary);
                    }
                    
                    
                    // Select the first valid link from the list
                    foreach (var lockConnection in lockNodeLinks)
                    {
                        var lockLinkId = lockConnection.LinkId;
                        var lockLink = graphQuery.GetLink(lockLinkId);
                        if (lockLink == null)
                        {
                            continue;
                        }

                        // Check if this link belongs to the main path
                        //var lockedNodeIds = GetLockedNodesInPath(graphQuery, lockLink);
                        var lockedNodeIds = new DungeonUID[]{ lockLink.destination };

                        Func<FlowLayoutGraphTraversal.FNodeInfo, bool> canTraverse = 
                                (traverseInfo) => traverseInfo.LinkId != lockLinkId;
                        
                        // 1. Make sure the key node is accessible from the entrance, after blocking off the selected lock link
                        bool canReachKey = FlowLayoutGraphUtils.CanReachNode(graphQuery, entranceNode.nodeId, keyNode.nodeId, 
                            false, false, true, canTraverse);
                        if (canReachKey) {
                            // 2. Make sure lock node is not accessible from the entrance after blocking off the selected lock link
                            bool canReachLockedNode = false;
                            foreach (var lockedNodeId in lockedNodeIds)
                            {
                                bool reachable = FlowLayoutGraphUtils.CanReachNode(graphQuery, entranceNode.nodeId, lockedNodeId,
                                    false, false, true, canTraverse);
                                
                                if (reachable)
                                {
                                    canReachLockedNode = true;
                                    break;
                                }
                            }
                            
                            if (!canReachLockedNode) {
                                // Validate the entire graph's key-lock setup by walking through it like a player would
                                if (ValidateFullKeyLockSetup(graphQuery, keyNode, lockLink))
                                {
                                    outKeyNode = keyNode;
                                    outLockLink = lockLink;
                                    errorMessage = "";
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            
            outKeyNode = null;
            outLockLink = null;
            errorMessage = "Cannot find key-lock";
            return false;
        }


        struct NodeConnectionInfo
        {
            public FlowLayoutGraphNode ConnectedNode;
            public bool ContainsLock;
            public DungeonUID LockId;

            public NodeConnectionInfo(FlowLayoutGraphNode connectedNode)
            {
                this.ConnectedNode = connectedNode;
                this.ContainsLock = false;
                this.LockId = DungeonUID.Empty;
            }
            public NodeConnectionInfo(FlowLayoutGraphNode connectedNode, DungeonUID lockId)
            {
                this.ConnectedNode = connectedNode;
                this.ContainsLock = true;
                this.LockId = lockId;
            }
        }
        
        private bool ValidateFullKeyLockSetup(FlowLayoutGraphQuery graphQuery, FlowLayoutGraphNode inputKeyNode, FlowLayoutGraphLink inputLockLink)
        {
            // We'll walk through the entire graph like how a player would and make sure it is a playable level
            // Step1:
            //      - Flood fill from the start node till we can't reach any more nodes.
            //      - Record all the keys we've encountered till now.
            //      - Do not pass through any locks we cannot open yet (based on the keys we've encountered)
            //      - Do not pass through one-way doors
            //
            // Step2: 
            //      - If all nodes were visited in the last flood fill, Return true
            //
            // Step3:
            //      - Did we visit at-least one new node since the last flood fill?
            //           + if not, return fail
            //
            // Step4:
            //      - Repeat Step1

            var graph = graphQuery.Graph;
            
            FlowLayoutGraphNode startNode = null;
            FlowLayoutGraphNode goalNode = null;
            var connectedNodes = new Dictionary<FlowLayoutGraphNode, List<NodeConnectionInfo>>();
            var keyToLockMap = new Dictionary<DungeonUID, HashSet<DungeonUID>>();
            var lockToKeyMap = new Dictionary<DungeonUID, HashSet<DungeonUID>>();
            int numActiveNodes = 0;
            
            // Create a new temp id for the key-lock pair that is being tested and add it to the list (this pair has not been added to the graph yet)
            var inputKeyID = DungeonUID.NewUID();
            var inputLockID = DungeonUID.NewUID();
            keyToLockMap.Add(inputKeyID, new HashSet<DungeonUID>(){ inputLockID });
            lockToKeyMap.Add(inputLockID, new HashSet<DungeonUID>(){ inputKeyID });
            
            #region Build Lookups
            {
                foreach (var node in graph.Nodes)
                {
                    if (node == null || !node.active) continue;
                    numActiveNodes++;
                    
                    foreach (var item in node.items)
                    {
                        if (item == null) continue;
                        if (item.type == FlowGraphItemType.Entrance)
                        {
                            startNode = node;
                        }
                        else if (item.type == FlowGraphItemType.Exit)
                        {
                            goalNode = node;
                        }
                        else if (item.type == FlowGraphItemType.Key)
                        {
                            var keyId = item.itemId;
                            var lockIds = item.referencedItemIds;
                            if (!keyToLockMap.ContainsKey(keyId))
                            {
                                keyToLockMap.Add(keyId, new HashSet<DungeonUID>());
                            }

                            foreach (var lockId in lockIds)
                            {
                                keyToLockMap[keyId].Add(lockId);

                                if (!lockToKeyMap.ContainsKey(lockId))
                                {
                                    lockToKeyMap.Add(lockId, new HashSet<DungeonUID>());
                                }

                                lockToKeyMap[lockId].Add(keyId);
                            }
                        }
                    }
                }

                if (startNode == null || goalNode == null)
                {
                    return false;
                }

                
                // build the connected node list
                foreach (var link in graph.Links)
                {
                    if (link == null || link.state.type == FlowLayoutGraphLinkType.Unconnected) continue;
                    var sourceNode = graphQuery.GetNode(link.source);
                    var destNode = graphQuery.GetNode(link.destination);
                    if (sourceNode == null || destNode == null) continue;

                    if (!connectedNodes.ContainsKey(sourceNode))
                    {
                        connectedNodes.Add(sourceNode, new List<NodeConnectionInfo>());
                    }

                    if (!connectedNodes.ContainsKey(destNode))
                    {
                        connectedNodes.Add(destNode, new List<NodeConnectionInfo>());
                    }

                    var connectionInfo = new NodeConnectionInfo(destNode);
                    foreach (var item in link.state.items)
                    {
                        if (item.type == FlowGraphItemType.Lock)
                        {
                            connectionInfo.ContainsLock = true;
                            connectionInfo.LockId = item.itemId;
                            break;
                        }
                    }

                    if (link == inputLockLink)
                    {
                        connectionInfo.ContainsLock = true;
                        connectionInfo.LockId = inputLockID;
                    }

                    connectedNodes[sourceNode].Add(connectionInfo);
                    if (link.state.type == FlowLayoutGraphLinkType.Connected && !connectionInfo.ContainsLock)
                    {
                        connectedNodes[destNode].Add(new NodeConnectionInfo(sourceNode));
                    }
                }
            }
            #endregion

            int lastVisitedNodes = 0;
            var visitedKeys = new HashSet<DungeonUID>();
            while (lastVisitedNodes < numActiveNodes)
            {
                // Step1: Flood fill from the start node
                var queue = new Queue<FlowLayoutGraphNode>();
                var visited = new HashSet<FlowLayoutGraphNode>();
                queue.Enqueue(startNode);
                visited.Add(startNode);
                while (queue.Count > 0)
                {
                    var node = queue.Dequeue();
                    Debug.Assert(node != null && node.active);
                    
                    // Save visited keys
                    foreach (var item in node.items)
                    {
                        if (item.type == FlowGraphItemType.Key)
                        {
                            visitedKeys.Add(item.itemId);
                        }
                    }

                    if (node == inputKeyNode)
                    {
                        visitedKeys.Add(inputKeyID);
                    }
                    
                    // Visit unexplored connected nodes
                    if (connectedNodes.ContainsKey(node))
                    {
                        var connections = connectedNodes[node];
                        foreach (var connection in connections)
                        {
                            if (visited.Contains(connection.ConnectedNode)) continue;
                            bool canTraverseDoor = true;
                            if (connection.ContainsLock)
                            {
                                // Make sure we have a key to pass through it
                                canTraverseDoor = false;
                                foreach (var keyId in visitedKeys)
                                {
                                    if (keyToLockMap.ContainsKey(keyId))
                                    {
                                        if (keyToLockMap[keyId].Contains(connection.LockId))
                                        {
                                            // We have a key that can open this lock
                                            canTraverseDoor = true;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (canTraverseDoor)
                            {
                                queue.Enqueue(connection.ConnectedNode);
                                visited.Add(connection.ConnectedNode);
                            }
                        }
                    }
                }
                
                var numVisited = visited.Count;
                if (numVisited <= lastVisitedNodes)
                {
                    // No change since last time
                    Debug.Assert(numVisited < numActiveNodes);
                    return false;
                }
                lastVisitedNodes = numVisited;
            }

            return true;
        }
    }
}