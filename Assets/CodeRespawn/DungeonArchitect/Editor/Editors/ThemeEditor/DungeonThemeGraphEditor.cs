//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;
using DungeonArchitect.Graphs;
using DungeonArchitect.SpatialConstraints;
using DungeonArchitect.Editors.SpatialConstraints;
using DungeonArchitect.Editors.Utils;
using DungeonArchitect.Editors.Visualization;
using DungeonArchitect.UI.Widgets.GraphEditors;
using DungeonArchitect.UI;

namespace DungeonArchitect.Editors
{
    public class DungeonThemeGraphEditor : GraphEditor
    {
        // tracks dungeon objects in the scene that have the same graph being edited. This is used for realtime updates
        readonly DungeonObjectTracker dungeonObjectTracker = new DungeonObjectTracker();

        [SerializeField]
        public bool realtimeUpdate = true;

        [SerializeField]
        public bool visualizeMarkers = false;
        
        [SerializeField]
        ThemeEditorSceneVisualizer visualizer = new ThemeEditorSceneVisualizer();
        
        
        protected override void InitializeNodeRenderers(GraphNodeRendererFactory nodeRenderers)
        {
            nodeRenderers.RegisterNodeRenderer(typeof(MarkerNode), new MarkerNodeRenderer());
            nodeRenderers.RegisterNodeRenderer(typeof(GameObjectNode), new MeshNodeRenderer());
            nodeRenderers.RegisterNodeRenderer(typeof(GameObjectArrayNode), new MeshArrayNodeRenderer());
            nodeRenderers.RegisterNodeRenderer(typeof(SpriteNode), new SpriteNodeRenderer());
            nodeRenderers.RegisterNodeRenderer(typeof(MarkerEmitterNode), new MarkerEmitterNodeRenderer());
            nodeRenderers.RegisterNodeRenderer(typeof(CommentNode), new CommentNodeRenderer(EditorStyle.commentTextColor));
        }

        protected override IGraphLinkRenderer CreateGraphLinkRenderer()
        {
            return new SplineGraphLinkRenderer();
        }

        public override GraphSchema GetGraphSchema()
        {
            return new ThemeGraphSchema();
        }

        public Dungeon TrackedDungeon
        {
            get => (dungeonObjectTracker != null) ? dungeonObjectTracker.ActiveDungeon : null;
        }

        
        /// <summary>
        /// Moves the graph editor viewport to show the marker on the screen
        /// </summary>
        /// <param name="markerName">The name of the marker to focus on</param>
        /// <param name="editorBounds">The bounds of the editor</param>
        public void FocusCameraOnMarker(string markerName, Rect editorBounds)
        {
            GraphNode nodeToFocus = null;
            foreach (var node in graph.Nodes)
            {
                if (node is MarkerNode)
                {
                    var markerNode = node as MarkerNode;
                    if (markerNode.Caption == markerName)
                    {
                        nodeToFocus = node;
                        break;
                    }
                }
            }

            camera.FocusOnNode(nodeToFocus, editorBounds);
        }
        
        public override void Update()
        {
            base.Update();

            if (realtimeUpdate)
            {
                dungeonObjectTracker.ActiveGraph = graph;
                dungeonObjectTracker.Update();
            }
        }

        public override void HandleGraphStateChanged(UISystem uiSystem)
        {
            base.HandleGraphStateChanged(uiSystem);
            if (realtimeUpdate)
            {
                dungeonObjectTracker.RequestRebuild();
            }
        }

        public override void Init(Graph graph, Rect editorBounds, UnityEngine.Object assetObject, UISystem uiSystem)
        {
            base.Init(graph, editorBounds, assetObject, uiSystem);

            // Make sure the spatial constraint assets are created and bound to the asset file 
            foreach (var node in graph.Nodes)
            {
                if (node is VisualNode)
                {
                    var visualNode = node as VisualNode;
                    if (visualNode.spatialConstraint == null)
                    {
                        CreateSpatialConstraintAsset(visualNode, uiSystem);
                    }
                }
            }

            dungeonObjectTracker.ActiveGraph = graph;
            
            UpdateSpatialConstraintEditorWindow(uiSystem);
            
                        
            if (visualizeMarkers) {
                UpdateMarkerVisualizer();
            }
        }

        public void ClearMarkerVisualizer()
        {
            if (visualizer != null)
            {
                visualizer.Clear();
            }
        }
        
