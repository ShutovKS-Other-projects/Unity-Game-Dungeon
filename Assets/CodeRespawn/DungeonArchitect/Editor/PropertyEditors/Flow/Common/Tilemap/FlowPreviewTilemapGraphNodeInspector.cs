//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Flow.Domains.Tilemap.Tooling;
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Editors.Flow.Common
{
    [CustomEditor(typeof(FlowTilemapToolGraphNode), false)]
    public class FlowPreviewTilemapGraphNodeInspector : Editor
    {
        SerializedObject sobject;
        SerializedProperty tileRenderSize;

        protected virtual void OnEnable()
        {
            sobject = new SerializedObject(target);
            tileRenderSize = sobject.FindProperty("tileRenderSize");
        }

        public override void OnInspectorGUI()
        {
            sobject.Update();

            EditorGUILayout.Space();
            GUILayout.Label("Tilemap Preview", InspectorStyles.HeaderStyle);
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(tileRenderSize);
                if (EditorGUI.EndChangeCheck())
                {
                    var node = target as FlowTilemapToolGraphNode;
                    node.RequestRecreatePreview = true;
                }
                EditorGUI.indentLevel--;
            }
            sobject.ApplyModifiedProperties();
        }
    }
}