//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Graphs;
using UnityEngine;
using UnityEditor;
using DMathUtils = DungeonArchitect.Utils.MathUtils;
using DungeonArchitect.Flow.Exec;
using DungeonArchitect.UI;
using DungeonArchitect.UI.Widgets.GraphEditors;

namespace DungeonArchitect.Editors.Flow
{
    public abstract class FlowExecNodeRendererBase : GraphNodeRenderer
    {
        protected abstract string GetCaption(GraphNode node);
        protected abstract Color GetPinColor(GraphNode node);
        protected virtual Color GetBodyColor(GraphNode node)
        {
            return new Color(0.1f, 0.1f, 0.1f, 1);
        }

        protected FlowExecTask GetHandler(GraphNode node)
        {
            var execNode = node as FlowExecRuleGraphNode;
            return (execNode != null) ? execNode.task : null;
        }

        protected GridFlowGraphNodeExecutionStatus GetNodeExecutionState(GraphNode node)
        {
            var execNode = node as FlowExecRuleGraphNode;
            return (execNode != null) ? execNode.executionStatus : null;
        }

        public override void Draw(UIRenderer renderer, GraphRendererContext rendererContext, GraphNode node, GraphCamera camera)
        {
            var pinColor = GetPinColor(node);

            // Update the node bounds
            NodeBoundLayoutData layoutData;
            {
                var style = GetNodeStyle(node, camera.ZoomLevel);
                var contentScreenSize = GetContentScreenSize(node, style);
                UpdateNodeBounds(node, camera.ZoomLevel, contentScreenSize, style, out layoutData);
            }

            // Draw the pins
            DrawPins(renderer, rendererContext, node, camera, pinColor);

            // Draw the node body
            DrawNodeBody(renderer, layoutData, node, camera);
        }

        void DrawPins(UIRenderer renderer, GraphRendererContext rendererContext, GraphNode node, GraphCamera camera, Color nodeColor)
        {
            var outputPin = node.OutputPin;
            var nodeSize = node.Bounds.size;
            var offset = nodeSize / 2.0f;
            outputPin.Position = offset;
            outputPin.BoundsOffset = new Rect(-offset, nodeSize);

            var inputPin = node.InputPin;
            inputPin.Position = offset;
            inputPin.BoundsOffset = outputPin.BoundsOffset;

            DrawPin(renderer, rendererContext, outputPin, camera, nodeColor);
        }

        class GridFlowExecEditorConstants
        {
            public static readonly Color PIN_COLOR_HOVER = new Color(1, 0.6f, 0.0f);
            public static readonly Color PIN_COLOR_CLICK = new Color(1, 0.9f, 0.0f);
            public static readonly Color FOCUS_HIGHLITE_COLOR = new Color(1, 0.5f, 0, 1);
            public static readonly Vector2 BASE_PADDING = new Vector2(16, 16);
        }

        static Color GetPinColor(GraphPin pin, Color nodeColor)
        {
            Color color;
            if (pin.ClickState == GraphPinMouseState.Clicked)
            {
                color = GridFlowExecEditorConstants.PIN_COLOR_CLICK;
            }
            else if (pin.ClickState == GraphPinMouseState.Hover)
            {
                color = GridFlowExecEditorConstants.PIN_COLOR_HOVER;
            }
            else
            {
                color = nodeColor;
            }
            return color;
        }

        void DrawPin(UIRenderer renderer, GraphRendererContext rendererContext, GraphPin pin, GraphCamera camera, Color nodeColor)
        {
            if (pin.Node == null)
            {
                Debug.Log("pin node null. skipping");
                return;

            }
            var guiState = new GUIState(renderer);

            var pinBounds = new Rect(pin.GetBounds());
            var positionWorld = pin.Node.Position + pinBounds.position;
            var positionScreen = camera.WorldToScreen(positionWorld);
            pinBounds.position = positionScreen;
            pinBounds.size /= camera.ZoomLevel;

            if (pin.Node != null && pin.Node.Selected)
            {
                var focusboundsScreen = DMathUtils.ExpandRect(pinBounds, 2);
                renderer.DrawRect(focusboundsScreen, GridFlowExecEditorConstants.FOCUS_HIGHLITE_COLOR);
            }

            renderer.DrawRect(pinBounds, GetPinColor(pin, nodeColor));

            guiState.Restore();

        }

        public void UpdateNodeBounds(GraphNode node, float zoomLevel)
        {
            var style = GetNodeStyle(node, zoomLevel);
            var contentScreenSize = GetContentScreenSize(node, style);
            NodeBoundLayoutData layoutData;
            UpdateNodeBounds(node, zoomLevel, contentScreenSize, style, out layoutData);
        }

