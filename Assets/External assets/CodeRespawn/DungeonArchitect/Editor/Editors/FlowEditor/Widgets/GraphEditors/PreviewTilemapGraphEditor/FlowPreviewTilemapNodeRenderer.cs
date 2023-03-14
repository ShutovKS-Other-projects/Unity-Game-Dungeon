//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Graphs;
using System.Collections.Generic;
using UnityEngine;
using DungeonArchitect.Editors.Flow.Layout;
using DungeonArchitect.Flow.Domains.Tilemap;
using DungeonArchitect.Flow.Domains.Tilemap.Tooling;
using DungeonArchitect.Flow.Items;
using DungeonArchitect.Utils;
using DungeonArchitect.UI;
using DungeonArchitect.UI.Widgets.GraphEditors;

namespace DungeonArchitect.Editors.Flow.Tilemap
{
    public class FlowPreviewTilemapNodeRenderer : GraphNodeRenderer
    {
        string tilemapNodeId;
        RenderTexture tilemapTexture;
        public event OnFlowLayoutGraphItemRendered FlowAbstractGraphItemRendered;


        IntVector2 GetNodeCoord(Vector3 coordF)
        {
            return new IntVector2(Mathf.RoundToInt(coordF.x), Mathf.RoundToInt(coordF.y));
        }
        
        private bool IsCellSelected(FlowTilemapCell cell, FlowTilemapToolGraphNode node)
        {
            if (node == null || node.SelectedNode == null || cell == null) return false;

            var nodeCoord = GetNodeCoord(node.SelectedNode.coord);
            return nodeCoord.Equals(cell.NodeCoord);
        }


        private void RecreateTilemapTexture(FlowTilemapRenderResources resources, FlowTilemapToolGraphNode node)
        {
            if (tilemapTexture != null) {
                tilemapTexture.Release();
                tilemapTexture = null;
            }

            node.RequestRecreatePreview = false;
            tilemapNodeId = node.Id;
            var tileSize = node.tileRenderSize;
            var tilemap = node.Tilemap;
            var textureSize = new IntVector2(tilemap.Width, tilemap.Height) * tileSize;
            tilemapTexture = new RenderTexture(textureSize.x, textureSize.y, 0);
            node.Bounds = new Rect(0, 0, textureSize.x, textureSize.y);

            FlowTilemapRenderer.Render(tilemapTexture, tilemap, tileSize, resources, cell => IsCellSelected(cell, node));
        }

        public override void Draw(UIRenderer renderer, GraphRendererContext rendererContext, GraphNode node, GraphCamera camera)
        {
            var guiState = new GUIState(renderer);

            var tilemapNode = node as FlowTilemapToolGraphNode;
            if (tilemapNode == null) return;

            if (tilemapTexture == null || tilemapNode.Id != tilemapNodeId || tilemapNode.RequestRecreatePreview)
            {
                var tileShader = Shader.Find("DungeonArchitect/UnlitColorShader");
                var resources = new FlowTilemapRenderResources();
                resources.materials = new TexturedMaterialInstances(tileShader);
                resources.iconOneWayDoor = renderer.GetResource<Texture2D>(UIResourceLookup.ICON_MOVEUP_16x) as Texture2D;
                RecreateTilemapTexture(resources, tilemapNode);
            }

            if (tilemapTexture == null)
            {
                return;
            }

            // Draw the tilemap texture
            {
                Rect tileBounds;
                var positionScreen = camera.WorldToScreen(node.Position);
                var sizeScreen = node.Size / camera.ZoomLevel;
                tileBounds = new Rect(positionScreen, sizeScreen);
                renderer.color = Color.white;
                renderer.DrawTexture(tileBounds, tilemapTexture);
            }

            // Draw the items
            {
                DrawItems(renderer, rendererContext, tilemapNode, camera);
            }

            guiState.Restore();
        }

