//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;

namespace DungeonArchitect.Builders.Grid
{
    /// <summary>
    /// A more specialized version of the EmptySpace emitter. Emits decorative markers in empty space near the layout
    /// </summary>
	public class MarkerEmitterFreeSpaceDecorator : DungeonMarkerEmitter
	{
		public int distanceFromEdge = 2;
		public string markerName = "EmtpySpaceDecoration";

		public float pushDownAmount = 6;
		public Vector3[] pushDownTestAxis = new Vector3[0];

		public override void EmitMarkers(DungeonBuilder builder)
		{
			if (!(builder is GridDungeonBuilder))
			{
				Debug.LogWarning("Unsupported builder type used with marker emitter MarkerEmitterFreeSpaceDecorator. Expected GridDungeonBuilder. Received:" + (builder != null ? builder.GetType().ToString() : "null"));
				return;
			}
			
			var model = builder.Model as GridDungeonModel;
			var gridSize = model.Config.GridCellSize;

			
			var visited = new HashSet<IntVector>();
			var occupied = new HashSet<IntVector>();
			foreach (var cell in model.Cells)
			{
				if (cell.CellType == CellType.Unknown) continue;

				for (var distance = 2; distance <= 2; distance++) {
					var bounds = cell.Bounds;
					bounds = Rectangle.ExpandBounds(bounds, distance);
					var points = bounds.GetBorderPoints();
					foreach (var point in points)
					{
						
						var hash = new IntVector(point.x, 0, point.z);
						if (!visited.Contains(hash))
						{
							visited.Add(hash);
							if (occupied.Contains(hash)) continue;

							// Check if this point does not lie in a cell
							var cellInfo = model.GetGridCellLookup(point.x, point.z);
							if (cellInfo.CellType == CellType.Unknown)
							{
								// Make sure the surrounding area is free so we can place a decorative item
								bool valid = true;
								var s = distanceFromEdge - 1;
								for (var dx = -s; dx <= s; dx++) {
									for (var dz = -s; dz <= s; dz++) {
										var x = point.x + dx;
										var z = point.z + dz;
										var neighborHash = new IntVector(x, 0, z);
										if (occupied.Contains(neighborHash)) {
											valid = false;
											break;
										}
										var neighborCellInfo = model.GetGridCellLookup(x, z);
										if (neighborCellInfo.CellType != CellType.Unknown) {
											// Occupied by an existing cell
											occupied.Add(neighborHash);
											valid = false;
											break;
										}
									}
									if (!valid) {
										break;
									}
								}


								if (valid) {
									// Valid space.  Occupy the space here
									for (var dx = -s; dx <= s; dx++) {
										for (var dz = -s; dz <= s; dz++) {
											var x = point.x + dx;
											var z = point.z + dz;
											occupied.Add (new IntVector(x, 0, z));
										}
									}

									var pushDownY = 0.0f;
									foreach (var pushDownAxis in pushDownTestAxis) {
										var delta = pushDownAxis * distanceFromEdge;
										var x = point.x + Mathf.RoundToInt(delta.x);
										var z = point.z + Mathf.RoundToInt(delta.z);
										var testCellInfo = model.GetGridCellLookup(x, z);
										if (testCellInfo.CellType != CellType.Unknown) {
											pushDownY = pushDownAmount;
										}
									}

									var position = point * gridSize + new Vector3(gridSize.x, 0, gridSize.z) * 0.5f;
									position.y -= pushDownY;
									var transform = Matrix4x4.TRS(position, Quaternion.identity, Vector3.one);

									builder.EmitMarker(markerName, transform, point, -1);
								}
							}
							else {
								// Occupied by a cell
								occupied.Add(hash);
							}
						}
					}
				}
			}
		}


	}
}
