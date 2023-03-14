//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace DungeonArchitect.RoomDesigner.Editors
{
    public class DungeonRoomDesignerTool_RoomBounds : DungeonRoomDesignerTools
    {
        private BoxBoundsHandle boundsHandle = new BoxBoundsHandle();

        public override void DrawSceneGUI(DungeonRoomDesigner room)
        {
            DrawBoundsHandle(room, PrimitiveBoundsHandle.Axes.All);
        }

        void DrawBoundsHandle(DungeonRoomDesigner room, PrimitiveBoundsHandle.Axes drawAxes)
        {
            var worldSize = GetWorldCoord(room.roomSize, room.gridSize);
            var worldPos = GetWorldCoord(room.roomPosition, room.gridSize);
            var center = worldPos + worldSize / 2.0f;

            boundsHandle.center = center;
            boundsHandle.size = worldSize;
            boundsHandle.axes = drawAxes;
            boundsHandle.midpointHandleDrawFunction = Handles.CircleHandleCap;
            boundsHandle.midpointHandleSizeFunction = HandleSizeFunction;
            boundsHandle.wireframeColor = Color.white;
            boundsHandle.handleColor = Color.white;
            EditorGUI.BeginChangeCheck();
            boundsHandle.DrawHandle();
            if (EditorGUI.EndChangeCheck())
            {
                center = boundsHandle.center;
                var minF = center - boundsHandle.size / 2.0f;
                var maxF = center + boundsHandle.size / 2.0f;
                var g = room.gridSize;
                var min = new IntVector();
                var max = new IntVector();
                min.x = Mathf.RoundToInt(minF.x / g.x);
                max.x = Mathf.RoundToInt(maxF.x / g.x);
                min.y = Mathf.RoundToInt(minF.y / g.y);
                max.y = Mathf.RoundToInt(maxF.y / g.y);
                min.z = Mathf.RoundToInt(minF.z / g.z);
                max.z = Mathf.RoundToInt(maxF.z / g.z);

                var targetPosition = min;
                var targetSize = max - min;

                targetSize.x = Mathf.Max(1, targetSize.x);
                targetSize.y = Mathf.Max(1, targetSize.y);
                targetSize.z = Mathf.Max(1, targetSize.z);

                if (!room.roomPosition.Equals(targetPosition) || !room.roomSize.Equals(targetSize))
                {
                    room.roomPosition = targetPosition;
                    room.roomSize = targetSize;

                    RequestRebuild(room);
                }
            }
        }

        float HandleSizeFunction(Vector3 position)
        {
            return HandleUtility.GetHandleSize(position) / 6.0f;
        }

    }
}
