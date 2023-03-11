//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.UI;
using DungeonArchitect.Graphs;
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Domains.Layout.Tooling.Graph2D;
using DungeonArchitect.Flow.Impl.GridFlow;
using UnityEngine;
using DungeonArchitect.UI.Widgets.GraphEditors;

namespace DungeonArchitect.Editors.Flow.Layout
{
    public class FlowPreviewLayoutNodeRenderer : FlowPreviewLayoutNodeRendererBase
    {
        public event OnFlowLayoutGraphItemRendered FlowLayoutGraphItemRendered;
        protected override string GetCaption(GraphNode node)
        {
            var caption = "";
            var previewNode = node as FlowLayoutToolGraph2DNode;
            if (previewNode != null && previewNode.LayoutNode.active)
            {
                var roomType = GetRoomType(previewNode.LayoutNode);
                switch (roomType)
                {
                    case GridFlowLayoutNodeRoomType.Room:
                        caption = "R";
                        break;

                    case GridFlowLayoutNodeRoomType.Corridor:
                        caption = "Co";
                        break;

                    case GridFlowLayoutNodeRoomType.Cave:
                        caption = "Ca";
                        break;
                }
            }

            return caption;
        }

        FlowLayoutGraphNode GetLayoutNode(GraphNode node)
        {
            var previewNode = node as FlowLayoutToolGraph2DNode;
            if (previewNode != null)
            {
                return previewNode.LayoutNode;
            }
            return null;
        }

        protected override Color GetBodyColor(GraphNode node)
        {
            var bodyColor = base.GetBodyColor(node);
            
            var nodeState = GetLayoutNode(node);
            if (nodeState != null && nodeState.active)
            {
                bodyColor = nodeState.color;
            }
            return bodyColor;
        }



        protected override void DrawNodeBody(UIRenderer renderer, GraphRendererContext rendererContext, GraphNode node, GraphCamera camera)
        {
            base.DrawNodeBody(renderer, rendererContext, node, camera);

            var layoutNode = GetLayoutNode(node);
            if (layoutNode != null)
            {
                int numItems = layoutNode.items.Count;
                if (numItems > 0)
                {
                    var hostPosScreen = camera.WorldToScreen(node.Position);
                    var hostSizeScreen = node.Bounds.size / camera.ZoomLevel;
                    var hostCenterScreen = hostPosScreen + hostSizeScreen * 0.5f;
                    var hostRadiusScreen = Mathf.Min(hostSizeScreen.x, hostSizeScreen.y) * 0.5f;

                    var itemRadiusScreen = hostRadiusScreen * FlowPreviewLayoutItemRenderer.ItemRadiusFactor;
                    var itemCenterDistance = hostRadiusScreen - itemRadiusScreen;

                    for (int i = 0; i < numItems; i++)
                    {
                        var item = layoutNode.items[i];
                        if (item == null) continue;
                        var angle = Mathf.PI * 2 * i / numItems - Mathf.PI * 0.5f;
                        var direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                        var itemCenterScreen = hostCenterScreen + direction * itemCenterDistance;
                        var itemBoundsSize = new Vector2(itemRadiusScreen, itemRadiusScreen) * 2;
                        var itemBounds = new Rect(itemCenterScreen - itemBoundsSize * 0.5f, itemBoundsSize);
                        
                        float scaleFactor = FlowPreviewLayoutItemRenderer.GetItemScaleFactor(rendererContext.GraphEditor.LastMousePosition, itemBounds);
                        var scaledSize = itemBounds.size * scaleFactor;
                        itemBounds = new Rect(itemBounds.center - scaledSize * 0.5f, scaledSize);

                        FlowPreviewLayoutItemRenderer.DrawItem(renderer, rendererContext, camera, item, itemBounds, scaleFactor);
                        if (FlowLayoutGraphItemRendered != null)
                        {
                            FlowLayoutGraphItemRendered.Invoke(item, itemBounds);
                        }
                    }
                }
            }
        }

        private GridFlowLayoutNodeRoomType GetRoomType(FlowLayoutGraphNode node)
        {
            var domainData = node.GetDomainData<GridFlowTilemapDomainData>();
            return domainData.RoomType;
        }
    }
}
