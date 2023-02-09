//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Grammar;
using UnityEditor;

namespace DungeonArchitect.Editors.SnapFlow
{
    [CustomEditor(typeof(WeightedGrammarGraph))]
    public class WeightedGrammarGraphEditor : Editor
    {
        SerializedObject sobject;
        SerializedProperty weight;
        
        private void OnEnable()
        {
            sobject = new SerializedObject(target);
            weight = sobject.FindProperty("weight");
        }

        public override void OnInspectorGUI()
        {
            sobject.Update();
            
            EditorGUILayout.PropertyField(weight);
            
            InspectorNotify.Dispatch(sobject, target);
        }
    }
}