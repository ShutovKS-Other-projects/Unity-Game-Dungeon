//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Builders.SimpleCity;
using DungeonArchitect.Themeing;
using UnityEngine;

namespace DungeonArchitect.Editors.Visualization.Implementation
{
    public class SimpleCityThemeMarkerVisualizationBuilder : ThemeMarkerVisualizationBuilderBase, IThemeMarkerVisualizationBuilder
    {
        public bool Build(Dungeon dungeon, string markerName, out ThemeEditorVisMarkerGeometry localGeometry, out Material material)
        {
            localGeometry = null;
            material = null;
            if (dungeon == null)
            {
                return false;
            }
            
            var builder = dungeon.GetComponent<SimpleCityDungeonBuilder>();
            var config = dungeon.GetComponent<SimpleCityDungeonConfig>();
            
            if (builder == null || config == null)
            {
                return false;
            }
            
            var gridSize = config.CellSize;
            
            if (markerName == SimpleCityDungeonMarkerNames.House ||
                markerName == SimpleCityDungeonMarkerNames.Park ||
                markerName == SimpleCityDungeonMarkerNames.Road_X ||
                markerName == SimpleCityDungeonMarkerNames.Road_T ||
                markerName == SimpleCityDungeonMarkerNames.Road_Corner ||
                markerName == SimpleCityDungeonMarkerNames.Road_S ||
                markerName == SimpleCityDungeonMarkerNames.Road_E ||
                markerName == SimpleCityDungeonMarkerNames.Road ||
                markerName == SimpleCityDungeonMarkerNames.CityWall ||
                markerName == SimpleCityDungeonMarkerNames.CityDoor ||
                markerName == SimpleCityDungeonMarkerNames.CityGround ||
                markerName == SimpleCityDungeonMarkerNames.CornerTower ||
                markerName == SimpleCityDungeonMarkerNames.CityWallPadding) 
            {
                CreateGroundGeometry(gridSize.x, gridSize.y, out localGeometry, out material);
                return true;
            }
            

            return false;
        }
    }
}
