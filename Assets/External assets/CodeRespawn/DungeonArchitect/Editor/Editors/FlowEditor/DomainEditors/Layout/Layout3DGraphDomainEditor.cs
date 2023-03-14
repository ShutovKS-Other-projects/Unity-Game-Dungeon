//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Flow.Domains;
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Domains.Layout.Tooling.Graph3D;
using DungeonArchitect.Flow.Exec;
using DungeonArchitect.UI;
using DungeonArchitect.UI.Widgets;
using UnityEngine;

namespace DungeonArchitect.Editors.Flow.DomainEditors.Layout3D
{
    public interface ILayout3DGraphDomainSettings
    {
        bool AutoFocusViewport { get; }
    }
    
    public class Layout3DGraphDomainEditor : FlowDomainEditor
    {
        private FlowLayoutGraph layoutGraph;
        private FlowLayoutToolGraph3D viewport;
        private ToolbarWidget toolbar;
        static readonly string BTN_RECENTER_VIEW = "RecenterView";

        public override void Init(IFlowDomain domain, FlowEditorConfig editorConfig, UISystem uiSystem)
        {
            base.Init(domain, editorConfig, uiSystem);

            // Setup the viewport
            {
                viewport = new FlowLayoutToolGraph3D();
                viewport.SetClearState(true, true, new Color(0.90f, 0.95f, 1.0f));
                viewport.MoveSpeed = 6;
            }
            
            // Setup the floating toolbar
            {
                toolbar = new ToolbarWidget();
                toolbar.ButtonSize = 24;
                toolbar.Padding = 0;
                toolbar.Background = new Color(0, 0, 0, 0);
                toolbar.AddButton(BTN_RECENTER_VIEW, UIResourceLookup.ICON_ZOOMFIT_16x);

                toolbar.ButtonPressed += ToolbarOnButtonPressed;
            }

            Content = new OverlayPanelWidget()
                .AddWidget(viewport)
                .AddWidget(toolbar, OverlayPanelHAlign.Right, OverlayPanelVAlign.Top, new Vector2(24, 24), new Vector2(10, 10));
            
            // Build the scene
            viewport.Build(null);
            
            viewport.RecenterView();
        }

        private void ToolbarOnButtonPressed(UISystem uisystem, string id)
        {
            viewport.RecenterView();
        }

        public override IWidget Content { get; protected set; }
        public override bool StateValid { get; }
        public override void UpdateNodePreview(FlowExecTaskState taskState)
        {
            if (taskState != null)
            {
                layoutGraph = taskState.GetState<FlowLayoutGraph>();
                viewport.Build(layoutGraph);
                
                // Recenter the view
                if (EditorConfig is ILayout3DGraphDomainSettings)
                {
                    var viewportSettings = EditorConfig as ILayout3DGraphDomainSettings;
                    if (viewportSettings.AutoFocusViewport)
                    {
                        viewport.ResetFrameTimer();
                        viewport.RecenterView();
                    }
                }
            }
        }

        public override bool RequiresRepaint()
        {
            return viewport != null && viewport.IsCameraMoving(); 
        }
        
        public override void HandleInput(UISystem uiSystem)
        {
            var e = uiSystem.Platform.CurrentEvent;
            if (e == null) return;
            if (e.type == EventType.KeyDown)
            {
                // Handle Key pressed
                if (e.keyCode == KeyCode.F)
                {
                    if (viewport != null)
                    {
                        viewport.RecenterView();
                    }
                }
            }
        }
        
        public FlowLayoutGraph FlowGraph
        {
            get => layoutGraph;
            set => layoutGraph = value;
        }
    }
}
