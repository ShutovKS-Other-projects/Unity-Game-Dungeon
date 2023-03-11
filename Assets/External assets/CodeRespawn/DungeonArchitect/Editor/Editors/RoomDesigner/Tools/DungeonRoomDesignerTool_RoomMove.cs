//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.RoomDesigner.Editors
{
    public class DungeonRoomDesignerTool_RoomMove : DungeonRoomDesignerTools
    {
        public override void DrawSceneGUI(DungeonRoomDesigner room)
        {
            var worldSize = GetWorldCoord(room.roomSize, room.gridSize);
            var worldPos = GetWorldCoord(room.roomPosition, room.gridSize);
            var center = worldPos + worldSize / 2.0f;

            var snap = room.gridSize;
            float size = HandleUtility.GetHandleSize(center) * 0.25f;
            Handles.color = Color.green;
            var fmh_18_60_638114807294330936 = Quaternion.identity; var newCenter = Handles.FreeMoveHandle(center, size, snap, Handles.SphereHandleCap);
            var diffF = newCenter - center;
            var diff = new IntVector(
                Mathf.RoundToInt(diffF.x / room.gridSize.x),
                Mathf.RoundToInt(diffF.y / room.gridSize.y),
                Mathf.RoundToInt(diffF.z / room.gridSize.z));

            Event e = Event.current;
            bool moveXZ = true;
            if (e.shift)
            {
                // Only move along Y if shift key is pressed
                moveXZ = false;
            }

            if (moveXZ)
            {
                diff.y = 0;
            }
            else
            {
                diff.x = 0;
                diff.z = 0;
            }

            room.roomPosition += diff;
            if (!diff.Equals(IntVector.Zero))
            {
                RequestRebuild(room);
            }
        }
    }
}
