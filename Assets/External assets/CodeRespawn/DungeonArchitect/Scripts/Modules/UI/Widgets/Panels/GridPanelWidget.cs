//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.UI.Widgets
{
    public enum GridPanelArrangementType
    {
        VerticalScroll
    }

    class GridPanelNode
    {
        public IWidget Widget;
    }


    public class GridPanelWidget : WidgetBase
    {
        List<GridPanelNode> nodes = new List<GridPanelNode>();
        GridPanelArrangementType arrangement = GridPanelArrangementType.VerticalScroll;
        IntVector2 padding = new IntVector2(10, 10);
        public Vector2 cellSize = new Vector2(100, 100);
        public Vector2 desiredCellSize = Vector2.zero;
        public bool autoSize = false;


        IntVector2 renderedCells = new IntVector2(0, 0);

        public GridPanelWidget(GridPanelArrangementType arrangement)
        {
            this.arrangement = arrangement;
        }

        public GridPanelWidget AddWidget(IWidget Widget)
        {
            var node = new GridPanelNode();
            node.Widget = Widget;
            nodes.Add(node);
            return this;
        }

        public GridPanelWidget SetArrangementType(GridPanelArrangementType arrangement)
        {
            this.arrangement = arrangement;
            return this;
        }

        public GridPanelArrangementType GetArrangementType()
        {
            return arrangement;
        }

        public GridPanelWidget SetPadding(int x, int y)
        {
            this.padding = new IntVector2(x, y);
            return this;
        }

        public GridPanelWidget SetAutoSize(bool autoSize)
        {
            this.autoSize = autoSize;
            return this;
        }

        public GridPanelWidget SetCellSize(float width, float height)
        {
            this.cellSize = new Vector2(width, height);
            return this;
        }

        public override Vector2 GetDesiredSize(Vector2 size, UISystem uiSystem)
        {
            UpdateDesiredCellSize(uiSystem);
            var cellHeight = padding.y + desiredCellSize.y;
            var targetWidth = size.x;
            var targetHeight = renderedCells.y * cellHeight + padding.y;
            return new Vector2(targetWidth, targetHeight);
        }


        void UpdateDesiredCellSize(UISystem uiSystem)
        {
            if (autoSize)
            {
                desiredCellSize = Vector2.zero;
                foreach (var node in nodes)
                {
                    if (node.Widget != null)
                    {
                        var nodeSize = node.Widget.GetDesiredSize(Vector2.zero, uiSystem);
                        desiredCellSize.x = Mathf.Max(desiredCellSize.x, nodeSize.x);
                        desiredCellSize.y = Mathf.Max(desiredCellSize.y, nodeSize.y);
                    }
                }
            }
            else
            {
                desiredCellSize = cellSize;
            }
        }

        public override void UpdateWidget(UISystem uiSystem, Rect bounds)
        {
            base.UpdateWidget(uiSystem, bounds);

            UpdateDesiredCellSize(uiSystem);
            if (autoSize)
            {
                cellSize = desiredCellSize;
            }

            var targetWidth = WidgetBounds.size.x;

            var cellWidth = padding.x + cellSize.x;
            int numCellsX = Mathf.FloorToInt(targetWidth / (float)cellWidth);
            numCellsX = Mathf.Max(1, numCellsX);
            int numCellsY = Mathf.CeilToInt(nodes.Count / (float)numCellsX);
            renderedCells = new IntVector2(numCellsX, numCellsY);

            for (int i = 0; i < nodes.Count; i++)
            {
                int ix = i % numCellsX;
                int iy = i / numCellsX;
                float x = (cellSize.x + padding.x) * ix;
                float y = (cellSize.y + padding.y) * iy;

                var nodeBounds = new Rect(
                    x + padding.x,
                    y + padding.y,
                    cellSize.x,
                    cellSize.y);

                var node = nodes[i];
                node.Widget.UpdateWidget(uiSystem, nodeBounds);
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
