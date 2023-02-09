//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect.Builders.FloorPlan.Tooling;

namespace DungeonArchitect.Builders.FloorPlan
{
    public class FloorPlanConfig : DungeonConfig
    {

        public Vector3 BuildingSize;

        public Vector3 GridSize = new Vector3(4, 2, 4);

        public int MinRoomSize = 2;

        public int MaxRoomSize = 3;

        public int HallWidth = 1;

        public int MinRoomChunkArea = 24;

        public int RoomSplitProbabilityOffset;

        public FloorPlanCorridorTool[] manualHallwayPoints;
    }
}

