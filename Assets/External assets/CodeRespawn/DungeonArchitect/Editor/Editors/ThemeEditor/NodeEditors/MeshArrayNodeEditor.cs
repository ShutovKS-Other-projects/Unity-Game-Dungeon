//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using UnityEditor;
using DungeonArchitect.Graphs;
using DungeonArchitect.UI;
using DungeonArchitect.UI.Widgets.GraphEditors;

namespace DungeonArchitect.Editors
{
	/// <summary>
	/// Custom property editors for GameObjectNode
	/// </summary>
	[CanEditMultipleObjects]
	[CustomEditor(typeof(GameObjectArrayNode))]
	public class MeshArrayNodeEditor : VisualNodeEditor
	{
		SerializedProperty Templates;

		public override void OnEnable()
		{
			base.OnEnable();
			drawOffset = true;
			drawAttachments = true;
			Templates = sobject.FindProperty("Templates");
		}

		protected override void DrawPreInspectorGUI()
		{
			DrawHeader("Game Object Array");
			using (new EditorGUI.IndentLevelScope())
			{
				EditorGUILayout.PropertyField(Templates, true);
			}

			base.DrawPreInspectorGUI();
		}

		protected override string GetNodeTitle()
		{
			return "Game Object Array Node";
		}
	}

	/// <summary>
	/// Renders a mesh node
	/// </summary>
	public class MeshArrayNodeRenderer : VisualNodeRenderer
	{
		protected override Object GetThumbObject(GraphNode node)
		{
			var meshNode = node as GameObjectArrayNode;
			if (meshNode == null || meshNode.Templates == null || meshNode.Templates.Length == 0) return null;
			return meshNode.Templates[0];
		}

		protected override void DrawFrameTexture(UIRenderer renderer, GraphRendererContext rendererContext, GraphNode node, GraphCamera camera) {
			DrawNodeTexture(renderer, rendererContext, node, camera, UIResourceLookup.TEXTURE_GO_NODE_FRAME);
			DrawNodeTexture(renderer, rendererContext, node, camera, UIResourceLookup.TEXTURE_MULTI_GO_NODE_FRAME);
		}
	}

}
