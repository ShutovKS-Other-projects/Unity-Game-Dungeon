//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Builders.GridFlow;
using DungeonArchitect.Themeing;
using UnityEngine;

namespace DungeonArchitect.Editors.Visualization.Implementation
{
    public class GridFlowThemeMarkerVisualizationBuilder : ThemeMarkerVisualizationBuilderBase, IThemeMarkerVisualizationBuilder
    {
        public bool Build(Dungeon dungeon, string markerName, out ThemeEditorVisMarkerGeometry localGeometry, out Material material)
        {
            localGeometry = null;
            material = null;
            if (dungeon == null)
            {
                return false;
            }

            var builder = dungeon.GetComponent<GridFlowDungeonBuilder>();
            var config = dungeon.GetComponent<GridFlowDungeonConfig>();
            var model = dungeon.GetComponent<GridFlowDungeonModel>();
            
            if (builder == null || config == null || model == null)
            {
                return false;
            }
            
            var gridSize = config.gridSize;
            var wallsAsEdges = model.wallsAsEdges;
            
            if (markerName == GridFlowDungeonMarkerNames.Ground) 
            {
                CreateGroundGeometry(gridSize.x, gridSize.z, out localGeometry, out material);
                return true;
            }
            
            if (markerName == GridFlowDungeonMarkerNames.Wall || 
                markerName == GridFlowDungeonMarkerNames.Door || 
                markerName == GridFlowDungeonMarkerNames.DoorOneWay) 
            {
                if (wallsAsEdges)
                {
                    CreateWallGeometry(gridSize.x, 4, out localGeometry, out material);
                }
                else
                {
                    CreateGroundGeometry(gridSize.x, gridSize.z, out localGeometry, out material);
                }
                return true;
            }

            if (markerName == GridFlowDungeonMarkerNames.Fence) 
            {
                CreateWallGeometry(gridSize.x, 2, out localGeometry, out material);
                return true;
            }
            
            if (markerName == GridFlowDungeonMarkerNames.WallSeparator || 
                markerName == GridFlowDungeonMarkerNames.FenceSeparator)
            {
                var radius = gridSize.x * 0.1f;
                CreatePillarGeometry(radius, out localGeometry, out material);
                return true;
            }

            return false;
        }
    }
}
