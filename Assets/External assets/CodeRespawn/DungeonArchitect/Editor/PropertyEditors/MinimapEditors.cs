//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Builders.GridFlow;
using UnityEditor;
using UnityEngine;


namespace DungeonArchitect.Editors
{
    [CustomEditor(typeof(GridFlowMinimapTrackedObject), true)]
    public class GridFlowMinimapTrackedObjectInspector : Editor
    {
        SerializedObject sobject;
        SerializedProperty icon;
        SerializedProperty iconScale;
        SerializedProperty rotateIcon;
        SerializedProperty tint;
        SerializedProperty exploresFogOfWar;
        SerializedProperty fogOfWarNumTileRadius;
        SerializedProperty fogOfWarLightFalloffStart;


        protected virtual void OnEnable()
        {
            sobject = new SerializedObject(target);
            icon = sobject.FindProperty("icon");
            iconScale = sobject.FindProperty("iconScale");
            rotateIcon = sobject.FindProperty("rotateIcon");
            tint = sobject.FindProperty("tint");
            exploresFogOfWar = sobject.FindProperty("exploresFogOfWar");
            fogOfWarNumTileRadius = sobject.FindProperty("fogOfWarNumTileRadius");
            fogOfWarLightFalloffStart = sobject.FindProperty("fogOfWarLightFalloffStart");
        }

        public override void OnInspectorGUI()
        {
            sobject.Update();
            var trackedObject = target as GridFlowMinimapTrackedObject;

            GUILayout.Label("Icon Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(icon);
            EditorGUILayout.PropertyField(iconScale);
            EditorGUILayout.PropertyField(rotateIcon);
            EditorGUILayout.PropertyField(tint);

            GUILayout.Label("Fog of War", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(exploresFogOfWar);
            if (trackedObject.exploresFogOfWar)
            {
                EditorGUILayout.PropertyField(fogOfWarNumTileRadius);
                EditorGUILayout.PropertyField(fogOfWarLightFalloffStart);
            }

            sobject.ApplyModifiedProperties();
        }

    }
}
