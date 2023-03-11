//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;
using STE = SharpNav.Geometry.TriangleEnumerable;
using SVector3 = SharpNav.Geometry.Vector3;
using Triangle3 = SharpNav.Geometry.Triangle3;
using Crowd = SharpNav.Crowds.Crowd;

namespace DungeonArchitect.Navigation {

    public class NavigationEvent
    {
        public DungeonNavMesh navMesh;
    }

    public delegate void OnNavmeshBuiltDelegate(NavigationEvent e);

	public class DungeonNavMesh : MonoBehaviour {
		public float agentHeight = 2;
		public float agentRadius = 0.5f;
		public float agentClimbHeight = 0.5f;
		public float cellSize = 0.2f;
		public int maxCrowdAgents = 50;
		public Mesh visualization;
		public Color visualizationColor = new Color(0, 0.5f, 1, 0.25f);
		public bool visualize2D = false;

        public event OnNavmeshBuiltDelegate OnNavmeshBuilt;
	    
	    SharpNav.NavMesh navMesh;
		public SharpNav.NavMesh NavMesh {
			get {
				return navMesh;
			}
		}


		SharpNav.NavMeshQuery navMeshQuery;
		public SharpNav.NavMeshQuery NavMeshQuery {
			get {
				return navMeshQuery;
			}
		}

		public Crowd crowd;
		public Crowd Crowd {
			get {
				return crowd;
			}
		}

		SharpNav.PolyMesh polyMesh = null;
		public SharpNav.PolyMesh PolyMesh {
			get {
				return polyMesh;
			}
		}
		
		SharpNav.PolyMeshDetail polyMeshDetail = null;
		public SharpNav.PolyMeshDetail PolyMeshDetail {
			get {
				return polyMeshDetail;
			}
		}

		void Awake() {
			SetNavMeshVisible(false);
		}

		void Update() {
			if (crowd != null) {
				crowd.Update(Time.deltaTime);
			}
		}

		public void SetNavMeshVisible(bool show) {
			var meshRenderer = GetComponent<MeshRenderer>();
			if (meshRenderer != null) {
				meshRenderer.enabled = show;
			}
		}

		// Use this for initialization
		public void Build() {
			List<Triangle3> triangles = new List<Triangle3>();
			// Get all the triangle provider components attached to this game object
			var triangleProviders = GetComponents<NavigationTriangleProvider>();
			foreach (var triangleProvider in triangleProviders) {
				triangleProvider.AddNavTriangles(triangles);
			}

			if (triangles.Count == 0) {
				// No geometry exists. Cannot create
				Debug.Log ("No geometry has been added to the nav mesh");
				return;
			}

			//use the default generation settings
			var settings = SharpNav.NavMeshGenerationSettings.Default;
			settings.AgentHeight = agentHeight;
			settings.AgentRadius = agentRadius;
			settings.MaxClimb = agentClimbHeight;
			settings.CellSize = cellSize;
			navMesh = SharpNav.NavMesh.Generate(triangles, settings, out polyMesh, out polyMeshDetail);

			SharpNav.TiledNavMesh tiledMesh = navMesh;
	        navMeshQuery = new SharpNav.NavMeshQuery(tiledMesh, 2048);
	        crowd = new Crowd(maxCrowdAgents, agentRadius, ref tiledMesh);

			BuildVisualization();

            if (OnNavmeshBuilt != null)
            {
                var e = new NavigationEvent();
                e.navMesh = this;
                OnNavmeshBuilt(e);
            }
		}

		public static Vector3 ToV3(SVector3 v) {
			return new Vector3(v.X, v.Y, v.Z);
		}

		void BuildVisualization() {
			//if (visualization == null) 
			{
				var filter = GetComponent<MeshFilter>();
				visualization = new Mesh();
				filter.mesh = visualization;
			}
			if (polyMesh == null) return;

			visualization.Clear();


			var tile = navMesh.GetTileAt(0, 0, 0);
			var vertices = new List<Vector3>();
			var triangles = new List<int>();

			for (int i = 0; i < tile.Polys.Length; i++)
			{
				for (int j = 1; j + 1 < tile.Polys[i].VertCount; j++)
				{
					int vertIndex0 = tile.Polys[i].Verts[0];
					int vertIndex1 = tile.Polys[i].Verts[j];
					int vertIndex2 = tile.Polys[i].Verts[j + 1];
					
					var v = tile.Verts[vertIndex0];
					var v0 = ToV3(v);
					
					v = tile.Verts[vertIndex1];
					var v1 = ToV3(v);
					
					v = tile.Verts[vertIndex2];
					var v2 = ToV3(v);

					var offset = vertices.Count;
					vertices.Add(v0);
					vertices.Add(v1);
					vertices.Add(v2);
					
					triangles.Add (offset);
					triangles.Add (offset + 1);
					triangles.Add (offset + 2);
				}
			}

			if (visualize2D) {
				// Flip YZ
				float t;
				for (int i = 0; i < vertices.Count; i++) {
					var v = vertices[i];
					t = v.y;
					v.y = v.z;
					v.z = t;
					vertices[i] = v;
				}
			}

			visualization.vertices = vertices.ToArray();
			visualization.SetIndices(triangles.ToArray(), MeshTopology.Triangles, 0);
		}
	}
}
