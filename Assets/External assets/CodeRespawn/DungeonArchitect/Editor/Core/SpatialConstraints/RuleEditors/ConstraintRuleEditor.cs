//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using UnityEditor;
using DungeonArchitect.SpatialConstraints;

namespace DungeonArchitect.Editors.SpatialConstraints
{
    [CustomEditor(typeof(ConstraintRule))]
    public class ConstraintRuleEditor : GraphNodeEditor
    {
        protected SerializedProperty exclusionRule;

        public override void OnEnable()
        {
            base.OnEnable();
            exclusionRule = sobject.FindProperty("exclusionRule");
        }

        protected override void HandleInspectorGUI()
        {
            GUILayout.Label("Constraint Rule", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(exclusionRule);
        }
    }
}
