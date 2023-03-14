//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using UnityEditor;
using DungeonArchitect.Graphs;

namespace DungeonArchitect.Editors
{
    /// <summary>
    /// Custom property editor for placeable node
    /// </summary>
    public abstract class PlaceableNodeEditor : GraphNodeEditor
    {
        SerializedProperty ConsumeOnAttach;
        SerializedProperty AttachmentProbability;
        protected bool drawOffset = false;
        protected bool drawAttachments = false;

        public override void OnEnable()
        {
            base.OnEnable();

            ConsumeOnAttach = sobject.FindProperty("consumeOnAttach");
            AttachmentProbability = sobject.FindProperty("attachmentProbability");
        }

        protected abstract string GetNodeTitle();

        public static void DrawHeader(string title)
        {
            GUILayout.Space(CATEGORY_SPACING);
            GUILayout.Label(title, InspectorStyles.HeaderStyle);
        }
        
        protected override void HandleInspectorGUI() {

            GUILayout.Label(GetNodeTitle(), InspectorStyles.TitleStyle);
            
            if (drawOffset)
            {
                // Draw the transform offset editor
                DrawHeader("Offset");
                if (targets != null && targets.Length > 1)
                {
                    // Multiple object editing not supported
                    EditorGUILayout.HelpBox("Multiple Objects selected", MessageType.Info);
                }
                else
                {
                    var node = target as PlaceableNode;
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(INDENT_SPACING);
                    GUILayout.BeginVertical();
                    InspectorUtils.DrawMatrixProperty("Offset", ref node.offset);
                    EditorGUILayout.HelpBox(new GUIContent("TIP: Visualize the correct alignment by turning it on from the theme editor's toolbar"));
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }

            if (drawAttachments)
            {
                // Draw the attachment properties
                DrawHeader("Attachment");
                using (new EditorGUI.IndentLevelScope()) {
                    EditorGUILayout.PropertyField(AttachmentProbability, new GUIContent("Probability"));
                    EditorGUILayout.PropertyField(ConsumeOnAttach);
                }

                // Clamp the probability to [0..1]
                if (!AttachmentProbability.hasMultipleDifferentValues)
                {
                    AttachmentProbability.floatValue = Mathf.Clamp01(AttachmentProbability.floatValue);
                }
            }
            DrawPreInspectorGUI();
            DrawPostInspectorGUI();
            DrawMiscInspectorGUI();
        }

        protected override void OnGuiChanged()
        {
            var themeEditorWindow = DungeonEditorHelper.GetWindowIfOpen<DungeonThemeEditorWindow>();
            if (themeEditorWindow != null)
            {
                var graphEditor = themeEditorWindow.GraphEditor;
                graphEditor.HandleGraphStateChanged(themeEditorWindow.uiSystem);
                graphEditor.HandleNodePropertyChanged(target as GraphNode);
            }
        }

        protected virtual void DrawPreInspectorGUI() { }
        protected virtual void DrawPostInspectorGUI() { }
        
        protected virtual void DrawMiscInspectorGUI() { }

        protected const int CATEGORY_SPACING = 10;
        protected const int INDENT_SPACING = 12;

    }
}
