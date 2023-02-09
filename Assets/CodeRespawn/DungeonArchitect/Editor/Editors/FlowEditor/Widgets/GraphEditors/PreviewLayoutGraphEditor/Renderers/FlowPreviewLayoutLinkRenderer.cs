//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Graphs;
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Domains.Layout.Tooling.Graph2D;
using UnityEngine;
using DungeonArchitect.UI;
using DungeonArchitect.UI.Widgets.GraphEditors;

namespace DungeonArchitect.Editors.Flow.Layout
{
    public class FlowPreviewLayoutLinkRenderer : IGraphLinkRenderer
    {
        public float padding = 4;
        public Color lineColorDirected = new Color(1, 1, 1, 1);
        public Color lineColorUndirected = new Color(1, 1, 1, 0.25f);
        public Color lineColorOneWay = new Color(1, 0.5f, 0, 1);
        public float lineThickness = 3;

        public event OnFlowLayoutGraphItemRendered GridFlowLayoutGraphItemRendered;

        public void DrawGraphLink(UIRenderer renderer, GraphRendererContext rendererContext, GraphLink link, GraphCamera camera)
        {
            if (link.Input == null || link.Output == null)
            {
                // Link not initialized yet. nothing to draw
                return;
            }

            var nodeA = link.Output.Node;
            var nodeB = link.Input.Node;
            if (nodeA == null || nodeB == null)
            {
                // invalid state
                return;
            }

            if (nodeA == nodeB)
            {
                return;
            }

            var positionA = nodeA.Bounds.center;
            var radiusA = nodeA.Bounds.width * 0.5f;

            var positionB = nodeB.Bounds.center;
            var radiusB = nodeB.Bounds.width * 0.5f;

            var vecAToB = (positionB - positionA);
            var length = vecAToB.magnitude;
            var direction = vecAToB / length;

            var worldStartPos = positionA + direction * (radiusA + padding);
            var worldEndPos = positionA + direction * (length - radiusB - padding);

            var startPos = camera.WorldToScreen(worldStartPos);
            var endPos = camera.WorldToScreen(worldEndPos);


            var previewLink = link as FlowLayoutToolGraph2DLink;
            if (previewLink != null)
            {
                bool directional = previewLink.layoutLinkState.type != FlowLayoutGraphLinkType.Unconnected;

                var color = directional ? lineColorDirected : lineColorUndirected;
                if (previewLink.layoutLinkState.type == FlowLayoutGraphLinkType.OneWay)
                {
                    color = lineColorOneWay;
                }
                renderer.DrawAAPolyLine(lineThickness, color, startPos, endPos);

                // Draw the arrow head if directional
                if (directional)
                {
                    var rotation = Quaternion.FromToRotation(new Vector3(1, 0, 0), (startPos - endPos).normalized);
                    float arrowSize = 10.0f / camera.ZoomLevel;
                    float arrowWidth = 0.5f / camera.ZoomLevel;
                    var arrowTails = new Vector2[] {
                        rotation * new Vector3(1, arrowWidth) * arrowSize,
                        rotation * new Vector3(1, -arrowWidth) * arrowSize,
                    };

                    var p0 = endPos;
                    var p1 = endPos + arrowTails[0];
                    var p2 = endPos + arrowTails[1];

                    renderer.DrawAAConvexPolygon(color, p0, p1, p2, p0);

                    if (previewLink.layoutLinkState.type == FlowLayoutGraphLinkType.OneWay)
                    {
                        // Draw another arrow head
                        var dirToTail = (startPos - endPos).normalized;
                        var offset = dirToTail * 10.0f / camera.ZoomLevel;

                        p0 += offset;
                        p1 += offset;
                        p2 += offset;
                        renderer.DrawAAConvexPolygon(color, p0, p1, p2, p0);
                    }

                    // Draw the items
                    {
                        float itemPadding = 2;
                        var items = previewLink.layoutLinkState.items;
                        for (int i = 0; i < items.Count; i++)
                        {
                            var item = items[i];

                            var sourceNode = link.Output.Node;
                            var destNode = link.Input.Node;

                            var hostSizeScreen = sourceNode.Bounds.size / camera.ZoomLevel;
                            var hostRadiusScreen = Mathf.Min(hostSizeScreen.x, hostSizeScreen.y) * 0.5f;

                            var itemRadiusScreen = hostRadiusScreen * FlowPreviewLayoutItemRenderer.ItemRadiusFactor;

                            var numLaneItems2 = items.Count / 2;
                            var numLaneItems1 = items.Count - numLaneItems2;

                            bool firstLane = i < numLaneItems1;
                            var laneIndex = i;
                            var laneItemCount = numLaneItems1;

                            var offsetY = itemRadiusScreen + itemPadding;
                            if (items.Count == 1)
                            {
                                offsetY = 0;
                            }

                            if (!firstLane)
                            {
                                laneIndex -= numLaneItems1;
                                laneItemCount = numLaneItems2;
                                offsetY = -offsetY;
                            }

                            var indexOffsetF = laneIndex - (laneItemCount - 1) * 0.5f;
                            var offsetX = (itemRadiusScreen + itemPadding) * indexOffsetF * 2;
                            Vector2 offset;
                            var linkDirection = (endPos - startPos).normalized;
                            float dot = Vector2.Dot(linkDirection, new Vector2(1, 0));
                            if (dot < 0.9999f)
                            {
                                offset = rotation * new Vector3(offsetX, offsetY);
                            }
                            else
                            {
                                offset = new Vector2(offsetX, -offsetY);
                            }


                            var centerScreen = (startPos + endPos) * 0.5f + offset;
                            var extentScreen = new Vector2(itemRadiusScreen, itemRadiusScreen);
                            var itemBounds = new Rect(centerScreen - extentScreen, extentScreen * 2);

                            float scaleFactor = FlowPreviewLayoutItemRenderer.GetItemScaleFactor(rendererContext.GraphEditor.LastMousePosition, itemBounds);
                            var scaledSize = itemBounds.size * scaleFactor;
                            itemBounds = new Rect(itemBounds.center - scaledSize * 0.5f, scaledSize);

                            FlowPreviewLayoutItemRenderer.DrawItem(renderer, rendererContext, camera, item, itemBounds, scaleFactor);
                            if (GridFlowLayoutGraphItemRendered != null)
                            {
                                GridFlowLayoutGraphItemRendered.Invoke(item, itemBounds);
                            }
                        }
                    }
                }
            }
        }
    }
}
