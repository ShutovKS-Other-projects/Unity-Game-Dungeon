//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.UI.Widgets
{
    class StackPanelNode
    {
        public IWidget Widget;
        public bool AutoSize = false;
        public bool AdjustToDynamicDesiredSize = false;
        public float Size = 100;
    }

    public enum StackPanelOrientation
    {
        Horizontal,
        Vertical
    }

    public class StackPanelWidget : WidgetBase
    {
        List<StackPanelNode> nodes = new List<StackPanelNode>();
        StackPanelOrientation Orientation = StackPanelOrientation.Vertical;

        public StackPanelWidget(StackPanelOrientation orientation)
        {
            this.Orientation = orientation;
        }

        public StackPanelWidget AddWidget(IWidget Widget)
        {
            var node = new StackPanelNode();
            node.Widget = Widget;
            node.AutoSize = true;
            nodes.Add(node);

            return this;
        }

        public StackPanelWidget AddWidget(IWidget Widget, float size)
        {
            return AddWidget(Widget, size, false);
        }

        public StackPanelWidget AddWidget(IWidget Widget, float size, bool adjustToDynamicDesiredSize)
        {
            var node = new StackPanelNode();
            node.Widget = Widget;
            node.Size = size;
            node.AdjustToDynamicDesiredSize = adjustToDynamicDesiredSize;
            nodes.Add(node);

            return this;
        }

        public override Vector2 GetDesiredSize(Vector2 size, UISystem uiSystem)
        {
            var fixedLength = 0.0f;
            foreach (var node in nodes)
            {
                if (!node.AutoSize)
                {
                    var nodeSize = node.Size;
                    if (Orientation == StackPanelOrientation.Horizontal)
                    {
                        var nodeDesiredSize = new Vector2(nodeSize, size.y);
                        nodeDesiredSize = node.Widget.GetDesiredSize(nodeDesiredSize, uiSystem);
                        nodeSize = nodeDesiredSize.x;
                        if (node.AdjustToDynamicDesiredSize)
                        {
                            size.y = nodeDesiredSize.y;
                        }
                    }
                    else if (Orientation == StackPanelOrientation.Vertical)
                    {
                        var nodeDesiredSize = new Vector2(size.x, nodeSize);
                        nodeDesiredSize = node.Widget.GetDesiredSize(nodeDesiredSize, uiSystem);
                        nodeSize = nodeDesiredSize.y;
                        if (node.AdjustToDynamicDesiredSize)
                        {
                            size.x = nodeDesiredSize.x;
                        }
                    }

                    fixedLength += nodeSize;
                }
            }

            var desiredSize = size;
            if (Orientation == StackPanelOrientation.Horizontal)
            {
                desiredSize.x = Mathf.Max(desiredSize.x, fixedLength);
            }
            else if (Orientation == StackPanelOrientation.Vertical)
            {
                desiredSize.y = Mathf.Max(desiredSize.y, fixedLength);
            }

            return desiredSize;
        }

        public override void UpdateWidget(UISystem uiSystem, Rect bounds)
        {
            base.UpdateWidget(uiSystem, bounds);

            foreach (var node in nodes)
            {
                if (!node.AutoSize && node.AdjustToDynamicDesiredSize)
                {
                    var size = bounds.size;
                    var ds = (Orientation == StackPanelOrientation.Horizontal)
                    ? new Vector2(node.Size, size.y)
                    : new Vector2(size.x, node.Size);

                    ds = node.Widget.GetDesiredSize(ds, uiSystem);
                    node.Size = (Orientation == StackPanelOrientation.Horizontal)
                        ? ds.x
                        : ds.y;
                }
            }

            float availableSize = (Orientation == StackPanelOrientation.Horizontal) ? WidgetBounds.width : WidgetBounds.height;
            int autoSizedNodeCount = 0;
            // find the available size after the fixed sized nodes have been processed
            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                if (!node.AutoSize)
                {
                    availableSize -= node.Size;
                }
                else
                {
                    autoSizedNodeCount++;
                }
            }

            float autoSize = autoSizedNodeCount > 0 ? Mathf.Max(0.0f, availableSize) / autoSizedNodeCount : 0;
            float offset = 0;
            foreach (var node in nodes)
            {
                float nodeSize;
                if (!node.AutoSize)
                {

                    nodeSize = node.Size;
                }
                else
                {
                    nodeSize = autoSize;
                }

                var nodeBounds = (Orientation == StackPanelOrientation.Horizontal)
                    ? new Rect(offset, 0, nodeSize, WidgetBounds.height)
                    : new Rect(0, offset, WidgetBounds.width, nodeSize);

                node.Widget.UpdateWidget(uiSystem, nodeBounds);

                offset += nodeSize;
            }

        }


        protected override void DrawImpl(UISystem uiSystem, UIRenderer renderer)
        {
            foreach (var childWidget in GetChildWidgets())
            {
                WidgetUtils.DrawWidgetGroup(uiSystem, renderer, childWidget);
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
