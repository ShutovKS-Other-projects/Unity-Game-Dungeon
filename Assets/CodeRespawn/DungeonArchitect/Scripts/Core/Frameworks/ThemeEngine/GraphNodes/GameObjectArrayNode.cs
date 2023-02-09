//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Graphs
{
	[System.Serializable]
	public class GameObjectArrayNode : VisualNode {
		public GameObject[] Templates;

		public override void Initialize(string id, Graph graph) {
			base.Initialize(id, graph);
			UpdateName("GameObjectArrayNode_");

			if (caption == null) {
				caption = "Game Object Array Node";
			}
		}

		public override void CopyFrom(GraphNode node)
		{
			base.CopyFrom(node);

			var goNode = node as GameObjectArrayNode;
			if (goNode == null) return;
			Templates = new GameObject[goNode.Templates.Length];
			System.Array.Copy (goNode.Templates, Templates, goNode.Templates.Length);
		}
	}

}