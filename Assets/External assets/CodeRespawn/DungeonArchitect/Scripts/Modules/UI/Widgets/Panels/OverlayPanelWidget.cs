//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.UI.Widgets
{
    public enum OverlayPanelVAlign
    {
        Fill,
        Top,
        Center,
        Bottom
    }

    public enum OverlayPanelHAlign
    {
        Fill,
        Left,
        Center,
        Right
    }

    class OverlayPanelNode
    {
        public IWidget Widget;
        public Vector2 Size = new Vector2(100, 100);
        public Vector2 Offset = Vector2.zero;

        public OverlayPanelVAlign VAlign = OverlayPanelVAlign.Fill;
        public OverlayPanelHAlign HAlign = OverlayPanelHAlign.Fill;

        public Rect Bounds = Rect.zero;
    }

    public class OverlayPanelWidget : WidgetBase
    {
        List<OverlayPanelNode> nodes = new List<OverlayPanelNode>();

        public OverlayPanelWidget AddWidget(IWidget widget, OverlayPanelHAlign HAlign, OverlayPanelVAlign VAlign, Vector2 size, Vector2 offset)
        {
            var node = new OverlayPanelNode();
            node.Widget = widget;
            node.Size = size;
            node.Offset = offset;
            node.VAlign = VAlign;
            node.HAlign = HAlign;
            nodes.Add(node);

            return this;
        }

        public OverlayPanelWidget AddWidget(IWidget widget)
        {
            return AddWidget(widget, OverlayPanelHAlign.Fill, OverlayPanelVAlign.Fill, Vector2.zero, Vector2.zero);
        }


        public override void UpdateWidget(UISystem uiSystem, Rect bounds)
        {
            base.UpdateWidget(uiSystem, bounds);

            UpdateNodeBounds(uiSystem);
                        
        }

        protected override void DrawImpl(UISystem uiSystem, UIRenderer renderer)
        {
            foreach (var node in nodes)
            {
                WidgetUtils.DrawWidgetGroup(uiSystem, renderer, node.Widget);
            }
        }

        OverlayPanelNode FindIntersectingNode(Vector2 position)
        {
            foreach (var node in nodes)
            {
                if (node.Bounds.Contains(position))
                {
                    return node;
                }
            }
            return null;
        }

        public void UpdateNodeBounds(UISystem uiSystem)
        {
            var windowSize = WidgetBounds.size;
            foreach (var node in nodes)
            {
                var position = Vector2.zero;
                var size = node.Size;

                // Find horizontal alignment
                if (node.HAlign == OverlayPanelHAlign.Fill)
                {
                    position.x = node.Offset.x;
                    size.x = windowSize.x - node.Offset.x * 2;
                }
                else if (node.HAlign == OverlayPanelHAlign.Left)
                {
                    position.x = node.Offset.x;
                }
                else if (node.HAlign == OverlayPanelHAlign.Right)
                {
                    position.x = windowSize.x - node.Size.x - node.Offset.x;
                }
                else if (node.HAlign == OverlayPanelHAlign.Center)
                {
                    position.x = (windowSize.x - node.Size.x) / 2.0f;
                }

                // Find vertical alignment
                if (node.VAlign == OverlayPanelVAlign.Fill)
                {
                    position.y = node.Offset.y;
                    size.y = windowSize.y - node.Offset.y * 2;
                }
                else if (node.VAlign == OverlayPanelVAlign.Top)
                {
                    position.y = node.Offset.y;
                }
                else if (node.VAlign == OverlayPanelVAlign.Bottom)
                {
                    position.y = windowSize.y - node.Size.y - node.Offset.y;
                }
                else if (node.VAlign == OverlayPanelVAlign.Center)
                {
                    position.y = (windowSize.y - node.Size.y) / 2.0f;
                }

                var nodeBounds = new Rect(position, size);
                node.Widget.UpdateWidget(uiSystem, nodeBounds);
            }
        }

        public override bool IsCompositeWidget()
        {
            return true;
        }

        public override IWidget[] GetChildWidgets()
        {
            var children = new List<IWidget>();

            foreach (var node in nodes)
            {
                if (node.Widget != null)
                {
                    children.Add(node.Widget);
                }
            }

            return children.ToArray();
        }
    }
}
