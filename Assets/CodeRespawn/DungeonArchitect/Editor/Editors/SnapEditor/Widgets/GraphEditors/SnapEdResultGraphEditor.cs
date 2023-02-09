//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using DungeonArchitect.Graphs;
using DungeonArchitect.Grammar;
using DungeonArchitect.RuntimeGraphs;
using DungeonArchitect.Graphs.Layouts;
using DungeonArchitect.Graphs.Layouts.Layered;
using DungeonArchitect.Graphs.Layouts.Spring;
using DungeonArchitect.UI.Widgets.GraphEditors;
using DungeonArchitect.UI;

namespace DungeonArchitect.Editors.SnapFlow
{
    public enum SnapEdResultGraphEditorAction
    {
        CreateTaskNode,
        CreateCommentNode
    }

    public class SnapEdResultGraphEditor : GraphEditor
    {
        [SerializeField]
        public SnapEdResultGraphEditorConfig ResultGraphPanelConfig { get; private set; }

        public override void Init(Graph graph, Rect editorBounds, UnityEngine.Object assetObject, UISystem uiSystem)
        {
            base.Init(graph, editorBounds, assetObject, uiSystem);
            if (ResultGraphPanelConfig == null)
            {
                ResultGraphPanelConfig = CreateInstance<SnapEdResultGraphEditorConfig>();
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();

            if (ResultGraphPanelConfig == null)
            {
                ResultGraphPanelConfig = CreateInstance<SnapEdResultGraphEditorConfig>();
            }
        }

        protected override IGraphLinkRenderer CreateGraphLinkRenderer()
        {
            return new StraightLineGraphLinkRenderer();
        }

        protected override GraphContextMenu CreateContextMenu()
        {
            return new SnapEdResultGraphContextMenu();
        }

        protected override void InitializeNodeRenderers(GraphNodeRendererFactory nodeRenderers)
        {
            nodeRenderers.RegisterNodeRenderer(typeof(GrammarTaskNode), new GrammarTaskNodeRenderer());
            nodeRenderers.RegisterNodeRenderer(typeof(CommentNode), new CommentNodeRenderer(EditorStyle.commentTextColor));
        }

        protected override void OnMenuItemClicked(object userdata, GraphContextMenuEvent e)
        {
            var action = (SnapEdResultGraphEditorAction)userdata;

            var mouseScreen = lastMousePosition;
            if (action == SnapEdResultGraphEditorAction.CreateTaskNode)
            {
                //CreateSpatialNodeAtMouse<SCRuleNode>(mouseScreen);
            }
            else if (action == SnapEdResultGraphEditorAction.CreateCommentNode)
            {
                CreateCommentNode(mouseScreen, e.uiSystem);
            }
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
                uiSystem.Platform.ShowObjectProperty(ResultGraphPanelConfig);
            }
        }

        public class RuntimeGraphLayoutNodeActions : IGraphLayoutNodeActions<RuntimeGraphNode<GrammarRuntimeGraphNodeData>>
        {
            public void SetNodePosition(RuntimeGraphNode<GrammarRuntimeGraphNodeData> node, Vector2 position)
            {
                node.Position = position;
            }

            public Vector2 GetNodePosition(RuntimeGraphNode<GrammarRuntimeGraphNodeData> node)
            {
                return node.Position;
            }

            public RuntimeGraphNode<GrammarRuntimeGraphNodeData>[] GetOutgoingNodes(RuntimeGraphNode<GrammarRuntimeGraphNodeData> node)
            {
                return node.Outgoing.ToArray();
            }
        }

        private void PerformLayout(GrammarRuntimeGraph runtimeGraph)
        {
            var layout = CreateGraphLayoutInstance();
            if (layout != null)
            {
                layout.Layout(runtimeGraph.Nodes.ToArray(), new RuntimeGraphLayoutNodeActions());
            }
        }

        private IGraphLayout<RuntimeGraphNode<GrammarRuntimeGraphNodeData>> CreateGraphLayoutInstance()
        {
            IGraphLayout<RuntimeGraphNode<GrammarRuntimeGraphNodeData>> layout = null;
            if (ResultGraphPanelConfig != null)
            {
                if (ResultGraphPanelConfig.layoutType == GraphLayoutType.Layered)
                {
                    var config = ResultGraphPanelConfig.configLayered;
                    if (config == null)
                    {
                        config = new GraphLayoutLayeredConfig();
                    }

                    layout = new GraphLayoutLayered<RuntimeGraphNode<GrammarRuntimeGraphNodeData>>(config);
                }
                else if (ResultGraphPanelConfig.layoutType == GraphLayoutType.Spring)
                {
                    var config = ResultGraphPanelConfig.configSpring;
                    if (config == null)
                    {
                        config = new GraphLayoutSpringConfig();
                    }
                    layout = new GraphLayoutSpring<RuntimeGraphNode<GrammarRuntimeGraphNodeData>>(config);
                }
            }

            return layout;
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

        public override GraphSchema GetGraphSchema()
        {
            return new SnapEdGrammarGraphSchema();
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

        protected override string GetGraphNotInitializedMessage()
        {
            return "Graph not initialize";
        }

        protected override GraphEditorStyle CreateEditorStyle()
        {
            var editorStyle = base.CreateEditorStyle();
            editorStyle.branding = "Result Graph";
            editorStyle.backgroundColor = new Color(0.15f, 0.2f, 0.15f);
            return editorStyle;
        }

        void ClearActiveGraph(UISystem uiSystem)
        {
            // Clear the existing graphs
            DeleteNodes(graph.Nodes.ToArray(), uiSystem);
            graph.Nodes.Clear();
            graph.Links.Clear();

        }
        public void RefreshGraph(Graph graph, GrammarRuntimeGraph runtimeGraph, UISystem uiSystem)
        {
            SetGraph(graph);

            PerformLayout(runtimeGraph);

            ClearActiveGraph(uiSystem);

            var map = new Dictionary<RuntimeGraphNode<GrammarRuntimeGraphNodeData>, GraphNode>();
            // Add nodes
            foreach (var runtimeNode in runtimeGraph.Nodes)
            {
                var screenCoord = camera.WorldToScreen(runtimeNode.Position);
                var graphNode = CreateNode<GrammarTaskNode>(screenCoord, uiSystem);
                graphNode.NodeType = runtimeNode.Payload.nodeType;
                graphNode.DisplayExecutionIndex = false;

                map.Add(runtimeNode, graphNode);
            }

            // Add links
            {
                foreach (var runtimeNode in runtimeGraph.Nodes)
                {
                    foreach (var outgoingRuntimeNode in runtimeNode.Outgoing)
                    {
                        GraphNode srcGraphNode = map.ContainsKey(runtimeNode) ? map[runtimeNode] : null;
                        GraphNode dstGraphNode = map.ContainsKey(outgoingRuntimeNode) ? map[outgoingRuntimeNode] : null;
                        if (srcGraphNode == null || dstGraphNode == null || srcGraphNode.OutputPin == null || dstGraphNode.InputPin == null)
                        {
                            Debug.LogWarning("Cannot create link in result graph due to invalid node state");
                            continue;
                        }
                        CreateLinkBetweenPins(srcGraphNode.OutputPin, dstGraphNode.InputPin, uiSystem);
                    }
                }
            }

            FocusCameraOnBestFit();
        }
    }
}
