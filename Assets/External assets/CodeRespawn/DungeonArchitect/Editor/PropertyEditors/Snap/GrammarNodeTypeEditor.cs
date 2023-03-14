//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Grammar;
using UnityEditor;

namespace DungeonArchitect.Editors.SnapFlow
{
    [CustomEditor(typeof(GrammarNodeType))]
    public class GrammarNodeTypeEditor : Editor
    {
        SerializedObject sobject;
        SerializedProperty nodeName;
        SerializedProperty description;
        SerializedProperty nodeColor;

        private void OnEnable()
        {
            sobject = new SerializedObject(target);
            nodeName = sobject.FindProperty("nodeName");
            description = sobject.FindProperty("description");
            nodeColor = sobject.FindProperty("nodeColor");
        }

        public override void OnInspectorGUI()
        {
            sobject.Update();
            
            EditorGUILayout.PropertyField(nodeName);
            EditorGUILayout.PropertyField(description);
            EditorGUILayout.PropertyField(nodeColor);
            
            InspectorNotify.Dispatch(sobject, target);
        }
    }
}