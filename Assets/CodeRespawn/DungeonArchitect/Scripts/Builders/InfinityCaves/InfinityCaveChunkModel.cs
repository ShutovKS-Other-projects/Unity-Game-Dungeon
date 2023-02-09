//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Builders.Infinity.Caves
{
	public enum MazeTileState {
		Empty,
        Rock,
        Wall
	}

	public class InfinityCaveChunkModel : DungeonModel {
		[HideInInspector]
		public InfinityCaveChunkConfig Config;

		[HideInInspector]
		public MazeTileState[,] tileStates;
    }
}
