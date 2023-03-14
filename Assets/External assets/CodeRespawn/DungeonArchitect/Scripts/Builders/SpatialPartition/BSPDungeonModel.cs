//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Builders.BSP
{
    [System.Serializable]
    public struct BSPNode
    {
        public DungeonUID id;
        public Rectangle bounds;
        public Rectangle paddedBounds;
        public int depthFromRoot;
        public string roomCategory;

        public DungeonUID parent;
        public DungeonUID[] children;

        public DungeonUID[] connectedRooms;

        public BSPNodeConnection[] subtreeLeafConnections;

        public Color debugColor;
        public bool discarded;
    }

    [System.Serializable]
    public struct BSPNodeConnection
    {
        public DungeonUID room0; 
        public DungeonUID room1;

        public IntVector doorPosition0;
        public IntVector doorPosition1;

        public bool doorFacingX;
    }

    public class BSPDungeonGraphQuery
    {
        DungeonUID rootNode;
        Dictionary<DungeonUID, BSPNode> nodeMap;

        public BSPDungeonGraphQuery(DungeonUID rootNode, BSPNode[] nodes)
        {
            this.rootNode = rootNode;
            nodeMap = new Dictionary<DungeonUID, BSPNode>();
            foreach (var node in nodes)
            {
                nodeMap.Add(node.id, node);
            }
        }

        public BSPNode RootNode
        {
            get { return GetNode(rootNode); }
        }

        public BSPNode GetNode(DungeonUID nodeId)
        {
            return nodeMap[nodeId];
        }

        public BSPNode[] GetChildren(DungeonUID nodeId)
        {
            var children = new List<BSPNode>();
            var node = GetNode(nodeId);
            foreach (var childId in node.children)
            {
                children.Add(GetNode(childId));
            }
            return children.ToArray();
        }

        public BSPNode GetParent(DungeonUID nodeId)
        {
            var node = GetNode(nodeId);
            return GetNode(node.parent);
        }
    }

    public class BSPDungeonModel : DungeonModel {

		[HideInInspector]
		public BSPDungeonConfig Config;

        [HideInInspector]
        public DungeonUID rootNode;
        
		[HideInInspector]
        public BSPNode[] nodes;

        [HideInInspector]
        public BSPNodeConnection[] connections;
        
        public BSPDungeonGraphQuery CreateGraphQuery()
        {
            return new BSPDungeonGraphQuery(rootNode, nodes);
        }

        public override void ResetModel() 
        { 
            nodes = new BSPNode[0];
            connections = new BSPNodeConnection[0];
            rootNode = DungeonUID.Empty;
        }
    }
}
