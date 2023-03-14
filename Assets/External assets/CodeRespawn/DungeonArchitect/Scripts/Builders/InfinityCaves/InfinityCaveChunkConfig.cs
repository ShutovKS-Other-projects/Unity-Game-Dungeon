//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect.Utils;

namespace DungeonArchitect.Builders.Infinity.Caves
{
	public class InfinityCaveChunkConfig : InfinityDungeonConfig
    {
        public Vector2 gridSize = new Vector2(4, 4);
        public int iterations = 4;
        public int neighborRocks = 5;

        public override Vector3 GetLogicalCoord(Vector3 p)
        {
            return MathUtils.Divide(p, new Vector3(gridSize.x, 1, gridSize.y));
        }

        public override bool BuildAlongX() { return true; }
        public override bool BuildAlongY() { return false; }
        public override bool BuildAlongZ() { return true; }
    }
}
