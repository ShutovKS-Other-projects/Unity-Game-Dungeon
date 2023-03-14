//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;

namespace DungeonArchitect.Builders.Grid
{
    /// <summary>
    /// Emits markers in the nearby empty space of the dungeon layout
    /// </summary>
    public class MarkerEmitterEmptySpace : DungeonMarkerEmitter
    {
        public int distanceToCover = 3;
        public string markerName = "EmptySpace";
        public string indexedMarkerNamePrefix = "EmptySpace_";

        public bool overrideY = false;
        public string overrideYBlackboardKey = "DungeonLowestY";

        public override void EmitMarkers(DungeonBuilder builder)
        {
            var visited = new HashSet<IntVector>();
            var model = builder.Model as GridDungeonModel;
            if (model == null) return;

            var config = model.Config as GridDungeonConfig;
            if (config == null) return;

            var gridSize = config.GridCellSize;
            float overrideYValue = 0;
            if (overrideY)
            {
                overrideYValue = builder.Blackboard.FloatEntries.GetValue(overrideYBlackboardKey);
            }

            for (int d = 1; d <= distanceToCover; d++)
            {
                var indexedMarkerName = indexedMarkerNamePrefix + d;
                foreach (var cell in model.Cells)
                {
                    if (cell.CellType == CellType.Unknown) continue;

                    var bounds = cell.Bounds;
                    bounds = Rectangle.ExpandBounds(bounds, d);
                    var points = bounds.GetBorderPoints();
                    foreach (var point in points)
                    {
                        var hash = new IntVector(point.x, 0, point.z);
                        if (!visited.Contains(hash))
                        {
                            // Check if this point does not lie in a cell
                            var cellInfo = model.GetGridCellLookup(point.x, point.z);
                            if (cellInfo.CellType == CellType.Unknown)
                            {
                                // Add an empty marker here
                                var position = point * gridSize;
                                position += Vector3.Scale(new Vector3(0.5f, 0, 0.5f), gridSize);
                                if (overrideY)
                                {
                                    position.y = overrideYValue;
                                }

                                var transform = Matrix4x4.TRS(position, Quaternion.identity, Vector3.one);
                                builder.EmitMarker(markerName, transform, point, -1);
                                builder.EmitMarker(indexedMarkerName, transform, point, -1);
                            }

                            visited.Add(hash);
                        }
                    }
                }
            }
        }

        
    }
}
