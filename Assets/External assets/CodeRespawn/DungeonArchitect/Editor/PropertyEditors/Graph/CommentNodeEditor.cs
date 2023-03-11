//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Editors;
using UnityEngine;
using UnityEditor;
using DungeonArchitect.Graphs;

namespace DungeonArchitect.UI.Widgets.GraphEditors
{
    /// <summary>
    /// Custom property editors for MarkerNode
    /// </summary>
    [CustomEditor(typeof(CommentNode))]
    public class CommentNodeEditor : Editor
    {
        SerializedObject sobject;
        SerializedProperty message;
        SerializedProperty background;
        const int CATEGORY_SPACING = 10;

        public void OnEnable()
        {
            sobject = new SerializedObject(target);
            message = sobject.FindProperty("message");
            background = sobject.FindProperty("background");
        }

        public override void OnInspectorGUI()
        {
            sobject.Update();
            
            GUILayout.Label("Comment Node", InspectorStyles.TitleStyle);
            GUILayout.Space(CATEGORY_SPACING);
            
            message.stringValue = EditorGUILayout.TextArea(message.stringValue, GUILayout.MinHeight(60));
            EditorGUILayout.PropertyField(background);
            sobject.ApplyModifiedProperties();
        }
        
    }

    /// <summary>
    /// Renders a comment node
    /// </summary>
    public class CommentNodeRenderer : GraphNodeRenderer
    {
        Color textColor = Color.white;
        public CommentNodeRenderer(Color textColor)
        {
            this.textColor = textColor;
        }

        public override void Draw(UIRenderer renderer, GraphRendererContext rendererContext, GraphNode node, GraphCamera camera)
        {
            var commentNode = node as CommentNode;

            DrawMessage(renderer, rendererContext, commentNode, camera);
            
            // Draw the pins
            base.Draw(renderer, rendererContext, node, camera);

            if (camera.ZoomLevel >= 1.5f)
            {
                var nodeScreen = camera.WorldToScreen(node.Bounds);
                if (nodeScreen.Contains(rendererContext.GraphEditor.LastMousePosition))
                {
                    GraphTooltip.message = commentNode.message;
                }
            }
        }

        void DrawMessage(UIRenderer renderer, GraphRendererContext rendererContext, CommentNode node, GraphCamera camera)
        {
            var style = new GUIStyle(EditorStyles.label);
            style.alignment = TextAnchor.UpperLeft;

            style.normal.textColor = node.Selected ? GraphEditorConstants.TEXT_COLOR_SELECTED : GraphEditorConstants.TEXT_COLOR;

            {
                style.font = EditorStyles.standardFont;
                float scaledFontSize = style.fontSize == 0 ? style.font.fontSize : style.fontSize;
                scaledFontSize = Mathf.Max(1.0f, scaledFontSize / camera.ZoomLevel);
                style.fontSize = Mathf.RoundToInt(scaledFontSize);
            }

            var guiState = new GUIState(renderer);
            renderer.backgroundColor = node.background;

            // Update the node bounds
            var padding = new Vector2(10, 10);
            var screenPadding = padding / camera.ZoomLevel;
            var textSize = style.CalcSize(new GUIContent(node.message));
            var nodeSize = textSize + screenPadding * 2;

            Rect boxBounds;
            {
                var positionScreen = camera.WorldToScreen(node.Position);
                var sizeScreen = nodeSize;
                boxBounds = new Rect(positionScreen, sizeScreen);
            }

            Rect textBounds;
            {
                var positionScreen = camera.WorldToScreen(node.Position + padding);
                var sizeScreen = textSize;
                textBounds = new Rect(positionScreen, sizeScreen);
            }

            renderer.DrawRect(boxBounds, node.background);
            style.normal.textColor = textColor;
            renderer.Label(textBounds, node.message, style);

            var updateWorldSize = nodeSize * camera.ZoomLevel;
            {
                var nodeBounds = node.Bounds;
                nodeBounds.size = updateWorldSize;
                node.Bounds = nodeBounds;
            }

            guiState.Restore();
        }
    }
}
