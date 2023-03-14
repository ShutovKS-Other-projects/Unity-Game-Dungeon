//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using DungeonArchitect.Graphs;
using DungeonArchitect.Graphs.SpatialConstraints;
using DungeonArchitect.SpatialConstraints;
using DungeonArchitect.UI;
using DungeonArchitect.UI.Widgets.GraphEditors;

namespace DungeonArchitect.Editors.SpatialConstraints
{
    enum SpatialConstraintEditorMenuAction
    {
        CreateRuleNode,
        CreateCommentNode,
        MakeOwnerNode
    }

    public enum SpatialConstraintsEditorAssignmentState
    {
        Assigned,
        NotAssigned,
        ConstraintsDisabled,
    }

    public class SpatialConstraintsGraphEditor : GraphEditor
    {
        [SerializeField]
        private bool realtimeUpdate = true;
        public bool RealtimeUpdate
        {
            get { return realtimeUpdate; }
            set {
                realtimeUpdate = value;
            }
        }

        [SerializeField]
        private SpatialConstraintsEditorAssignmentState assignmentState;
        public SpatialConstraintsEditorAssignmentState AssignmentState
        {
            get { return assignmentState; }
            set { assignmentState = value; }
        }

        [SerializeField]
        private SpatialConstraintAsset assetBeingEdited;
        public SpatialConstraintAsset AssetBeingEdited
        {
            get { return assetBeingEdited; }
            set { assetBeingEdited = value; }
        }

        protected override GraphContextMenu CreateContextMenu()
        {
            return new SpatialConstraintsEditorContextMenu();
        }

        protected override void InitializeNodeRenderers(GraphNodeRendererFactory nodeRenderers)
        {
            nodeRenderers.RegisterNodeRenderer(typeof(SCRuleNode), new SCDomainNodeRenderer());
            nodeRenderers.RegisterNodeRenderer(typeof(SCReferenceNode), new SCDomainNodeRenderer());
            nodeRenderers.RegisterNodeRenderer(typeof(CommentNode), new CommentNodeRenderer(EditorStyle.commentTextColor));
        }

        public override void HandleGraphStateChanged(UISystem uiSystem)
        {
            base.HandleGraphStateChanged(uiSystem);

            if (RealtimeUpdate)
            {
                var themeEditorWindow = DungeonEditorHelper.GetWindowIfOpen<DungeonThemeEditorWindow>();
                if (themeEditorWindow != null)
                {
                    var themeEditor = themeEditorWindow.GraphEditor;
                    themeEditor.HandleGraphStateChanged(themeEditorWindow.uiSystem);
                }
            }

            HandleMarkedDirty(uiSystem);
        }

        public override void HandleMarkedDirty(UISystem uiSystem)
        {
            // Mark the host graph as dirty

            if (assetBeingEdited != null && assetBeingEdited.hostThemeNode != null)
            {
                var hostGraph = assetBeingEdited.hostThemeNode.Graph;
                if (hostGraph != null)
                {
                    uiSystem.Platform.MarkAssetDirty(hostGraph);
                }
            }
        }

        protected override void OnMenuItemClicked(object userdata, GraphContextMenuEvent e)
        {
            var action = (SpatialConstraintEditorMenuAction)userdata;
            HandleAction(action, e);
        }

