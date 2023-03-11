//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect.UI.Widgets;
using DungeonArchitect.Graphs;
using DungeonArchitect.UI.Widgets.GraphEditors;

namespace DungeonArchitect.UI
{
    public interface IDeferredUICommand
    {
        void Execute(UISystem uiSystem);
    }

    public abstract class DeferredUICommandBase : IDeferredUICommand
    {
        public abstract void Execute(UISystem uiSystem);
    }

    public class EditorCommand_InitializeGraphCameras : DeferredUICommandBase
    {
        IWidget host;
        public EditorCommand_InitializeGraphCameras(IWidget host)
        {
            this.host = host;
        }

        public override void Execute(UISystem uiSystem)
        {
            var graphEditors = WidgetUtils.GetWidgetsOfType<GraphEditor>(host);
            foreach (var graphEditor in graphEditors)
            {
                var bounds = new Rect(Vector2.zero, graphEditor.WidgetBounds.size);
                graphEditor.FocusCameraOnBestFit(bounds);
            }
        }
    }

    public class EditorCommand_FocusOnGraphNode : DeferredUICommandBase
    {
        GraphEditor graphEditor;
        GraphNode graphNode;
        public EditorCommand_FocusOnGraphNode(GraphEditor graphEditor, GraphNode graphNode)
        {
            this.graphEditor = graphEditor;
            this.graphNode = graphNode;
        }

        public override void Execute(UISystem uiSystem)
        {
            graphEditor.FocusCameraOnNode(graphNode);
            graphEditor.SelectNode(graphNode, uiSystem);
            uiSystem.Platform.ShowObjectProperty(graphNode);
        }
    }

    public class EditorCommand_SetBorderContent : DeferredUICommandBase
    {
        BorderWidget border;
        IWidget content;
        public EditorCommand_SetBorderContent(BorderWidget border, IWidget content)
        {
            this.border = border;
            this.content = content;
        }

        public override void Execute(UISystem uiSystem)
        {
            border.SetContent(content);
        }
    }

    public class EditorCommand_UpdateWidget : DeferredUICommandBase
    {
        IWidget content;
        Rect bounds;
        public EditorCommand_UpdateWidget(IWidget content, Rect bounds)
        {
            this.content = content;
            this.bounds = bounds;
        }

        public override void Execute(UISystem uiSystem)
        {
            content.UpdateWidget(uiSystem, bounds);
        }
    }
}
