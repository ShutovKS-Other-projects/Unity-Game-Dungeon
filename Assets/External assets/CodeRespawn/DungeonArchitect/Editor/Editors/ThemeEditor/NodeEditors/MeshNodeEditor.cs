//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using UnityEditor;
using DungeonArchitect.Graphs;

namespace DungeonArchitect.Editors
{
    /// <summary>
    /// Custom property editors for GameObjectNode
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GameObjectNode))]
    public class MeshNodeEditor : VisualNodeEditor
    {
        SerializedProperty Template;

        public override void OnEnable()
        {
            base.OnEnable();
            drawOffset = true;
            drawAttachments = true;
            Template = sobject.FindProperty("Template");
        }

        protected override void DrawPreInspectorGUI()
        {
            DrawHeader("Game Object");
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(Template);
            }

            base.DrawPreInspectorGUI();
        }
        
        protected override string GetNodeTitle()
        {
            return "Game Object Node";
        }
    }

    /// <summary>
    /// Renders a mesh node
    /// </summary>
    public class MeshNodeRenderer : VisualNodeRenderer
    {
        protected override Object GetThumbObject(GraphNode node)
        {
            var meshNode = node as GameObjectNode;
            if (meshNode == null) return null;
            return meshNode.Template;
        }
    }

}
