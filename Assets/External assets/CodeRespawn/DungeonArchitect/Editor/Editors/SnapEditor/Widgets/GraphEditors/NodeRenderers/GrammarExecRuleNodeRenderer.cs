//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.UI;
using DungeonArchitect.Grammar;
using DungeonArchitect.Graphs;
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Editors.SnapFlow
{
    public class GrammarExecRuleNodeRenderer : GrammarNodeRendererBase
    {
        protected override string GetCaption(GraphNode node)
        {
            var ruleNode = node as GrammarExecRuleNode;
            var rule = (ruleNode != null) ? ruleNode.rule : null;
            return (rule != null) ? rule.ruleName : "[DELETED]";
        }

        protected override Color GetPinColor(GraphNode node)
        {
            var ruleNode = node as GrammarExecRuleNode;
            var rule = (ruleNode != null) ? ruleNode.rule : null;
            return (rule != null) ? new Color(0.3f, 0.3f, 0.5f) : Color.red;
        }

        protected override Color GetBodyColor(GraphNode node)
        {
            var ruleNode = node as GrammarExecRuleNode;
            var prodRule = (ruleNode != null) ? ruleNode.rule : null;
            return (prodRule != null) ? new Color(0.1f, 0.1f, 0.1f, 1) : Color.red;
        }

        string GetModeText(GraphNode node)
        {
            if (node is GrammarExecRuleNode)
            {
                var execNode = node as GrammarExecRuleNode;
                if (execNode.runMode == GrammarExecRuleRunMode.RunOnce)
                {
                    return "Run Once";
                }
                else if (execNode.runMode == GrammarExecRuleRunMode.RunWithProbability)
                {
                    return string.Format("Run Probability: {0}", execNode.runProbability);
                }
                else if (execNode.runMode == GrammarExecRuleRunMode.Iterate)
                {
                    return string.Format("Run {0} times", execNode.iterateCount);
                }
                else if (execNode.runMode == GrammarExecRuleRunMode.IterateRange)
                {
                    return string.Format("Run {0}-{1} times", execNode.minIterateCount, execNode.maxIterateCount);
                }

                return execNode.runMode.ToString();
            }
            return "";
        }

        GUIStyle GetModeStyle(GUIStyle style)
        {
            var modeStyle = new GUIStyle(style);
            modeStyle.fontSize = 12;
            return modeStyle;
        }

        struct ExecRuleNodeLayoutInfo
        {
            public GUIContent messageContent;
            public Vector2 messageSize;
            public GUIStyle messageStyle;

            public GUIContent modeContent;
            public Vector2 modeSize;
            public GUIStyle modeStyle;
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

            // Calculate the mode size
            {
                var modeStyle = GetModeStyle(style);
                modeStyle.font = EditorStyles.standardFont;
                modeStyle.fontSize = Mathf.RoundToInt(style.fontSize * 0.8f);
                string modeText = GetModeText(node);

                layout.modeContent = new GUIContent(modeText);
                layout.modeSize = modeStyle.CalcSize(layout.modeContent);
                layout.modeStyle = modeStyle;
            }

            return layout;
        }

        protected override Vector2 GetContentScreenSize(GraphNode node, GUIStyle style)
        {
            var layout = CalcLayoutInfo(node, style);

            var size = new Vector2(
                Mathf.Max(layout.messageSize.x, layout.modeSize.x),
                layout.messageSize.y + layout.modeSize.y);

            return size;
        }

        protected override void DrawNodeContent(UIRenderer renderer, GraphNode node, GUIStyle style, Rect bounds)
        {
            var layout = CalcLayoutInfo(node, style);
            layout.messageStyle.normal.textColor = Color.white;
            layout.modeStyle.normal.textColor = Color.white;

            var messageOffsetX = Mathf.Max(0, (bounds.width - layout.messageSize.x) * 0.5f);
            var modeOffsetX = Mathf.Max(0, (bounds.width - layout.modeSize.x) * 0.5f);

            var messageBounds = new Rect(bounds.position + new Vector2(messageOffsetX, 0), layout.messageSize);
            renderer.Label(messageBounds, layout.messageContent, layout.messageStyle);

            var modeBounds = new Rect(bounds.position + new Vector2(modeOffsetX, layout.messageSize.y), layout.modeSize);
            renderer.Label(modeBounds, layout.modeContent, layout.modeStyle);
        }

    }
}
