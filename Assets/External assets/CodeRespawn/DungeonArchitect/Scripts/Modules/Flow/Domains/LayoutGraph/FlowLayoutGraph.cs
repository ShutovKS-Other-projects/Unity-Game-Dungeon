//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System;
using System.Collections.Generic;
using DungeonArchitect.Flow.Items;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Flow.Domains.Layout
{
    [System.Serializable]
    public class FlowLayoutGraph : ICloneable
    {
        [SerializeField]
        public List<FlowLayoutGraphNode> Nodes = new List<FlowLayoutGraphNode>();

        [SerializeField]
        public List<FlowLayoutGraphLink> Links = new List<FlowLayoutGraphLink>();

        public void RemoveNode(FlowLayoutGraphNode node)
        {
            BreakAllLinks(node);
            Nodes.Remove(node);
        }

        public void AddNode(FlowLayoutGraphNode node)
        {
            Nodes.Add(node);
        }

        public void RemoveLink(FlowLayoutGraphLink link)
        {
            Links.Remove(link);
        }

        public FlowLayoutGraphNode CreateNode()
        {
            var node = new FlowLayoutGraphNode();
            AddNode(node);
            return node;
        }

        public FlowLayoutGraphNode GetNode(DungeonUID nodeId)
        {
            foreach (var node in Nodes)
            {
                if (node.nodeId == nodeId)
                {
                    return node;
                }
            }

            return null;
        }

        public FlowLayoutGraphLink GetLink(FlowLayoutGraphNode sourceNode, FlowLayoutGraphNode destNode)
        {
            return GetLink(sourceNode, destNode, false);
        }

        public FlowLayoutGraphLink GetLink(FlowLayoutGraphNode sourceNode, FlowLayoutGraphNode destNode, bool ignoreDirection)
        {
            if (sourceNode == null || destNode == null)
            {
                return null;
            }

            return GetLink(sourceNode.nodeId, destNode.nodeId, ignoreDirection);
        }

        public FlowLayoutGraphLink GetLink(DungeonUID sourceNodeId, DungeonUID destNodeId, bool ignoreDirection)
        {
            foreach (var link in Links)
            {
                if (link.source == sourceNodeId && link.destination == destNodeId)
                {
                    return link;
                }
                if (ignoreDirection)
                {
                    if (link.source == destNodeId && link.destination == sourceNodeId)
                    {
                        return link;
                    }
                }
            }
            return null;
        }
        
        public FlowLayoutGraphLink[] GetLinks(DungeonUID sourceNodeId, DungeonUID destNodeId)
        {
            return GetLinks(sourceNodeId, destNodeId, false);
        }

        public FlowLayoutGraphLink[] GetLinks(DungeonUID sourceNodeId, DungeonUID destNodeId, bool ignoreDirection)
        {
            var result = new List<FlowLayoutGraphLink>();
            
            foreach (var link in Links)
            {
                if (link.source == sourceNodeId && link.destination == destNodeId) {
                    result.Add(link);
                }
                else if (ignoreDirection && link.source == destNodeId && link.destination == sourceNodeId) {
                    result.Add(link);
                }
            }

            return result.ToArray();
        }

        public FlowLayoutGraphLink MakeLink(FlowLayoutGraphNode sourceNode, FlowLayoutGraphNode destNode)
        {
            if (sourceNode == null || destNode == null)
            {
                return null;
            }

            // Make sure an existing link doesn't exist
            {
                FlowLayoutGraphLink existingLink = GetLink(sourceNode, destNode);
                if (existingLink != null)
                {
                    // Link already exists
                    return null;
                }
            }

            // Create a new link
            var link = new FlowLayoutGraphLink();
            link.source = sourceNode.nodeId;
            link.destination = destNode.nodeId;
            Links.Add(link);
            return link;
        }

        public FlowLayoutGraphLink MakeLinkFastUnsafe(FlowLayoutGraphNode sourceNode, FlowLayoutGraphNode destNode)
        {
            if (sourceNode == null || destNode == null)
            {
                return null;
            }

            // Create a new link
            var link = new FlowLayoutGraphLink();
            link.source = sourceNode.nodeId;
            link.destination = destNode.nodeId;
            Links.Add(link);
            return link;
        }

        public void BreakLink(FlowLayoutGraphNode sourceNode, FlowLayoutGraphNode destNode)
        {
            FlowLayoutGraphLink link = GetLink(sourceNode, destNode);
            if (link != null)
            {
                Links.Remove(link);
            }
        }

        public void BreakAllOutgoingLinks(FlowLayoutGraphNode node)
        {
            if (node != null)
            {
                var linkArray = Links.ToArray();
                foreach (var link in linkArray)
                {
                    if (link.source == node.nodeId)
                    {
                        Links.Remove(link);
                    }
                }
            }
        }

        public void BreakAllIncomingLinks(FlowLayoutGraphNode node)
        {
            if (node != null)
            {
                var linkArray = Links.ToArray();
                foreach (var link in linkArray)
                {
                    if (link.destination == node.nodeId)
                    {
                        Links.Remove(link);
                    }
                }
            }
        }

        public void BreakAllLinks(FlowLayoutGraphNode node)
        {
            if (node != null)
            {
                var linkArray = Links.ToArray();
                foreach (var link in linkArray)
                {
                    if (link.source == node.nodeId || link.destination == node.nodeId)
                    {
                        Links.Remove(link);
                    }
                }
            }
        }

        public void Clear()
        {
            var nodeList = Nodes.ToArray();
            foreach (var node in nodeList)
            {
                RemoveNode(node);
            }
        }

        public FlowLayoutGraphNode[] GetOutgoingNodes(FlowLayoutGraphNode node)
        {
            var result = new List<FlowLayoutGraphNode>();
            if (node != null)
            {
                foreach (var link in Links)
                {
                    if (link.source == node.nodeId)
                    {
                        result.Add(GetNode(link.destination));
                    }
                }
            }
            return result.ToArray();
        }

        public FlowLayoutGraphNode[] GetIncomingNodes(FlowLayoutGraphNode node)
        {
            var result = new List<FlowLayoutGraphNode>();
            if (node != null)
            {
                foreach (var link in Links)
                {
                    if (link.destination == node.nodeId)
                    {
                        result.Add(GetNode(link.source));
                    }
                }
            }
            return result.ToArray();
        }

        public FlowLayoutGraphLink[] GetOutgoingLinks(FlowLayoutGraphNode node)
        {
            var result = new List<FlowLayoutGraphLink>();
            if (node != null)
            {
                foreach (var link in Links)
                {
                    if (link.source == node.nodeId)
                    {
                        result.Add(link);
                    }
                }
            }
            return result.ToArray();
        }

        public FlowLayoutGraphLink[] GetIncomingLinks(FlowLayoutGraphNode node)
        {
            var result = new List<FlowLayoutGraphLink>();
            if (node != null)
            {
                foreach (var link in Links)
                {
                    if (link.destination == node.nodeId)
                    {
                        result.Add(link);
                    }
                }
            }
            return result.ToArray();
        }

        public FlowLayoutGraphNode[] GetConnectedNodes(FlowLayoutGraphNode node)
        {
            var result = new List<FlowLayoutGraphNode>();
            if (node != null)
            {
                foreach (var link in Links)
                {
                    if (link.destination == node.nodeId)
                    {
                        result.Add(GetNode(link.source));
                    }
                    else if (link.source == node.nodeId)
                    {
                        result.Add(GetNode(link.destination));
                    }
                }
            }
            return result.ToArray();
        }

        public DungeonUID[] GetConnectedNodes(DungeonUID nodeId)
        {
            var result = new List<DungeonUID>();
            foreach (var link in Links)
            {
                if (link.destination == nodeId)
                {
                    result.Add(link.source);
                }
                else if (link.source == nodeId)
                {
                    result.Add(link.destination);
                }
            }
            return result.ToArray();
        }
        
        public object Clone()
        {
            var newGraph = Activator.CreateInstance(GetType()) as FlowLayoutGraph;

            // Create nodes
            foreach (var oldNode in Nodes)
            {
                var newNode = oldNode.Clone();
                newGraph.Nodes.Add(newNode);
            }

            // Create the links
            foreach (var oldLink in Links)
            {
                var newLink = oldLink.Clone();
                newGraph.Links.Add(newLink);
            }

            return newGraph;
        }

        public FlowItem[] GetAllItems()
        {
            var items = new List<FlowItem>();

            foreach (var node in Nodes)
            {
                items.AddRange(node.items);
            }

            foreach (var link in Links)
            {
                items.AddRange(link.state.items);
            }

            return items.ToArray();
        }
    }

}