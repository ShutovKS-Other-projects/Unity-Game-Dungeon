//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Graphs.Layouts;
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Editors.SnapFlow
{
    [CustomEditor(typeof(SnapEdResultGraphEditorConfig), true)]
    public class SnapFlowResultGraphEditorConfigInspector : Editor
    {
        SerializedObject sobject;
        SerializedProperty layoutType;

        SerializedProperty configLayered_Separation;

        SerializedProperty configSpring_springDistance;
        SerializedProperty configSpring_nodeDistance;

        protected virtual void OnEnable()
        {
            sobject = new SerializedObject(target);
            layoutType = sobject.FindProperty("layoutType");

            var configLayered = sobject.FindProperty("configLayered");
            configLayered_Separation = configLayered.FindPropertyRelative("separation");

            var configSpring = sobject.FindProperty("configSpring");
            configSpring_springDistance = configSpring.FindPropertyRelative("springDistance");
            configSpring_nodeDistance = configSpring.FindPropertyRelative("interNodeDistance");
        }

        public override void OnInspectorGUI()
        {
            sobject.Update();


            GUILayout.Label("Layout Config", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(layoutType);
            GUILayout.Space(10);

            var targetConfig = target as SnapEdResultGraphEditorConfig;
            if (targetConfig.layoutType == GraphLayoutType.Layered)
            {
                EditorGUILayout.PropertyField(configLayered_Separation);
            }
            else if (targetConfig.layoutType == GraphLayoutType.Spring)
            {
                EditorGUILayout.PropertyField(configSpring_springDistance);
                EditorGUILayout.PropertyField(configSpring_nodeDistance);
            }

            InspectorNotify.Dispatch(sobject, target);
        }
    }
}