//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect.RoadNetworks;

namespace DungeonArchitect.Builders.CircularCity
{

    public enum CircularCityCellType
    {
        Road,
        House,
        Park,
        CityWallPadding,
        UserDefined,
        Empty
    }

    public class CircularCityCell
    {
        public IntVector Position;
        public CircularCityCellType CellType;
        public Quaternion Rotation;
        public Vector3 BlockSize = new Vector3(1, 0, 1);
        public string MarkerNameOverride;
    }

    public class CircularCityDungeonModel : DungeonModel
    {
        [HideInInspector]
        public CircularCityDungeonConfig Config;

        //[HideInInspector]
        public RoadGraph roadGraph;

        //[HideInInspector]
        public RoadGraph layoutGraph;
    }

}