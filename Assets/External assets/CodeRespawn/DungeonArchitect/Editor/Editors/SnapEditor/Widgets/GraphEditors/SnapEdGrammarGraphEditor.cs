//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DungeonArchitect.Grammar;
using DungeonArchitect.Graphs;
using DungeonArchitect.UI;
using UnityEditor;
using DungeonArchitect.UI.Widgets.GraphEditors;

namespace DungeonArchitect.Editors.SnapFlow
{
    public enum SnapEdGrammarGraphEditorAction
    {
        CreateTaskNode,
        CreateWildcard,
        CreateCommentNode
    }

    public class SnapEdGrammarGraphEditorContextMenuData
    {
        public SnapEdGrammarGraphEditorContextMenuData(UISystem uiSystem, SnapEdGrammarGraphEditorAction action)
            : this(uiSystem, action, null)
        {
        }

        public SnapEdGrammarGraphEditorContextMenuData(UISystem uiSystem, SnapEdGrammarGraphEditorAction action, object userData)
        {
            this.uiSystem = uiSystem;
            this.Action = action;
            this.UserData = userData;
        }

        public UISystem uiSystem;
        public SnapEdGrammarGraphEditorAction Action;
        public object UserData;
    }

    public class SnapEdGrammarGraphSchema : GraphSchema
    {

        public override bool CanCreateLink(GraphPin output, GraphPin input, out string errorMessage)
        {
            errorMessage = "";
            if (output == null || input == null)
            {
                errorMessage = "Invalid connection";
                return false;
            }

            if (input.Node != null)
            {
                input = input.Node.InputPin;
            }

            var sourceNode = output.Node;
            var destNode = input.Node;

            // Make sure we don't already have this connection
            foreach (var link in output.GetConntectedLinks())
            {
                if (link.Input == input)
                {
                    errorMessage = "Not Allowed: Already connected";
                    return false;
                }
            }

            return true;
        }
    }

    public class SnapEdGrammarGraphEditor : GraphEditor
    {
        public SnapFlowAsset FlowAsset { get; private set; }

        public void SetBranding(string branding)
        {
            if (EditorStyle != null)
            {
                EditorStyle.branding = branding;
            }
        }

        public override void Init(Graph graph, Rect editorBounds, UnityEngine.Object assetObject, UISystem uiSystem)
        {
            FlowAsset = assetObject as SnapFlowAsset;

            base.Init(graph, editorBounds, assetObject, uiSystem);
        }

        T GetDragData<T>() where T : Object
        {
            var dragDropData = DragAndDrop.GetGenericData(NodeListViewConstants.DragDropID);
            if (dragDropData != null && dragDropData is T)
            {
                return dragDropData as T;
            }
            return null;
        }

        public override GraphSchema GetGraphSchema()
        {
            return new SnapEdGrammarGraphSchema();
        }

        protected override IGraphLinkRenderer CreateGraphLinkRenderer()
        {
            return new StraightLineGraphLinkRenderer();
        }

        public override void OnNodeSelectionChanged(UISystem uiSystem)
        {
            base.OnNodeSelectionChanged(uiSystem);

            // Fetch all selected nodes
            var selectedNodes = from node in graph.Nodes
                                where node.Selected
                                select node;

            if (selectedNodes.Count() == 0)
            {
                uiSystem.Platform.ShowObjectProperty(graph);
            }
        }

        public override T CreateLink<T>(Graph graph, GraphPin output, GraphPin input, UISystem uiSystem)
        {
            if (input != null && input.Node != null)
            {
                input = input.Node.InputPin;
            }

            // If we have a link in the opposite direction, then break that link
            var sourceNode = output.Node;
            var destNode = input.Node;
            if (sourceNode != null && destNode != null)
            {
                var links = destNode.OutputPin.GetConntectedLinks();
                foreach (var link in links)
                {
                    if (link.Output.Node == input.Node && link.Input.Node == output.Node)
                    {
                        GraphOperations.DestroyLink(link, uiSystem.Undo);
                    }
                }
            }

            if (input.Node != null)
            {
                input = input.Node.InputPin;
                if (input != null)
                {
                    return base.CreateLink<T>(graph, output, input, uiSystem);
                }
            }
            return null;
        }

        bool IsDragDataSupported()
        {
            // We are dragging. Check if we support this data type
            return GetDragData<GrammarNodeType>() != null;
        }
        
        public override void Draw(UISystem uiSystem, UIRenderer renderer)
        {
            base.Draw(uiSystem, renderer);
            if (IsPaintEvent(uiSystem))
            {
                var bounds = new Rect(Vector2.zero, WidgetBounds.size);

                bool isDragging = (uiSystem != null && uiSystem.IsDragDrop);
                if (isDragging && IsDragDataSupported())
                {
                    // Show the drag drop overlay
                    var dragOverlayColor = new Color(1, 0, 0, 0.25f);
                    renderer.DrawRect(bounds, dragOverlayColor);
                }
            }
        }

