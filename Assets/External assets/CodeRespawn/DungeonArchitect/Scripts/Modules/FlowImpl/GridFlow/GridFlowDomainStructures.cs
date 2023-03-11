//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System;
using DungeonArchitect.Flow.Domains;
using DungeonArchitect.Flow.Domains.Tilemap.Tasks;

namespace DungeonArchitect.Flow.Impl.GridFlow
{
    [System.Serializable]
    public enum GridFlowLayoutNodeRoomType
    {
        Unknown,
        Room,
        Corridor,
        Cave
    }
    
    public class GridFlowTilemapDomainData : IFlowDomainData
    {
        public GridFlowLayoutNodeRoomType RoomType = GridFlowLayoutNodeRoomType.Unknown;
        public IFlowDomainData Clone()
        {
            var clone = new GridFlowTilemapDomainData();
            clone.RoomType = RoomType;
            return clone;
        }
    }


    public class GridFlowTilemapState : ICloneable
    {
        public TilemapFlowNodeWallGenerationMethod WallGenerationMethod = TilemapFlowNodeWallGenerationMethod.WallAsTiles;

        public object Clone()
        {
            var clone = new GridFlowTilemapState();
            clone.WallGenerationMethod = WallGenerationMethod;
            return clone;
        }
    }
    
    public class GridFlowLayoutNodeState : IFlowDomainData
    {
        public bool CanPerturb = true;
        public IFlowDomainData Clone()
        {
            var clone = new GridFlowLayoutNodeState();
            clone.CanPerturb = CanPerturb;
            return clone;
        }
    }
}
