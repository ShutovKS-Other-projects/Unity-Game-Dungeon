//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Builders.FloorPlan;
using DungeonArchitect.Themeing;
using UnityEngine;

namespace DungeonArchitect.Editors.Visualization.Implementation
{
    public class FloorPlanThemeMarkerVisualizationBuilder : ThemeMarkerVisualizationBuilderBase, IThemeMarkerVisualizationBuilder
    {
        public bool Build(Dungeon dungeon, string markerName, out ThemeEditorVisMarkerGeometry localGeometry, out Material material)
        {
            localGeometry = null;
            material = null;
            if (dungeon == null)
            {
                return false;
            }

            var builder = dungeon.GetComponent<FloorPlanBuilder>();
            var config = dungeon.GetComponent<FloorPlanConfig>();
            var model = dungeon.GetComponent<FloorPlanModel>();
            
            if (builder == null || config == null || model == null)
            {
                return false;
            }
            
            var gridSize = config.GridSize;
            
            if (markerName == FloorPlanMarkerNames.Ground || markerName == FloorPlanMarkerNames.Ceiling) 
            {
                CreateGroundGeometry(gridSize.x, gridSize.z, out localGeometry, out material);
                return true;
            }
            
            if (markerName == FloorPlanMarkerNames.Wall ||
                markerName == FloorPlanMarkerNames.Door ||
                markerName == FloorPlanMarkerNames.BuildingWall) 
            {
                CreateWallGeometry(gridSize.x, 4, out localGeometry, out material);
                return true;
            }

            return false;
        }
    }
}
