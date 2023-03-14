//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Grammar;
using DungeonArchitect.Graphs;
using DungeonArchitect.UI;
using DungeonArchitect.UI.Widgets.GraphEditors;

namespace DungeonArchitect.Editors.SnapFlow
{
    public interface ISnapEdValidatorAction
    {
        void Execute(SnapEditorWindow editor);
    }

    public abstract class SnapEdValidatorActionBase : ISnapEdValidatorAction
    {
        public abstract void Execute(SnapEditorWindow editor);
    }

    public class EmptyNodeListSnapEdValidatorAction : SnapEdValidatorActionBase
    {
        public override void Execute(SnapEditorWindow editor)
        {
            var layoutRoot = editor.GetRootLayout();
            DungeonFlowEditorHighlighter.HighlightObjects(editor.uiSystem, layoutRoot, DungeonFlowEditorHighlightID.NodePanel);
        }
    }
    public class EmptyRuleListSnapEdValidatorAction : SnapEdValidatorActionBase
    {
        public override void Execute(SnapEditorWindow editor)
        {
            var layoutRoot = editor.GetRootLayout();
            DungeonFlowEditorHighlighter.HighlightObjects(editor.uiSystem, layoutRoot, DungeonFlowEditorHighlightID.RulePanel);
        }
    }

    public class InvalidExecutionGraphSnapEdValidatorAction : SnapEdValidatorActionBase
    {
        public override void Execute(SnapEditorWindow editor)
        {
            var layoutRoot = editor.GetRootLayout();
            var flowAsset = editor.FlowAsset;
            var execGraph = (flowAsset != null) ? flowAsset.executionGraph : null;
            DungeonFlowEditorHighlighter.HighlightObjects(editor.uiSystem, layoutRoot, execGraph);
        }
    }


    public class InvalidProductionRuleAction : SnapEdValidatorActionBase
    {
        GrammarProductionRule productionRule;
        public InvalidProductionRuleAction(GrammarProductionRule productionRule)
        {
            this.productionRule = productionRule;
        }
        public override void Execute(SnapEditorWindow editor)
        {
            var rulePanel = editor.GetRuleListPanel();
            rulePanel.ListView.SetSelectedItem(editor.uiSystem, productionRule, true);
            DungeonFlowEditorHighlighter.HighlightObjects(editor.uiSystem, rulePanel, productionRule);
        }
    }

    public class MissingProductionRuleRHSAction : SnapEdValidatorActionBase
    {
        GrammarProductionRule productionRule;
        public MissingProductionRuleRHSAction(GrammarProductionRule productionRule)
        {
            this.productionRule = productionRule;
        }
        public override void Execute(SnapEditorWindow editor)
        {
            var rulePanel = editor.GetRuleListPanel();
            rulePanel.ListView.SetSelectedItem(editor.uiSystem, productionRule);
            editor.ForceUpdateWidgetFromCache(rulePanel);

            DungeonFlowEditorHighlighter.HighlightObjects(editor.uiSystem, editor.GetProductionRuleWidget(), DungeonFlowEditorHighlightID.ProductionAddRHSButton);
        }
    }

    public class InvalidNodeTypeAction : SnapEdValidatorActionBase
    {
        GrammarNodeType nodeType;
        public InvalidNodeTypeAction(GrammarNodeType nodeType)
        {
            this.nodeType = nodeType;
        }
        public override void Execute(SnapEditorWindow editor)
        {
            var nodesPanel = editor.GetNodeListPanel();
            nodesPanel.ListView.SetSelectedItem(editor.uiSystem, nodeType, true);
            DungeonFlowEditorHighlighter.HighlightObjects(editor.uiSystem, nodesPanel, nodeType);
        }
    }

    public class InvalidProductionGraphAction : SnapEdValidatorActionBase
    {
        GrammarGraph graph;
        GrammarProductionRule productionRule;

        public InvalidProductionGraphAction(GrammarProductionRule productionRule, GrammarGraph graph)
        {
            this.productionRule = productionRule;
            this.graph = graph;
        }

        public override void Execute(SnapEditorWindow editor)
        {
            var rulePanel = editor.GetRuleListPanel();
            rulePanel.ListView.SetSelectedItem(editor.uiSystem, productionRule);
            editor.ForceUpdateWidgetFromCache(rulePanel);

            var productionRuleEditor = editor.GetProductionRuleWidget();
            DungeonFlowEditorHighlighter.HighlightObjects(editor.uiSystem, productionRuleEditor, graph);
        }
    }

    public class InvalidProductionGraphNodeAction : SnapEdValidatorActionBase
    {
        GraphNode node;
        GrammarProductionRule productionRule;

        public InvalidProductionGraphNodeAction(GrammarProductionRule productionRule, GraphNode node)
        {
            this.productionRule = productionRule;
            this.node = node;
        }

        GraphEditor FindGraphEditor(SnapEditorWindow editor, Graph graph)
        {
            var graphEditors = editor.GetProductionRuleWidget().GetGraphEditors();
            foreach (var graphEditor in graphEditors)
            {
                if (graphEditor.Graph == graph)
                {
                    return graphEditor;
                }
            }
            return null;
        }

        public override void Execute(SnapEditorWindow editor)
        {
            var rulePanel = editor.GetRuleListPanel();
            rulePanel.ListView.SetSelectedItem(editor.uiSystem, productionRule);
            editor.ForceUpdateWidgetFromCache(rulePanel);

            var productionRuleEditor = editor.GetProductionRuleWidget();
            DungeonFlowEditorHighlighter.HighlightObjects(editor.uiSystem, productionRuleEditor, node.Graph);

            var graphEditor = FindGraphEditor(editor, node.Graph);
            if (graphEditor != null)
            {
                editor.AddDeferredCommand(new EditorCommand_FocusOnGraphNode(graphEditor, node));
            }
        }
    }

    public class InvalidExecutionGraphNodeAction : SnapEdValidatorActionBase
    {
        GraphNode node;

        public InvalidExecutionGraphNodeAction(GraphNode node)
        {
            this.node = node;
        }

        public override void Execute(SnapEditorWindow editor)
        {
            var execGraphPanel = editor.GetExecGraphPanel();
            if (execGraphPanel != null)
            {
                DungeonFlowEditorHighlighter.HighlightObjects(editor.uiSystem, execGraphPanel, node.Graph);

                var graphEditor = execGraphPanel.GraphEditor;
                if (graphEditor != null)
                {
                    editor.AddDeferredCommand(new EditorCommand_FocusOnGraphNode(graphEditor, node));
                }
            }
        }
    }
}
