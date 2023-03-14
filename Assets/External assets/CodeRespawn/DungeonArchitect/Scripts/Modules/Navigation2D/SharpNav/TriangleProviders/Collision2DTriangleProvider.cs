//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;
using DungeonArchitect.Utils;
using STE = SharpNav.Geometry.TriangleEnumerable;
using SVector3 = SharpNav.Geometry.Vector3;
using Triangle3 = SharpNav.Geometry.Triangle3;

namespace DungeonArchitect.Navigation {
	public class Collision2DTriangleProvider : NavigationTriangleProvider {
		
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
		
		void AddTriangles(List<Triangle3> triangles, GameObject gameObject) {
			var colliders = gameObject.GetComponentsInChildren<Collider2D>();
			foreach (var collider in colliders) {
				var transform = Matrix.FromGameTransform(collider.gameObject.transform);
				if (collider is BoxCollider2D) {
					var boxCollider = collider as BoxCollider2D;
					Vector3 goPosition;
					Quaternion goRotation;
					Vector3 goScale;
					Matrix.DecomposeMatrix(ref transform, out goPosition, out goRotation, out goScale);

					var collisionheight = 5;
					var size = new Vector3(boxCollider.size.x, collisionheight, boxCollider.size.y);
					Vector3 scale = Vector3.Scale(goScale, size);
					var boxCenterXZ = boxCollider.offset;
					var boxCenter = new Vector3(boxCenterXZ.x, 0, boxCenterXZ.y); 
					Vector3 position = goPosition + goRotation * Vector3.Scale(boxCenter, goScale);
                    position = MathUtils.FlipYZ(position);
                    position.y = 0;
					transform = Matrix4x4.TRS (position, goRotation, scale);
					
					StaticMeshTriangleProvider.AddMeshTriangles(triangles, cubeVertices, cubeIndices, transform);
				}
				else if (collider is CircleCollider2D) {
					// TODO: Implement
				}
			}
		}
		
		
		static readonly Vector3[] cubeVertices = new Vector3[] { 
			new Vector3(-0.5f, -0.5f, 0.5f), 
			new Vector3(0.5f, -0.5f, 0.5f), 
			new Vector3(0.5f, 0.5f, 0.5f), 
			new Vector3(-0.5f, 0.5f, 0.5f), 
			new Vector3(-0.5f, -0.5f, -0.5f), 
			new Vector3(0.5f, -0.5f, -0.5f), 
			new Vector3(0.5f, 0.5f, -0.5f), 
			new Vector3(-0.5f, 0.5f, -0.5f) 
		};
		
		static readonly int[] cubeIndices = new int[] { 
			// front
			0, 1, 2,
			2, 3, 0,
			// top
			3, 2, 6,
			6, 7, 3,
			// back
			7, 6, 5,
			5, 4, 7,
			// bottom
			4, 5, 1,
			1, 0, 4,
			// left
			4, 0, 3,
			3, 7, 4,
			// right
			1, 5, 6,
			6, 2, 1,
		};
	}
}
