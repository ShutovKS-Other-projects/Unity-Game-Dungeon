//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using DungeonArchitect.Graphs;
using DungeonArchitect.Graphs.SpatialConstraints;
using DungeonArchitect.SpatialConstraints;

namespace DungeonArchitect.Editors.SpatialConstraints
{

    [CustomEditor(typeof(SCRuleNode))]
    public class SCRuleNodeEditor : GraphNodeEditor
    {
        RuleNodeEditorUIState uiState;
        SerializedProperty constraintEvaluationMode;
        SerializedProperty exclusionRuleSearchRadius;
        SerializedProperty exclusionRuleMarkersToRemove;
        int activeTab = 0;

        public override void OnEnable()
        {
            base.OnEnable();

            uiState = new RuleNodeEditorUIState();
            constraintEvaluationMode = sobject.FindProperty("constraintEvaluationMode");
            exclusionRuleSearchRadius = sobject.FindProperty("exclusionRuleSearchRadius");
            exclusionRuleMarkersToRemove = sobject.FindProperty("exclusionRuleMarkersToRemove");
        }

        protected override void HandleInspectorGUI()
        {
            var ruleNode = target as SCRuleNode;
            var domainName = ruleNode.RuleDomain.ToString();
            GUILayout.Label(domainName + " Rule Node", EditorStyles.boldLabel);

            string[] tabs = new string[]
            {
                "Constraint Rules",
                "Removal Rules"
            };
            activeTab = GUILayout.Toolbar(activeTab, tabs);
            EditorGUILayout.Space();

            if (activeTab == 0)
            {
                ShowTabConstraintRules();
            }
            else if (activeTab == 1)
            {
                ShowTabRemovalRules();
            }
        }

        protected override void OnGuiChanged()
        {
            var editorWindow = DungeonEditorHelper.GetWindowIfOpen<SpatialConstraintsEditorWindow>();
            if (editorWindow != null)
            {
                var graphEditor = editorWindow.GraphEditor;
                graphEditor.HandleGraphStateChanged(editorWindow.uiSystem);
                graphEditor.HandleNodePropertyChanged(target as GraphNode);
            }
        }

        void ShowTabRemovalRules()
        {
            string helpMessage = "If the spatial constraint graph succeeds, you can optionally remove markers from the scene to accommodate your new setup.  Here is where you specify the markers to be removed, which will be searched for relative to this rule node";
            EditorGUILayout.HelpBox(helpMessage, MessageType.Info);
            
            EditorGUILayout.PropertyField(exclusionRuleMarkersToRemove, new GUIContent("Markers to Remove"), true);
            EditorGUILayout.PropertyField(exclusionRuleSearchRadius, new GUIContent("Search Radius"));
        }

        void ShowTabConstraintRules()
        {
            EmitAddConstraintWidget();

            var ruleNode = target as SCRuleNode;
            EmitEvaluationModeWidget(ruleNode);

            var constraints = ruleNode.constraints.Where(c => c != null);
            foreach (var constraint in constraints)
            {
                if (!uiState.constraintStates.ContainsKey(constraint))
                {
                    var state = new RuleNodeEditorUIState.ConstraintUIState();
                    state.foldout = true;
                    state.enabled = constraint.enabled;
                    uiState.constraintStates.Add(constraint, state);
                }

                var constraintUI = uiState.constraintStates[constraint];
                EmitConstraintScript(constraint, constraintUI);
            }

            if (constraints.Count() == 0)
            {
                EditorGUILayout.HelpBox("Constraint rules allow you to enforce various conditions on the rule node", MessageType.Info);
            }
        }

        // Returns the theme graph asset by moving up the chain from the constraint rule node
        Object GetAssetObject()
        {
            var node = target as SCRuleNode;
            var scGraph = node.Graph as SpatialConstraintGraph;
            if (scGraph == null) return null;
            var scAsset = scGraph.asset;
            if (scAsset.hostThemeNode == null) return null;
            return scAsset.hostThemeNode.Graph;
        }

        void EmitEvaluationModeWidget(SCRuleNode node)
        {
            if (node.constraints.Length <= 1)
            {
                // We have too few rules. No need to show this
                return;
            }

            EditorGUILayout.PropertyField(constraintEvaluationMode);
        }

