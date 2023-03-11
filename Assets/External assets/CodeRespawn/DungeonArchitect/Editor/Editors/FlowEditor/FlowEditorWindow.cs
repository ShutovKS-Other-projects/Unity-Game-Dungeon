//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Editors.Flow.DomainEditors;
using DungeonArchitect.Flow.Domains;
using DungeonArchitect.Flow.Exec;
using DungeonArchitect.Flow;
using DungeonArchitect.UI.Impl.UnityEditor;
using DungeonArchitect.UI.Widgets.GraphEditors;
using DungeonArchitect.UI.Widgets;
using DungeonArchitect.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Editors.Flow
{
    public interface IFlowEditorWindow
    {
        
    }
    public abstract class FlowEditorWindow : EditorWindow
    {
        protected UISystem uiSystem;
        protected UIRenderer renderer;
        protected List<IDeferredUICommand> deferredCommands = new List<IDeferredUICommand>();
        private FlowExecNodeOutputRegistry previewNodeOutputRegistry = null;
        private BorderWidget domainHostWidget;
        private IWidget domainEmptyWidget;
        private SpacerWidget toolbarPadding;
        private bool requestRepaint = false;

        [SerializeField]
        public FlowAssetBase flowAsset;
        
        [SerializeField]
        protected GraphPanel<FlowExecGraphEditor> execGraphPanel;
        
        [SerializeField]
        protected FlowEditorConfig editorConfig = null;
        
        [SerializeField]
        private List<FlowDomainEditor> domainEditors = new List<FlowDomainEditor>();
        
        protected abstract FlowEditorConfig CreateEditorConfig();
        protected abstract FlowExecNodeOutputRegistry GetLinkedDungeonNodeOutputRegistry();
        protected abstract bool IsDomainStateInvalid();
        protected abstract IWidget DomainLayoutWidget { get; set; }
        protected abstract void InitDomains();
        protected abstract string WindowTitle { get; }
        
        public virtual void Init(FlowAssetBase flowAsset)
        {
            CreateUISystem();
            this.flowAsset = flowAsset;
            titleContent = new GUIContent(WindowTitle);
            editorConfig = CreateEditorConfig();

            if (flowAsset == null)
            {
                uiSystem.ClearLayout();
                return;
            }
            
            domainEmptyWidget = new BorderWidget()
                .SetPadding(0, 0, 0, 0)
                .SetColor(new Color(0.15f, 0.15f, 0.15f))
                .SetContent(
                    new LabelWidget("Execute the graph and \r\nselect a node to preview")
                        .SetColor(new Color(1, 1, 1, 0.5f))
                        .SetFontSize(24)
                        .SetTextAlign(TextAnchor.MiddleCenter));
            
            DomainLayoutWidget = domainEmptyWidget;
            
            domainEditors.Clear();
            InitDomains();
            
            // Build the exec graph panel
            {
                execGraphPanel = new GraphPanel<FlowExecGraphEditor>(flowAsset.execGraph, flowAsset, uiSystem);
                execGraphPanel.GraphEditor.Events.OnNodeSelectionChanged.Event += OnExecNodeSelectionChanged;
                execGraphPanel.Border.SetTitle("Execution Graph");
                execGraphPanel.Border.SetColor(new Color(0.2f, 0.2f, 0.5f));
                
                // Add the flow domains
                var domains = new List<IFlowDomain>();
                foreach (var domainEditor in domainEditors)
                { 
                    if (domainEditor != null && domainEditor.FlowDomain != null)
                    {
                        domains.Add(domainEditor.FlowDomain);
                    }
                }

                execGraphPanel.GraphEditor.Domains = domains.ToArray();
            }

            BuildLayout();

            // Initialize the UI state
            {
                var bounds = new Rect(Vector2.zero, position.size);
                uiSystem.Update(bounds);
                
                // Reset the camera state of the exec graph editor
                execGraphPanel.GraphEditor.FocusCameraOnBestFit();
            }
        }

        void CreateUISystem()
        {
            uiSystem = new UnityEditorUISystem();
            renderer = new UnityEditorUIRenderer();
        }

        public virtual void SetReadOnly(bool readOnly)
        {
            if (execGraphPanel != null)
            {
                execGraphPanel.GraphEditor.SetReadOnly(readOnly);
            }

        }

        private void OnExecNodeSelectionChanged(object sender, GraphNodeEventArgs e)
        {
            var selectedNode = (e.Nodes.Length > 0) ? e.Nodes[0] as FlowExecRuleGraphNode : null;
            FlowExecRuleGraphNode targetNode = (selectedNode != null) ? selectedNode : flowAsset.execGraph.resultNode;
            UpdatePreview(targetNode);
            
            // Show the selected node in the inspector
            {
                var graph = flowAsset.execGraph;
                // Fetch the first selected node
                var selectedNodes = (from node in graph.Nodes
                    where node.Selected
                    select node).ToArray();

                if (selectedNodes.Length > 0)
                {
                    var node = selectedNodes[0];
                    if (node is FlowExecRuleGraphNode)
                    {
                        var ruleNode = node as FlowExecRuleGraphNode;
                        uiSystem.Platform.ShowObjectProperty(ruleNode.task);
                    }
                    else
                    {
                        uiSystem.Platform.ShowObjectProperty(node);
                    }
                }
                else
                {
                    uiSystem.Platform.ShowObjectProperty(editorConfig);
                }
            }
        }

        protected virtual IWidget CreateMainLayout(IWidget execGraphWidget, IWidget domainsWidget)
        {
            return new Splitter(SplitterDirection.Vertical)
                .AddWidget(execGraphWidget, 1)
                .AddWidget(domainsWidget, 2);
        }

        protected void BuildLayout()
        {
            domainHostWidget = new BorderWidget()
                .SetPadding(0, 0, 0, 0);

            if (DomainLayoutWidget == null)
            {
                DomainLayoutWidget = domainEmptyWidget;
            }
            domainHostWidget.SetContent(DomainLayoutWidget);

            IWidget layout = CreateMainLayout(execGraphPanel, domainHostWidget);

            toolbarPadding = new SpacerWidget(new Vector2(20, 20)); 
            layout = new StackPanelWidget(StackPanelOrientation.Vertical)
                .AddWidget(toolbarPadding, 0, true)
                .AddWidget(layout);
            
            uiSystem.SetLayout(layout);

            deferredCommands.Add(new EditorCommand_InitializeGraphCameras(layout));
        }

        void ShowEditorSettings()
        {
            uiSystem.Platform.ShowObjectProperty(editorConfig);
        }
        
        public void HandleExecuteButtonPressed()
        {
            ExecuteGraph();

            // Select the result node
            execGraphPanel.GraphEditor.SelectNode(flowAsset.execGraph.resultNode, uiSystem);

            UpdatePreview(flowAsset.execGraph.resultNode);
            
            requestRepaint = true;
        }

        protected virtual void AddDomainExtenders(FlowDomainExtensions domainExtensions)
        {
        }
        
        private void ExecuteGraph()
        {
            if (editorConfig.randomizeSeed)
            {
                editorConfig.seed = new System.Random().Next();
            }

            if (flowAsset == null)
            {
                return;
            }
            
            var execGraph = flowAsset.execGraph;
            var random = new System.Random(editorConfig.seed);

            var domainExtensions = new FlowDomainExtensions();
            AddDomainExtenders(domainExtensions);
            
            FlowExecutor executor = new FlowExecutor();
            if (!executor.Execute(execGraph, random, domainExtensions, 100, out previewNodeOutputRegistry))
            {
                Debug.LogError("Failed to produce graph");
            }

            // Build the reference scene dungeon, if specified
            if (editorConfig.FlowBuilder != null)
            {
                var dungeonGameObject = editorConfig.FlowBuilder.gameObject;
                if (dungeonGameObject != null)
                {
                    // Build the dungeon only if all the domains have a valid state
                    var allDomainsValid = true;
                    foreach (var domainEditor in domainEditors)
                    {
                        if (domainEditor != null && !domainEditor.StateValid)
                        {
                            allDomainsValid = false;
                            break;
                        }
                    }

                    if (allDomainsValid)
                    {
                        if (editorConfig.randomizeSeed)
                        {
                            var dungeonConfig = dungeonGameObject.GetComponent<DungeonConfig>();
                            if (dungeonConfig != null)
                            {
                                dungeonConfig.Seed = (uint) editorConfig.seed;
                            }
                        }

                        var dungeon = dungeonGameObject.GetComponent<Dungeon>();
                        if (dungeon != null)
                        {
                            dungeon.Build(new EditorDungeonSceneObjectInstantiator());
                            DungeonEditorHelper.MarkSceneDirty();
                        }
                    }
                }
            }
        }
       
        void UpdatePreview(FlowExecRuleGraphNode node)
        {
            if (node == null || node.task == null) return;
            if (previewNodeOutputRegistry == null) return;
            var execState = previewNodeOutputRegistry.Get(node.Id);

            var taskState = execState != null ? execState.State : null;
            UpdateDomainPreview(taskState);
            
            if (DomainLayoutWidget == null)
            {
                DomainLayoutWidget = domainEmptyWidget;
            }
            domainHostWidget.SetContent(DomainLayoutWidget);

            // Request a layout update
            var windowBounds = new Rect(Vector2.zero, position.size);
            deferredCommands.Add(new EditorCommand_UpdateWidget(uiSystem.Layout, windowBounds));
            deferredCommands.Add(new EditorCommand_InitializeGraphCameras(domainHostWidget));
        }

        protected virtual void OnEnable()
        {
            this.wantsMouseMove = true;

            Init(flowAsset);

            var graphEditors = WidgetUtils.GetWidgetsOfType<GraphEditor>(uiSystem.Layout);
            graphEditors.ForEach(g => g.OnEnable());

            DungeonPropertyEditorHook.Get().DungeonBuilt += OnLinkedDungeonBuilt;
            InspectorNotify.FlowTaskPropertyChanged += OnTaskPropertyChanged;
        }

        protected virtual void OnDisable()
        {
            var graphEditors = WidgetUtils.GetWidgetsOfType<GraphEditor>(uiSystem.Layout);
            graphEditors.ForEach(g => g.OnDisable());

            DungeonPropertyEditorHook.Get().DungeonBuilt -= OnLinkedDungeonBuilt;
            InspectorNotify.FlowTaskPropertyChanged -= OnTaskPropertyChanged;
        }

        private void OnTaskPropertyChanged(FlowExecTask task)
        {
            requestRepaint = true;
        }

        protected void RegisterDomainEditor(FlowDomainEditor domainEditor)
        {
            domainEditors.Add(domainEditor);
        }

        protected virtual void UpdateDomainPreview(FlowExecTaskState taskState)
        {
            foreach (var domainEditor in domainEditors)
            {
                if (domainEditor != null)
                {
                    domainEditor.UpdateNodePreview(taskState);
                }
            }
        }

        protected virtual void OnDestroy()
        {
            var graphEditors = WidgetUtils.GetWidgetsOfType<GraphEditor>(uiSystem.Layout);
            graphEditors.ForEach(g =>
            {
                g.OnDisable();
                g.OnDestroy();
            });

            DungeonPropertyEditorHook.Get().DungeonBuilt -= OnLinkedDungeonBuilt;
            
            foreach (var domainEditor in domainEditors)
            {
                if (domainEditor != null)
                {
                    domainEditor.Destroy();
                }
            }
        }


        protected virtual void Update()
        {
            if (uiSystem == null || renderer == null)
            {
                CreateUISystem();
            }

            if (IsEditorStateInvalid())
            {
                Init(flowAsset);
            }
            
            if (uiSystem != null)
            {
                var bounds = new Rect(Vector2.zero, position.size);
                uiSystem.Update(bounds);
                
                var graphEditors = WidgetUtils.GetWidgetsOfType<GraphEditor>(uiSystem.Layout);
                graphEditors.ForEach(g => g.Update());
            }

            foreach (var domainEditor in domainEditors)
            {
                if (domainEditor != null)
                {
                    domainEditor.Update();
                }
            }

            // check if any of the domains require a repaint
            if (!requestRepaint)
            {
                foreach (var domainEditor in domainEditors)
                {
                    if (domainEditor != null && domainEditor.RequiresRepaint())
                    {
                        requestRepaint = true;
                        break;
                    }
                }
            }

            
            if (requestRepaint)
            {
                Repaint();
                requestRepaint = false;
            }
        }

        private void OnLinkedDungeonBuilt(Dungeon dungeon)
        {
            if (dungeon == null || flowAsset == null || flowAsset.execGraph == null || flowAsset.execGraph.resultNode == null) return;
            if (execGraphPanel == null) return;

            var linkedDungeonBuilder = editorConfig.FlowBuilder;
            if (linkedDungeonBuilder == null || linkedDungeonBuilder.gameObject == null) return;
            var linkedDungeon = linkedDungeonBuilder.gameObject.GetComponent<Dungeon>();
            if (dungeon != linkedDungeon) return;

            previewNodeOutputRegistry = GetLinkedDungeonNodeOutputRegistry();

            var resultNode = flowAsset.execGraph.resultNode;
            if (resultNode != null)
            {
                // Select the result node
                execGraphPanel.GraphEditor.SelectNode(resultNode, uiSystem);
                UpdatePreview(resultNode);
                uiSystem.Platform.ShowObjectProperty(dungeon.gameObject);
                requestRepaint = true;
            }
        }

        void UpdateDragDropState(Event e)
        {
            if (uiSystem != null)
            {
                if (e.type == EventType.DragUpdated)
                {
                    uiSystem.SetDragging(true);
                }
                else if (e.type == EventType.DragPerform || e.type == EventType.DragExited)
                {
                    uiSystem.SetDragging(false);
                }
            }
        }

        void ProcessDeferredCommands()
        {
            // Execute the deferred UI commands
            foreach (var command in deferredCommands)
            {
                command.Execute(uiSystem);
            }

            deferredCommands.Clear();
        }

        void DrawToolbar()
        {
            var guiState = new GUIState(renderer);
            renderer.backgroundColor = EditorGUIUtility.isProSkin
                ? new Color(0.5f, 0.5f, 1.0f, 1.0f)
                : new Color(0.85f, 0.85f, 1.0f, 1.0f);
            var rect = EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Height(30));

            var iconBuild = renderer.GetResource<Texture2D>(UIResourceLookup.ICON_PLAY_16xb) as Texture2D;
            if (GUILayout.Button(new GUIContent("Build", iconBuild), EditorStyles.toolbarButton))
            {
                HandleExecuteButtonPressed();
            }
            GUILayout.Space(5);

            var iconSettings = renderer.GetResource<Texture2D>(UIResourceLookup.ICON_SETTINGS_16x) as Texture2D;
            if (GUILayout.Button(new GUIContent(" Settings", iconSettings), EditorStyles.toolbarButton))
            {
                ShowEditorSettings();
            }

            GUILayout.FlexibleSpace();

            {
                renderer.backgroundColor = new Color(1, 0.25f, 0.25f, 1);
                renderer.color = Color.white;
                
                var iconDiscord = renderer.GetResource<Texture2D>(UIResourceLookup.ICON_DISCORD_16x) as Texture2D;
                if (GUILayout.Button(new GUIContent(" Discord Support", iconDiscord), DungeonEditorStyles.discordToolButtonStyle))
                {
                    ExternalLinks.LaunchUrl(ExternalLinks.DiscordInvite);
                }

                renderer.backgroundColor = new Color(0.25f, 0.25f, 1, 1);
                renderer.color = Color.white;
                var iconDocs = renderer.GetResource<Texture2D>(UIResourceLookup.ICON_DOCS_16x) as Texture2D;
                if (GUILayout.Button(new GUIContent("Documentation", iconDocs), DungeonEditorStyles.discordToolButtonStyle))
                {
                    ExternalLinks.LaunchUrl(ExternalLinks.Documentation);
                }

            }
            
            EditorGUILayout.EndHorizontal();

            if (toolbarPadding != null && rect.height > 0)
            {
                toolbarPadding.SetSize(new Vector2(1, rect.height));
            }
            
            guiState.Restore();
        }

        bool IsEditorStateInvalid()
        {
            if (domainEditors == null) return true;
            foreach (var domainEditor in domainEditors) {
                if (domainEditor == null)
                {
                    return true;
                }
            }

            if (IsDomainStateInvalid())
            {
                return true;
            }

            return false;
        }
        
        void OnGUI()
        {
            if (uiSystem == null || renderer == null)
            {
                CreateUISystem();
            }

            if (uiSystem.Layout == null)
            {
                BuildLayout();
            }

            if (IsEditorStateInvalid())
            {
                // Wait for the next update cycle to fix this
                return;
            }

            var bounds = new Rect(Vector2.zero, position.size);
            renderer.DrawRect(bounds, new Color(0.5f, 0.5f, 0.5f));

            DrawToolbar();
            uiSystem.Draw(renderer);

            var e = Event.current;
            if (e != null)
            {
                if (e.isScrollWheel)
                {
                    requestRepaint = true;
                }

                switch (e.type)
                {
                    case EventType.MouseMove:
                    case EventType.MouseDrag:
                    case EventType.MouseDown:
                    case EventType.MouseUp:
                    case EventType.KeyDown:
                    case EventType.KeyUp:
                    case EventType.MouseEnterWindow:
                    case EventType.MouseLeaveWindow:
                        requestRepaint = true;
                        break;
                }
            }

            ProcessDeferredCommands();
            HandleInput(Event.current);
        }

        public void RequestRepaint()
        {
            requestRepaint = true;
        }

        void HandleInput(Event e)
        {
            if (uiSystem != null)
            {
                if (renderer == null)
                {
                    CreateUISystem();
                }

                if (uiSystem.Layout != null)
                {
                    var layout = uiSystem.Layout;
                    if (e.type == EventType.MouseDown || e.type == EventType.ScrollWheel)
                    {
                        WidgetUtils.ProcessInputFocus(e.mousePosition, uiSystem, layout);
                    }

                    if (uiSystem.IsDragDrop)
                    {
                        WidgetUtils.ProcessDragOperation(e, layout, uiSystem);
                    }

                    UpdateDragDropState(e);

                    if (uiSystem.FocusedWidget != null)
                    {
                        Vector2 resultMousePosition = Vector2.zero;
                        if (WidgetUtils.BuildWidgetEvent(e.mousePosition, layout, uiSystem.FocusedWidget, ref resultMousePosition))
                        {
                            Event widgetEvent = new Event(e);
                            widgetEvent.mousePosition = resultMousePosition;
                            uiSystem.FocusedWidget.HandleInput(widgetEvent, uiSystem);
                        }
                    }
                }
                
                foreach (var domainEditor in domainEditors)
                {
                    if (domainEditor != null)
                    {
                        domainEditor.HandleInput(uiSystem);
                    }
                }
            }
        }
    }
}