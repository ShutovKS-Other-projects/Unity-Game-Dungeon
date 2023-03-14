//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System;
using UnityEngine;
using System.Collections.Generic;
using DungeonArchitect.Graphs;

namespace DungeonArchitect.UI.Widgets.GraphEditors
{
    /// <summary>
    /// Renders the graph node in the graph editor
    /// </summary>
    public class GraphNodeRenderer
    {

        protected virtual Color getBackgroundColor(GraphNode node)
        {
            return node.Selected ? GraphEditorConstants.NODE_COLOR_SELECTED : GraphEditorConstants.NODE_COLOR;
        }

        public virtual void Draw(UIRenderer renderer, GraphRendererContext rendererContext, GraphNode node, GraphCamera camera)
        {
            // Draw the pins
            if (node.InputPins != null)
            {
                foreach (var pin in node.InputPins)
                {
                    GraphPinRenderer.Draw(renderer, rendererContext, pin, camera);
                }
            }
            if (node.OutputPin != null)
            {
                foreach (var pin in node.OutputPins)
                {
                    GraphPinRenderer.Draw(renderer, rendererContext, pin, camera);
                }
            }
        }

        public virtual void Release() { }

		protected virtual void DrawTextCentered(UIRenderer renderer, GraphRendererContext rendererContext, GraphNode node, GraphCamera camera, string text) {
			DrawTextCentered (renderer, rendererContext, node, camera, text, Vector2.zero);
		}

		protected virtual void DrawTextCentered(UIRenderer renderer, GraphRendererContext rendererContext, GraphNode node, GraphCamera camera, string text, Vector2 offset) {
			var style = new GUIStyle(GUI.skin.GetStyle("Label"));
			style.alignment = TextAnchor.MiddleCenter;

			var positionScreen = camera.WorldToScreen(node.Position + offset);
			var labelSize = new Vector2 (node.Bounds.width, node.Bounds.height) / camera.ZoomLevel;
			var labelBounds = new Rect(positionScreen.x, positionScreen.y, labelSize.x, labelSize.y);
			style.normal.textColor = node.Selected ? GraphEditorConstants.TEXT_COLOR_SELECTED : GraphEditorConstants.TEXT_COLOR;

			var originalFont = style.font;
			var originalFontSize = style.fontSize;
			var miniFontBaseSize = 20;

			if (camera.ZoomLevel >= 2) {

				float scaledFontSize = originalFontSize;
				if (scaledFontSize == 0) {
					scaledFontSize = miniFontBaseSize;
				}
				scaledFontSize = Mathf.Max(1.0f, scaledFontSize / camera.ZoomLevel);

				style.fontSize = Mathf.RoundToInt(scaledFontSize);
                style.font = renderer.StyleManager.GetFontMini();
			}

            renderer.Label(labelBounds, text, style);

			style.font = originalFont;
			style.fontSize = originalFontSize;
		}

        protected void DrawNodeTexture(UIRenderer renderer, GraphRendererContext rendererContext, GraphNode node, GraphCamera camera, string textureName)
        {
            var nodeTexture = renderer.GetResource<Texture2D>(textureName) as Texture2D;
			var textureSize = new Vector2 (nodeTexture.width, nodeTexture.height);
			if (nodeTexture != null)
            {
                var center = camera.WorldToScreen(node.Bounds.center);

				var size = textureSize / camera.ZoomLevel;
				var position = center - size / 2.0f;

                var rect = new Rect(position.x, position.y, size.x, size.y);
				renderer.DrawTexture(rect, nodeTexture);
            }
        }
    }

    public class GraphNodeRendererFactory
    {
        GraphNodeRenderer defaultRenderer = new GraphNodeRenderer();
        Dictionary<Type, GraphNodeRenderer> renderers = new Dictionary<Type, GraphNodeRenderer>();


        public void RegisterNodeRenderer(Type nodeType, GraphNodeRenderer renderer)
        {
            if (!renderers.ContainsKey(nodeType))
            {
                renderers.Add(nodeType, renderer);
            }
        }

        public GraphNodeRenderer GetRenderer(Type nodeType)
        {
            if (renderers.ContainsKey(nodeType))
            {
                return renderers[nodeType];
            }
            return defaultRenderer;
        }

        public void Release()
        {
            foreach (var renderer in renderers.Values)
            {
                renderer.Release();
            }

            renderers.Clear();
        }
    }
}
