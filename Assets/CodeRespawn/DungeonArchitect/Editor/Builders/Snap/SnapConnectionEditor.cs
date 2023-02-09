//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Frameworks.Snap;
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Editors
{
	[CustomEditor(typeof(SnapConnection))]
	public class SnapConnectionEditor : Editor
	{
		private SerializedObject sobject;
		private SerializedProperty doorObject;
		private SerializedProperty wallObject;
		private SerializedProperty category;
		
		private SerializedProperty oneWayDoorObject;
		private SerializedProperty lockedDoors;
		
		public void OnEnable()
		{
			sobject = new SerializedObject(target);
			
			doorObject = sobject.FindProperty("doorObject");
			wallObject = sobject.FindProperty("wallObject");
			category = sobject.FindProperty("category");
			
			oneWayDoorObject = sobject.FindProperty("oneWayDoorObject");
			lockedDoors = sobject.FindProperty("lockedDoors");
		}

		public override void OnInspectorGUI()
		{
			sobject.Update();

			GUILayout.Label("Snap Connection", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(doorObject);
			EditorGUILayout.PropertyField(wallObject);
			EditorGUILayout.PropertyField(category);
			EditorGUILayout.Space();
			
			GUILayout.Label("Advanced Doors", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(oneWayDoorObject);
			EditorGUILayout.PropertyField(lockedDoors);

			sobject.ApplyModifiedProperties();
		}
	}
}
