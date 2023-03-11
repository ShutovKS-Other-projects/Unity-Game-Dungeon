//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Linq;
using DungeonArchitect.Editors.Graphs;
using DungeonArchitect.Grammar;
using DungeonArchitect.Graphs;
using DungeonArchitect.Graphs.Layouts;
using DungeonArchitect.Graphs.Layouts.Spring;
using DungeonArchitect.UI;
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Editors.SnapFlow
{
    [CustomEditor(typeof(GrammarGraph), true)]
    public class GrammarGraphInspector : Editor
    {
        SerializedObject sobject;
        SerializedProperty useProceduralScript;

        protected virtual void OnEnable()
        {
            sobject = new SerializedObject(target);
            useProceduralScript = sobject.FindProperty("useProceduralScript");
        }

        ScriptableObject generatorInstanceCache = null;
        MonoScript generatorMonoScriptCache = null;

        void UpdateGeneratorInstance(string scriptClassName)
        {
            if (scriptClassName == null || scriptClassName.Length == 0)
            {
                generatorInstanceCache = null;
                generatorMonoScriptCache = null;
                return;
            }

            var type = System.Type.GetType(scriptClassName);
            if (type == null)
            {
                generatorInstanceCache = null;
                generatorMonoScriptCache = null;
                return;
            }

            if (generatorInstanceCache == null || generatorInstanceCache.GetType() != type)
            {
                generatorInstanceCache = CreateInstance(type);
                generatorMonoScriptCache = MonoScript.FromScriptableObject(generatorInstanceCache);
            }
        }

        public override void OnInspectorGUI()
        {
            sobject.Update();

            var grammar = target as GrammarGraph;
            GUILayout.Label("Grammar Properties", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Use a script to generate your graph", MessageType.Info);
            EditorGUILayout.PropertyField(useProceduralScript);
            if (grammar.useProceduralScript)
            {
                UpdateGeneratorInstance(grammar.generatorScriptClass);
                var newScript = EditorGUILayout.ObjectField("Script", generatorMonoScriptCache, typeof(MonoScript), false) as MonoScript;
                if (newScript != generatorMonoScriptCache)
                {
                    if (newScript == null)
                    {
                        grammar.generatorScriptClass = null;
                    }
                    else
                    {
                        if (!newScript.GetClass().GetInterfaces().Contains(typeof(IGrammarGraphBuildScript)))
                        {
                            // The script doesn't implement the interface
                            grammar.generatorScriptClass = null;
                        }
                        else
                        {
                            grammar.generatorScriptClass = newScript.GetClass().AssemblyQualifiedName;
                        }
                    }
                }

                EditorGUILayout.HelpBox("Warning: Clicking build will replace your existing graph!", MessageType.Warning);
                var generatorScript = generatorInstanceCache as IGrammarGraphBuildScript;
                if (generatorScript == null)
                {
                    GUI.enabled = false;
                }
                if (GUILayout.Button("Build"))
                {
                    // Find the active editor window and grab the flow asset that is being modified 
                    var editorWindow = EditorWindow.GetWindow<SnapEditorWindow>();
                    if (editorWindow != null && editorWindow.FlowAsset != null)
                    {
                        var flowAsset = editorWindow.FlowAsset;
                        var productionRule = GetProductionRule(flowAsset, grammar);
                        if (productionRule != null && generatorScript != null)
                        {
                            ExecuteGeneratorScript(generatorScript, grammar, flowAsset, editorWindow.uiSystem.Platform, editorWindow.uiSystem.Undo);
                            PerformLayout(grammar);
                            FocusEditorOnGrammar(editorWindow, grammar, productionRule);
                        }
                    }
                }
                GUI.enabled = true;
            }

            InspectorNotify.Dispatch(sobject, target);
        }

        void PerformLayout(GrammarGraph graph)
        {
            var config = new GraphLayoutSpringConfig();
            var layout = new GraphLayoutSpring<GraphNode>(config);
            var nodes = graph.Nodes.ToArray();
            nodes = nodes.Where(n => !(n is CommentNode)).ToArray();
            layout.Layout(nodes, new DefaultGraphLayoutNodeActions(graph));
        }

        void ExecuteGeneratorScript(IGrammarGraphBuildScript generatorScript, GrammarGraph grammar, SnapFlowAsset flowAsset, UIPlatform platform, UIUndoSystem undo)
        {
            var graphBuilder = new EditorGraphBuilder(grammar, flowAsset, platform, undo);
            GrammarGraphBuilder grammarBuilder = new GrammarGraphBuilder(grammar, flowAsset.nodeTypes, graphBuilder);
            grammarBuilder.ClearGraph();
            generatorScript.Generate(grammarBuilder);
        }

        // Focus on the graph editor panel
        void FocusEditorOnGrammar(SnapEditorWindow editorWindow, GrammarGraph grammar, GrammarProductionRule productionRule)
        {
            var rulePanel = editorWindow.GetRuleListPanel();
            rulePanel.ListView.SetSelectedItem(editorWindow.uiSystem, productionRule);
            editorWindow.ForceUpdateWidgetFromCache(rulePanel);

            var productionRuleEditor = editorWindow.GetProductionRuleWidget();
            DungeonFlowEditorHighlighter.HighlightObjects(editorWindow.uiSystem, productionRuleEditor, grammar);

            // Center the camera
            foreach (var graphEditor in productionRuleEditor.GetGraphEditors())
            {
                if (graphEditor.Graph == grammar)
                {
                    editorWindow.AddDeferredCommand(new EditorCommand_InitializeGraphCameras(graphEditor));
                }
            }
        }

        GrammarProductionRule GetProductionRule(SnapFlowAsset flowAsset, GrammarGraph grammar)
        {
            // make sure this object belongs to the asset
            if (flowAsset != null)
            {
                foreach (var rule in flowAsset.productionRules)
                {
                    if (grammar == rule.LHSGraph)
                    {
                        return rule;
                    }

                    foreach (var rhs in rule.RHSGraphs)
                    {
                        if (rhs.graph == grammar)
                        {
                            return rule;
                        }
                    }
                }
            }
            return null;
        }

    }

}