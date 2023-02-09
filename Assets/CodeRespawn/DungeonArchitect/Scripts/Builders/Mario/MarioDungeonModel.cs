//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Builders.Mario
{
	public enum MarioTileType {
		Ground,
        Corridor,
        Gap
	}

	public class MarioTile {
		public IntVector position;
		public MarioTileType tileType;
        public string[] chunkMarkers;
    }

	public class MarioDungeonModel : DungeonModel {

		[HideInInspector]
		public MarioDungeonConfig Config;

		[HideInInspector]
		public MarioTile[] tiles;

        [HideInInspector]
        public int levelWidth;


    }
}
