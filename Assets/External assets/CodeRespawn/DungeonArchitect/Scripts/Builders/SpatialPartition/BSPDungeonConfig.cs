//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Builders.BSP
{
    [System.Serializable]
    public struct BSPRoomCategory
    {
        public string category;
        public int width;
        public int length;
        public int minOccurance;
        public int maxOccurance;
        public Color debugColor;
    }

    public class BSPDungeonConfig : DungeonConfig {
        public Vector2 gridSize = new Vector2(4, 4);

        /// <summary>
        /// The width of the dungeon in tile coords
        /// </summary>
        public int dungeonWidth = 32;
        
        /// <summary>
        /// The length of the dungeon in tile coords
        /// </summary>
        public int dungeonLength = 24;


        public int minRoomSize = 3;
        public int maxRoomSize = 8;

        /// <summary>
        /// Larger split probability will create small rooms close to the minRoomSize
        /// Smaller values will create larger rooms closers to the max room size since they are not split further
        /// </summary>
        public float smallerRoomProbability = 0.5f;

        public float unevenSplitProbability = 0.0f;
        
        public int roomPadding = 1;

        public float loopingProbability = 0;

        public int randomKillDepthStart = 3;
        public float randomKillProbability = 0.2f;

        public float minAspectRatio = 0.7f;

        public BSPRoomCategory[] customRooms;
        
        public bool Mode2D = false;
        
        public override bool IsMode2D()
        {
            return Mode2D;
        }
    }
}
