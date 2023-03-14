//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;
using DungeonArchitect.Utils;
using STE = SharpNav.Geometry.TriangleEnumerable;
using SVector3 = SharpNav.Geometry.Vector3;
using Triangle3 = SharpNav.Geometry.Triangle3;
using DungeonArchitect.Builders.Grid;

namespace DungeonArchitect.Navigation {
	public class LayoutFloorTriangleProvider : NavigationTriangleProvider {
		public Dungeon dungeon;

		public override void AddNavTriangles(List<Triangle3> triangles) {
			if (dungeon == null) {
				Debug.LogWarning("LayoutFloorTriangleProvider: Dungeon is not assigned");
				return;
			}

			var model = dungeon.ActiveModel as GridDungeonModel;
			if (model == null) {
				Debug.LogWarning("LayoutFloorTriangleProvider: Dungeon model is invalid. Rebuild the dungeon");
				return;
			}

			var config = model.Config;
			var verts = new SVector3[4];
			for (int i = 0; i < verts.Length; i++) {
				verts[i] = new SVector3();
			}

			foreach (var cell in model.Cells) {
				//if (cell.CellType == CellType.Unknown) continue;

				var bounds = cell.Bounds;
				var location = MathUtils.GridToWorld(config.GridCellSize, bounds.Location);
				var size = MathUtils.GridToWorld(config.GridCellSize, bounds.Size);

				verts[0].Set (location.x, location.y, location.z);
				verts[1].Set (location.x + size.x, location.y, location.z);
				verts[2].Set (location.x + size.x, location.y, location.z + size.z);
				verts[3].Set (location.x, location.y, location.z + size.z);

				triangles.Add (new Triangle3(
					verts[0],
					verts[1],
					verts[2]));

				triangles.Add (new Triangle3(
					verts[2],
					verts[3],
					verts[0]));
			}
		}

	}
}
