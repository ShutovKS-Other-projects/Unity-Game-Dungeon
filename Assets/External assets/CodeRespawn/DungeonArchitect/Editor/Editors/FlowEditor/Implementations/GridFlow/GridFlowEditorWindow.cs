//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Builders.GridFlow;
using DungeonArchitect.Editors.Flow.DomainEditors.Layout2D;
using DungeonArchitect.Editors.Flow.DomainEditors.Tilemap;
using DungeonArchitect.Flow.Domains.Tilemap;
using DungeonArchitect.Flow.Exec;
using DungeonArchitect.Flow.Impl.GridFlow;
using DungeonArchitect.Flow.Items;
using DungeonArchitect.UI.Widgets;
using DungeonArchitect.UI.Widgets.GraphEditors;
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Editors.Flow.Impl
{
    public class GridFlowEditorWindow : FlowEditorWindow
    {
        private Layout2DGraphDomainEditor layoutDomainEditor;
        private TilemapFlowDomainEditor tilemapDomainEditor;
        
        IWidget widgetSetupLayoutOnly;
        IWidget widgetSetupLayoutAndTilemap;

        protected override IWidget DomainLayoutWidget { get; set; }
        protected override string WindowTitle { get => "Grid Flow Editor"; }
        
        public static void ShowWindow()
        {
            var window = EditorWindow.GetWindow<GridFlowEditorWindow>();
            window.Init(null);
        }

        protected override FlowEditorConfig CreateEditorConfig()
        {
            return CreateInstance<GridFlowEditorConfig>();
        }

        protected override FlowExecNodeOutputRegistry GetLinkedDungeonNodeOutputRegistry()
        {
            var config = editorConfig as GridFlowEditorConfig;
            return config != null ? config.dungeonBuilder.ExecNodeOutputRegistry : null;
        }

        protected override void InitDomains()
        {
            layoutDomainEditor = CreateInstance<Layout2DGraphDomainEditor>();
            layoutDomainEditor.Init(new GridFlowLayoutGraphDomain(), editorConfig, uiSystem);
            layoutDomainEditor.NodeSelectionChanged += LayoutDomainEditor_OnNodeSelectionChanged; 
            layoutDomainEditor.ItemSelectionChanged += LayoutDomainEditor_OnItemSelectionChanged;
            RegisterDomainEditor(layoutDomainEditor);

            tilemapDomainEditor = CreateInstance<TilemapFlowDomainEditor>();
            tilemapDomainEditor.SetLayoutDomainEditor(layoutDomainEditor);
            tilemapDomainEditor.Init(new GridFlowTilemapDomain(), editorConfig, uiSystem);
            tilemapDomainEditor.TileClicked += TilemapDomainEditor_OnTileClicked;
            tilemapDomainEditor.TileDoubleClicked += TilemapDomainEditor_OnTileDoubleClicked;
            RegisterDomainEditor(tilemapDomainEditor);
            
            widgetSetupLayoutOnly = layoutDomainEditor.Content; 
            widgetSetupLayoutAndTilemap =
                new Splitter(SplitterDirection.Horizontal)
                    .AddWidget(layoutDomainEditor.Content, 1)
                    .AddWidget(tilemapDomainEditor.Content, 1);
        }

        private void LayoutDomainEditor_OnItemSelectionChanged(FlowItem item)
        {
            tilemapDomainEditor.Invalidate();
        }

        private void LayoutDomainEditor_OnNodeSelectionChanged(object sender, GraphNodeEventArgs e)
        {
            tilemapDomainEditor.Invalidate();
        }

        protected override void UpdateDomainPreview(FlowExecTaskState taskState)
        {
            base.UpdateDomainPreview(taskState);
            
            DomainLayoutWidget = null; 

            if (layoutDomainEditor.StateValid && tilemapDomainEditor.StateValid)
            {
                // Show the result layout for tilemap
                DomainLayoutWidget = widgetSetupLayoutAndTilemap;
            }
            else if (layoutDomainEditor.StateValid)
            {
                // Show the result layout graphs
                DomainLayoutWidget = widgetSetupLayoutOnly;
            }
        }
        
        protected override bool IsDomainStateInvalid()
        {
            return (layoutDomainEditor == null || tilemapDomainEditor == null || !layoutDomainEditor.IsInitialized() || !tilemapDomainEditor.IsInitialized());
        }


        private void TilemapDomainEditor_OnTileDoubleClicked(FlowTilemap tilemap, int tileX, int tileY)
        {
            if (tilemap != null)
            {
                bool valid = false;
                var bounds = new Bounds();
                var cellId = tileY * tilemap.Width + tileX;
                var items = GameObject.FindObjectsOfType<DungeonSceneProviderData>();
                GameObject selectedObject = null;
                foreach (var item in items)
                {
                    if (item.userData == cellId)
                    {
                        var renderer = item.gameObject.GetComponent<Renderer>();
                        if (renderer != null)
                        {
                            if (!valid)
                            {
                                valid = true;
                                bounds = renderer.bounds;
                            }
                            else
                            {
                                bounds.Encapsulate(renderer.bounds);
                            }
                        }

                        selectedObject = item.gameObject;
                    }
                }

                if (valid && SceneView.lastActiveSceneView != null)
                {
                    bounds.center += new Vector3(0, bounds.extents.y, 0);
                    bounds.extents = new Vector3(bounds.extents.x, 1, bounds.extents.z);

                    SceneView.lastActiveSceneView.Frame(bounds, false);
                }

                if (selectedObject != null)
                {
                    Selection.activeGameObject = selectedObject;
                    EditorApplication.RepaintHierarchyWindow();
                }
            }
        }

        private void TilemapDomainEditor_OnTileClicked(FlowTilemap tilemap, int tileX, int tileY)
        {
            var cell = tilemap.Cells[tileX, tileY];
            if (cell.CellType == FlowTilemapCellType.Floor)
            {
                var nodeCoord = cell.NodeCoord;
                layoutDomainEditor.GraphEditor.SelectNodeAtCoord(nodeCoord, uiSystem);
            }

            layoutDomainEditor.GraphEditor.SelectNodeItem(cell.Item);
        }
        
        public override void SetReadOnly(bool readOnly)
        {
            base.SetReadOnly(readOnly);
            
            layoutDomainEditor.SetReadOnly(readOnly);
            tilemapDomainEditor.SetReadOnly(readOnly);
        }
    }

    public class GridFlowEditorConfig : FlowEditorConfig
    {
        public override DungeonBuilder FlowBuilder
        {
            get => dungeonBuilder;
        }
        
        public GridFlowDungeonBuilder dungeonBuilder;
    } 

    [CustomEditor(typeof(GridFlowEditorConfig), true)]
    public class GridFlowExecGraphEditorConfigInspector : FlowExecGraphEditorConfigInspector
    {
        SerializedProperty dungeonObject;

        protected override void OnEnable()
        {
            base.OnEnable();
            dungeonObject = sobject.FindProperty("dungeonBuilder");
        }

        protected override void DrawDungeonProperty()
        {
            EditorGUILayout.PropertyField(dungeonObject);
        }
    }
}