//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.RoomDesigner.Editors
{
    public abstract class DungeonRoomDesignerTools
    {
        public abstract void DrawSceneGUI(DungeonRoomDesigner room);

        protected Vector3 GetWorldCoord(IntVector iv, Vector3 gridSize)
        {
            return Vector3.Scale(iv.ToVector3(), gridSize);
        }

        protected void RequestRebuild(DungeonRoomDesigner room)
        {
            if (room.realtimeUpdate && room.dungeon != null)
            {
                room.dungeon.Build();
            }
        }

    }
}

