//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Graphs
{
	
	[System.Serializable]
	public class GameObjectNode : VisualNode {
		public GameObject Template;

		public override void Initialize(string id, Graph graph) {
			base.Initialize(id, graph);
			UpdateName("MeshNode_");

			if (caption == null) {
				caption = "Game Object Node";
			}
		}

        public override void CopyFrom(GraphNode node)
        {
            base.CopyFrom(node);

            var goNode = node as GameObjectNode;
            if (goNode == null) return;

            Template = goNode.Template;
        }
	}

}