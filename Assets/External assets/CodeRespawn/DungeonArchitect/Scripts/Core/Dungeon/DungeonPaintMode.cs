//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect
{
    /// <summary>
    /// Manage the editor paint mode so you can paint the layout of you dungeon.
    /// You should implement your own paint mode depending on your dungeon builder's 
    /// data structures and requirements
    /// </summary>
	public abstract class DungeonPaintMode : MonoBehaviour {
		private Dungeon dungeon;
		private DungeonModel dungeonModel;
		private DungeonConfig dungeonConfig;
		private DungeonToolData dungeonToolData;

        /// <summary>
        /// Gets the configuration of the dungeon
        /// </summary>
        /// <returns></returns>
		public DungeonConfig GetDungeonConfig() {
			if (dungeonConfig == null) {
				dungeonConfig = GetSiblingComponent<DungeonConfig>();
			}
			return dungeonConfig;
		}

        /// <summary>
        /// Gets the model used by the owning dungeon
        /// </summary>
        /// <returns></returns>
        public DungeonModel GetDungeonModel()
        {
			if (dungeonModel == null) {
				dungeonModel = GetSiblingComponent<DungeonModel>();
			}
			return dungeonModel;
		}

        /// <summary>
        /// Gets the owning dungeon
        /// </summary>
        /// <returns>The owning dungeon</returns>
        public Dungeon GetDungeon()
        {
			if (dungeon == null) {
				dungeon = GetSiblingComponent<Dungeon>();
			}
			return dungeon;
		}

        public DungeonToolData GetToolData()
        {
	        if (dungeonToolData == null) {
		        dungeonToolData = GetSiblingComponent<DungeonToolData>();
	        }
	        return dungeonToolData;
        }

        
		public T GetSiblingComponent<T>() {
			var parentTransform = gameObject.transform.parent;
			if (parentTransform != null && parentTransform.gameObject != null) {
				var dungeonGO = parentTransform.gameObject;
				return dungeonGO.GetComponent<T>();
			}
			return default(T);
		}
	}
}