        public void UpdateMarkerVisualizer()
        {
            if (visualizer == null) return;
            if (!visualizeMarkers)
            {
                visualizer.Clear();
                return;
            }

            if (Graph == null)
            {
                return;
            }
            
            var selectedNodes = new List<GraphNode>();
            foreach (var node in Graph.Nodes)
            {
                if (node.Selected)
                {
                    selectedNodes.Add(node);
                }
            }

            
            MarkerNode markerNode = (selectedNodes.Count == 1) 
                ? ThemeEditorUtils.GetMarkerNodeInHierarchy(selectedNodes[0]) 
                : null;

            var dungeon = TrackedDungeon;
            if (markerNode != null && dungeon != null)
            {
                if (!dungeon.IsLayoutBuilt)
                {
                    dungeon.Build(new EditorDungeonSceneObjectInstantiator());
                }
                visualizer.Build(dungeon, markerNode.MarkerName);
            }
            else
            {
                visualizer.Clear();
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            
            if (visualizer != null)
            {
                visualizer.Clear();
            }
        }

        public override GraphNode CreateNode(Vector2 screenCoord, System.Type nodeType, UISystem uiSystem)
        {
            var node = base.CreateNode(screenCoord, nodeType, uiSystem);

            if (node is VisualNode)
            {
                var visualNode = node as VisualNode;
                CreateSpatialConstraintAsset(visualNode, uiSystem);
            }

            return node;
        }

        void CreateSpatialConstraintAsset(VisualNode visualNode, UISystem uiSystem)
        {
            if (visualNode.spatialConstraint == null)
            {
                visualNode.spatialConstraint = CreateInstance<SpatialConstraintAsset>();
                visualNode.spatialConstraint.Init(visualNode);
                AssetDatabase.AddObjectToAsset(visualNode.spatialConstraint, graph);
                AssetDatabase.AddObjectToAsset(visualNode.spatialConstraint.Graph, graph);
                DungeonEditorHelper.CreateDefaultSpatialConstraintNodes(visualNode.spatialConstraint, null);
            }
        }

        protected override void DestroyNode(GraphNode node, UISystem uiSystem)
        {
            if (node is VisualNode)
            {
                // Destroy the spatial constraint asset
                var visualNode = node as VisualNode;
                DungeonEditorHelper.DestroySpatialConstraintAsset(visualNode.spatialConstraint);
                visualNode.spatialConstraint = null;
            }
            
            // If this is a marker node, delete all the referenced emitter nodes as well
            if (node is MarkerNode)
            {
                DestroyEmitterNodes(graph, node as MarkerNode, uiSystem.Undo);
            }
            
            base.DestroyNode(node, uiSystem);
        }

        private static void DestroyEmitterNodes(Graph graph, MarkerNode markerNode, UIUndoSystem undo)
        {
            var emitterNodes = new List<GraphNode>();
            foreach (var node in graph.Nodes)
            {
                if (node is MarkerEmitterNode)
                {
                    var emitterNode = node as MarkerEmitterNode;
                    if (emitterNode.Marker == markerNode)
                    {
                        emitterNodes.Add(emitterNode);
                    }
                }
            }

            // delete the emitter nodes
            foreach (var emitterNode in emitterNodes)
            {
                GraphOperations.DestroyNode(emitterNode, undo);
            }
        }

        protected override GraphContextMenu CreateContextMenu()
        {
            return new DungeonThemeEditorContextMenu();
        }

        protected override void SortNodesForDeletion(GraphNode[] nodesToDelete)
        {
            System.Array.Sort(nodesToDelete, new NodeDeletionOrderComparer());
        }

        public override void SortPinsForDrawing(GraphPin[] pins)
        {
            Array.Sort(pins, new GraphPinHierarchyComparer());
        }

        protected override void OnMenuItemClicked(object userdata, GraphContextMenuEvent e)
        {
            var action = (ThemeEditorMenuAction)userdata;
            var mouseScreen = lastMousePosition;
            GraphNode node = null;
            if (action == ThemeEditorMenuAction.AddGameObjectNode)
            {
                node = CreateNode<GameObjectNode>(mouseScreen, e.uiSystem);
                SelectNode(node, e.uiSystem);
            }
            if (action == ThemeEditorMenuAction.AddGameObjectArrayNode)
            {
                node = CreateNode<GameObjectArrayNode>(mouseScreen, e.uiSystem);
                SelectNode(node, e.uiSystem);
            }
            else if (action == ThemeEditorMenuAction.AddSpriteNode)
            {
                node = CreateNode<SpriteNode>(mouseScreen, e.uiSystem);
                SelectNode(node, e.uiSystem);
            }
            else if (action == ThemeEditorMenuAction.AddMarkerNode)
            {
                node = CreateNode<MarkerNode>(mouseScreen, e.uiSystem);
                SelectNode(node, e.uiSystem);
            }
            else if (action == ThemeEditorMenuAction.AddCommentNode)
            {
                node = CreateNode<CommentNode>(mouseScreen, e.uiSystem);
                SelectNode(node, e.uiSystem);
            }
            else if (action == ThemeEditorMenuAction.AddMarkerEmitterNode)
            {
                if (e.userdata != null)
                {
                    var markerName = e.userdata as String;
                    node = CreateMarkerEmitterNode(mouseScreen, markerName, e.uiSystem);
                    if (node != null)
                    {
                        SelectNode(node, e.uiSystem);
                    }
                }
            }


            if (node != null)
            {
                // Check if the menu was created by dragging out a link
                if (e.sourcePin != null)
                {
                    GraphPin targetPin =
                            e.sourcePin.PinType == GraphPinType.Input ?
                            node.OutputPins[0] :
                            node.InputPins[0];

                    // Align the target pin with the mouse position where the link was dragged and released
                    node.Position = e.mouseWorldPosition - targetPin.Position;

                    GraphPin inputPin, outputPin;
                    if (e.sourcePin.PinType == GraphPinType.Input)
                    {
                        inputPin = e.sourcePin;
                        outputPin = targetPin;
                    }
                    else
                    {
                        inputPin = targetPin;
                        outputPin = e.sourcePin;
                    }
                    CreateLinkBetweenPins(outputPin, inputPin, e.uiSystem);
                }
            }
        }
        
        public override void OnNodeSelectionChanged(UISystem uiSystem)
        {
            base.OnNodeSelectionChanged(uiSystem);

            UpdateSpatialConstraintEditorWindow(uiSystem);
            
            if (visualizeMarkers) {
                UpdateMarkerVisualizer();
            }
        }

        void UpdateSpatialConstraintEditorWindow(UISystem uiSystem)
        {
            var selectedNodes = from node in graph.Nodes
                                where (node.Selected && node is VisualNode)
                                select node as VisualNode;

            var nodes = selectedNodes.ToArray();

            // Set the active spatial constraint asset in the editor window
            UpdateSpatialConstraintEditorWindow(nodes);
        }

        public override void HandleNodePropertyChanged(GraphNode node)
        {
            UpdateSpatialConstraintEditorWindow(new GraphNode[] { node });
        }

        void UpdateSpatialConstraintEditorWindow(GraphNode[] nodes)
        {
            SpatialConstraintAsset spatialAsset = null;
            SpatialConstraintsEditorAssignmentState assetState;
            VisualNode node = null;
            if (nodes.Length == 1 && nodes[0] is VisualNode)
            {
                node = nodes[0] as VisualNode;
            }

            if (node != null)
            {
                if (node.spatialConstraint == null)
                {
                    Debug.LogError("Spatial Constraint asset not assigned. Please reload the theme editor");
                    return;
                }

                if (node.useSpatialConstraint)
                {
                    spatialAsset = node.spatialConstraint;
                    assetState = SpatialConstraintsEditorAssignmentState.Assigned;
                }
                else
                {
                    assetState = SpatialConstraintsEditorAssignmentState.ConstraintsDisabled;
                }
            }
            else
            {
                assetState = SpatialConstraintsEditorAssignmentState.NotAssigned;
            }

            SetSpatialConstraintEditorAsset(spatialAsset, assetState);
        }

        class SpatialConstraintWindowState
        {
            public SpatialConstraintAsset spatialAsset = null;
            public SpatialConstraintsEditorAssignmentState assignmentState = SpatialConstraintsEditorAssignmentState.NotAssigned;
            public bool initialized = false;
        }
        SpatialConstraintWindowState spatailWindowState = new SpatialConstraintWindowState();

        void SetSpatialConstraintEditorAsset(SpatialConstraintAsset spatialAsset, SpatialConstraintsEditorAssignmentState state)
        {
            if (spatailWindowState.initialized
                && spatailWindowState.spatialAsset == spatialAsset 
                && spatailWindowState.assignmentState == state)
            {
                // The states are the same. no need to assign
                return;
            }

            spatailWindowState.initialized = true;
            spatailWindowState.spatialAsset = spatialAsset;
            spatailWindowState.assignmentState = state;

            var existingWindow = DungeonEditorHelper.GetWindowIfOpen<SpatialConstraintsEditorWindow>();

            // Open a window only if we are assigning
            if (state == SpatialConstraintsEditorAssignmentState.Assigned && existingWindow == null)
            {
                existingWindow = EditorWindow.GetWindow<SpatialConstraintsEditorWindow>();
            }

            if (existingWindow != null)
            {
                existingWindow.Init(spatialAsset, state);
            }
        }

        protected override string GetGraphNotInitializedMessage()
        {
            return "Please open a theme file to edit";
        }

        MarkerEmitterNode CreateMarkerEmitterNode(Vector2 mouseScreenPos, string markerName, UISystem uiSystem)
        {
            // find the marker node with this name
            MarkerNode markerNode = null;
            foreach (var node in graph.Nodes)
            {
                if (node is MarkerNode)
                {
                    var marker = node as MarkerNode;
                    if (marker.Caption == markerName)
                    {
                        markerNode = marker;
                        break;
                    }
                }
            }

            if (markerNode == null)
            {
                // No marker node found with this ids
                return null;
            }
            var emitterNode = CreateNode<MarkerEmitterNode>(mouseScreenPos, uiSystem);
            emitterNode.Marker = markerNode;
            return emitterNode;
        }
    }

