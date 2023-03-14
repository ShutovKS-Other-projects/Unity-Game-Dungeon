//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System;
using System.Collections.Generic;
using DungeonArchitect.Utils;
using DungeonArchitect.Builders.Grid;
using DungeonArchitect.Builders.FloorPlan;
using DungeonArchitect.Builders.Isaac;
using DungeonArchitect.Builders.SimpleCity;
using DungeonArchitect.Builders.CircularCity;
using DungeonArchitect.Builders.Snap;
using DungeonArchitect.Builders.Mario;
using DungeonArchitect.Builders.Maze;
using DungeonArchitect.Builders.BSP;
using DungeonArchitect.Builders.GridFlow;
using DungeonArchitect.Builders.Infinity.Caves;
using DungeonArchitect.Builders.SnapGridFlow;

namespace DungeonArchitect.Builders
{
    public static class DungeonBuilderDefaultMarkers
    {
        static Dictionary<Type, string[]> DefaultMarkersByBuilder = new Dictionary<Type, string[]>();

        static DungeonBuilderDefaultMarkers()
        {
            DefaultMarkersByBuilder.Add(typeof(GridDungeonBuilder), new string[] {
	            GridDungeonMarkerNames.Ground,
	            GridDungeonMarkerNames.Wall,
	            GridDungeonMarkerNames.WallSeparator,
	            GridDungeonMarkerNames.Fence,
	            GridDungeonMarkerNames.FenceSeparator,
	            GridDungeonMarkerNames.Door,
	            GridDungeonMarkerNames.Stair,
	            GridDungeonMarkerNames.Stair2X,
	            GridDungeonMarkerNames.WallHalf,
	            GridDungeonMarkerNames.WallHalfSeparator
	        });

            DefaultMarkersByBuilder.Add(typeof(GridFlowDungeonBuilder), new string[] {
                GridFlowDungeonMarkerNames.Ground,
                GridFlowDungeonMarkerNames.Wall,
                GridFlowDungeonMarkerNames.WallSeparator,
                GridFlowDungeonMarkerNames.Fence,
                GridFlowDungeonMarkerNames.FenceSeparator,
                GridFlowDungeonMarkerNames.Door,
                GridFlowDungeonMarkerNames.DoorOneWay
            });

            DefaultMarkersByBuilder.Add(typeof(SimpleCityDungeonBuilder), new string[] {
	            SimpleCityDungeonMarkerNames.House,
	            SimpleCityDungeonMarkerNames.Park,
	            SimpleCityDungeonMarkerNames.Road_X,
	            SimpleCityDungeonMarkerNames.Road_T,
	            SimpleCityDungeonMarkerNames.Road_Corner,
	            SimpleCityDungeonMarkerNames.Road_S,
	            SimpleCityDungeonMarkerNames.Road_E,
	            SimpleCityDungeonMarkerNames.Road,

	            SimpleCityDungeonMarkerNames.CityWall,
	            SimpleCityDungeonMarkerNames.CityDoor,
	            SimpleCityDungeonMarkerNames.CityGround,
	            SimpleCityDungeonMarkerNames.CornerTower,
	            SimpleCityDungeonMarkerNames.CityWallPadding,
	        });

            DefaultMarkersByBuilder.Add(typeof(CircularCityDungeonBuilder), new string[] {
                CircularCityDungeonMarkerNames.House,
                CircularCityDungeonMarkerNames.WallMarkerName,
                CircularCityDungeonMarkerNames.DoorMarkerName,
                CircularCityDungeonMarkerNames.GroundMarkerName,
                CircularCityDungeonMarkerNames.CornerTowerMarkerName,
                CircularCityDungeonMarkerNames.WallPaddingMarkerName,
            });

            DefaultMarkersByBuilder.Add(typeof(FloorPlanBuilder), new string[] {
	            FloorPlanMarkerNames.Ground,
	            FloorPlanMarkerNames.Ceiling,
	            FloorPlanMarkerNames.Wall,
	            FloorPlanMarkerNames.Door,
	            FloorPlanMarkerNames.BuildingWall
	        });

			DefaultMarkersByBuilder.Add(typeof(IsaacDungeonBuilder), new string[] {
				IsaacDungeonMarkerNames.Ground,
				IsaacDungeonMarkerNames.Wall,
				IsaacDungeonMarkerNames.Door
			});

			DefaultMarkersByBuilder.Add(typeof(MarioDungeonBuilder), new string[] {
                MarioDungeonMarkerNames.Ground,
                MarioDungeonMarkerNames.WallFront,
                MarioDungeonMarkerNames.WallBack,
                MarioDungeonMarkerNames.WallSide,
                MarioDungeonMarkerNames.BackgroundGround,
                MarioDungeonMarkerNames.BackgroundCeiling,
                MarioDungeonMarkerNames.BackgroundWall,
                MarioDungeonMarkerNames.Stair,
                MarioDungeonMarkerNames.Corridor,
            });

            DefaultMarkersByBuilder.Add(typeof(MazeDungeonBuilder), new string[] {
                MazeDungeonMarkerNames.GroundBlock,
                MazeDungeonMarkerNames.WallBlock,
            });

            DefaultMarkersByBuilder.Add(typeof(BSPDungeonBuilder), new string[] {
                BSPDungeonMarkerNames.GroundRoom,
                BSPDungeonMarkerNames.GroundCorridor,
                BSPDungeonMarkerNames.Door,
                BSPDungeonMarkerNames.WallRoom,
                BSPDungeonMarkerNames.WallCorridor,
                BSPDungeonMarkerNames.WallSeparator,
            });

            DefaultMarkersByBuilder.Add(typeof(InfinityCaveChunkBuilder), new string[] {
                InfinityCaveChunkMarkerNames.GroundBlock,
                InfinityCaveChunkMarkerNames.WallBlock,
                InfinityCaveChunkMarkerNames.RockBlock
            });

            DefaultMarkersByBuilder.Add(typeof(SnapBuilder), new string[] {

            });

            DefaultMarkersByBuilder.Add(typeof(SnapGridFlowBuilder), new string[] {

            });
        }

        public static string[] GetDefaultMarkers(Type builderClass)
        {
            if (!DefaultMarkersByBuilder.ContainsKey(builderClass))
            {
                return new string[0];
            }

            return DefaultMarkersByBuilder[builderClass];
        }
    }
}
