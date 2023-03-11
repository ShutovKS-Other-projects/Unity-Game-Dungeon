//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.RoomDesigner
{
    public class DungeonRoomMarkerEmitter
    {
        public static void EmitMarkers(DungeonRoomDesigner room, LevelMarkerList markerList)
        {
            var min = room.roomPosition;
            var max = room.roomPosition + room.roomSize;

            var markers = new List<PropSocket>();
            markers.AddRange(RectFillMarkers(min, max, min.y, "Ground", room.gridSize));
            int y;
            for (y = min.y; y < max.y; y += 2)
            {
                string markerName = (y == max.y - 1) ? "WallHalf" : "Wall";
                markers.AddRange(RectBoundaryMarkers(min, max, y, markerName, room.gridSize));
            }
            markers.AddRange(RectFillMarkers(min, max, y, "Ceiling", room.gridSize));

            markerList.AddRange(markers.ToArray());
        }

        private static PropSocket[] RectFillMarkers(IntVector min, IntVector max, int y, string markerName, Vector3 gridSize)
        {
            var result = new List<PropSocket>();
            for (int x = min.x; x < max.x; x++)
            {
                for (int z = min.z; z < max.z; z++)
                {
                    var gridCoord = Vector3.Scale(new Vector3(x + 0.5f, y, z + 0.5f), gridSize);
                    result.Add(CreateMarker(gridCoord, Quaternion.identity, markerName));
                }
            }
            return result.ToArray();
        }

        private static PropSocket[] RectBoundaryMarkers(IntVector min, IntVector max, int y, string markerName, Vector3 gridSize)
        {
            var result = new List<PropSocket>();
            for (int x = min.x; x < max.x; x++)
            {
                var p0 = new Vector3(x + 0.5f, y, min.z);
                var p1 = new Vector3(x + 0.5f, y, max.z);
                var r0 = Quaternion.Euler(0, 180, 0);
                var r1 = Quaternion.Euler(0, 0, 0);
                result.Add(CreateMarker(Vector3.Scale(p0, gridSize), r0, markerName));
                result.Add(CreateMarker(Vector3.Scale(p1, gridSize), r1, markerName));
            }

            for (int z = min.z; z < max.z; z++)
            {
                var p0 = new Vector3(min.x, y, z + 0.5f);
                var p1 = new Vector3(max.x, y, z + 0.5f);
                var r0 = Quaternion.Euler(0, 270, 0);
                var r1 = Quaternion.Euler(0, 90, 0);
                result.Add(CreateMarker(Vector3.Scale(p0, gridSize), r0, markerName));
                result.Add(CreateMarker(Vector3.Scale(p1, gridSize), r1, markerName));
            }

            return result.ToArray();
        }

        private static PropSocket CreateMarker(Vector3 position, Quaternion rotation, string name)
        {
            var marker = new PropSocket();
            marker.Id = 0;
            marker.SocketType = name;
            marker.Transform = Matrix4x4.TRS(position, rotation, Vector3.one);
            return marker;
        }

    }
}
