//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Editors.Flow.DomainEditors.Layout2D;
using DungeonArchitect.Editors.Flow.Tilemap;
using DungeonArchitect.Flow.Domains;
using DungeonArchitect.Flow.Domains.Tilemap;
using DungeonArchitect.Flow.Domains.Tilemap.Tooling;
using DungeonArchitect.Flow.Exec;
using DungeonArchitect.Graphs;
using DungeonArchitect.UI;
using DungeonArchitect.UI.Widgets;
using DungeonArchitect.UI.Widgets.GraphEditors;
using UnityEngine;

namespace DungeonArchitect.Editors.Flow.DomainEditors.Tilemap
{
    public class TilemapFlowDomainEditor : FlowDomainEditor
    {
        FlowTilemap tilemap = null;
        
        [SerializeField]
        FlowTilemapToolGraph tilemapTool;
        
        [SerializeField]
        GraphPanel<FlowPreviewTilemapGraphEditor> tilemapPanel;
        
        Layout2DGraphDomainEditor layoutDomainEditor;
        IntVector2 lastTilemapCellClicked = IntVector2.Zero;
        bool tilemapDirty = false;
        
        public delegate void TileEventDelegate(FlowTilemap tilemap, int tileX, int tileY);
        public event TileEventDelegate TileClicked;
        public event TileEventDelegate TileDoubleClicked;
        
        FlowTilemapToolBuildContext BuildContext { get; } = new FlowTilemapToolBuildContext();

        public override IWidget Content { get; protected set; }

        public override bool StateValid
        {
            get => tilemap != null;
        }
        
        public FlowTilemap Tilemap
        {
            get => tilemap;
            set => tilemap = value;
        }

        public void SetLayoutDomainEditor(Layout2DGraphDomainEditor layoutDomainEditor)
        {
            this.layoutDomainEditor = layoutDomainEditor;
        }

        public override void Init(IFlowDomain domain, FlowEditorConfig editorConfig, UISystem uiSystem)
        {
            base.Init(domain, editorConfig, uiSystem);
            tilemapTool = ScriptableObject.CreateInstance<FlowTilemapToolGraph>();
            tilemapPanel = new GraphPanel<FlowPreviewTilemapGraphEditor>(tilemapTool, null, uiSystem);
            tilemapPanel.Border.SetTitle("Result: Tilemap");
            tilemapPanel.Border.SetColor(new Color(0.3f, 0.2f, 0.2f));
            tilemapPanel.GraphEditor.EditorStyle.branding = "Tilemap";
            tilemapPanel.GraphEditor.EditorStyle.displayAssetFilename = false;
            tilemapPanel.GraphEditor.TileClicked += TilemapGraphEditor_TileClicked;
            tilemapPanel.GraphEditor.Events.OnNodeDoubleClicked.Event += TilemapGraphEditor_OnNodeDoubleClicked;
            Content = tilemapPanel;
        }
        
        private void TilemapGraphEditor_TileClicked(FlowTilemap tilemap, int tileX, int tileY)
        {
            lastTilemapCellClicked.Set(tileX, tileY);
            
            if (TileClicked != null)
            {
                TileClicked.Invoke(tilemap, tileX, tileY);
            }
        }

        private void TilemapGraphEditor_OnNodeDoubleClicked(object sender, GraphNodeEventArgs e)
        {
            if (TileDoubleClicked != null)
            {
                TileDoubleClicked.Invoke(tilemap, lastTilemapCellClicked.x, lastTilemapCellClicked.y);
            }
        }
        
        public override void UpdateNodePreview(FlowExecTaskState taskState)
        {
            if (taskState != null)
            {
                tilemap = taskState.GetState<FlowTilemap>();
                RebuildTilemap();
            }
        }
        
        public override void Update()
        {   
            if (tilemapDirty)
            {
                RebuildTilemap();
                tilemapDirty = false;
            }
        }

        public override void Invalidate()
        {
            tilemapDirty = true;
        }
        
        public override void SetReadOnly(bool readOnly)
        {
            if (tilemapPanel != null && tilemapPanel.GraphEditor != null)
            {
                tilemapPanel.GraphEditor.SetReadOnly(readOnly);
            }
        }

        void RebuildTilemap()
        {
            if (tilemap != null)
            {
                BuildContext.tilemap = tilemap;
                BuildContext.graphBuilder = new NonEditorGraphBuilder(tilemapTool);
                BuildContext.LayoutGraph = layoutDomainEditor.FlowGraph;
                BuildContext.selectedNode = layoutDomainEditor.SelectedLayoutNode;
                BuildContext.selectedItem = layoutDomainEditor.SelectedItem;
                FlowTilemapToolGraphBuilder.Build(BuildContext);

                if (tilemapPanel != null)
                {
                    tilemapPanel.GraphEditor.UpdateGridSpacing();
                }
            }
        }
    }
}