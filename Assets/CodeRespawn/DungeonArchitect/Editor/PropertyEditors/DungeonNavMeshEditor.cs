//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using UnityEditor;
using SVector3 = SharpNav.Geometry.Vector3;
using PolyMesh = SharpNav.PolyMesh;
using DungeonArchitect.Navigation;

namespace DungeonArchitect.Editors
{
	[CustomEditor(typeof(DungeonNavMesh))]
	public class DungeonNavMeshEditor : Editor
	{
		SerializedObject sobject;
		SerializedProperty agentHeight;
		SerializedProperty agentRadius;
		SerializedProperty agentClimbHeight;
		SerializedProperty cellSize;
		SerializedProperty maxCrowdAgents;
		SerializedProperty visualize2D;

		public void OnEnable()
		{
			sobject = new SerializedObject(target);
			agentHeight = sobject.FindProperty("agentHeight");
			agentRadius = sobject.FindProperty("agentRadius");
			agentClimbHeight = sobject.FindProperty("agentClimbHeight");
			cellSize = sobject.FindProperty("cellSize");
			maxCrowdAgents = sobject.FindProperty("maxCrowdAgents");
			visualize2D = sobject.FindProperty("visualize2D");

			var navMesh = target as DungeonNavMesh;
			navMesh.SetNavMeshVisible(true);
		}

		public void OnDisable()
		{
			var navMesh = target as DungeonNavMesh;
			navMesh.SetNavMeshVisible(false);
		}

		public override void OnInspectorGUI()
		{
			sobject.Update();
			GUILayout.Label("Dungeon Nav Mesh Builder", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(agentHeight);
			EditorGUILayout.PropertyField(agentRadius);
			EditorGUILayout.PropertyField(agentClimbHeight);
			EditorGUILayout.PropertyField(cellSize);
			EditorGUILayout.PropertyField(maxCrowdAgents);
			EditorGUILayout.PropertyField(visualize2D);

			agentHeight.floatValue = Mathf.Max(0, agentHeight.floatValue);
			agentRadius.floatValue = Mathf.Max(0, agentRadius.floatValue);
			agentClimbHeight.floatValue = Mathf.Max(0, agentClimbHeight.floatValue);
			cellSize.floatValue = Mathf.Max(0.01f, cellSize.floatValue);
			maxCrowdAgents.intValue = Mathf.Max(0, maxCrowdAgents.intValue);

			if (GUILayout.Button("Build Nav Mesh"))
			{
				BuildNavMesh();
			}

			sobject.ApplyModifiedProperties();
		}

		void BuildNavMesh()
		{
			var navMesh = target as DungeonNavMesh;
			navMesh.Build();

			/*
			// Since this is called from the editor, save the nav mesh on to the disk 
			var scenePath = EditorApplication.currentScene;
			if (scenePath == null || scenePath.Length == 0) {
				Debug.Log ("Save the scene and rebuild to serialize the nav mesh");
			} else {
				var file = new FileInfo(scenePath);
				var name = Path.GetFileNameWithoutExtension(file.Name);
	
				var navName = file.Directory.FullName + Path.DirectorySeparatorChar + name + "_Navigation.asset";
				var graph = ScriptableObject.CreateInstance<Graph>();
				AssetDatabase.CreateAsset(graph, navName);
	            Debug.Log ("Saved navmesh to: " + navName);
			}
			*/
		}

		Vector3 ToV3(SharpNav.PolyVertex v)
		{
			return new Vector3(v.X, v.Y, v.Z);
		}

		void OnSceneGUI()
		{
			var navMesh = target as DungeonNavMesh;
			var polyMesh = navMesh.PolyMesh;
			if (polyMesh == null) return;
			var polyScale = new Vector3(polyMesh.CellSize, polyMesh.CellHeight, polyMesh.CellSize);
			var polyTrans = new Vector3(polyMesh.Bounds.Min.X, polyMesh.Bounds.Min.Y, polyMesh.Bounds.Min.Z);

			var color = new Color(0, 0, 0, 1);
			for (int i = 0; i < polyMesh.PolyCount; i++)
			{
				for (int j = 0; j < polyMesh.NumVertsPerPoly; j++)
				{
					if (polyMesh.Polys[i].Vertices[j] == PolyMesh.NullId)
						break;

					if (PolyMesh.IsInteriorEdge(polyMesh.Polys[i].NeighborEdges[j]))
						continue;

					int nj = (j + 1 >= polyMesh.NumVertsPerPoly || polyMesh.Polys[i].Vertices[j + 1] == PolyMesh.NullId) ? 0 : j + 1;

					int vertIndex0 = polyMesh.Polys[i].Vertices[j];
					int vertIndex1 = polyMesh.Polys[i].Vertices[nj];

					var v = polyMesh.Verts[vertIndex0];
					var v0 = ToV3(v);
					v0 = Vector3.Scale(v0, polyScale) + polyTrans;

					v = polyMesh.Verts[vertIndex1];
					var v1 = ToV3(v);
					v1 = Vector3.Scale(v1, polyScale) + polyTrans;

					if (visualize2D.boolValue)
					{
						FlipFor2D(ref v0);
						FlipFor2D(ref v1);
					}

					Handles.color = color;
					Handles.DrawLine(v0, v1);
					//Debug.DrawLine(v0, v1, color);
				}
			}
		}

		void FlipFor2D(ref Vector3 v)
		{
			var t = v.y;
			v.y = v.z;
			v.z = t;
		}
	}
}
