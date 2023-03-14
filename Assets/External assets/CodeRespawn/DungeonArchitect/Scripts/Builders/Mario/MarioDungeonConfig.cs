//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Builders.Mario
{
    [System.Serializable]
    public class MarioDungeonLevelChunkRule
    {
        public int numTiles = 5;
        public string markerName = "LevelChunk5x";
        public float probablity = 0.5f;
    }

	public class MarioDungeonConfig : DungeonConfig {
		public int minLength = 20;  
		public int maxLength = 25;

        public int minY = -2;
        public int maxY = 20;
        public int minDepth = -4;
        public int maxDepth = 15;

        public int minGap = 2;
        public int maxGap = 6;

        public int minNonGap = 4;

        public int maxStairHeight = 1;
        public float heightVariationProbablity = 0.1f;

        public float gapProbability = 0.1f;

        public int maxJumpTileDistance = 1;

        public Vector3 gridSize = new Vector3(4, 2, 4);

        public MarioDungeonLevelChunkRule[] chunkMarkers;
        
        public bool Mode2D = false;
        
        public override bool IsMode2D()
        {
            return Mode2D;
        }
    }
}
