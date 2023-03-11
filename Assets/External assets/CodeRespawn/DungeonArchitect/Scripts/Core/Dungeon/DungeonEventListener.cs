//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect
{

	/// <summary>
	/// Listen to various dungeon events during the build and destroy phase
	/// </summary>
	public class DungeonEventListener : MonoBehaviour {
		/// <summary>
        /// Called after the layout is built in memory, but before the markers are emitted
		/// </summary>
		/// <param name="model">The dungeon model</param>
		public virtual void OnPostDungeonLayoutBuild(Dungeon dungeon, DungeonModel model) {}

        /// <summary>
        /// Called after all the markers have been emitted for the level (but before the theming engine is run on those markers)
        /// This gives you an opportunity to modify the markers 
        /// </summary>
        /// <param name="dungeon"></param>
        /// <param name="model"></param>
        public virtual void OnDungeonMarkersEmitted(Dungeon dungeon, DungeonModel model, LevelMarkerList markers) { }

        /// <summary>
        /// Called before the dungeon is built
        /// </summary>
        /// <param name="model">The dungeon model</param>
        public virtual void OnPreDungeonBuild(Dungeon dungeon, DungeonModel model) { }

        /// <summary>
        /// Called after the dungeon is completely built
        /// </summary>
        /// <param name="model">The dungeon model</param>
		public virtual void OnPostDungeonBuild(Dungeon dungeon, DungeonModel model) {}

        /// <summary>
        /// Called after the dungeon is destroyed
        /// </summary>
        /// <param name="model">The dungeon model</param>
        public virtual void OnPreDungeonDestroy(Dungeon dungeon) { }

        /// <summary>
        /// Called after the dungeon is destroyed
        /// </summary>
        /// <param name="model">The dungeon model</param>
        public virtual void OnDungeonDestroyed(Dungeon dungeon) {}
	}
}