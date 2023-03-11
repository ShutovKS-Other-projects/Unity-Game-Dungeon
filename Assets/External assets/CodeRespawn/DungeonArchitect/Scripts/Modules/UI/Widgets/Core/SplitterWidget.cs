//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.UI.Widgets
{
    public enum SplitterDirection
    {
        Horizontal,
        Vertical
    }

    public class SplitterNode
    {
        public IWidget Content;
        public float Weight;
        public bool IsSplitBar = false;
    }

    public delegate void OnSplitBarResized(SplitterNode prev, SplitterNode next);

    public class Splitter : WidgetBase
    {
        SplitterDirection direction;
        public SplitterDirection Direction { get { return direction; } }

        public bool freeSize = false;
        public float minWindowSize = 100;
        public float barSize = 6;
        public Color barColor = new Color(0.20f, 0.20f, 0.20f);
        public Color barHighlightColor = new Color(0.15f, 0.15f, 0.15f);
        public event OnSplitBarResized SplitBarDragged;

        public Splitter(SplitterDirection direction)
        {
            this.direction = direction;
        }

        private List<SplitterNode> nodes = new List<SplitterNode>();

        public Splitter SetMinWindowSize(float minWindowSize)
        {
            this.minWindowSize = minWindowSize;
            return this;
        }

        public Splitter SetBarSize(float barSize)
        {
            this.barSize = barSize;
            return this;
        }

        public Splitter SetFreeSize(bool freeSize)
        {
            this.freeSize = freeSize;
            return this;
        }

        public Splitter AddWidget(IWidget widget)
        {
            return AddWidget(widget, 1);
        }

        public Splitter AddWidget(IWidget widget, float weight)
        {
            if (nodes.Count > 0)
            {
                // Add a split bar widget
                var barNode = new SplitterNode();
                barNode.Content = new SplitterDragBarWidget(this);
                barNode.Weight = freeSize ? barSize : 0;
                barNode.IsSplitBar = true;
                nodes.Add(barNode);
            }

            var node = new SplitterNode();
            node.Content = widget;
            node.Weight = weight;
            nodes.Add(node);

            return this;
        }

        public override bool IsCompositeWidget() { return true; }
        public override IWidget[] GetChildWidgets()
        {
            var widgets = new List<IWidget>();
            foreach (var node in nodes)
            {
                if (node.Content != null)
                {
                    widgets.Add(node.Content);
                }
            }
            return widgets.ToArray();
        }

        public void OnSplitBarDragged(SplitterDragBarWidget barWidget, Vector2 delta)
        {
            float dragDistance = (direction == SplitterDirection.Horizontal) ? delta.x : delta.y;
            var sizes = GetLayoutSizes(WidgetBounds.size);
            var totalContentSize = 0.0f;

            for (int i = 0; i < nodes.Count; i++)
            {
                if (!nodes[i].IsSplitBar)
                {
                    totalContentSize += sizes[i];
                }

            }

            for (int i = 1; i + 1 < sizes.Length; i++)
            {
                var node = nodes[i];
                if (node.Content == barWidget)
                {
                    var sizePrev = sizes[i - 1];
                    var sizeNext = sizes[i + 1];

                    var newSizePrev = sizePrev + dragDistance;
                    var newSizeNext = freeSize ? sizeNext : sizeNext - dragDistance;

                    var invalid = (newSizePrev < sizePrev && newSizePrev < minWindowSize)
                        || (newSizeNext < sizeNext && newSizeNext < minWindowSize);

                    if (!invalid)
                    {
                        sizes[i - 1] = newSizePrev;
                        sizes[i + 1] = newSizeNext;

                        // Recalculate the weights 
                        for (int n = 0; n < nodes.Count; n++)
                        {
                            if (freeSize)
                            {
                                nodes[n].Weight = sizes[n];
                            }
                            else
                            {
                                nodes[n].Weight = sizes[n] / totalContentSize;
                            }
                        }

                        // Notify the event
                        if (SplitBarDragged != null)
                        {
                            var nodePrev = nodes[i - 1];
                            var nodeNext = nodes[i + 1];
                            SplitBarDragged.Invoke(nodePrev, nodeNext);
                        }
                    }
                    break;
                }
            }
        }

        public override void UpdateWidget(UISystem uiSystem, Rect bounds)
        {
            base.UpdateWidget(uiSystem, bounds);

            var sizes = GetLayoutSizes(bounds.size);
            float offset = 0;
            for (int i = 0; i < nodes.Count; i++)
            {
                var size = sizes[i];
                var node = nodes[i];

                if (node.Content != null)
                {
                    var nodeBounds = GetWidgetBounds(WidgetBounds.size, offset, size);
                    node.Content.UpdateWidget(uiSystem, nodeBounds);
                }

                offset += size;
            }
        }

        protected override void DrawImpl(UISystem uiSystem, UIRenderer renderer)
        {
            var children = GetChildWidgets();
            foreach (var childWidget in children)
            {
                WidgetUtils.DrawWidgetGroup(uiSystem, renderer, childWidget);
            }
        }

        public override void HandleInput(Event e, UISystem uiSystem)
        {
            base.HandleInput(e, uiSystem);
            

        }

        Rect GetWidgetBounds(Vector2 hostSize, float offset, float size)
        {
            Rect bounds = new Rect(Vector2.zero, hostSize);
            if (Direction == SplitterDirection.Horizontal)
            {
                bounds.x += offset;
                bounds.width = size;
            }
            else
            {
                bounds.y += offset;
                bounds.height = size;
            }
            return bounds;
        }

        float[] GetLayoutSizes(Vector2 windowSize)
        {
            float totalSize = (Direction == SplitterDirection.Horizontal) ? windowSize.x : windowSize.y;
            float totalWeight = 0;
            float totalBarSizes = 0;
            foreach (var node in nodes)
            {
                if (node.IsSplitBar)
                {
                    totalBarSizes += barSize;
                }
                else
                {
                    totalWeight += node.Weight;
                }
            }

            if (freeSize) {
                totalSize = totalWeight + totalBarSizes;
            }

            float availableSize = totalSize - totalBarSizes;

            var sizes = new List<float>();
            foreach (var node in nodes)
            {
                if (node.IsSplitBar)
                {
                    sizes.Add(barSize);
                }
                else
                {
                    var ratio = node.Weight / totalWeight;
                    var size = availableSize * ratio;
                    sizes.Add(size);
                }
            }

            return sizes.ToArray();
        }


        public override Vector2 GetDesiredSize(Vector2 size, UISystem uiSystem)
        {
            if (freeSize)
            {
                var totalSize = 0.0f;
                foreach (var node in nodes)
                {
                    totalSize += node.Weight;
                }
                return (direction == SplitterDirection.Horizontal)
                    ? new Vector2(totalSize, size.x)
                    : new Vector2(size.x, totalSize);
            }
            else
            {
                return size;
            }
        }
    }


    public class SplitterDragBarWidget : WidgetBase
    {
        Splitter parent;

        public SplitterDragBarWidget(Splitter parent) 
        {
            this.parent = parent;
            ShowFocusHighlight = true;
        }


        public override bool CanAcquireFocus() { return true; }

        protected override void DrawImpl(UISystem uiSystem, UIRenderer renderer)
        {
            var bounds = new Rect(Vector2.zero, WidgetBounds.size);
            DrawBar(renderer, bounds, parent.barColor);

            // Draw focus highlight
            bool isFocused = (uiSystem != null) ? uiSystem.FocusedWidget == this as IWidget : false;
            if (isFocused && ShowFocusHighlight)
            {
                DrawFocusHighlight(uiSystem, renderer);
            }
        }

        void DrawBar(UIRenderer renderer, Rect bounds, Color barColor)
        {
            renderer.Box(bounds, new GUIContent());

            renderer.DrawRect(bounds, barColor);
            var cursor = parent.Direction == SplitterDirection.Horizontal
                ? UICursorType.ResizeHorizontal
                : UICursorType.ResizeVertical;

            renderer.AddCursorRect(bounds, cursor);
        }

        public override void HandleInput(Event e, UISystem uiSystem)
        {
            base.HandleInput(e, uiSystem);

            if (e.type == EventType.MouseDrag)
            {
                parent.OnSplitBarDragged(this, e.delta);
            }
            else if (e.type == EventType.MouseUp)
            {
                uiSystem.RequestFocus(null);
            }
            
        }

        protected override void DrawFocusHighlight(UISystem uiSystem, UIRenderer renderer)
        {
            var bounds = new Rect(Vector2.zero, WidgetBounds.size);
            DrawBar(renderer, bounds, parent.barHighlightColor);
        }

    }
}
