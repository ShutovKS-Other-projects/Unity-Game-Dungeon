//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using System.Linq;
using DungeonArchitect.Flow.Items;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Flow.Domains.Layout
{
    public class FlowLayoutGraphTraversal
    {
        private Dictionary<DungeonUID, FNodeInfo[]> outgoingNodes = new Dictionary<DungeonUID, FNodeInfo[]>();
        private Dictionary<DungeonUID, FNodeInfo[]> incomingNodes = new Dictionary<DungeonUID, FNodeInfo[]>();
        private Dictionary<DungeonUID, DungeonUID> teleporters = new Dictionary<DungeonUID, DungeonUID>();   // Node -> Node mapping of teleporters
        
        public void Build(FlowLayoutGraph graph)
        {
            outgoingNodes.Clear();
            incomingNodes.Clear();
            teleporters.Clear();

            if (graph == null)
            {
                return;
            }
            
            var outgoingList = new Dictionary<DungeonUID, List<FNodeInfo>>();
            var incomingList = new Dictionary<DungeonUID, List<FNodeInfo>>();
            
            foreach (var link in graph.Links)
            {
                if (link.state.type == FlowLayoutGraphLinkType.Unconnected)
                {
                    continue;
                }
                
                // Add outgoing nodes
                {
                    if (!outgoingList.ContainsKey(link.source))
                    {
                        outgoingList.Add(link.source, new List<FNodeInfo>());
                    }

                    var info = new FNodeInfo();
                    info.NodeId = link.destination;
                    info.LinkId = link.linkId;
                    info.Outgoing = true;
                    
                    outgoingList[link.source].Add(info);
                }
                
                // Add incoming nodes
                {
                    if (!incomingList.ContainsKey(link.destination))
                    {
                        incomingList.Add(link.destination, new List<FNodeInfo>());
                    }

                    var info = new FNodeInfo();
                    info.NodeId = link.source;
                    info.LinkId = link.linkId;
                    info.Outgoing = false;
                    
                    incomingList[link.destination].Add(info);
                }
            }

            // Finalize the incoming/outgoing list
            {
                foreach (var entry in outgoingList)
                {
                    outgoingNodes.Add(entry.Key, entry.Value.ToArray());
                }
                foreach (var entry in incomingList)
                {
                    incomingNodes.Add(entry.Key, entry.Value.ToArray());
                }
            }
            
            // Build the teleporter list
            {
                // Build a mapping of the teleporter item to their host node mapping
                var teleporterHostMap = new Dictionary<DungeonUID, FlowLayoutGraphNode>();     // Teleporter to owning node map
                foreach (var node in graph.Nodes)
                {
                    if (node == null || !node.active) continue;
                    foreach (var item in node.items)
                    {
                        if (item != null && item.type == FlowGraphItemType.Teleporter)
                        {
                            if (teleporterHostMap.ContainsKey(item.itemId))
                            {
                                teleporterHostMap.Remove(item.itemId);
                            }
                            teleporterHostMap.Add(item.itemId, node);
                        }
                    }
                }
                
                // Make another pass to build the teleporter list
                foreach (var node in graph.Nodes)
                {
                    if (node == null || !node.active) continue;
                    foreach (var item in node.items)
                    {
                        if (item != null && item.type == FlowGraphItemType.Teleporter)
                        {
                            if (item.referencedItemIds.Count > 0)
                            {
                                var otherTeleporterId = item.referencedItemIds[0];
                                if (teleporterHostMap.ContainsKey(otherTeleporterId))
                                {
                                    var teleNodeA = node;
                                    var teleNodeB = teleporterHostMap[otherTeleporterId];

                                    if (!teleporters.ContainsKey(teleNodeA.nodeId))
                                    {
                                        teleporters.Add(teleNodeA.nodeId, teleNodeB.nodeId);
                                    }
                                    if (!teleporters.ContainsKey(teleNodeB.nodeId))
                                    {
                                        teleporters.Add(teleNodeB.nodeId, teleNodeA.nodeId);
                                    }
                                } 
                            }
                        }
                    }
                }
            }
        }

        public FNodeInfo[] GetOutgoingNodes(DungeonUID nodeId)
        {
            if (outgoingNodes.ContainsKey(nodeId))
            {
                return outgoingNodes[nodeId];
            }

            return new FNodeInfo[0];
        }
        
        public FNodeInfo[] GetIncomingNodes(DungeonUID nodeId)
        {
            if (incomingNodes.ContainsKey(nodeId))
            {
                return incomingNodes[nodeId];
            }

            return new FNodeInfo[0];
        }
        
        public FNodeInfo[] GetConnectedNodes(DungeonUID nodeId)
        {
            var connectedNodes = new List<FNodeInfo>();
            connectedNodes.AddRange(GetOutgoingNodes(nodeId));
            connectedNodes.AddRange(GetIncomingNodes(nodeId));
            return connectedNodes.ToArray();
        }

        public bool GetTeleportNode(DungeonUID nodeId, out DungeonUID connectedNodeId)
        {
            if (!teleporters.ContainsKey(nodeId))
            {
                connectedNodeId = DungeonUID.Empty;
                return false;
            }

            connectedNodeId = teleporters[nodeId];
            return true;
        }
        
        public struct FNodeInfo
        {
            public DungeonUID NodeId;
            public DungeonUID LinkId;
            public bool Outgoing;
        }

    }

    public class FlowLayoutGraphQuery
    {
        public FlowLayoutGraphQuery(FlowLayoutGraph graph)
        {
            this.graph = graph;
            Build();
        }
        
        public FlowLayoutGraphTraversal Traversal
        {
            get => traversal;
        }

        public FlowLayoutGraph Graph
        {
            get => graph;
        }
        
        public FlowLayoutGraph GetGraph()
        {
            return graph;
        }
        
        public T GetGraphOfType<T>() where T : FlowLayoutGraph
        {
            return (T)graph;
        }

        public FlowLayoutGraphNode GetNode(DungeonUID nodeId)
        {
            return nodeMap.ContainsKey(nodeId) ? nodeMap[nodeId] : null;
        }

        public FlowLayoutGraphLink GetLink(DungeonUID linkId)
        {
            return linkMap.ContainsKey(linkId) ? linkMap[linkId] : null;
        }

        public FlowLayoutGraphNode GetSubNode(DungeonUID nodeId)
        {
            
            return subNodeMap.ContainsKey(nodeId) ? subNodeMap[nodeId] : null;
        }

        public bool GetParentNode(DungeonUID nodeId, out DungeonUID parentNodeId)
        {
            if (parentNodes.ContainsKey(nodeId))
            {
                parentNodeId = parentNodes[nodeId];
                return true;
            }

            parentNodeId = DungeonUID.Empty;
            return false;
        }
        
        public DungeonUID GetNodeAtCoord(Vector3 coord)
        {
            return coordToNodeMap.ContainsKey(coord) ? coordToNodeMap[coord] : DungeonUID.Empty;
        }

        public FlowLayoutGraphNode GetNodeObjAtCoord(Vector3Int nodeCoord)
        {
            if (nodeCoord.x >= 0 && nodeCoord.x < nodeArray3D.GetLength(0)
                && nodeCoord.y >= 0 && nodeCoord.y < nodeArray3D.GetLength(1)
                && nodeCoord.z >= 0 && nodeCoord.z < nodeArray3D.GetLength(2))
            {
                return nodeArray3D[nodeCoord.x, nodeCoord.y, nodeCoord.z];
            }

            return null;
        }

        public DungeonUID[] GetConnectedNodes(DungeonUID nodeId)
        {
            return connectedNodes.ContainsKey(nodeId) ? connectedNodes[nodeId] : new DungeonUID[0];
        }

        public DungeonUID[] GetIncomingNodes(DungeonUID nodeId)
        {
            return incomingNodes.ContainsKey(nodeId) ? incomingNodes[nodeId] : new DungeonUID[0];
        }
        
        public DungeonUID[] GetOutgoingNodes(DungeonUID nodeId)
        {
            return outgoingNodes.ContainsKey(nodeId) ? outgoingNodes[nodeId] : new DungeonUID[0];
        }

        public FlowLayoutGraphLink[] GetConnectedLinks(DungeonUID nodeId)
        {
            return connectedLinks.ContainsKey(nodeId) ? connectedLinks[nodeId] : new FlowLayoutGraphLink[0];
        }

        public FlowLayoutGraphLink GetConnectedLink(DungeonUID nodeA, DungeonUID nodeB)
        {
            var connectedLinks = GetConnectedLinks(nodeA);
            FlowLayoutGraphLink targetLink = null;
            foreach (var connectedLink in connectedLinks)
            {
                if (connectedLink == null) continue;
                if (connectedLink.source == nodeB
                    || connectedLink.sourceSubNode == nodeB
                    || connectedLink.destination == nodeB
                    || connectedLink.destinationSubNode == nodeB)
                {
                    targetLink = connectedLink;
                    break;
                }
            }

            return targetLink;
        }
        
        public void GetConnectedNodes(DungeonUID nodeId, out DungeonUID[] outConnectedNodeIds, out FlowLayoutGraphLink[] outConnectedLinks)
        {
            outConnectedNodeIds = GetConnectedNodes(nodeId);
            outConnectedLinks = GetConnectedLinks(nodeId);
        }

        public void Rebuild()
        {
            Build();
        }
        private void Build()
        {
            nodeMap.Clear();
            linkMap.Clear();
            connectedNodes.Clear();
            connectedLinks.Clear();
            outgoingNodes.Clear();
            incomingNodes.Clear();
            subNodeMap.Clear();
            coordToNodeMap.Clear();
            parentNodes.Clear();
            
            foreach (var node in graph.Nodes)
            {
                nodeMap.Add(node.nodeId, node);
                coordToNodeMap[node.coord] = node.nodeId;
                foreach (var subNode in node.MergedCompositeNodes)
                {
                    subNodeMap[subNode.nodeId] = subNode;
                    parentNodes[subNode.nodeId] = node.nodeId;
                }
            }

            {
                graphGridSize = IntVector.Zero;
                foreach (var node in graph.Nodes)
                {
                    var nodeCoord = MathUtils.RoundToVector3Int(node.coord);
                    graphGridSize.x = Mathf.Max(graphGridSize.x, nodeCoord.x + 1);
                    graphGridSize.y = Mathf.Max(graphGridSize.y, nodeCoord.y + 1);
                    graphGridSize.z = Mathf.Max(graphGridSize.z, nodeCoord.z + 1);
                }
                
                nodeArray3D = new FlowLayoutGraphNode[graphGridSize.x, graphGridSize.y, graphGridSize.z];
                foreach (var node in graph.Nodes)
                {
                    var nodeCoord = MathUtils.RoundToVector3Int(node.coord);
                    nodeArray3D[nodeCoord.x, nodeCoord.y, nodeCoord.z] = node;
                }
            }

            foreach (var link in graph.Links)
            {
                linkMap.Add(link.linkId, link);
            }

            {
                var connectedNodesMap = new Dictionary<DungeonUID, List<DungeonUID>>();
                foreach (var link in graph.Links)
                {
                    if (!connectedNodesMap.ContainsKey(link.source))
                    {
                        connectedNodesMap.Add(link.source, new List<DungeonUID>());
                    }
                    if (!connectedNodesMap.ContainsKey(link.destination))
                    {
                        connectedNodesMap.Add(link.destination, new List<DungeonUID>());
                    }
                    
                    connectedNodesMap[link.source].Add(link.destination);
                    connectedNodesMap[link.destination].Add(link.source);
                }

                foreach (var key in connectedNodesMap.Keys)
                {
                    connectedNodes[key] = connectedNodesMap[key].ToArray();
                }
            }

            {
                var connectedLinkMap = new Dictionary<DungeonUID, List<FlowLayoutGraphLink>>();
                var outgoingNodeMap = new Dictionary<DungeonUID, List<DungeonUID>>();
                var incomingNodeMap = new Dictionary<DungeonUID, List<DungeonUID>>();
                foreach (var link in graph.Links)
                {
                    if (!connectedLinkMap.ContainsKey(link.source)) connectedLinkMap.Add(link.source, new List<FlowLayoutGraphLink>());
                    if (!connectedLinkMap.ContainsKey(link.sourceSubNode)) connectedLinkMap.Add(link.sourceSubNode, new List<FlowLayoutGraphLink>());
                    if (!connectedLinkMap.ContainsKey(link.destination)) connectedLinkMap.Add(link.destination, new List<FlowLayoutGraphLink>());
                    if (!connectedLinkMap.ContainsKey(link.destinationSubNode)) connectedLinkMap.Add(link.destinationSubNode, new List<FlowLayoutGraphLink>());

                    connectedLinkMap[link.source].Add(link);
                    connectedLinkMap[link.sourceSubNode].Add(link);
                    connectedLinkMap[link.destination].Add(link);
                    connectedLinkMap[link.destinationSubNode].Add(link);

                    if (link.state.type != FlowLayoutGraphLinkType.Unconnected)
                    {
                        if (!outgoingNodeMap.ContainsKey(link.source)) outgoingNodeMap.Add(link.source, new List<DungeonUID>());
                        if (!incomingNodeMap.ContainsKey(link.destination)) incomingNodeMap.Add(link.destination, new List<DungeonUID>());

                        outgoingNodeMap[link.source].Add(link.destination);
                        incomingNodeMap[link.destination].Add(link.source);
                    }
                }

                foreach (var key in connectedLinkMap.Keys)
                {
                    connectedLinks[key] = connectedLinkMap[key].ToArray();
                }
                
                foreach (var key in outgoingNodeMap.Keys)
                {
                    outgoingNodes[key] = outgoingNodeMap[key].ToArray();
                }
                
                foreach (var key in incomingNodeMap.Keys)
                {
                    incomingNodes[key] = incomingNodeMap[key].ToArray();
                }
            }
            
            traversal.Build(graph);
        }
        
        private FlowLayoutGraph graph;
        private Dictionary<DungeonUID, FlowLayoutGraphNode> nodeMap = new Dictionary<DungeonUID, FlowLayoutGraphNode>();
        private Dictionary<DungeonUID, FlowLayoutGraphLink> linkMap = new Dictionary<DungeonUID, FlowLayoutGraphLink>();
        private Dictionary<DungeonUID, DungeonUID[]> connectedNodes = new Dictionary<DungeonUID, DungeonUID[]>();
        private Dictionary<DungeonUID, DungeonUID[]> outgoingNodes = new Dictionary<DungeonUID, DungeonUID[]>();
        private Dictionary<DungeonUID, DungeonUID[]> incomingNodes = new Dictionary<DungeonUID, DungeonUID[]>();
        private Dictionary<DungeonUID, DungeonUID> parentNodes = new Dictionary<DungeonUID, DungeonUID>();
        private Dictionary<DungeonUID, FlowLayoutGraphLink[]> connectedLinks = new Dictionary<DungeonUID, FlowLayoutGraphLink[]>();
        private FlowLayoutGraphTraversal traversal = new FlowLayoutGraphTraversal();
        private Dictionary<DungeonUID, FlowLayoutGraphNode> subNodeMap = new Dictionary<DungeonUID, FlowLayoutGraphNode>();
        private Dictionary<Vector3, DungeonUID> coordToNodeMap = new Dictionary<Vector3, DungeonUID>();
        private IntVector graphGridSize;
        private FlowLayoutGraphNode[,,] nodeArray3D = new FlowLayoutGraphNode[0,0,0];
    }
}