//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System;
using System.Collections.Generic;
using DungeonArchitect.Flow.Items;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Flow.Domains.Layout
{
    [System.Serializable]
    public class FlowLayoutGraphNode
    {
        public DungeonUID nodeId;
        public Vector3 position = Vector3.zero;
        public bool active = false;
        public Color color = Color.green;
        public Vector3 coord = Vector3.zero;
        public string pathName = "";
        public List<FlowItem> items = new List<FlowItem>();
        public int pathIndex = -1;
        public int pathLength = 0;
        public bool mainPath = false;
        
        // This node may be a composite node which was created by merging these nodes
        [NonSerialized] // TODO: Fix serialization
        public List<FlowLayoutGraphNode> MergedCompositeNodes = new List<FlowLayoutGraphNode>();
        
        [HideInInspector]
        public FlowDomainDataRegistry domainData = new FlowDomainDataRegistry();
        
        public FlowLayoutGraphNode()
        {
            nodeId = DungeonUID.NewUID();
        }

        public FlowLayoutGraphNode Clone()
        {
            var newNode = new FlowLayoutGraphNode();
            newNode.nodeId = nodeId;
            newNode.position = position;
            newNode.active = active;
            newNode.color = color;
            newNode.coord = coord;
            newNode.pathName = pathName;
            newNode.pathIndex = pathIndex;
            newNode.pathLength = pathLength;
            newNode.mainPath = mainPath;
            newNode.domainData = domainData.Clone();

            foreach (var item in items)
            {
                newNode.AddItem(item.Clone());
            }
            
            foreach (var compositeNode in MergedCompositeNodes)
            {
                newNode.MergedCompositeNodes.Add(compositeNode.Clone());
            }
            
            return newNode;
        }

        public FlowItem CreateItem<T>() where T : FlowItem, new()
        {
            var item = new T();
            items.Add(item);
            return item;
        }
        
        public void AddItem(FlowItem item)
        {
            items.Add(item);
        }
        
        public T GetDomainData<T>() where T : IFlowDomainData, new()
        {
            if (domainData == null)
            {
                return default;
            }

            return domainData.Get<T>();
        }
        
        public void SetDomainData<T>(T data) where T : IFlowDomainData, new()
        {
            if (domainData != null)
            {
                domainData.Set(data);
            }
        }
    }

    public class FlowLayoutGraphNodeGroup
    {
        public DungeonUID GroupId = DungeonUID.Empty;
        public List<DungeonUID> GroupNodes = new List<DungeonUID>();

        public FlowLayoutGraphNodeGroup Clone()
        {
            var clone = new FlowLayoutGraphNodeGroup();
            clone.GroupId = GroupId;
            clone.GroupNodes = new List<DungeonUID>(GroupNodes);
            return clone;
        }
    }
}
