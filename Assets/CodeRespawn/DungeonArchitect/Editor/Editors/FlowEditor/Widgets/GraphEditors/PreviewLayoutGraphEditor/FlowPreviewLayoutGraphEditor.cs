//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DungeonArchitect.Graphs;
using DungeonArchitect.Flow.Domains.Layout.Tooling.Graph2D;
using DungeonArchitect.Flow.Items;
using DungeonArchitect.UI;
using DungeonArchitect.UI.Widgets.GraphEditors;
using DungeonArchitect.Utils;

namespace DungeonArchitect.Editors.Flow.Layout
{
    public delegate void OnGridFlowAbstractItemSelectionChanged(FlowItem item);

    public class FlowPreviewLayoutGraphEditor : GraphEditor
    {
        private Dictionary<DungeonUID, List<Rect>> itemScreenPositions = new Dictionary<DungeonUID, List<Rect>>();
        FlowItem selectedItem = null;

        public event OnGridFlowAbstractItemSelectionChanged ItemSelectionChanged;

        public override void Init(Graph graph, Rect editorBounds, UnityEngine.Object assetObject, UISystem uiSystem)
        {
            base.Init(graph, editorBounds, assetObject, uiSystem);
            EditorStyle.branding = "Result";
            selectedItem = null;
        }

        protected override GraphContextMenu CreateContextMenu()
        {
            return new DungeonGridFlowAbstractGraphContextMenu();
        }

        protected override void InitializeNodeRenderers(GraphNodeRendererFactory nodeRenderers)
        {
            var abstractNodeRenderer = new FlowPreviewLayoutNodeRenderer();
            abstractNodeRenderer.FlowLayoutGraphItemRendered += OnGraphItemRendered;
            nodeRenderers.RegisterNodeRenderer(typeof(FlowLayoutToolGraph2DNode), abstractNodeRenderer);
            nodeRenderers.RegisterNodeRenderer(typeof(CommentNode), new CommentNodeRenderer(EditorStyle.commentTextColor));
        }

        protected override IGraphLinkRenderer CreateGraphLinkRenderer()
        {
            var linkRenderer = new FlowPreviewLayoutLinkRenderer();
            linkRenderer.GridFlowLayoutGraphItemRendered += OnGraphItemRendered;
            return linkRenderer;
        }

        protected override void OnMenuItemClicked(object userdata, GraphContextMenuEvent e)
        {
            var action = (DungeonGridFlowAbstractGraphEditorAction)userdata;

            if (action == DungeonGridFlowAbstractGraphEditorAction.CreateCommentNode)
            {
                var mouseScreen = lastMousePosition;
                CreateCommentNode(mouseScreen, e.uiSystem);
            }
        }

        protected override string GetGraphNotInitializedMessage()
        {
            return "Graph not initialized";
        }

        private void OnGraphItemRendered(FlowItem item, Rect screenBounds)
        {
            if (!itemScreenPositions.ContainsKey(item.itemId))
            {
                itemScreenPositions[item.itemId] = new List<Rect>();
            }
            itemScreenPositions[item.itemId].Add(screenBounds);
        }

        IntVector2 GetNodeCoord(Vector3 coordF)
        {
            return new IntVector2(Mathf.RoundToInt(coordF.x), Mathf.RoundToInt(coordF.y));
        }

        public void SelectNodeAtCoord(IntVector2 nodeCoord, UISystem uiSystem)
        {
            FlowLayoutToolGraph2DNode target = null;
            foreach (var node in graph.Nodes)
            {
                var previewNode = node as FlowLayoutToolGraph2DNode;
                if (previewNode != null)
                {
                    var previewNodeCoord = GetNodeCoord(previewNode.LayoutNode.coord);
                    if (previewNodeCoord.Equals(nodeCoord))
                    {
                        target = previewNode;
                        break;
                    }
                }
            }

            if (target != null)
            {
                SelectNode(target, uiSystem);
            }
        }