        struct NodeBoundLayoutData
        {
            public Vector2 screenTextSize;
            public Vector2 screenNodeSize;
        }
        static void UpdateNodeBounds(GraphNode node, float zoomLevel, Vector2 contentScreenSize, GUIStyle style, out NodeBoundLayoutData outLayoutData)
        {
            var screenPadding = GridFlowExecEditorConstants.BASE_PADDING / zoomLevel;
            var screenTextSize = contentScreenSize;
            var screenNodeSize = screenTextSize + screenPadding * 2;
            var updateWorldSize = screenNodeSize * zoomLevel;
            var nodeBounds = node.Bounds;
            nodeBounds.size = updateWorldSize;
            node.Bounds = nodeBounds;

            outLayoutData = new NodeBoundLayoutData();
            outLayoutData.screenTextSize = screenTextSize;
            outLayoutData.screenNodeSize = screenNodeSize;
        }

        public static GUIStyle GetNodeStyle(GraphNode node, float zoomLevel)
        {
            var style = new GUIStyle(EditorStyles.boldLabel);
            style.alignment = TextAnchor.UpperLeft;

            style.normal.textColor = node.Selected ? GraphEditorConstants.TEXT_COLOR_SELECTED : GraphEditorConstants.TEXT_COLOR;

            {
                style.font = EditorStyles.boldFont;
                float scaledFontSize = style.fontSize == 0 ? style.font.fontSize : style.fontSize;
                scaledFontSize = Mathf.Max(1.0f, scaledFontSize / zoomLevel);
                style.fontSize = Mathf.RoundToInt(scaledFontSize);
            }
            return style;
        }

        protected virtual Vector2 GetContentScreenSize(GraphNode node, GUIStyle style)
        {
            var layout = CalcLayoutInfo(node, style);

            var size = layout.messageSize;
            size.x = Mathf.Max(size.x, layout.descSize.x);
            size.x = Mathf.Max(size.x, layout.errorSize.x);

            if (layout.drawDescription)
            {
                size.y += layout.descSize.y;
            }

            if (layout.drawErrorMessage)
            {
                size.y += layout.errorSize.y;
            }

            return size;
        }

        void DrawNodeExecStatus(UIRenderer renderer, GraphNode node, GraphCamera camera, Rect boxBounds, ExecRuleNodeLayoutInfo layout)
        {
            // Draw the execution status bar
            var execState = GetNodeExecutionState(node);
            if (execState != null && execState.ExecutionStage != GridFlowGraphNodeExecutionStage.NotExecuted)
            {
                bool drawErrorBar = false;
                Color barColor = Color.grey;
                if (execState.ExecutionStage == GridFlowGraphNodeExecutionStage.WaitingToExecute)
                {
                    barColor = new Color(1, 0.5f, 0);
                }
                else if (execState.ExecutionStage == GridFlowGraphNodeExecutionStage.Executed)
                {
                    if (execState.Success == FlowTaskExecutionResult.Success)
                    {
                        barColor = Color.green;
                    }
                    else
                    {
                        barColor = Color.red;
                        drawErrorBar = true;
                    }
                }

                if (drawErrorBar && layout.drawErrorMessage)
                {
                    var execHighlightBounds = boxBounds;
                    execHighlightBounds.y += boxBounds.height - layout.errorSize.y;
                    execHighlightBounds.height = layout.errorSize.y;
                    renderer.DrawRect(execHighlightBounds, barColor);
                    renderer.Box(execHighlightBounds, layout.errorContent, layout.errorStyle);
                }
                else
                {
                    var barHeight = 4.0f / camera.ZoomLevel;
                    var execHighlightBounds = boxBounds;
                    execHighlightBounds.y += execHighlightBounds.height - barHeight;
                    execHighlightBounds.height = Mathf.Max(barHeight, 2);
                    renderer.DrawRect(execHighlightBounds, barColor);
                }
            }
        }

        protected struct ExecRuleNodeLayoutInfo
        {
            public GUIContent messageContent;
            public Vector2 messageSize;
            public GUIStyle messageStyle;

            public bool drawDescription;
            public GUIContent descContent;
            public Vector2 descSize;
            public GUIStyle descStyle;

            public bool drawErrorMessage;
            public GUIContent errorContent;
            public Vector2 errorSize;
            public GUIStyle errorStyle;

        }

        protected virtual string GetDescText(GraphNode node) { return ""; }

        GUIStyle GetDescStyle(GUIStyle style)
        {
            var descStyle = new GUIStyle(style);
            descStyle.fontSize = 12;
            return descStyle;
        }