        void AddConstraintRule(System.Type constraintType)
        {
            var constraint = CreateInstance(constraintType) as ConstraintRule;
            constraint.ruleName = ConstraintRule.GetScriptName(constraintType);

            var ruleNode = target as SCRuleNode;
            var constraintList = new List<ConstraintRule>(ruleNode.constraints);
            constraintList.Add(constraint);
            ruleNode.constraints = constraintList.ToArray();

            // Add this constraint to the theme graph's asset file so it is serialized and not garbage collected
            AssetDatabase.AddObjectToAsset(constraint, GetAssetObject());
        }

        Object GetOwningAsset()
        {
            var ruleNode = target as SCRuleNode;
            var graph = ruleNode.Graph as SpatialConstraintGraph;
            if (graph == null) return null;
            if (graph.asset == null) return null;
            if (graph.asset.hostThemeNode == null) return null;
            return graph.asset.hostThemeNode.Graph;
        }

        void DestroyConstraintRule(ConstraintRule rule)
        {
            var ruleNode = target as SCRuleNode;
            var constraints = new List<ConstraintRule>(ruleNode.constraints);

            Undo.RegisterCompleteObjectUndo(GetOwningAsset(), "Delete Node Spatial Constraint");
            Undo.RecordObject(ruleNode, "Delete Constraint");

            constraints.Remove(rule);
            Undo.DestroyObjectImmediate(rule);

            ruleNode.constraints = constraints.ToArray();
        }

        void EmitConstraintScript(ConstraintRule constraint, RuleNodeEditorUIState.ConstraintUIState constraintUI)
        {
            EditorGUILayout.BeginHorizontal();

            // Draw the enabled toggle box
            constraintUI.enabled = EditorGUILayout.Toggle(constraintUI.enabled, GUILayout.Width(26));

            // Draw the fold out rule text (in bold)
            var origFontStyle = EditorStyles.label.fontStyle;
            EditorStyles.foldout.fontStyle = FontStyle.Bold;
            constraintUI.foldout = EditorGUILayout.Foldout(constraintUI.foldout, constraint.ToString(), true);
            EditorStyles.label.fontStyle = origFontStyle;

            // Draw the close button
            if (GUILayout.Button("x", GUILayout.Width(20)))
            {
                DestroyConstraintRule(constraint);
                constraintUI.foldout = false;
            }
            EditorGUILayout.EndHorizontal();

            if (constraintUI.foldout)
            {
                EditorGUI.indentLevel++;

                // Emit the property fields
                {
                    var csobject = new SerializedObject(constraint);
                    csobject.Update();

                    foreach (var field in constraint.GetType().GetFields())
                    {
                        if (field.GetCustomAttributes(typeof(HideInInspector), true).Length > 0)
                        {
                            // Do not show this property
                            continue;
                        }

                        SerializedProperty property = csobject.FindProperty(field.Name);
                        EditorGUILayout.PropertyField(property);
                    }

                    csobject.ApplyModifiedProperties();
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
        }

        void HandleAddConstratinMenuClick(object constraintTypeObject)
        {
            var constraintType = constraintTypeObject as System.Type;
            if (constraintType == null)
            {
                Debug.LogError("Invalid constraint type object passed to HandleAddConstratinMenuClick");
                return;
            }

            AddConstraintRule(constraintType);
        }

        void EmitAddConstraintWidget()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            int buttonWidth = 250;
            int buttonHeight = 40;
            if (GUILayout.Button(new GUIContent("Add Constraint Rule"), GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
            {
                var menuPosition = Event.current.mousePosition;
                var rect = new Rect(menuPosition, Vector2.one);
                var menu = new GenericMenu();

                var constraintTypes = Assembly.GetAssembly(typeof(ConstraintRule)).GetTypes()
                    .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(ConstraintRule))).ToArray();

                foreach (var type in constraintTypes)
                {
                    var path = ConstraintRule.GetFullMenuPath(type);
                    menu.AddItem(new GUIContent(path), false, HandleAddConstratinMenuClick, type);
                }

                menu.DropDown(rect);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

        }

        class RuleNodeEditorUIState
        {
            public int ruleDropDownIndex = 0;
            public Dictionary<ConstraintRule, ConstraintUIState> constraintStates = new Dictionary<ConstraintRule, ConstraintUIState>();

            public class ConstraintUIState
            {
                public bool foldout;
                public bool enabled = true;
            }
        }


    }
}