        public override void HandleInput(Event e, UISystem uiSystem)
        {
            base.HandleInput(e, uiSystem);

            switch (e.type)
            {
                case EventType.DragUpdated:
                    if (IsDragDataSupported())
                    {
                        HandleDragUpdate(e, uiSystem);
                    }
                    break;

                case EventType.DragPerform:
                    if (IsDragDataSupported())
                    {
                        HandleDragPerform(e, uiSystem);
                    }
                    break;
            }
        }

        private void HandleDragUpdate(Event e, UISystem uiSystem)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
        }

        private void HandleDragPerform(Event e, UISystem uiSystem)
        {
            // TODO: Create a node here
            var nodeType = GetDragData<GrammarNodeType>();
            if (nodeType != null)
            {
                if (uiSystem != null)
                {
                    uiSystem.RequestFocus(this);
                }

                CreateNewTaskNode(nodeType, e.mousePosition, true, uiSystem);
            }

            DragAndDrop.AcceptDrag();
        }

        int FindNextAvailableIndex()
        {
            if (graph.Nodes.Count == 0)
            {
                return 0;
            }

            // Find an appropriate execution index for this node
            var usedIndices = new HashSet<int>();
            foreach (var graphNode in graph.Nodes)
            {
                if (graphNode is GrammarTaskNode)
                {
                    var taskNode = graphNode as GrammarTaskNode;
                    usedIndices.Add(taskNode.executionIndex);
                }
            }

            for (int i = 0; i < usedIndices.Count + 1; i++)
            {
                if (!usedIndices.Contains(i))
                {
                    return i;
                }
            }
            return 0;
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

        GrammarTaskNode CreateNewTaskNode(GrammarNodeType nodeType, Vector2 mousePosition, bool selectAfterCreate, UISystem uiSystem)
        {
            int index = FindNextAvailableIndex();
            var node = CreateNode<GrammarTaskNode>(mousePosition, uiSystem);
            node.NodeType = nodeType;
            node.executionIndex = index;
            node.DisplayExecutionIndex = true;

            // Adjust the initial position of the placed node
            {
                var nodeRenderer = nodeRenderers.GetRenderer(node.GetType());
                if (nodeRenderer is GrammarNodeRendererBase)
                {
                    var grammarNodeRenderer = nodeRenderer as GrammarNodeRendererBase;
                    grammarNodeRenderer.UpdateNodeBounds(node, 1.0f);
                }

                var mouseWorld = camera.ScreenToWorld(mousePosition);
                var bounds = node.Bounds;
                bounds.position = mouseWorld - bounds.size / 2.0f;
                node.Bounds = bounds;
            }

            if (selectAfterCreate)
            {
                BringToFront(node);
                SelectNode(node, uiSystem);
            }

            return node;
        }

        protected override GraphContextMenu CreateContextMenu()
        {
            return new SnapEdGrammarGraphContextMenu();
        }

        protected override void InitializeNodeRenderers(GraphNodeRendererFactory nodeRenderers)
        {
            nodeRenderers.RegisterNodeRenderer(typeof(GrammarTaskNode), new GrammarTaskNodeRenderer());
            nodeRenderers.RegisterNodeRenderer(typeof(CommentNode), new CommentNodeRenderer(EditorStyle.commentTextColor));
        }

        protected override void OnMenuItemClicked(object userdata, GraphContextMenuEvent e)
        {
            var data = userdata as SnapEdGrammarGraphEditorContextMenuData;
            if (data == null) return;

            var mouseScreen = lastMousePosition;
            if (data.Action == SnapEdGrammarGraphEditorAction.CreateTaskNode)
            {
                if (data.UserData != null && data.UserData is GrammarNodeType)
                {
                    var nodeType = data.UserData as GrammarNodeType;
                    var node = CreateNewTaskNode(nodeType, lastMousePosition, true, e.uiSystem);
                    CreateLinkBetweenPins(e.sourcePin, node.InputPin, e.uiSystem);
                }
            }
            else if (data.Action == SnapEdGrammarGraphEditorAction.CreateWildcard)
            {
                var nodeType = FlowAsset.wildcardNodeType;
                if (nodeType != null)
                {
                    var node = CreateNewTaskNode(nodeType, lastMousePosition, true, e.uiSystem);
                    CreateLinkBetweenPins(e.sourcePin, node.InputPin, e.uiSystem);
                }
            }
            else if (data.Action == SnapEdGrammarGraphEditorAction.CreateCommentNode)
            {
                CreateCommentNode(mouseScreen, e.uiSystem);
            }
        }

        protected override void DrawHUD(UISystem uiSystem, UIRenderer renderer, Rect bounds) { }

        protected override string GetGraphNotInitializedMessage()
        {
            return "Graph not initialize";
        }

        protected override GraphEditorStyle CreateEditorStyle()
        {
            var editorStyle = base.CreateEditorStyle();
            editorStyle.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            return editorStyle;
        }

    }
}
