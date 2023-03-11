//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Editors.Flow.Layout;
using DungeonArchitect.Flow.Domains;
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Domains.Layout.Tooling.Graph2D;
using DungeonArchitect.Flow.Exec;
using DungeonArchitect.Flow.Items;
using DungeonArchitect.Graphs;
using DungeonArchitect.UI;
using DungeonArchitect.UI.Widgets;
using DungeonArchitect.UI.Widgets.GraphEditors;
using UnityEngine;

namespace DungeonArchitect.Editors.Flow.DomainEditors.Layout2D
{
    public class Layout2DGraphDomainEditor : FlowDomainEditor
    {
        FlowLayoutGraph layoutGraph;
        
        [SerializeField]
        FlowLayoutToolGraph2D layoutGraphTool;
        
        [SerializeField]
        GraphPanel<FlowPreviewLayoutGraphEditor> layoutGraphPanel;

        public FlowItem SelectedItem { get; set; }  = null;
        public FlowLayoutGraphNode SelectedLayoutNode { get; set; } = null;

        public delegate void GraphNodeDelegate(object sender, GraphNodeEventArgs e);
        public delegate void ItemStateDelegate(FlowItem item);

        public event GraphNodeDelegate NodeSelectionChanged;
        public event GraphNodeDelegate NodeDoubleClicked;
        public event ItemStateDelegate ItemSelectionChanged;
        
        
        public override IWidget Content { get; protected set; }
        public override bool StateValid
        {
            get => (layoutGraph != null);
        }
        
        public override void Init(IFlowDomain domain, FlowEditorConfig editorConfig, UISystem uiSystem)
        {
            base.Init(domain, editorConfig, uiSystem);
            layoutGraphTool = ScriptableObject.CreateInstance<FlowLayoutToolGraph2D>();
            layoutGraphPanel = new GraphPanel<FlowPreviewLayoutGraphEditor>(layoutGraphTool, null, uiSystem);
            layoutGraphPanel.Border.SetTitle("Result: Layout Graph");
            layoutGraphPanel.Border.SetColor(new Color(0.2f, 0.3f, 0.2f));
            layoutGraphPanel.GraphEditor.EditorStyle.branding = "Layout";
            layoutGraphPanel.GraphEditor.EditorStyle.displayAssetFilename = false;
            layoutGraphPanel.GraphEditor.ItemSelectionChanged += OnItemSelectionChanged;
            layoutGraphPanel.GraphEditor.Events.OnNodeSelectionChanged.Event += OnNodeSelectionChanged;
            layoutGraphPanel.GraphEditor.Events.OnNodeDoubleClicked.Event += OnNodeDoubleClicked;
            Content = layoutGraphPanel;
        }
        
        private void OnNodeSelectionChanged(object sender, GraphNodeEventArgs e)
        {
            var previewNode = (e.Nodes.Length == 1) ? e.Nodes[0] as FlowLayoutToolGraph2DNode : null;
            var previewLayoutNode = (previewNode != null) ? previewNode.LayoutNode : null;

            if (previewLayoutNode != SelectedLayoutNode)
            {
                SelectedLayoutNode = previewLayoutNode;
            }

            if (NodeSelectionChanged != null)
            {
                NodeSelectionChanged.Invoke(sender, e);
            }
        }

        private void OnNodeDoubleClicked(object sender, GraphNodeEventArgs e)
        {
            if (NodeDoubleClicked != null)
            {
                NodeDoubleClicked.Invoke(sender, e);
            }
        }

        private void OnItemSelectionChanged(FlowItem item)
        {
            if (item != SelectedItem)
            {
                SelectedItem = item;
            }

            if (ItemSelectionChanged != null)
            {
                ItemSelectionChanged.Invoke(item);
            }
        }
        
        public override void Destroy()
        {
            if (layoutGraphTool != null)
            {
                Object.DestroyImmediate(layoutGraphTool);
                layoutGraphTool = null;
            }
        }
        
        public override void UpdateNodePreview(FlowExecTaskState taskState)
        {
            if (taskState != null)
            {
                layoutGraph = taskState.GetState<FlowLayoutGraph>();
                FlowLayoutToolGraph2DBuilder.Build(layoutGraph, new NonEditorGraphBuilder(layoutGraphTool));
            }
        }

        public override void SetReadOnly(bool readOnly)
        {
            if (layoutGraphPanel != null && layoutGraphPanel.GraphEditor != null)
            {
                layoutGraphPanel.GraphEditor.SetReadOnly(readOnly);
            }
        }

        public FlowLayoutGraph FlowGraph
        {
            get => layoutGraph;
            set => layoutGraph = value;
        }
        
        public FlowPreviewLayoutGraphEditor GraphEditor
        {
            get => (layoutGraphPanel != null) ? layoutGraphPanel.GraphEditor : null;
        }

    }
}