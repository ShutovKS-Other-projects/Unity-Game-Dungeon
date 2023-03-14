//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.UI;
using DungeonArchitect.Grammar;
using DungeonArchitect.Graphs;
using UnityEditor;
using UnityEngine;
using DMathUtils = DungeonArchitect.Utils.MathUtils;
using DungeonArchitect.UI.Widgets.GraphEditors;

namespace DungeonArchitect.Editors.SnapFlow
{

    public abstract class GrammarNodeRendererBase : GraphNodeRenderer
    {
        protected abstract string GetCaption(GraphNode node);
        protected abstract Color GetPinColor(GraphNode node);
        protected virtual Color GetBodyColor(GraphNode node)
        {
            return new Color(0.1f, 0.1f, 0.1f, 1);
        }

        public override void Draw(UIRenderer renderer, GraphRendererContext rendererContext, GraphNode node, GraphCamera camera)
        {
            var pinColor = GetPinColor(node);

            NodeBoundLayoutData layoutData;
            {
                // Update the node bounds
                var style = GetNodeStyle(node, camera.ZoomLevel);
                var contentScreenSize = GetContentScreenSize(node, style);
                UpdateNodeBounds(node, camera.ZoomLevel, contentScreenSize, style, out layoutData);
            }
            
            // Draw the pins
            DrawPins(renderer, rendererContext, node, camera, pinColor);

            // Draw the node body
            DrawNodeBody(renderer, layoutData, node, camera);
        }

        void DrawPins(UIRenderer renderer, GraphRendererContext rendererContext, GraphNode node, GraphCamera camera, Color nodeColor)
        {
            var outputPin = node.OutputPin;
            var nodeSize = node.Bounds.size;
            var offset = nodeSize / 2.0f;
            outputPin.Position = offset;
            outputPin.BoundsOffset = new Rect(-offset, nodeSize);

            var inputPin = node.InputPin;
            inputPin.Position = offset;
            inputPin.BoundsOffset = outputPin.BoundsOffset;

            DrawPin(renderer, rendererContext, outputPin, camera, nodeColor);
        }

        class GrammarNodeEditorConstants
        {
            public static readonly Color PIN_COLOR_HOVER = new Color(1, 0.6f, 0.0f);
            public static readonly Color PIN_COLOR_CLICK = new Color(1, 0.9f, 0.0f);
            public static readonly Color FOCUS_HIGHLITE_COLOR = new Color(1, 0.5f, 0, 1);
            public static readonly Vector2 BASE_PADDING = new Vector2(16, 16);
        }

        static Color GetPinColor(GraphPin pin, Color nodeColor)
        {
            Color color;
            if (pin.ClickState == GraphPinMouseState.Clicked)
            {
                color = GrammarNodeEditorConstants.PIN_COLOR_CLICK;
            }
            else if (pin.ClickState == GraphPinMouseState.Hover)
            {
                color = GrammarNodeEditorConstants.PIN_COLOR_HOVER;
            }
            else
            {
                color = nodeColor;
            }
            return color;
        }

        void DrawPin(UIRenderer renderer, GraphRendererContext rendererContext, GraphPin pin, GraphCamera camera, Color nodeColor)
        {
            if (pin.Node == null)
            {
                Debug.Log("pin node null. skipping");
                return;

            }
            var guiState = new GUIState(renderer);

            var pinBounds = new Rect(pin.GetBounds());
            var positionWorld = pin.Node.Position + pinBounds.position;
            var positionScreen = camera.WorldToScreen(positionWorld);
            pinBounds.position = positionScreen;
            pinBounds.size /= camera.ZoomLevel;

            if (pin.Node != null && pin.Node.Selected)
            {
                var focusboundsScreen = DMathUtils.ExpandRect(pinBounds, 2);
                renderer.DrawRect(focusboundsScreen, GrammarNodeEditorConstants.FOCUS_HIGHLITE_COLOR);
            }

            renderer.DrawRect(pinBounds, GetPinColor(pin, nodeColor));

            guiState.Restore();

        }

        public void UpdateNodeBounds(GraphNode node, float zoomLevel)
        {
            var style = GetNodeStyle(node, zoomLevel);
            var contentScreenSize = GetContentScreenSize(node, style);
            NodeBoundLayoutData layoutData;
            UpdateNodeBounds(node, zoomLevel, contentScreenSize, style, out layoutData);
        }

        struct NodeBoundLayoutData
        {
            public Vector2 screenTextSize;
            public Vector2 screenNodeSize;
        }
        static void UpdateNodeBounds(GraphNode node, float zoomLevel, Vector2 contentScreenSize, GUIStyle style, out NodeBoundLayoutData outLayoutData)
        {
            var screenPadding = GrammarNodeEditorConstants.BASE_PADDING / zoomLevel;
            var screenTextSize = contentScreenSize;
            var screenNodeSize = screenTextSize + screenPadding * 2;
            var updateWorldSize = screenNodeSize * zoomLevel;
            var nodeBounds = node.Bounds;
            nodeBounds.size = updateWorldSize;
            node.Bounds = nodeBounds;

            outLayoutData = new NodeBoundLayoutData();
            outLayoutData.screenTextSize = screenTextSize;
            outLayoutData.screenNodeSize = screenNodeSize;
        }

        public static GUIStyle GetNodeStyle(GraphNode node, float zoomLevel)
        {
            var style = new GUIStyle(EditorStyles.boldLabel);
            style.alignment = TextAnchor.UpperLeft;

            style.normal.textColor = node.Selected ? GraphEditorConstants.TEXT_COLOR_SELECTED : GraphEditorConstants.TEXT_COLOR;

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

        protected virtual void DrawNodeContent(UIRenderer renderer, GraphNode node, GUIStyle style, Rect bounds)
        {
            string nodeMessage = GetCaption(node);
            var content = new GUIContent(nodeMessage);

            Color textColor = Color.white;
            style.normal.textColor = textColor;
            renderer.Label(bounds, content, style);
        }

        void DrawNodeBody(UIRenderer renderer, NodeBoundLayoutData layoutData, GraphNode node, GraphCamera camera)
        {
            var style = GetNodeStyle(node, camera.ZoomLevel);

            var guiState = new GUIState(renderer);
            Color nodeColor = GetBodyColor(node);

            var outputPin = node.OutputPin as GrammarNodePin;
            var pinPadding = new Vector2(12, 12);
            var screenPinPadding = pinPadding / camera.ZoomLevel;
            if (outputPin != null)
            {
                outputPin.Padding = pinPadding;
            }

            // Draw the node pin
            Rect boxBounds;
            {
                var positionScreen = camera.WorldToScreen(node.Position + pinPadding);
                var sizeScreen = layoutData.screenNodeSize - screenPinPadding * 2;
                boxBounds = new Rect(positionScreen, sizeScreen);
            }
            renderer.DrawRect(boxBounds, nodeColor);

            // Draw the node content
            Rect textBounds;
            {
                var positionScreen = camera.WorldToScreen(node.Position + GrammarNodeEditorConstants.BASE_PADDING);
                var sizeScreen = layoutData.screenTextSize;
                textBounds = new Rect(positionScreen, sizeScreen);
            }

            DrawNodeContent(renderer, node, style, textBounds);

            guiState.Restore();
        }
    }
}
