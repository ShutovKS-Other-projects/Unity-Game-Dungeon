//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonArchitect.Editors.Flow.Impl
{
    [CustomEditor(typeof(SnapGridFlowEditorConfig), true)]
    public class SnapGridFlowEditorConfigEditor : Editor
    {
        private SerializedObject sobject;
        SerializedProperty randomizeSeed;
        SerializedProperty seed;
        SerializedProperty moduleDatabase;
        SerializedProperty autoFocusViewport;
        SerializedProperty layoutMode;
        
        private void OnEnable()
        {
            sobject = new SerializedObject(target);
            randomizeSeed = sobject.FindProperty("randomizeSeed");
            seed = sobject.FindProperty("seed");
            moduleDatabase = sobject.FindProperty("moduleDatabase");
            autoFocusViewport = sobject.FindProperty("autoFocusViewport");
            layoutMode = sobject.FindProperty("layoutMode");
        }

        public override void OnInspectorGUI()
        {
            sobject.Update();
            
            GUILayout.Label("SGF Editor Config", EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(randomizeSeed);
            EditorGUILayout.PropertyField(seed);
            EditorGUILayout.PropertyField(moduleDatabase);
            EditorGUILayout.PropertyField(autoFocusViewport);

            /*
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(layoutMode);
                if (changeScope.changed)
                {
                    RebuildEditorLayout();
                }
            }
            */

            sobject.ApplyModifiedProperties();
        }

        void RebuildEditorLayout()
        {
            var window = EditorWindow.GetWindow<SnapGridFlowEditorWindow>();
            if (window != null)
            {
                window.RequestRebuildLayout();
            }
        }
    }
}