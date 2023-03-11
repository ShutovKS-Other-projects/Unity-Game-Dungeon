//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.UI.Widgets;
using DungeonArchitect.Grammar;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DungeonArchitect.UI;
using DungeonArchitect.UI.Widgets.GraphEditors;

namespace DungeonArchitect.Editors.SnapFlow
{
    public class ProductionRuleWidgetRHSState
    {
        public GraphPanel<SnapEdGrammarGraphEditor> GraphPanel;
        public WeightedGrammarGraph WeightedGraph;
        public ProductionRuleRHSTitleWidget WidgetTitle;
    }

    public class ProductionRuleWidget : WidgetBase
    {
        SnapFlowAsset flowAsset;
        GrammarProductionRule productionRule;
        GraphPanel<SnapEdGrammarGraphEditor> LHSGraphPanel;
        List<ProductionRuleWidgetRHSState> RHSEditorStates = new List<ProductionRuleWidgetRHSState>();
        Splitter splitter;
        IWidget layout;
        bool layoutDirty = false;

        public void Init(SnapFlowAsset flowAsset, GrammarProductionRule productionRule, UISystem uiSystem)
        {
            this.flowAsset = flowAsset;
            this.productionRule = productionRule;

            if (productionRule != null)
            {
                var LHSGraph = productionRule ? productionRule.LHSGraph : null;
                LHSGraphPanel = new GraphPanel<SnapEdGrammarGraphEditor>(LHSGraph, flowAsset, uiSystem);
                LHSGraphPanel.GraphEditor.SetBranding("LHS");
                LHSGraphPanel.Border.SetTitleWidget(new ProductionRuleLHSTitleWidget());
                LHSGraphPanel.Border.SetColor(new Color(0.3f, 0.3f, 0.3f));

                RHSEditorStates = new List<ProductionRuleWidgetRHSState>();
                foreach (var RHSGraph in productionRule.RHSGraphs)
                {
                    AddRHSState(RHSGraph, uiSystem);
                }
            }

            BuildLayout();
        }

        public GraphEditor[] GetGraphEditors()
        {
            var graphEditors = new List<GraphEditor>();
            graphEditors.Add(LHSGraphPanel.GraphEditor);
            foreach (var rhsState in RHSEditorStates)
            {
                graphEditors.Add(rhsState.GraphPanel.GraphEditor);
            }
            return graphEditors.ToArray();
        }

        void BuildLayout()
        {
            float GRAPH_HEIGHT = 300;

            splitter = new Splitter(SplitterDirection.Vertical);
            splitter.SetFreeSize(true);
            splitter.SplitBarDragged += Splitter_SplitBarDragged;

            float panelHeight = GRAPH_HEIGHT;
            GetGraphPanelEditorSize(LHSGraphPanel, ref panelHeight);

            splitter.AddWidget(
                    new BorderWidget(LHSGraphPanel)
                    .SetPadding(0, 0, 0, 0)
                    .SetBorderColor(new Color(0, 0, 0, 0))
                    .SetColor(new Color(0, 0, 0, 0)),
                panelHeight);

            foreach (var rhsEditorState in RHSEditorStates)
            {
                panelHeight = GRAPH_HEIGHT;
                GetGraphPanelEditorSize(rhsEditorState.GraphPanel, ref panelHeight);
                splitter.AddWidget(
                    new BorderWidget(rhsEditorState.GraphPanel)
                    .SetPadding(0, 0, 0, 0)
                    .SetBorderColor(new Color(0, 0, 0, 0))
                    .SetColor(new Color(0, 0, 0, 0)),
                panelHeight);
            }

            var addRhsButton = new ButtonWidget(new GUIContent("Add RHS"))
                    .SetColor(new Color(0.8f, 0.8f, 0.8f));
            addRhsButton.ButtonPressed += AddRhsButton_ButtonPressed;

            splitter.AddWidget(new HighlightWidget()
                .SetContent(addRhsButton)
                .SetObjectOfInterest(DungeonFlowEditorHighlightID.ProductionAddRHSButton)
            , 30);

            var splitterHost = new ScrollPanelWidget(splitter, false);

            var layoutContent = new BorderWidget(splitterHost)
                .SetPadding(0, 0, 0, 0)
                .SetDrawOutline(false)
                .SetColor(new Color(0.2f, 0.2f, 0.2f));

            layout = new BorderWidget(layoutContent)
                .SetTitleGetter(GetProductionTitle)
                .SetColor(new Color(0.2f, 0.1f, 0.3f, 1))
                .SetDrawOutline(true)
                ;

        }

