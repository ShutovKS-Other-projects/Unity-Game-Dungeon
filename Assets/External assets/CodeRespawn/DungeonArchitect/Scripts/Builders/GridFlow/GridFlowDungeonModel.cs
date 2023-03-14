//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using System.Collections.Generic;
using UnityEngine;
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Domains.Tilemap;
using DungeonArchitect.Flow.Impl.GridFlow;
using DungeonArchitect.Utils;

namespace DungeonArchitect.Builders.GridFlow
{
    public class GridFlowDungeonModel : DungeonModel
    {
        /// <summary>
        /// The high level node based layout graph
        /// </summary>
        [HideInInspector]
        [SerializeField]
        private FlowLayoutGraph layoutGraph;

        public FlowLayoutGraph LayoutGraph => layoutGraph;

        /// <summary>
        /// Rasterized tilemap representation of the abstract graph
        /// </summary>
        [HideInInspector]
        [SerializeField]
        private FlowTilemap tilemap;

        public FlowTilemap Tilemap => tilemap;
        
        /// <summary>
        /// The size of the grid this dungeon was built with
        /// </summary>
        [HideInInspector]
        [SerializeField]
        private Vector3 gridSize;
        
        /// <summary>
        /// Walls in the grid flow builder can either take up a full tile or are built as edges
        /// This is controlled from the "Initial Tilemap" node's property in the grid flow execution graph
        /// </summary>
        [HideInInspector]
        public bool wallsAsEdges = false;

        [HideInInspector]
        [SerializeField]
        private GridFlowDungeonQuery query;
        
        public GridFlowDungeonQuery Query { get => query; }
        
        public void Initialize(FlowLayoutGraph inLayoutGraph, FlowTilemap inTilemap, Vector3 inGridSize)
        {
            layoutGraph = inLayoutGraph;
            tilemap = inTilemap;
            gridSize = inGridSize;
            query = GetComponent<GridFlowDungeonQuery>();
            lookups.Build(this);
        }

        public void Reset()
        {
            layoutGraph = null;
            tilemap = null;
        }
        
        [HideInInspector]
        [SerializeField]
        GridFlowModelLookups lookups = new GridFlowModelLookups();

        public FlowTilemapCell GetTilemapCell(Vector3 worldPosition)
        {
            var tileCoord = WorldPositionToTilemapCoord(worldPosition);
            return tilemap.Cells.GetCell(tileCoord.x, tileCoord.y);
        }

        public FlowLayoutGraphNode GetLayoutNode(Vector3 worldPosition)
        {
            if (lookups == null) return null;
            
            var tile = GetTilemapCell(worldPosition);
            if (tile == null) return null;
            var nodeCoord = tile.NodeCoord;
            if (!lookups.NodeCoordLookup.ContainsKey(nodeCoord))
            {
                return null;
            }

            var nodeIndex = lookups.NodeCoordLookup[nodeCoord];
            return (nodeIndex >= 0 && nodeIndex < layoutGraph.Nodes.Count) 
                ? layoutGraph.Nodes[nodeIndex] 
                : null;
        }

        public GridFlowLayoutNodeRoomType GetRoomType(Vector3 worldPosition)
        {
            var layoutNode = GetLayoutNode(worldPosition);
            if (layoutNode != null)
            {
                var domainData = layoutNode.GetDomainData<GridFlowTilemapDomainData>();
                if (domainData != null)
                {
                    return domainData.RoomType;
                }
            }

            return GridFlowLayoutNodeRoomType.Unknown;
        }
        
        public IntVector2 WorldPositionToTilemapCoord(Vector3 worldPosition)
        {
            var basePosition = transform.position;
            var localWorld = worldPosition - basePosition;
            var localTileF = MathUtils.Divide(localWorld, gridSize);
            return new IntVector2(
                Mathf.FloorToInt(localTileF.x), 
                Mathf.FloorToInt(localTileF.z));
        }
    }
    
    [System.Serializable]
    public class GridFlowModelLookups : ISerializationCallbackReceiver
    {
        public Dictionary<IntVector2, int> NodeCoordLookup { get; } = new Dictionary<IntVector2, int>();

        public void Build(GridFlowDungeonModel model)
        {
            for (var i = 0; i < model.LayoutGraph.Nodes.Count; i++)
            {
                var node = model.LayoutGraph.Nodes[i];
                var coord = new IntVector2(
                    Mathf.RoundToInt(node.coord.x),
                    Mathf.RoundToInt(node.coord.y));
                NodeCoordLookup[coord] = i;
            }
        }
        
        #region Serialization
        [System.Serializable]
        struct SerializedNodeLookup
        {
            public IntVector2 coord;
            public int index;
        }

        [SerializeField]
        private SerializedNodeLookup[] serializedNodeCoordLookup;
        
        
        public void OnBeforeSerialize()
        {
            var serializedNodeLookupList = new List<SerializedNodeLookup>();
            foreach (var entry in NodeCoordLookup)
            {
                var lookup = new SerializedNodeLookup();
                lookup.coord = entry.Key;
                lookup.index = entry.Value;
                serializedNodeLookupList.Add(lookup);
            }

            serializedNodeCoordLookup = serializedNodeLookupList.ToArray();
        }

        public void OnAfterDeserialize()
        {
            NodeCoordLookup.Clear();
            if (serializedNodeCoordLookup != null)
            {
                foreach (var lookup in serializedNodeCoordLookup)
                {
                    NodeCoordLookup[lookup.coord] = lookup.index;
                }
            }
        }
        #endregion
        
    }
}
