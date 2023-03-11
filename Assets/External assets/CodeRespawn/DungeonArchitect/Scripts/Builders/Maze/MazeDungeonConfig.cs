//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Builders.Maze
{
	public class MazeDungeonConfig : DungeonConfig {
		public int mazeWidth = 20;  
		public int mazeHeight = 25;
        
        public Vector2 gridSize = new Vector2(4, 4);
        
        public bool Mode2D = false;
        
        public override bool IsMode2D()
        {
	        return Mode2D;
        }
	}
}
