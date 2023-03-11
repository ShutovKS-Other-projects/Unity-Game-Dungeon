//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Builders.Grid
{
    /// <summary>
    /// Finds the lowest dungeon point and emits a marker at that position.  Also sets the scale of the marker to match the width / height
    /// </summary>
    public class MarkerEmitterFindLowestPoint : DungeonMarkerEmitter
    {
        public string MarkerName = "LowestPoint";
        public string BlackboardKeyLowestY = "DungeonLowestY";

        public override void EmitMarkers(DungeonBuilder builder)
        {
            if (!(builder is GridDungeonBuilder))
            {
                Debug.LogWarning("Unsupported builder type used with marker emitter MarkerEmitterFindLowestPoint. Expected GridDungeonBuilder. Received:" + (builder != null ? builder.GetType().ToString() : "null"));
                return;
            }

            var gridModel = builder.Model as GridDungeonModel;
            var Min = new Vector3(int.MaxValue, int.MaxValue, int.MaxValue);
            var Max = new Vector3(-int.MaxValue, -int.MaxValue, -int.MaxValue);
            var gridSize = gridModel.Config.GridCellSize;

            if (gridModel.Cells.Count == 0)
            {
                Min = Vector3.zero;
                Max = Vector3.zero;
            }
            else
            {
                foreach (var cell in gridModel.Cells)
                {
                    var location = cell.Bounds.Location * gridSize;
                    var size = cell.Bounds.Size * gridSize;
                    Min.x = Mathf.Min(Min.x, location.x);
                    Min.y = Mathf.Min(Min.y, location.y);
                    Min.z = Mathf.Min(Min.z, location.z);

                    Max.x = Mathf.Max(Max.x, location.x + size.x);
                    Max.y = Mathf.Max(Max.y, location.y + size.y);
                    Max.z = Mathf.Max(Max.z, location.z + size.z);
                }
            }

            var rangeSize = Max - Min;

            var position = (Max + Min) / 2;
            position.y = Min.y;

            var scale = new Vector3(rangeSize.x, 1, rangeSize.z);
            var transform = Matrix4x4.TRS(position, Quaternion.identity, scale);

            builder.EmitMarker(MarkerName, transform, IntVector.Zero, -1);

            // Save this for other markers to use, if needed
            builder.Blackboard.FloatEntries.SetValue(BlackboardKeyLowestY, Min.y);
        }
    }
}