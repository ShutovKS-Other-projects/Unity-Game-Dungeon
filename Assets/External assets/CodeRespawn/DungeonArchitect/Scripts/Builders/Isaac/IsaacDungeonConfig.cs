//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Builders.Isaac
{
    public class IsaacDungeonConfig : DungeonConfig
    {
        public int minRooms;
        public int maxRooms;

        public int roomWidth = 10;
        public int roomHeight = 6;

        public Vector2 tileSize = new Vector2(1, 1);
        public Vector2 roomPadding = new Vector2(1, 1);

        public float growForwardProbablity = 0.75f;
        public float growSidewaysProbablity = 0.25f;

        public float spawnRoomBranchProbablity = 0.75f;
        public float cycleProbability = 1.0f;
        
        public bool Mode2D = false;
        
        public override bool IsMode2D()
        {
            return Mode2D;
        }
    }
}
