//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using UnityEditor;

namespace DungeonArchitect.Editors
{
    [CustomEditor(typeof(DungeonBuilder), true)]
    public class DungeonBuilderEditor : Editor
    {
        SerializedObject sobject;
        SerializedProperty asyncBuild;
        SerializedProperty maxBuildTimePerFrame;
        SerializedProperty asyncBuildStartPosition;
        
        protected virtual void OnEnable()
        {
            sobject = new SerializedObject(target);
            asyncBuild = sobject.FindProperty("asyncBuild");
            maxBuildTimePerFrame = sobject.FindProperty("maxBuildTimePerFrame");
            asyncBuildStartPosition = sobject.FindProperty("asyncBuildStartPosition");
        }
        
        public override void OnInspectorGUI()
        {
            sobject.Update();

            GUILayout.Label("Async Building", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(asyncBuild);
            EditorGUILayout.PropertyField(maxBuildTimePerFrame);
            EditorGUILayout.PropertyField(asyncBuildStartPosition);

            sobject.ApplyModifiedProperties();
        }
    }
}
