//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using UnityEditor;
using DungeonArchitect.Graphs;
using DungeonArchitect.UI;
using DungeonArchitect.UI.Widgets.GraphEditors;

namespace DungeonArchitect.Editors
{
    /// <summary>
    /// Custom property editors for MarkerEmitterNode
    /// </summary>
    [CustomEditor(typeof(MarkerEmitterNode))]
    public class MarkerEmitterNodeEditor : PlaceableNodeEditor
    {

        public override void OnEnable()
        {
            base.OnEnable();
            drawOffset = true;
            drawAttachments = false;
        }

        protected override string GetNodeTitle()
        {
            var emitterNode = target as MarkerEmitterNode;
            var markerCaption = (emitterNode.Marker != null) ? emitterNode.Marker.Caption : "Unknown";
            return "Marker Emitter: " + markerCaption;
        }
    }

    /// <summary>
    /// Renders a MarkerEmitterNode
    /// </summary>
    public class MarkerEmitterNodeRenderer : GraphNodeRenderer
    {
        public override void Draw(UIRenderer renderer, GraphRendererContext rendererContext, GraphNode node, GraphCamera camera)
        {
            // Draw the background base texture
            DrawNodeTexture(renderer, rendererContext, node, camera, UIResourceLookup.TEXTURE_MARKER_NODE_BG);
            
            var emitterNode = node as MarkerEmitterNode;
            var title = (emitterNode.Marker != null) ? emitterNode.Marker.Caption : "{NONE}";

			DrawTextCentered(renderer, rendererContext, node, camera, title, new Vector2(0, -2));

            // Draw the foreground frame textures
            DrawNodeTexture(renderer, rendererContext, node, camera, UIResourceLookup.TEXTURE_MARKER_EMITTER_NODE_FRAME);

            if (node.Selected)
            {
                DrawNodeTexture(renderer, rendererContext, node, camera, UIResourceLookup.TEXTURE_MARKER_NODE_SELECTION);
            }

            // Draw the pins
            base.Draw(renderer, rendererContext, node, camera);
        }

        protected override Color getBackgroundColor(GraphNode node)
        {
            var color = new Color(0.2f, 0.25f, 0.4f);
            return node.Selected ? GraphEditorConstants.NODE_COLOR_SELECTED : color;
        }

    }
}