        public override void HandleInput(Event e, UISystem uiSystem)
        {
            base.HandleInput(e, uiSystem);


            var buttonId = 0;
            if (e.type == EventType.MouseDown && e.button == buttonId)
            {
                // A button was pressed. Check if any of the items were clicked
                FlowItem newSelectedItem = null;
                var items = FlowLayoutToolGraph2DUtils.GetAllItems(graph as FlowLayoutToolGraph2D);
                foreach (var item in items)
                {
                    if (itemScreenPositions.ContainsKey(item.itemId))
                    {
                        var itemBoundsList = itemScreenPositions[item.itemId];
                        foreach (var itemBounds in itemBoundsList)
                        {
                            if (itemBounds.Contains(lastMousePosition))
                            {
                                newSelectedItem = item;
                                break;
                            }
                        }
                    }
                }

                if (selectedItem != newSelectedItem)
                {
                    SelectNodeItem(newSelectedItem);
                }
            }
        }

        public void SelectNodeItem(DungeonUID itemId)
        {
            if (itemId == DungeonUID.Empty)
            {
                SelectNodeItem(null);
                return;
            }

            var items = FlowLayoutToolGraph2DUtils.GetAllItems(graph as FlowLayoutToolGraph2D);
            foreach (var item in items)
            {
                if (item.itemId == itemId)
                {
                    SelectNodeItem(item);
                    break;
                }
            }
        }

        public void SelectNodeItem(FlowItem newSelectedItem)
        {
            if (selectedItem != newSelectedItem)
            {
                var oldSelectedItem = selectedItem;
                selectedItem = newSelectedItem;

                if (oldSelectedItem != null)
                {
                    oldSelectedItem.editorSelected = false;
                }

                if (newSelectedItem != null)
                {
                    newSelectedItem.editorSelected = true;
                }

                if (ItemSelectionChanged != null)
                {
                    ItemSelectionChanged.Invoke(selectedItem);
                }
            }
        }

        public override void Draw(UISystem uiSystem, UIRenderer renderer)
        {
            itemScreenPositions.Clear();

            base.Draw(uiSystem, renderer);

            // Draw the graph item references
            if (graph != null && IsPaintEvent(uiSystem))
            {
                foreach (var node in graph.Nodes)
                {
                    var previewNode = node as FlowLayoutToolGraph2DNode;
                    if (previewNode != null)
                    {
                        foreach (var item in previewNode.LayoutNode.items)
                        {
                            foreach (var refItem in item.referencedItemIds)
                            {
                                // Draw a reference between the two bounds
                                if (itemScreenPositions.ContainsKey(item.itemId) && itemScreenPositions.ContainsKey(refItem))
                                {
                                    var startBoundsList = itemScreenPositions[item.itemId];
                                    var endBoundsList = itemScreenPositions[refItem];

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

        void CreateCommentNode(Vector2 screenPos, UISystem uiSystem)
        {
            var worldPos = camera.ScreenToWorld(screenPos);
            var commentNode = CreateNode<CommentNode>(worldPos, uiSystem);
            commentNode.Position = worldPos;
            commentNode.background = new Color(0.224f, 1.0f, 0.161f, 0.7f);
            BringToFront(commentNode);
            SelectNode(commentNode, uiSystem);
        }
    }

    public enum DungeonGridFlowAbstractGraphEditorAction
    {
        CreateCommentNode
    }

    public class DungeonGridFlowAbstractGraphContextMenu : GraphContextMenu
    {
        class ItemInfo
        {
            public ItemInfo(UISystem uiSystem, DungeonGridFlowAbstractGraphEditorAction action)
            {
                this.uiSystem = uiSystem;
                this.action = action;
            }

            public UISystem uiSystem;
            public DungeonGridFlowAbstractGraphEditorAction action;
        }

        public override void Show(GraphEditor graphEditor, GraphPin sourcePin, Vector2 mouseWorld, UISystem uiSystem)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add Comment Node"), false, HandleContextMenu, new ItemInfo(uiSystem, DungeonGridFlowAbstractGraphEditorAction.CreateCommentNode));
            menu.ShowAsContext();
        }

        void HandleContextMenu(object userdata)
        {
            var item = userdata as ItemInfo;
            DispatchMenuItemEvent(item.action, BuildEvent(null, item.uiSystem));
        }
    }

}