        struct ItemDrawInfo
        {
            public FlowItem item;
            public Vector2 position;
            public Vector2 size;
        }

        void DrawItems(UIRenderer renderer, GraphRendererContext rendererContext, FlowTilemapToolGraphNode node, GraphCamera camera)
        {
            if (node == null || node.LayoutGraph == null || node.Tilemap == null) return;
            var offset = node.Position;
            var tilemap = node.Tilemap;
            var abstractGraph = node.LayoutGraph;

            var allItems = abstractGraph.GetAllItems();
            var itemMap = new Dictionary<DungeonUID, List<FlowItem>>();
            foreach (var item in allItems)
            {
                if (!itemMap.ContainsKey(item.itemId))
                {
                    itemMap[item.itemId] = new List<FlowItem>();
                }
                itemMap[item.itemId].Add(item);
            }

            var itemDrawList = new List<ItemDrawInfo>();
            foreach (var cell in tilemap.Cells)
            {
                if (cell.Item == DungeonUID.Empty) continue;
                var items = itemMap.ContainsKey(cell.Item) ? itemMap[cell.Item] : null;
                if (items == null || items.Count == 0) continue;

                var tileCoord = cell.TileCoord;
                tileCoord.y = tilemap.Height - tileCoord.y - 1;
                var position = offset + tileCoord.ToVector2() * node.tileRenderSize;
                var size = new Vector2(node.tileRenderSize, node.tileRenderSize);

                var drawInfo = new ItemDrawInfo();
                drawInfo.item = items[0];
                drawInfo.position = position;
                drawInfo.size = size;
                itemDrawList.Add(drawInfo);
            }

            foreach (var edge in tilemap.Edges)
            {
                if (edge == null || edge.Item == DungeonUID.Empty) continue;
                var items = itemMap.ContainsKey(edge.Item) ? itemMap[edge.Item] : null;
                if (items == null || items.Count == 0) continue;

                var tileCoord = edge.EdgeCoord;
                tileCoord.y = tilemap.Height - tileCoord.y - 1;
                var position = offset + tileCoord.ToVector2() * node.tileRenderSize;        // TODO: Update me for edge
                if (edge.HorizontalEdge)
                {
                    position.y += node.tileRenderSize * 0.5f;
                }
                else
                {
                    position.x -= node.tileRenderSize * 0.5f;
                }
                var size = new Vector2(node.tileRenderSize, node.tileRenderSize);

                var drawInfo = new ItemDrawInfo();
                drawInfo.item = items[0];
                drawInfo.position = position;
                drawInfo.size = size;
                itemDrawList.Add(drawInfo);
            }

            foreach (var itemDrawInfo in itemDrawList)
            {
                var position = itemDrawInfo.position;
                var size = itemDrawInfo.size;
                position = camera.WorldToScreen(position);
                size /= camera.ZoomLevel;

                var itemBounds = new Rect(position, size);

                float scaleFactor = FlowPreviewLayoutItemRenderer.GetItemScaleFactor(rendererContext.GraphEditor.LastMousePosition, itemBounds);
                var scaledSize = itemBounds.size * scaleFactor;
                itemBounds = new Rect(itemBounds.center - scaledSize * 0.5f, scaledSize);

                float preferedNodeSizeForText = 14.0f;
                float textScaleFactor = node.tileRenderSize / preferedNodeSizeForText;
                FlowPreviewLayoutItemRenderer.DrawItem(renderer, rendererContext, camera, itemDrawInfo.item, itemBounds, scaleFactor * textScaleFactor);
                if (FlowAbstractGraphItemRendered != null)
                {
                    FlowAbstractGraphItemRendered.Invoke(itemDrawInfo.item, itemBounds);
                }
            }
        }

        public override void Release()
        {
            base.Release();
            if (tilemapTexture != null)
            {
                tilemapTexture.Release();
                tilemapTexture = null;
            }
        }
    }
}
