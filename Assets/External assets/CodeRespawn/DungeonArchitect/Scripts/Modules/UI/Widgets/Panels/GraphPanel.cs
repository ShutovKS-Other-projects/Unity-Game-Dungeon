//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.UI.Widgets.GraphEditors;
using DungeonArchitect.Graphs;
using UnityEngine;

namespace DungeonArchitect.UI.Widgets
{
    public class GraphPanel<TGraphEditor> : WidgetBase where TGraphEditor : GraphEditor
    {
        IWidget host;

        public BorderWidget Border;
        public ToolbarWidget FloatingToolbar;
        public ToolbarWidget Toolbar;
        public TGraphEditor GraphEditor;

        readonly static string BTN_FOCUS_ON_GRAPH = "FocusOnGraph";

        public GraphPanel(Graph graph, Object assetObject, UISystem uiSystem)
            : this(graph, assetObject, uiSystem, null)
        {
        }

        public GraphPanel(Graph graph, Object assetObject, UISystem uiSystem, ToolbarWidget toolbar)
        {
            GraphEditor = ScriptableObject.CreateInstance<TGraphEditor>();
            GraphEditor.Init(graph, Rect.zero, assetObject, uiSystem);
            GraphEditor.ShowFocusHighlight = true;

            FloatingToolbar = new ToolbarWidget();
            FloatingToolbar.ButtonSize = 24;
            FloatingToolbar.Padding = 0;
            FloatingToolbar.Background = new Color(0, 0, 0, 0);
            FloatingToolbar.AddButton(BTN_FOCUS_ON_GRAPH, UIResourceLookup.ICON_ZOOMFIT_16x);

            FloatingToolbar.ButtonPressed += Toolbar_ButtonPressed;

            IWidget widget = new OverlayPanelWidget()
                        .AddWidget(new HighlightWidget()
                            .SetContent(GraphEditor)
                            .SetObjectOfInterest(graph)
                        )
                        .AddWidget(FloatingToolbar, OverlayPanelHAlign.Right, OverlayPanelVAlign.Top, new Vector2(24, 24), new Vector2(10, 10));

            if (toolbar != null)
            {
                var toolbarSize = new Vector2(
                    toolbar.Padding * 2 + toolbar.ButtonSize * toolbar.buttons.Count, 
                    toolbar.Padding * 2 + toolbar.ButtonSize);

                IWidget toolWidget = new StackPanelWidget(StackPanelOrientation.Horizontal)
                                    .AddWidget(toolbar, toolbarSize.x)
                                    .AddWidget(new NullWidget())
                                    ;

                toolWidget = new BorderWidget(toolWidget)
                    .SetPadding(0, 0, 0, 0)
                    .SetDrawOutline(false)
                    .SetColor(new Color(0, 0, 0, 0.25f));

                widget = new StackPanelWidget(StackPanelOrientation.Vertical)
                        .AddWidget(toolWidget, toolbarSize.y)
                        .AddWidget(widget);
            }

            Border = new BorderWidget()
                   .SetContent(widget);

            host = Border;
        }

        private void Toolbar_ButtonPressed(UISystem uiSystem, string id)
        {
            if (uiSystem != null)
            {
                uiSystem.RequestFocus(GraphEditor);
            }

            if (id == BTN_FOCUS_ON_GRAPH)
            {
                ResetCamera();
            }
        }

        public void ResetCamera()
        {
            GraphEditor.FocusCameraOnBestFit();
        }

        public override void UpdateWidget(UISystem uiSystem, Rect bounds)
        {
            base.UpdateWidget(uiSystem, bounds);

            if (host != null)
            {
                var childBounds = new Rect(Vector2.zero, bounds.size);
                host.UpdateWidget(uiSystem, childBounds);
            }
        }

        protected override void DrawImpl(UISystem uiSystem, UIRenderer renderer)
        {
            host.Draw(uiSystem, renderer);
        }

        public override bool IsCompositeWidget()
        {
            return true;
        }

        public override IWidget[] GetChildWidgets()
        {
            return new[] { host };
        }
    }
}
