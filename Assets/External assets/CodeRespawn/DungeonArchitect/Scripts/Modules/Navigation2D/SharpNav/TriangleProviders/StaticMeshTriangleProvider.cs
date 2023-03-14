//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;
using DungeonArchitect.Utils;
using STE = SharpNav.Geometry.TriangleEnumerable;
using SVector3 = SharpNav.Geometry.Vector3;
using Triangle3 = SharpNav.Geometry.Triangle3;

namespace DungeonArchitect.Navigation {
	public class StaticMeshTriangleProvider : NavigationTriangleProvider {
		
		public override void AddNavTriangles(List<Triangle3> triangles) {
            var dataList = GameObject.FindObjectsOfType<DungeonSceneProviderData>();
            foreach (var data in dataList)
            {
                if (data == null) continue;
                if (data.affectsNavigation)
                {
                    AddTriangles(triangles, data.gameObject);
                }
            }
		}

		void AddTriangles (List<Triangle3> triangles, GameObject gameObject) {
			var filters = gameObject.GetComponentsInChildren<MeshFilter>();
			foreach (MeshFilter filter in filters) {
				var transform = Matrix.FromGameTransform(filter.gameObject.transform);
				AddMeshTriangles(triangles, filter.sharedMesh, transform);
			}
		}
		
		public static void AddMeshTriangles(List<Triangle3> triangles, Mesh mesh, Matrix4x4 transform) {
			AddMeshTriangles(triangles, mesh.vertices, mesh.triangles, transform);
		}
		
		public static void AddMeshTriangles(List<Triangle3> triangles, Vector3[] vertices, int[] indices, Matrix4x4 transform) {
			List<SVector3> svertices = new List<SVector3>();
			foreach (var vert in vertices) {
				var gvert = transform.MultiplyPoint(vert);
				svertices.Add (new SVector3(gvert.x, gvert.y, gvert.z));
			}
			
			for (int i = 0; i + 2 < indices.Length; i += 3) {
				var tri = new Triangle3(
					svertices[indices[i]],
					svertices[indices[i + 1]],
					svertices[indices[i + 2]]);
				triangles.Add(tri);
			}
		}
	}
}
