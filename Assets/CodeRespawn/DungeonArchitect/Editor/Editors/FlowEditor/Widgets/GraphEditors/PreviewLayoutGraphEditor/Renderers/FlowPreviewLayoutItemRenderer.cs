//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Flow.Items;
using DungeonArchitect.Graphs;
using DungeonArchitect.UI;
using DungeonArchitect.UI.Widgets.GraphEditors;
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Editors.Flow.Layout
{
    public delegate void OnFlowLayoutGraphItemRendered(FlowItem item, Rect screenBounds);
    public class FlowPreviewLayoutItemRenderer 
    {
        public static float ItemRadiusFactor { get; private set; }
        public static float ItemHoverScaleFactor { get; private set; }

        static FlowPreviewLayoutItemRenderer()
        {
            ItemRadiusFactor = 0.35f;
            ItemHoverScaleFactor = 1.25f;
        }

        public static float GetItemScaleFactor(Vector2 mousePosition, Rect itemBounds)
        {
            var radius = itemBounds.size.x * 0.5f;
            var itemCenter = itemBounds.center;
            var distance = (mousePosition - itemCenter).magnitude - radius;
            distance = 1 - Mathf.Clamp01(distance / (itemBounds.size.x * 0.25f));
            distance = distance * distance;

            var scaleFactor = Mathf.Lerp(1.0f, ItemHoverScaleFactor, distance);
            return scaleFactor;
        }

        public static void DrawItem(UIRenderer renderer, GraphRendererContext rendererContext, GraphCamera camera, FlowItem item, Rect itemBounds, float textScaleFactor)
        {
            // Draw the item background circle
            Color colorBackground, colorForeground;
            FlowItemUtils.GetFlowItemColor(item, out colorBackground, out colorForeground);
            var borderColor = item.editorSelected ? Color.red : Color.black;
            var thickness = item.editorSelected ? 3 : 1;
            FlowPreviewLayoutNodeRendererBase.DrawCircle(renderer, itemBounds, colorBackground, borderColor, thickness);

            // Draw the item text
            {
                var text = FlowItemUtils.GetFlowItemText(item);
                var style = new GUIStyle(EditorStyles.boldLabel);
                style.normal.textColor = colorForeground;
                style.alignment = TextAnchor.MiddleCenter;
                style.font = EditorStyles.boldFont;
                float scaledFontSize = (style.fontSize == 0) ? style.font.fontSize : style.fontSize;
                scaledFontSize *= textScaleFactor;
                scaledFontSize = Mathf.Max(1.0f, scaledFontSize / camera.ZoomLevel);
                style.fontSize = Mathf.RoundToInt(scaledFontSize);
                renderer.Label(itemBounds, text, style);
            }
        }

    }
}