        GrammarGraph GetWidgetGraph(IWidget widget)
        {
            var graphPanels = WidgetUtils.GetWidgetsOfType<GraphPanel<SnapEdGrammarGraphEditor>>(widget);
            if (graphPanels.Count > 0)
            {
                var graphPanel = graphPanels[0];
                if (graphPanel != null && graphPanel.GraphEditor != null)
                {
                    var graph = graphPanel.GraphEditor.Graph as GrammarGraph;
                    return graph;
                }
            }
            return null;
        }

        void SetGraphPanelEditorSize(IWidget widget, float size)
        {
            var graph = GetWidgetGraph(widget);
            if (graph != null && graph.editorData != null)
            {
                graph.editorData.Set("panelSize", size);
                EditorUtility.SetDirty(graph);
            }
        }

        bool GetGraphPanelEditorSize(IWidget widget, ref float size)
        {
            var graph = GetWidgetGraph(widget);
            if (graph != null && graph.editorData != null)
            {
                return graph.editorData.GetFloat("panelSize", ref size);
            }
            return false;
        }

        private void Splitter_SplitBarDragged(SplitterNode prev, SplitterNode next)
        {
            if (prev != null)
            {
                SetGraphPanelEditorSize(prev.Content, prev.Weight);
            }
            if (next != null)
            {
                SetGraphPanelEditorSize(next.Content, next.Weight);
            }

        }

        string GetProductionTitle()
        {
            if (productionRule != null)
            {
                return "Production Rule: " + productionRule.ruleName;
            }
            else
            {
                return "Production Rule";
            }
        }

        private void AddRhsButton_ButtonPressed(UISystem uiSystem)
        {
            AddNewRHSState(uiSystem);
            layoutDirty = true;
        }

        ProductionRuleWidgetRHSState AddNewRHSState(UISystem uiSystem)
        {
            WeightedGrammarGraph RHSGraph = SnapEditorUtils.AddProductionRuleRHS(flowAsset, productionRule);
            return AddRHSState(RHSGraph, uiSystem);
        }

        ProductionRuleWidgetRHSState AddRHSState(WeightedGrammarGraph RHSGraph, UISystem uiSystem)
        {
            if (RHSGraph == null)
            {
                return null;
            }
            var state = new ProductionRuleWidgetRHSState();

            var rhsTitle = new ProductionRuleRHSTitleWidget();
            rhsTitle.State = state;
            rhsTitle.DeletePressed += RhsTitle_DeletePressed;

            var RHSGraphPanel = new GraphPanel<SnapEdGrammarGraphEditor>(RHSGraph.graph, flowAsset, uiSystem);
            RHSGraphPanel.GraphEditor.SetBranding("RHS");
            RHSGraphPanel.Border.SetTitleWidget(rhsTitle);
            RHSGraphPanel.Border.SetColor(new Color(0.3f, 0.3f, 0.3f));

            state.GraphPanel = RHSGraphPanel;
            state.WeightedGraph = RHSGraph;
            state.WidgetTitle = rhsTitle;

            RHSEditorStates.Add(state);

            return state;
        }

        private void RhsTitle_DeletePressed(ProductionRuleWidgetRHSState state)
        {
            string message = "Are you sure you want to delete this RHS graph?";
            bool removeItem = EditorUtility.DisplayDialog("Delete RHS Graph?", message, "Delete", "Cancel");
            if (removeItem)
            {
                RemoveRHSState(state);
                layoutDirty = true;
            }
        }

        void RemoveRHSState(ProductionRuleWidgetRHSState state)
        {
            RHSEditorStates.Remove(state);
            state.GraphPanel.GraphEditor.OnDestroy();

            SnapEditorUtils.RemoveProductionRuleRHS(productionRule, state.WeightedGraph);
        }

        public override void UpdateWidget(UISystem uiSystem, Rect bounds)
        {
            base.UpdateWidget(uiSystem, bounds);

            if (productionRule == null)
            {
                return;
            }

            if (RHSEditorStates == null)
            {
                RHSEditorStates = new List<ProductionRuleWidgetRHSState>();
            }

            if (layoutDirty)
            {
                BuildLayout();
                layoutDirty = false;
            }

            layout.UpdateWidget(uiSystem, bounds);
        }

        protected override void DrawImpl(UISystem uiSystem, UIRenderer renderer)
        {
            if (flowAsset == null || productionRule == null)
            {
                return;
            }

            layout.Draw(uiSystem, renderer);
        }

        public override bool IsCompositeWidget()
        {
            return true;
        }

        public override IWidget[] GetChildWidgets()
        {
            return new[] { layout };
        }
    }
}