    /// <summary>
    /// Tracks active dungeon objects in the scene and finds ones that have the active graph being edited
    /// This is used for real-time updates on the dungeon object as the graph is modified from the editor
    /// </summary>
    class DungeonObjectTracker
    {
        Graph activeGraph;

        /// <summary>
        /// The active graph being edited by the theme graph editor
        /// </summary>
        public Graph ActiveGraph
        {
            get
            {
                return activeGraph;
            }
            set
            {
                activeGraph = value;
            }
        }

        public Dungeon ActiveDungeon
        {
            get
            {
                if (_dungeons.Length == 0)
                {
                    FindDungeonObjects();
                }
                return _dungeons.Length > 0 ? _dungeons[0] : null;  
            } 
        }

        bool requestRebuild = false;

        Dungeon[] _dungeons = new Dungeon[0];


        public void Update()
        {
            if (activeGraph == null)
            {
                return;
            }

            _dungeons = _dungeons.Where(d => d != null && d.dungeonThemes.Contains(activeGraph)).ToArray();

            if (requestRebuild)
            {
                RebuildDungeon();
                requestRebuild = false;
            }
        }

        /// <summary>
        /// Finds all dungeon objects in the scene that use the theme graph tracked by this object
        /// </summary>
        void FindDungeonObjects()
        {
            _dungeons = UnityEngine.Object.FindObjectsOfType<Dungeon>();
            _dungeons = _dungeons.Where(d => d != null && d.dungeonThemes.Contains(activeGraph)).ToArray();
        }

