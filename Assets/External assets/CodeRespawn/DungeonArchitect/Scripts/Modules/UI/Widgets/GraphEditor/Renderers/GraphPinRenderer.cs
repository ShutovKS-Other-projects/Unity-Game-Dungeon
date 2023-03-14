//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect.Graphs;

namespace DungeonArchitect.UI.Widgets.GraphEditors
{
    /// <summary>
    /// Renders a graph pin hosted inside a node
    /// </summary>
    public class GraphPinRenderer
    {

        public static void Draw(UIRenderer renderer, GraphRendererContext rendererContext, GraphPin pin, GraphCamera camera)
        {
            var pinBounds = new Rect(pin.GetBounds());
			var positionWorld = pin.Node.Position + pinBounds.position;
            var positionScreen = camera.WorldToScreen(positionWorld);
			pinBounds.position = positionScreen;
			pinBounds.size /= camera.ZoomLevel;

            var guiState = new GUIState(renderer);
            renderer.DrawRect(pinBounds, GetPinColor(pin));
            guiState.Restore();

            // Draw the pin texture
            var pinTexture = renderer.GetResource<Texture2D>(UIResourceLookup.TEXTURE_PIN_GLOW) as Texture2D;
			if (pinTexture != null)
            {
                renderer.DrawTexture(pinBounds, pinTexture);
            }
        }

        static Color GetPinColor(GraphPin pin)
        {
            Color color;
            if (pin.ClickState == GraphPinMouseState.Clicked)
            {
                color = GraphEditorConstants.PIN_COLOR_CLICK;
            }
            else if (pin.ClickState == GraphPinMouseState.Hover)
            {
                color = GraphEditorConstants.PIN_COLOR_HOVER;
            }
            else
            {
                color = GraphEditorConstants.PIN_COLOR;
            }
            return color;
        }

    }
}
