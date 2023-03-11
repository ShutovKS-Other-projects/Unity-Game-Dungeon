//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Grammar;
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Editors.SnapFlow
{
    [CustomEditor(typeof(GrammarTaskNode))]
    public class GrammarTaskNodeEditor : Editor
    {
        SerializedObject sobject;
        SerializedProperty executionIndex;

        public void OnEnable()
        {
            sobject = new SerializedObject(target);
            executionIndex = sobject.FindProperty("executionIndex");
        }

        public override void OnInspectorGUI()
        {
            sobject.Update();

            GUILayout.Label("Task Node", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(executionIndex);
            
            InspectorNotify.Dispatch(sobject, target);
        }
    }

}
