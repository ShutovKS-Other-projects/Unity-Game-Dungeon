//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.UI;
using DungeonArchitect.UI.Widgets.GraphEditors;
using System.Collections.Generic;
using DungeonArchitect.Flow.Domains.Tilemap;
using DungeonArchitect.Flow.Domains.Tilemap.Tooling;
using DungeonArchitect.Flow.Items;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Editors.Flow.Tilemap
{

    public delegate void OnFlowPreviewTileClicked(FlowTilemap tilemap, int tileX, int tileY);

    public class FlowPreviewTilemapGraphEditor : GraphEditor
    {
        private Dictionary<DungeonUID, List<Rect>> itemScreenPositions = new Dictionary<DungeonUID, List<Rect>>();
        public event OnFlowPreviewTileClicked TileClicked;

        protected override GraphContextMenu CreateContextMenu()
        {
            return null;
        }

        protected override IGraphLinkRenderer CreateGraphLinkRenderer()
        {
            return new StraightLineGraphLinkRenderer();
        }

        protected override string GetGraphNotInitializedMessage()
        {
            return "Graph not initialized";
        }

        protected override void InitializeNodeRenderers(GraphNodeRendererFactory nodeRenderers)
        {
            var tilemapRenderer = new FlowPreviewTilemapNodeRenderer();
            tilemapRenderer.FlowAbstractGraphItemRendered += OnGraphItemRendered;
            nodeRenderers.RegisterNodeRenderer(typeof(FlowTilemapToolGraphNode), tilemapRenderer);
        }

        private void OnGraphItemRendered(FlowItem item, Rect screenBounds)
        {
            if (!itemScreenPositions.ContainsKey(item.itemId))
            {
                itemScreenPositions[item.itemId] = new List<Rect>();
            }
            itemScreenPositions[item.itemId].Add(screenBounds);
        }

        public override void Draw(UISystem uiSystem, UIRenderer renderer)
        {
            itemScreenPositions.Clear();
            base.Draw(uiSystem, renderer);

            if (IsPaintEvent(uiSystem))
            {
                var node = graph.Nodes.Count > 0 ? graph.Nodes[0] as FlowTilemapToolGraphNode : null;
                if (node != null)
                {
                    var items = node.LayoutGraph.GetAllItems();
                    foreach (var item in items)
                    {
                        if (!itemScreenPositions.ContainsKey(item.itemId)) continue;
                        foreach (var referencedItemId in item.referencedItemIds)
                        {
                            if (!itemScreenPositions.ContainsKey(referencedItemId)) continue;
                            var startBoundsList = itemScreenPositions[item.itemId];
                            var endBoundsList = itemScreenPositions[referencedItemId];
                            
                            foreach (var startBounds in startBoundsList)
                            {
                                foreach (var endBounds in endBoundsList)
                                {
                                    DrawReferenceLink(renderer, startBounds, endBounds);
                                }
                            }
                        }
                    }
                }
            }
        }

        void DrawReferenceLink(UIRenderer renderer, Rect start, Rect end)
        {
            var centerA = start.center;
            var centerB = end.center;
            var radiusA = Mathf.Max(start.size.x, start.size.y) * 0.5f;
            var radiusB = Mathf.Max(end.size.x, end.size.y) * 0.5f;
            var vecAtoB = (centerB - centerA);
            var lenAtoB = vecAtoB.magnitude;
            var dirAtoB = vecAtoB / lenAtoB;

            var startPos = centerA + dirAtoB * radiusA;
            var endPos = centerA + dirAtoB * (lenAtoB - radiusB);
            StraightLineGraphLinkRenderer.DrawLine(renderer, startPos, endPos, camera, Color.red, 2);
        }

        public override void HandleInput(Event e, UISystem uiSystem)
        {
            base.HandleInput(e, uiSystem);


            var buttonId = 0;
            if (e.type == EventType.MouseDown && e.button == buttonId)
            {
                var clickPos = camera.ScreenToWorld(e.mousePosition);

                var node = graph.Nodes.Count > 0 ? graph.Nodes[0] as FlowTilemapToolGraphNode : null;
                if (node != null)
                {
                    var tilemap = node.Tilemap;
                    clickPos -= node.Position;
                    var tileX = Mathf.FloorToInt(clickPos.x / node.tileRenderSize);
                    var tileY = Mathf.FloorToInt(clickPos.y / node.tileRenderSize);
                    tileY = tilemap.Height - tileY - 1;
                    if (tileX >= 0 && tileX < tilemap.Width && tileY >= 0 && tileY < tilemap.Height)
                    {
                        if (TileClicked != null)
                        {
                            TileClicked.Invoke(tilemap, tileX, tileY);
                        }
                    }
                }
            }
        }

        protected override void OnMenuItemClicked(object userdata, GraphContextMenuEvent e)
        {

        }

        public void UpdateGridSpacing()
        {
            var node = graph.Nodes.Count > 0 ? graph.Nodes[0] as FlowTilemapToolGraphNode : null;
            if (node != null)
            {
                EditorStyle.gridCellSpacing = node.tileRenderSize * 2;
            }
        }
    }
}