        /// <summary>
        /// Rebuilds the dungeons that reference the theme graphs tracked by this object
        /// </summary>
        public void RequestRebuild()
        {
            requestRebuild = true;
        }

        void RebuildDungeon()
        {
            if (_dungeons.Length == 0)
            {
                FindDungeonObjects();
            }

            foreach (var dungeon in _dungeons)
            {
                if (dungeon == null) continue;
                if (!dungeon.IsLayoutBuilt)
                {
                    dungeon.Build(new EditorDungeonSceneObjectInstantiator());
                }
                else
                {
                    // Do not rebuild the layout as it has already been built. Just reapply the theme on the existing layout
                    dungeon.ReapplyTheme(new EditorDungeonSceneObjectInstantiator());
                }
            }
        }
    }


    /// <summary>
    /// Sorts based on the node's Z-index in descending order
    /// </summary>
    class NodeDeletionOrderComparer : IComparer<GraphNode>
    {
        int GetWeight(GraphNode node)
        {
            if (node is MarkerEmitterNode) return 0;
            if (node is VisualNode) return 1;
            if (node is MarkerNode) return 2;
            return 3;
        }

        public int Compare(GraphNode x, GraphNode y)
        {
            int wx = GetWeight(x);
            int wy = GetWeight(y);
            if (wx == wy) return 0;
            return (wx < wy) ? -1 : 1;
        }
    }

    /// <summary>
    /// Sorts the pins based on their owning node's type 
    /// </summary>
    class GraphPinHierarchyComparer : IComparer<GraphPin>
    {
        int GetWeight(GraphNode node)
        {
            if (node is MarkerNode) return 1;
            else if (node is VisualNode) return 2;
            else if (node is MarkerEmitterNode) return 3;
            else return 4;
        }

        public int Compare(GraphPin x, GraphPin y)
        {
            if (x == null || y == null) return 0;
            var wx = GetWeight(x.Node);
            var wy = GetWeight(y.Node);

            if (wx == wy) return 0;
            return wx < wy ? -1 : 1;
        }
    }
}
