//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEditor;
using UnityEngine;
using DMathUtils = DungeonArchitect.Utils.MathUtils;
using DungeonArchitect.Graphs;
using DungeonArchitect.UI;
using DungeonArchitect.UI.Widgets.GraphEditors;

namespace DungeonArchitect.Editors.Flow.Layout
{
    public abstract class FlowPreviewLayoutNodeRendererBase : GraphNodeRenderer
    {
        protected abstract string GetCaption(GraphNode node);
        protected virtual Color GetBodyColor(GraphNode node)
        {
            return new Color(0.2f, 0.3f, 0.2f, 1);
        }

        public override void Draw(UIRenderer renderer, GraphRendererContext rendererContext, GraphNode node, GraphCamera camera)
        {
            // Draw the node body
            DrawNodeBody(renderer, rendererContext, node, camera);
        }

        class GridFlowPreviewEditorConstants
        {
            public static readonly Color NODE_BORDER_COLOR = new Color(0.1f, 0.1f, 0.1f);
            public static readonly Color NODE_BORDER_COLOR_SELECTED = new Color(1, .5f, 0, 1);
            public static readonly Vector2 BASE_PADDING = new Vector2(24, 24);
        }

        public static GUIStyle GetNodeStyle(GraphNode node, float zoomLevel)
        {
            var style = new GUIStyle(EditorStyles.boldLabel);
            style.alignment = TextAnchor.UpperLeft;

            style.normal.textColor = node.Selected ? GraphEditorConstants.TEXT_COLOR_SELECTED : GraphEditorConstants.TEXT_COLOR;
            style.alignment = TextAnchor.MiddleCenter;
            {
                style.font = EditorStyles.boldFont;
                float scaledFontSize = style.fontSize == 0 ? style.font.fontSize : style.fontSize;
                scaledFontSize = Mathf.Max(1.0f, scaledFontSize / zoomLevel);
                style.fontSize = Mathf.RoundToInt(scaledFontSize);
            }
            return style;
        }

        protected virtual Vector2 GetContentScreenSize(GraphNode node, GUIStyle style)
        {
            string nodeMessage = GetCaption(node);
            var content = new GUIContent(nodeMessage);
            return style.CalcSize(content);
        }

        protected virtual void DrawNodeContent(UIRenderer renderer, GraphNode node, GUIStyle style, Rect bounds, Color textColor)
        {
            string nodeMessage = GetCaption(node);
            var content = new GUIContent(nodeMessage);

            style.normal.textColor = textColor;
            renderer.Label(bounds, content, style);
        }

        public static void DrawCircle(UIRenderer renderer, Rect bounds, Color bodyColor, Color borderColor, float border)
        {
            var bodyTexture = renderer.GetResource<Texture2D>(UIResourceLookup.TEXTURE_CURSOR_RING_SOLID) as Texture2D;
            if (border > 0)
            {
                var borderBounds = bounds;
                borderBounds.position -= new Vector2(border, border);
                borderBounds.size += new Vector2(border, border) * 2;
                renderer.DrawTexture(borderBounds, bodyTexture, ScaleMode.StretchToFill, true, borderColor);
            }
            renderer.DrawTexture(bounds, bodyTexture, ScaleMode.StretchToFill, true, bodyColor);
        }

        protected virtual void DrawNodeBody(UIRenderer renderer, GraphRendererContext rendererContext, GraphNode node, GraphCamera camera)
        {
            var guiState = new GUIState(renderer);

            // Draw the node pin
            Rect boxBounds;
            {
                var positionScreen = camera.WorldToScreen(node.Position);
                var sizeScreen = node.Bounds.size / camera.ZoomLevel;
                boxBounds = new Rect(positionScreen, sizeScreen);
            }

            Color bodyColor = GetBodyColor(node);
            var borderColor = node.Selected
                ? GridFlowPreviewEditorConstants.NODE_BORDER_COLOR_SELECTED 
                : GridFlowPreviewEditorConstants.NODE_BORDER_COLOR;

            DrawCircle(renderer, boxBounds, bodyColor, borderColor, 2);


            var style = GetNodeStyle(node, camera.ZoomLevel);
            DrawNodeContent(renderer, node, style, boxBounds, Color.black);

            guiState.Restore();
        }
    }
}
