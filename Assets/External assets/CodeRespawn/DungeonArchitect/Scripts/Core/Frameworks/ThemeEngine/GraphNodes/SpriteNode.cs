//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Graphs
{
	public enum DungeonSpriteCollisionType {
		None,
		Box,
		Circle,
		Polygon,
	}

	[System.Serializable]
	public class SpriteNode : VisualNode {
		public Sprite sprite;
		public Color color = new Color(1, 1, 1, 1);
		public Material materialOverride;
		public string sortingLayerName;
		public int orderInLayer;

		// Physics properties
		public DungeonSpriteCollisionType collisionType = DungeonSpriteCollisionType.None;
		public PhysicsMaterial2D physicsMaterial;
		public Vector2 physicsOffset = Vector2.zero;
		public Vector2 physicsSize = Vector2.one;
		public float physicsRadius = 0.5f;


		public override void Initialize(string id, Graph graph) {
			base.Initialize(id, graph);
			UpdateName("SpriteNode_");
			
			if (caption == null) {
				caption = "Sprite Node";
			}

		}
		
		public override void CopyFrom(GraphNode node)
		{
			base.CopyFrom(node);
			
			var spriteNode = node as SpriteNode;
			if (spriteNode == null) return;
			
			sprite = spriteNode.sprite;
			color = spriteNode.color;
			materialOverride = spriteNode.materialOverride;
			sortingLayerName = spriteNode.sortingLayerName;
			orderInLayer = spriteNode.orderInLayer;

		}
	}
	
}