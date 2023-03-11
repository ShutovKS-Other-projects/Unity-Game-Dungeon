//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using UnityEditor;
using DungeonArchitect.Graphs;
using DungeonArchitect.UI;
using DungeonArchitect.UI.Widgets.GraphEditors;

namespace DungeonArchitect.Editors
{
    /// <summary>
    /// Custom property editors for MarkerNode
    /// </summary>
    [CustomEditor(typeof(MarkerNode))]
    public class MarkerNodeEditor : Editor
    {
        SerializedObject sobject;
        SerializedProperty caption;
        const int CATEGORY_SPACING = 10;
        
        public void OnEnable()
        {
            sobject = new SerializedObject(target);
            caption = sobject.FindProperty("caption");
        }

        public override void OnInspectorGUI()
        {
            sobject.Update();
            GUILayout.Label("Marker Node", InspectorStyles.TitleStyle);
            GUILayout.Space(CATEGORY_SPACING);
            
            EditorGUILayout.PropertyField(caption, new GUIContent("Marker Name"));
            sobject.ApplyModifiedProperties();
        }
    }


    /// <summary>
    /// Renders a marker node
    /// </summary>
    public class MarkerNodeRenderer : GraphNodeRenderer
    {
        public override void Draw(UIRenderer renderer, GraphRendererContext rendererContext, GraphNode node, GraphCamera camera)
        {
            // Draw the background base texture
            DrawNodeTexture(renderer, rendererContext, node, camera, UIResourceLookup.TEXTURE_MARKER_NODE_BG);

			var pinHeight = node.OutputPins[0].BoundsOffset.height;
			DrawTextCentered(renderer, rendererContext, node, camera, node.Caption, new Vector2(0, -3));

            // Draw the foreground frame textures
            DrawNodeTexture(renderer, rendererContext, node, camera, UIResourceLookup.TEXTURE_MARKER_NODE_FRAME);

            if (node.Selected)
            {
                DrawNodeTexture(renderer, rendererContext, node, camera, UIResourceLookup.TEXTURE_MARKER_NODE_SELECTION);
            }

            // Draw the pins
            base.Draw(renderer, rendererContext, node, camera);

        }
    }
}
