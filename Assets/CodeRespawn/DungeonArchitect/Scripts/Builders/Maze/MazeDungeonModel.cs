//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Builders.Maze
{
	public enum MazeTileState {
		Empty,
        Blocked
	}

	public class MazeDungeonModel : DungeonModel {

		[HideInInspector]
		public MazeDungeonConfig Config;

		[HideInInspector]
		public MazeTileState[,] tileStates;
    }
}
