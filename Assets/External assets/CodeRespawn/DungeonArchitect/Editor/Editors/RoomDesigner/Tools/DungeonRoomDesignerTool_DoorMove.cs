//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEditor;
using UnityEngine;
using MU = DungeonArchitect.Utils.MathUtils;

namespace DungeonArchitect.RoomDesigner.Editors
{
    public class DungeonRoomDesignerTool_DoorMove : DungeonRoomDesignerTools
    {
        float SnapToGrid(float value, float gridValue)
        {
            return Mathf.RoundToInt(value / gridValue) * gridValue;
        }

        void ClampToWalls(DungeonRoomDesigner room, Vector3 p, out Quaternion rotation, out Vector3 freeMovePosition, out Vector3 snappedPosition)
        {
            var min = IntVector.Zero;
            var max = min + room.roomSize;
            
            p.x = Mathf.Clamp(p.x, min.x, max.x);
            p.y = Mathf.Clamp(p.y, min.y, max.y);
            p.z = Mathf.Clamp(p.z, min.z, max.z);
            
            // Clamp from the inside
            float minDistance = float.MaxValue;
            Vector3 bestResult = p;
            rotation = Quaternion.identity;
            var offsetFix = Vector3.zero;
            if (p.x - min.x < minDistance)
            {
                minDistance = p.x - min.x;
                bestResult = p;
                bestResult.x = min.x;
                rotation = Quaternion.Euler(0, 90, 0);
                offsetFix = new Vector3(0, 0, 0.5f);
            }
            if (max.x - p.x < minDistance)
            {
                minDistance = max.x - p.x;
                bestResult = p;
                bestResult.x = max.x;
                rotation = Quaternion.Euler(0, 270, 0);
                offsetFix = new Vector3(0, 0, 0.5f);
            }
            if (p.z - min.z < minDistance)
            {
                minDistance = p.z - min.z;
                bestResult = p;
                bestResult.z = min.z;
                rotation = Quaternion.Euler(0, 0, 0);
                offsetFix = new Vector3(0.5f, 0, 0);
            }
            if (max.z - p.z < minDistance)
            {
                minDistance = max.z - p.z;
                bestResult = p;
                bestResult.z = max.z;
                rotation = Quaternion.Euler(0, 180, 0);
                offsetFix = new Vector3(0.5f, 0, 0);
            }

            freeMovePosition = bestResult;
            snappedPosition = MU.V3FloorToInt(bestResult) + offsetFix;

            snappedPosition.y = Mathf.Min(snappedPosition.y, max.y - 2);

            if (snappedPosition.x > max.x) --snappedPosition.x;
            if (snappedPosition.z > max.z) --snappedPosition.z;
        }

        void DrawDoorEditor(DungeonRoomDesigner room, ref DungeonRoomDoorDesigner door)
        {
            var snap = Vector3.one;
            var baseOffset = room.roomPosition.ToVector3();

            var doorWorldPos = Vector3.Scale(baseOffset + door.logicalCursorPosition, room.gridSize);
            Handles.color = Color.green;
            float size = HandleUtility.GetHandleSize(doorWorldPos) * 0.25f;
            var fmh_79_65_638114807294055264 = Quaternion.identity; doorWorldPos = Handles.FreeMoveHandle(doorWorldPos, size, snap, Handles.SphereHandleCap);
            var logicalPos = MU.Divide(doorWorldPos, room.gridSize) - baseOffset;

            var oldLogicalPosition = door.logicalPosition;
            ClampToWalls(room, logicalPos, out door.rotation, out door.logicalCursorPosition, out door.logicalPosition);
            
            DrawDoorOutline(room, door);

            if (oldLogicalPosition != door.logicalPosition)
            {
                RequestRebuild(room);
            }
        }

        public void DrawDoorOutline(DungeonRoomDesigner room, DungeonRoomDoorDesigner door)
        {
            // Draw the door outline
            var offset = room.gridSize.x / 2.0f;
            var doorHeight = room.gridSize.y * 2;
            var points = new Vector3[]
            {
                new Vector3(-offset, 0, 0),
                new Vector3( offset, 0, 0),
                new Vector3( offset, doorHeight, 0),
                new Vector3(-offset, doorHeight, 0)
            };

            var doorWorldPos = Vector3.Scale(room.roomPosition.ToVector3() + door.logicalPosition, room.gridSize);
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = doorWorldPos + door.rotation * points[i];
            }

            var indices = new int[] { 0, 1, 1, 2, 2, 3, 3, 0 };
            Handles.DrawDottedLines(points, indices, 4);
        }

        public override void DrawSceneGUI(DungeonRoomDesigner room)
        {
            for (int i = 0; i < room.doors.Length; i++)
            {
                DrawDoorEditor(room, ref room.doors[i]);
            }
        }
    }
}
