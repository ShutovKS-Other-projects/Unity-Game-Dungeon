//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using System.Linq;
using DungeonArchitect.Flow.Domains;
using UnityEngine;
using DungeonArchitect.Graphs;
using DungeonArchitect.Flow.Exec;
using DungeonArchitect.UI.Widgets.GraphEditors;
using DungeonArchitect.UI;

namespace DungeonArchitect.Editors.Flow
{
    public class FlowExecGraphEditor : GraphEditor
    {

        public IFlowDomain[] Domains = new IFlowDomain[0];

        public override void Init(Graph graph, Rect editorBounds, UnityEngine.Object assetObject, UISystem uiSystem)
        {
            base.Init(graph, editorBounds, assetObject, uiSystem);
            EditorStyle.branding = "Execution Flow";
        }

        protected override GraphContextMenu CreateContextMenu()
        {
            return new FlowExecGraphContextMenu(this);
        }

        public override GraphSchema GetGraphSchema()
        {
            return new FlowExecGraphSchema();
        }

        protected override void InitializeNodeRenderers(GraphNodeRendererFactory nodeRenderers)
        {
            nodeRenderers.RegisterNodeRenderer(typeof(FlowExecRuleGraphNode), new FlowExecRuleNodeRenderer());
            nodeRenderers.RegisterNodeRenderer(typeof(FlowExecResultGraphNode), new FlowExecResultNodeRenderer());
            nodeRenderers.RegisterNodeRenderer(typeof(CommentNode), new CommentNodeRenderer(EditorStyle.commentTextColor));
        }

        protected override IGraphLinkRenderer CreateGraphLinkRenderer()
        {
            return new StraightLineGraphLinkRenderer();
        }

        private FlowExecTask GetNodeHandler(GraphNode node)
        {
            if (node != null)
            {
                var ruleNode = node as FlowExecRuleGraphNode;
                if (ruleNode != null)
                {
                    return ruleNode.task;
                }
            }
            return null;
        }

        protected override void OnMenuItemClicked(object userdata, GraphContextMenuEvent e)
        {
            var menuContext = userdata as FlowExecGraphContextMenuUserData;

            if (menuContext != null)
            {
                if (menuContext.Action == FlowExecGraphEditorAction.CreateRuleNode)
                {
                    CreateExecRuleNode(lastMousePosition, menuContext.NodeHandlerType, e.uiSystem);
                }
                else if (menuContext.Action == FlowExecGraphEditorAction.CreateCommentNode)
                {
                    CreateCommentNode(lastMousePosition, e.uiSystem);
                }
            }
        }

        protected override string GetGraphNotInitializedMessage()
        {
            return "Graph not initialized";
        }

        void CreateExecRuleNode(Vector2 screenPos, System.Type nodeHandlerType, UISystem uiSystem)
        {
            if (nodeHandlerType == null)
            {
                return;
            }

            var nodeHandler = ScriptableObject.CreateInstance(nodeHandlerType) as FlowExecTask;
            if (nodeHandler != null)
            {
                nodeHandler.hideFlags = HideFlags.HideInHierarchy;
                uiSystem.Platform.AddObjectToAsset(nodeHandler, assetObject);

                var worldPos = camera.ScreenToWorld(screenPos);
                var execNode = CreateNode<FlowExecRuleGraphNode>(worldPos, uiSystem);
                execNode.task = nodeHandler;
                execNode.Position = worldPos;

                BringToFront(execNode);
                SelectNode(execNode, uiSystem);
            }
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


    public class FlowExecGraphSchema : GraphSchema
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

            if (sourceNode is FlowExecResultGraphNode)
            {
                errorMessage = "Not Allowed: Cannot connect from Result node";
                return false;
            }

            var graph = sourceNode.Graph;
            foreach (var link in graph.Links)
            {
                if (link.Input.Node == input.Node && link.Output.Node == output.Node)
                {
                    errorMessage = "Not Allowed: Already connected";
                    return false;
                }
            }

            return true;
        }
    }

    public enum FlowExecGraphEditorAction
    {
        CreateCommentNode,
        CreateRuleNode
    }

    class FlowExecGraphContextMenuUserData
    {
        public FlowExecGraphContextMenuUserData(UISystem uiSystem, FlowExecGraphEditorAction action)
            : this(uiSystem, action, null)
        {
        }

        public FlowExecGraphContextMenuUserData(UISystem uiSystem, FlowExecGraphEditorAction action, System.Type nodeHandlerType)
        {
            this.uiSystem = uiSystem;
            this.Action = action;
            this.NodeHandlerType = nodeHandlerType;
        }

        public FlowExecGraphEditorAction Action { get; set; }
        public System.Type NodeHandlerType { get; set; }
        public UISystem uiSystem { get; set; }
    }

    class FlowExecGraphContextMenu : GraphContextMenu
    {
        private FlowExecGraphEditor host;
        
        struct MenuItemInfo
        {
            public MenuItemInfo(string title, float weight, System.Type handlerType)
            {
                this.title = title;
                this.weight = weight;
                this.handlerType = handlerType;
            }

            public string title;
            public float weight;
            public System.Type handlerType;
        }

        public FlowExecGraphContextMenu(FlowExecGraphEditor host)
        {
            this.host = host;
        }
        
        public override void Show(GraphEditor graphEditor, GraphPin sourcePin, Vector2 mouseWorld, UISystem uiSystem)
        {
            var menu = uiSystem.Platform.CreateContextMenu();
            //var handlerTypes = GetNodeHandlerTypes();
            var supportedTasks = new List<System.Type>();
            foreach (var domain in host.Domains)
            {
                supportedTasks.AddRange(domain.SupportedTasks);
            }

            var items = new List<MenuItemInfo>();
            foreach (var taskType in supportedTasks)
            {
                var menuAttribute = FlowExecNodeInfoAttribute.GetHandlerAttribute(taskType);
                var weight = 0.0f;
                if (menuAttribute != null)
                {
                    var nodeTitle = menuAttribute.MenuPrefix + menuAttribute.Title;
                    weight = menuAttribute.Weight;
                    items.Add(new MenuItemInfo(nodeTitle, weight, taskType));
                }
            }
            items.Sort((a, b) => a.weight < b.weight ? -1 : 1);
            foreach (var item in items)
            {
                menu.AddItem(item.title, HandleContextMenu, new FlowExecGraphContextMenuUserData(uiSystem, FlowExecGraphEditorAction.CreateRuleNode, item.handlerType));
            }

            menu.AddSeparator("");
            menu.AddItem("Add Comment Node", HandleContextMenu, new FlowExecGraphContextMenuUserData(uiSystem, FlowExecGraphEditorAction.CreateCommentNode));
            menu.Show();
        }

        void HandleContextMenu(object action)
        {
            var item = action as FlowExecGraphContextMenuUserData;
            DispatchMenuItemEvent(action, BuildEvent(null, item.uiSystem));
        }

        System.Type[] GetNodeHandlerTypes()
        {
            var assembly = System.Reflection.Assembly.GetAssembly(typeof(FlowExecTask)); // Search the runtime module
            var handlers = from t in assembly.GetTypes()
                           where t.IsClass && t.IsSubclassOf(typeof(FlowExecTask))
                           select t;

            return handlers.ToArray();
        }
    }
}
