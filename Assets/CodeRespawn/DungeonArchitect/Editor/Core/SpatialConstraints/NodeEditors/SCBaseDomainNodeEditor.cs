//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect.Graphs;
using DungeonArchitect.Graphs.SpatialConstraints;
using DungeonArchitect.UI;
using DungeonArchitect.UI.Widgets.GraphEditors;

namespace DungeonArchitect.Editors.SpatialConstraints
{
    /// <summary>
    /// Renders a marker node
    /// </summary>
    public class SCDomainNodeRenderer : GraphNodeRenderer
    {
        Color GetNodeColor(SCBaseDomainNode node)
        {
            var color = node.GetColor();

            if (!node.IsSnapped)
            {
                color = Color.red;
            }

            color.a = node.Selected ? 1.0f : 0.33f;
            return color;
        }

        public override void Draw(UIRenderer renderer, GraphRendererContext rendererContext, GraphNode node, GraphCamera camera)
        {
            var guiState = new GUIState(renderer);
            var domainNode = node as SCBaseDomainNode;
            var ringTexture = renderer.GetResource<Texture2D>(UIResourceLookup.TEXTURE_CURSOR_RING_SOLID) as Texture2D;
            var bounds = camera.WorldToScreen(node.Bounds);
            var color = GetNodeColor(domainNode);
            renderer.DrawTexture(bounds, ringTexture, ScaleMode.ScaleToFit, true, color);

            // Draw the domain, if we are snapped
            if (domainNode.IsSnapped && !domainNode.Dragging)
            {
                const float DomainSizeHi = 0.75f;
                const float DomainSizeLo = 0.15f;
                var domainRectSize = bounds.size;
                if (domainNode.RuleDomain == SCRuleNodeDomain.Corner)
                {
                    domainRectSize = Vector2.one * (SCBaseDomainNode.TileSize * DomainSizeLo);
                }
                else if (domainNode.RuleDomain == SCRuleNodeDomain.Tile)
                {
                    domainRectSize = Vector2.one * (SCBaseDomainNode.TileSize * DomainSizeHi);
                }
                else if (domainNode.RuleDomain == SCRuleNodeDomain.Edge)
                {
                    var coords = domainNode.GetHalfGridLogicalCoords();
                    var localSize = (coords.x == 1) ? new Vector2(DomainSizeHi, DomainSizeLo) : new Vector2(DomainSizeLo, DomainSizeHi);
                    domainRectSize = localSize * SCBaseDomainNode.TileSize;
                }
                domainRectSize /= camera.ZoomLevel;
                var domainBounds = new Rect(bounds.center - domainRectSize / 2.0f, domainRectSize);
                var domainColor = color;
                domainColor.a *= 0.5f;
                renderer.color = domainColor;
                renderer.DrawRect(domainBounds, domainColor);
            }
            guiState.Restore();
        }
    }
}
