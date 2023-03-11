//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Builders.Grid
{
    /// <summary>
    /// Emits markers to beautify the level around corners based on the surrounding tiles
    /// </summary>
	public class MarkerEmitterCornerBeautifier : DungeonMarkerEmitter {
		public override void EmitMarkers(DungeonBuilder builder)
		{
			if (!(builder is GridDungeonBuilder))
			{
				Debug.LogWarning("Unsupported builder type used with marker emitter MarkerEmitterFindLowestPoint. Expected GridDungeonBuilder. Received:" + (builder != null ? builder.GetType().ToString() : "null"));
				return;
			}
			
			var gridModel = builder.Model as GridDungeonModel;

			foreach (var cell in gridModel.Cells) {
				var borderPoints = cell.Bounds.GetBorderPoints();
				foreach (var borderPoint in borderPoints) {
					EmitForPoint(builder, gridModel, borderPoint);
				}
			}
		}

		void EmitForPoint(DungeonBuilder builder, GridDungeonModel model, IntVector point) {
			foreach (var config in CornerConfigs) {
				if (ConfigMatches(model, point, config)) {
					EmitCornerMarker(builder, model, point, config.markerName);
					break;
				}
			}
		}

		bool ConfigMatches(GridDungeonModel model, IntVector point, CellSpatialConfig config) {
			var neighbors = config.neighborConfig;
			for (int i = 0; i < neighbors.Length; i++) {
				var code = neighbors[i];
				if (code == 0) {
					// Don't care about this cell
					continue;
				}
				var dx = i % 3;
				var dz = i / 3;
				dx--; dz--;	 // bring to -1..1 range (from previous 0..2)
				dz *= -1;
				var x = point.x + dx;
				var z = point.z + dz;

				var cellInfo = model.GetGridCellLookup(x, z);
				bool empty = cellInfo.CellType == CellType.Unknown;
				if (code == 1 && empty) {
					// We were expecting a non-empty space here, but it is empty
					return false;
				}
				else if (code == 2 && !empty) {
					// We were expecting a empty space here, but it is not empty
					return false;
				}
			}

			// Matches, all tests have passed
			return true;
		}

		void EmitCornerMarker(DungeonBuilder builder, GridDungeonModel model, IntVector point, string markerName) {
			// Add an empty marker here
			var gridSize = model.Config.GridCellSize;
			var position = point * gridSize;
			position += Vector3.Scale(new Vector3(0.5f, 0, 0.5f), gridSize);
			var transform = Matrix4x4.TRS(position, Quaternion.identity, Vector3.one);
			builder.EmitMarker(markerName, transform, point, -1);
		}

		
		class CellSpatialConfig {
			public CellSpatialConfig(string markerName, int[] neighborConfig) {
				this.markerName = markerName;
				this.neighborConfig = neighborConfig;
			}
			public string markerName;
			public int[] neighborConfig;
		}

		//// Neighbor config flags
		//  0: Dont care
		//  1: Land
		//  2: Empty Space
		/////////////////////////
		
		static CellSpatialConfig[] CornerConfigs = new CellSpatialConfig[] {
			new CellSpatialConfig("Corner_N", new int[] {
				0, 2, 0,
				1, 1, 1,
				0, 0, 0
			}),
			
			new CellSpatialConfig("Corner_S", new int[] {
				0, 0, 0,
				1, 1, 1,
				0, 2, 0
			}),
			
			new CellSpatialConfig("Corner_W", new int[] {
				0, 1, 0,
				2, 1, 0,
				0, 1, 0
			}),
			
			new CellSpatialConfig("Corner_E", new int[] {
				0, 1, 0,
				0, 1, 2,
				0, 1, 0
			}),
			
			/*
			*/
			new CellSpatialConfig("Corner_BSlash", new int[] {
				1, 1, 2,
				1, 1, 1,
				2, 1, 1
			}),
			new CellSpatialConfig("Corner_Slash", new int[] {
				2, 1, 1,
				1, 1, 1,
				1, 1, 2
			}),

			new CellSpatialConfig("Corner_ISlash", new int[] {
				1, 2, 2,
				2, 1, 2,
				2, 2, 1
			}),
			new CellSpatialConfig("Corner_IBSlash", new int[] {
				2, 2, 1,
				2, 1, 2,
				1, 2, 2
			}),

			new CellSpatialConfig("Corner_NW", new int[] {
				0, 2, 0,
				2, 1, 1,
				0, 1, 0
			}),
			
			new CellSpatialConfig("Corner_NE", new int[] {
				0, 2, 0,
				1, 1, 2,
				0, 1, 0
			}),
			
			new CellSpatialConfig("Corner_SW", new int[] {
				0, 1, 0,
				2, 1, 1,
				0, 2, 0
			}),
			
			new CellSpatialConfig("Corner_SE", new int[] {
				0, 1, 0,
				1, 1, 2,
				0, 2, 0
			}),
			
			// Inverted
			new CellSpatialConfig("Corner_INW", new int[] {
				2, 1, 0,
				1, 1, 0,
				0, 0, 0
			}),
			
			new CellSpatialConfig("Corner_INE", new int[] {
				0, 1, 2,
				0, 1, 1,
				0, 0, 0
			}),
			
			new CellSpatialConfig("Corner_ISW", new int[] {
				0, 0, 0,
				1, 1, 0,
				2, 1, 0
			}),
			
			new CellSpatialConfig("Corner_ISE", new int[] {
				0, 0, 0,
				0, 1, 1,
				0, 1, 2
			}),

		};

	}

}