        void HandleAction(SpatialConstraintEditorMenuAction action, GraphContextMenuEvent e)
        {
            var mouseScreen = lastMousePosition;
            if (action == SpatialConstraintEditorMenuAction.CreateRuleNode)
            {
                CreateSpatialNodeAtMouse<SCRuleNode>(mouseScreen, e.uiSystem);
            }
            else if (action == SpatialConstraintEditorMenuAction.CreateCommentNode)
            {
                CreateCommentNode(mouseScreen, e.uiSystem);
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

        void CreateSpatialNodeAtMouse<T>(Vector2 screenPos, UISystem uiSystem) where T : SCBaseDomainNode
        {
            var worldPos = camera.ScreenToWorld(screenPos);
            var node = DungeonEditorHelper.CreateSpatialConstraintNode<T>(assetBeingEdited, worldPos, uiSystem.Undo);
            BringToFront(node);
            node.SnapNode();

            SelectNode(node, uiSystem);
        }

        protected override void DrawOverlay(UIRenderer renderer, Rect bounds)
        {
            if (draggingNodes)
            {
                var selectedNodes = graph.Nodes.Where(node => node.Selected && node is SCBaseDomainNode);
                if (selectedNodes.Count() > 0)
                {
                    DrawNodeDragOverlay(renderer, bounds);
                }

            }
        }

        private UnityEngine.Object GetAssetObject()
        {
            if (assetBeingEdited == null || assetBeingEdited.hostThemeNode == null) return null;
            return assetBeingEdited.hostThemeNode.Graph;
        }

        protected override GraphNode DuplicateNode(GraphNode sourceNode, UISystem uiSystem)
        {
            var copiedNode = base.DuplicateNode(sourceNode, uiSystem);

            if (copiedNode is SCRuleNode && sourceNode is SCRuleNode)
            {
                var sourceRuleNode = sourceNode as SCRuleNode;
                var copiedRuleNode = copiedNode as SCRuleNode;
                var assetObject = GetAssetObject();

                var constraintList = new List<ConstraintRule>();
                foreach (var sourceConstraint in sourceRuleNode.constraints)
                {
                    var constraint = Instantiate(sourceConstraint) as ConstraintRule;
                    constraintList.Add(constraint);
                    
                    AssetDatabase.AddObjectToAsset(constraint, assetObject);
                }
                copiedRuleNode.constraints = constraintList.ToArray();
            }

            return copiedNode;
        }

        protected override IGraphLinkRenderer CreateGraphLinkRenderer()
        {
            return new StraightLineGraphLinkRenderer();
        }

        void DrawNodeDragOverlay(UIRenderer renderer, Rect bounds)
        {
            var cursorWorld = camera.ScreenToWorld(lastMousePosition);

            var ringTexture = renderer.GetResource<Texture2D>(UIResourceLookup.TEXTURE_CURSOR_RING_SOLID) as Texture2D;

            int sx = Mathf.RoundToInt(cursorWorld.x / SCBaseDomainNode.TileSize);
            int sy = Mathf.RoundToInt(cursorWorld.y / SCBaseDomainNode.TileSize);
            int drawNodeRangeRadius = 4;
            float drawPixelRangeRadius = drawNodeRangeRadius * SCBaseDomainNode.TileSize;
            float maxCursorSize = 20;
            

            for (int x = sx - drawNodeRangeRadius; x <= sx + drawNodeRangeRadius; x++)
            {
                for (int y = sy - drawNodeRangeRadius; y <= sy + drawNodeRangeRadius; y++)
                {
                    for (int ox = 0; ox < 2; ox++)
                    {
                        for (int oy = 0; oy < 2; oy++)
                        {
                            var offset = new Vector2(ox, oy) * 0.5f;
                            var pos = (new Vector2(x, y) + offset) * SCBaseDomainNode.TileSize;

                            var distanceToCursor = (pos - cursorWorld).magnitude;
                            var cursorScale = Mathf.Clamp(1 - distanceToCursor / drawPixelRangeRadius, 0.0f, 1.0f);

                            // Ease the size
                            {
                                cursorScale = Mathf.Pow(cursorScale, 3);
                            }

                            float cursorSize = maxCursorSize * cursorScale;
                            var size = new Vector2(cursorSize, cursorSize);
                            pos -= size / 2.0f;
                            pos = camera.WorldToScreen(pos);

                            size /= camera.ZoomLevel;
                            var ringBounds = new Rect(pos, size);
                            var color = new Color(1, 0.3f, 0, cursorScale / 2.0f);
                            renderer.DrawTexture(ringBounds, ringTexture, ScaleMode.ScaleToFit, true, color);
                        }
                    }
                }
            }

            var selectedNodes = GetSelectedNodes();
            foreach (var node in selectedNodes)
            {
                var start = node.Bounds.center;
                var end = SCBaseDomainNode.GetSnapPosition(start);

                start = camera.WorldToScreen(start);
                end = camera.WorldToScreen(end);

                var color = new Color(0, 0, 0, 0.5f);
                renderer.DrawLine(color, start, end);
            }
        }
        
        public override void OnNodeSelectionChanged(UISystem uiSystem)
        {
            // Fetch all selected nodes
            var selectedNodes = (from node in graph.Nodes
                                where node.Selected
                                select node).ToArray();

            if (selectedNodes.Length > 0)
            {
                uiSystem.Platform.ShowObjectProperties(selectedNodes);
            }
            else
            {
                uiSystem.Platform.ShowObjectProperty(assetBeingEdited.hostThemeNode);
            }

        }
        
        protected override void SortNodesForDeletion(GraphNode[] nodesToDelete)
        {
        }

        public override void SortPinsForDrawing(GraphPin[] pins)
        {
        }

        protected override string GetGraphNotInitializedMessage()
        {
            if (AssignmentState == SpatialConstraintsEditorAssignmentState.NotAssigned)
            {
                return "Select a node in the theme editor to edit the Spatial Constraints";
            }
            else if (AssignmentState == SpatialConstraintsEditorAssignmentState.ConstraintsDisabled)
            {
                return "Enable Spatial Constraints in the node settings to start editing";
            }
            else
            {
                return "";
            }
        }

        public override void Init(Graph graph, Rect editorBounds, UnityEngine.Object assetObject, UISystem uiSystem)
        {
            base.Init(graph, editorBounds, assetObject, uiSystem);

            events.OnNodeDragStart.Event += OnNodeDragStart_Event;
            events.OnNodeDragEnd.Event += OnNodeDragEnd_Event;
            events.OnNodeDragged.Event += OnNodeDragged_Event;
            events.OnNodeCreated.Event += OnNodeCreated_Event;

            camera.MaxAllowedZoom = 2.0f;
            renderCullingBias = SCBaseDomainNode.TileSize / 2.0f;

            OnEnable();
        }

        private void OnNodeCreated_Event(object sender, GraphNodeEventArgs e)
        {
            foreach (var node in e.Nodes)
            {
                if (node is SCBaseDomainNode)
                {
                    var baseNode = node as SCBaseDomainNode;
                    baseNode.SnapNode();
                }
            }
        }

        void SnapNodes(GraphNode[] nodes)
        {
            foreach (var node in nodes)
            {
                if (node is SCBaseDomainNode)
                {
                    var baseNode = node as SCBaseDomainNode;
                    baseNode.SnapNode();
                }
            }
        }

        private void OnNodeDragged_Event(object sender, GraphNodeEventArgs e)
        {
            foreach (var node in e.Nodes)
            {
                if (node is SCBaseDomainNode)
                {
                    var baseNode = node as SCBaseDomainNode;
                    var snappedCenter = SCBaseDomainNode.GetSnapPosition(baseNode.Bounds.center);
                    bool bCannotBeSnapped = baseNode.ContainsOtherNodeAt(snappedCenter);
                    baseNode.IsSnapped = !bCannotBeSnapped;
                }
            }
        }

        private void OnNodeDragEnd_Event(object sender, GraphNodeEventArgs e)
        {
            SnapNodes(e.Nodes);
            HandleGraphStateChanged(e.uiSystem);
        }

        private void OnNodeDragStart_Event(object sender, GraphNodeEventArgs e)
        {
        }

        protected override GraphEditorStyle CreateEditorStyle()
        {
            var editorStyle = new GraphEditorStyle();
            editorStyle.backgroundColor = Color.white;
            editorStyle.gridCellSpacing = SCBaseDomainNode.TileSize / 2.0f;
            editorStyle.gridLineColorThick = new Color(0, 0, 0, 0.2f);
            editorStyle.gridLineColorThin = new Color(0, 0, 0, 0.05f);
            editorStyle.gridScaling = true;
            editorStyle.branding = "Spatial Constraints";
            editorStyle.brandingColor = new Color(0, 0, 0, 0.2f);
            editorStyle.overlayTextColorLo = new Color(0, 0, 0, 0.2f);
            editorStyle.overlayTextColorHi = new Color(0, 0, 0, 0.6f);
            editorStyle.selectionBoxColor = new Color(0, 0.6f, 1, 0.6f);
            editorStyle.commentTextColor = Color.black;
            editorStyle.displayAssetFilename = false;
            editorStyle.brandingSize = 30;

            return editorStyle;
        }
    }

}