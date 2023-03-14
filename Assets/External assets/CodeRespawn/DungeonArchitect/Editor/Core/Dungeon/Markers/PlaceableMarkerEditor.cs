//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Editors
{
    [CustomEditor(typeof(PlaceableMarker))]
    public class PlaceableMarkerEditor : Editor
    {
        private SerializedObject sobject;
        private SerializedProperty supportedMarkers;
        
        private SerializedProperty debugColor;
        private SerializedProperty debugText;

        private void OnEnable()
        {
            sobject = new SerializedObject(target);
            supportedMarkers = sobject.FindProperty("supportedMarkers");
            
            debugColor = sobject.FindProperty("debugColor");
            debugText = sobject.FindProperty("debugText");
        }

        public override void OnInspectorGUI()
        {
            sobject.Update();
            
            GUILayout.Label("Placeable Marker", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(new GUIContent("Provide a list of possible markers that can be spawned here.  If the builder needs to spawn one of these, it will be spawned here"));
            EditorGUILayout.PropertyField(supportedMarkers, true);
            EditorGUILayout.Space();

            GUILayout.Label("Debug Visuals", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(debugColor);
            EditorGUILayout.PropertyField(debugText);
            EditorGUILayout.Space();
            
            sobject.ApplyModifiedProperties();
        }

        protected virtual void OnSceneGUI()
        {
            if (Event.current.type == EventType.Repaint)
            {
                var placeableMarker = target as PlaceableMarker;
                Transform transform = placeableMarker.transform;
                Handles.color = Handles.xAxisColor;

                {
                    Vector3 position = placeableMarker.transform.position + Vector3.up * 1.0f;
                    string posString = position.ToString();

                    GUIStyle style = new GUIStyle(GUI.skin.button);
                    style.alignment = TextAnchor.MiddleCenter;
                    Handles.Label(position, placeableMarker.debugText, style);
                }

                {
                    Handles.color = placeableMarker.debugColor;
                    Handles.ArrowHandleCap(
                        0,
                        transform.position,
                        transform.rotation,
                        1.0f,
                        EventType.Repaint
                    );
                }
            }
        }
    }
}