//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Navigation {
	/// <summary>
	/// Drop this script into your dungeon object and assign the nav mesh prefab to
	/// automatically rebuild the nav mesh whenever the dungeon is rebuild (works both with runtime and design time)
	/// </summary>
	public class NavigationBuildInvoker : DungeonEventListener {
		public DungeonNavMesh navMesh;

		/// <summary>
		/// Called after the dungeon is completely built
		/// </summary>
		/// <param name="model">The dungeon model</param>
		public override void OnPostDungeonBuild(Dungeon dungeon, DungeonModel model) {
			if (navMesh != null) {
				navMesh.Build();
			}
			else {
				Debug.LogWarning("Cannot automatically rebuild nav mesh as it is not assigned to the dungeon event listener");
			}
		}

	}
}
