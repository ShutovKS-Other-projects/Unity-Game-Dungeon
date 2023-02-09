//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.UI.Widgets.GraphEditors
{
    /// <summary>
    /// Graph tooltip singleton
    /// </summary>
    public class GraphTooltip
    {
        /// <summary>
        /// Set this to display a tooltip in the graph editor
        /// </summary>
        public static string message = "";
        public static void Clear()
        {
            message = "";
        }
    }

    /// <summary>
    /// Renders a tooltip in the graph editor. The tooltip message is defined in GraphTooltip.message
    /// </summary>
    public class GraphTooltipRenderer
    {
        public static void Draw(UIRenderer renderer, GraphRendererContext rendererContext, Vector2 mousePosition)
        {
            if (GraphTooltip.message == null || GraphTooltip.message.Trim().Length == 0) return;

            var tooltipPadding = new Vector2(4, 4);

            var drawPosition = mousePosition + new Vector2(15, 5);

            var tooltipString = GraphTooltip.message;
            var style = new GUIStyle(GUI.skin.GetStyle("label"));
            var contentSize = style.CalcSize(new GUIContent(tooltipString));
            drawPosition -= tooltipPadding;
            contentSize += tooltipPadding * 2;
            var bounds = new Rect(drawPosition.x, drawPosition.y, contentSize.x, contentSize.y);

            var guiState = new GUIState(renderer);
            renderer.backgroundColor = new Color(.15f, .15f, .15f);
            renderer.DrawRect(bounds, new Color(.15f, .15f, .15f));

            var innerGlow = renderer.GetResource<Texture2D>(UIResourceLookup.TEXTURE_PIN_GLOW) as Texture2D;
            renderer.DrawTexture(bounds, innerGlow);

            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.white;
            renderer.Label(bounds, tooltipString, style);
            guiState.Restore();
        }
    }
}