        GUIStyle GetErrorStyle(GUIStyle style)
        {
            var errorStyle = new GUIStyle(style);
            errorStyle.fontSize = 12;
            errorStyle.alignment = TextAnchor.MiddleCenter;
            return errorStyle;
        }

        ExecRuleNodeLayoutInfo CalcLayoutInfo(GraphNode node, GUIStyle style)
        {
            var layout = new ExecRuleNodeLayoutInfo();

            // Calculate the message size
            {
                string nodeMessage = GetCaption(node);

                layout.messageContent = new GUIContent(nodeMessage);
                layout.messageSize = style.CalcSize(layout.messageContent);
                layout.messageStyle = style;
            }

            // Calculate the desc size
            {
                string description = GetDescText(node);
                layout.drawDescription = description.Length > 0;
                var descStyle = GetDescStyle(style);
                descStyle.font = EditorStyles.standardFont;
                descStyle.fontSize = Mathf.RoundToInt(style.fontSize * 0.8f);

                layout.descContent = new GUIContent(description);
                layout.descSize = description.Length > 0
                    ? descStyle.CalcSize(layout.descContent)
                    : Vector2.zero;
                layout.descStyle = descStyle;
            }

            // Calculate the error size
            {
                layout.drawErrorMessage = false;

                var ruleNode = node as FlowExecRuleGraphNode;
                string errorMessage;

                if (ruleNode != null && ruleNode.executionStatus != null)
                {
                    layout.drawErrorMessage =
                        (ruleNode.executionStatus.Success != FlowTaskExecutionResult.Success)
                        && (ruleNode.executionStatus.ExecutionStage == GridFlowGraphNodeExecutionStage.Executed)
                        && (ruleNode.executionStatus.ErrorMessage.Length > 0);
                    errorMessage = ruleNode.executionStatus.ErrorMessage;
                }
                else
                {
                    layout.drawErrorMessage = false;
                    errorMessage = "";
                }

                var errorStyle = GetErrorStyle(style);
                errorStyle.font = EditorStyles.standardFont;
                errorStyle.fontSize = Mathf.RoundToInt(style.fontSize * 0.8f);

                layout.errorContent = new GUIContent(errorMessage);
                layout.errorSize = layout.drawErrorMessage
                    ? errorStyle.CalcSize(layout.errorContent)
                    : Vector2.zero;
                layout.errorStyle = errorStyle;
            }
            return layout;
        }

        void DrawNodeBody(UIRenderer renderer, NodeBoundLayoutData layoutData, GraphNode node, GraphCamera camera)
        {
            var style = GetNodeStyle(node, camera.ZoomLevel);

            var guiState = new GUIState(renderer);
            Color nodeColor = GetBodyColor(node);

            var outputPin = node.OutputPin as FlowExecGraphNodePin;
            var pinPadding = new Vector2(12, 12);
            var screenPinPadding = pinPadding / camera.ZoomLevel;
            if (outputPin != null)
            {
                outputPin.Padding = pinPadding;
            }

            // Draw the node pin
            Rect boxBounds;
            {
                var positionScreen = camera.WorldToScreen(node.Position + pinPadding);
                var sizeScreen = layoutData.screenNodeSize - screenPinPadding * 2;
                boxBounds = new Rect(positionScreen, sizeScreen);
            }
            renderer.DrawRect(boxBounds, nodeColor);


            var layout = CalcLayoutInfo(node, style);

            // Draw the node content
            {
                Rect contentBounds;
                {
                    var positionScreen = camera.WorldToScreen(node.Position + GridFlowExecEditorConstants.BASE_PADDING);
                    var sizeScreen = layoutData.screenTextSize;
                    contentBounds = new Rect(positionScreen, sizeScreen);
                }

                layout.messageStyle.normal.textColor = Color.white;
                layout.descStyle.normal.textColor = Color.white;

                var messageOffsetX = Mathf.Max(0, (contentBounds.width - layout.messageSize.x) * 0.5f);

                var messageBounds = new Rect(contentBounds.position + new Vector2(messageOffsetX, 0), layout.messageSize);
                renderer.Label(messageBounds, layout.messageContent, layout.messageStyle);
                if (layout.drawDescription)
                {
                    var modeOffsetX = Mathf.Max(0, (contentBounds.width - layout.descSize.x) * 0.5f);
                    var descBounds = new Rect(contentBounds.position + new Vector2(modeOffsetX, layout.messageSize.y), layout.descSize);

                    renderer.Label(descBounds, layout.descContent, layout.descStyle);
                }
            }

            DrawNodeExecStatus(renderer, node, camera, boxBounds, layout);

            guiState.Restore();
        }
    }
}
