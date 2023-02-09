//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Builders.Grid;
using DungeonArchitect.Themeing;
using UnityEngine;

namespace DungeonArchitect.Editors.Visualization.Implementation
{
    public class GridThemeMarkerVisualizationBuilder : ThemeMarkerVisualizationBuilderBase, IThemeMarkerVisualizationBuilder
    {
        public bool Build(Dungeon dungeon, string markerName, out ThemeEditorVisMarkerGeometry localGeometry, out Material material) 
        {
            localGeometry = null;
            material = null;
            if (dungeon == null)
            {
                return false;
            }

            var builder = dungeon.GetComponent<GridDungeonBuilder>();
            var config = dungeon.GetComponent<GridDungeonConfig>();
            if (builder == null || config == null)
            {
                return false;
            }
            
            var gridSize = config.GridCellSize;

            if (markerName == GridDungeonMarkerNames.Ground) 
            {
                CreateGroundGeometry(gridSize.x, gridSize.z, out localGeometry, out material);
                return true;
            }
            
            if (markerName == GridDungeonMarkerNames.Wall || markerName == GridDungeonMarkerNames.Door) 
            {
                CreateWallGeometry(gridSize.x, 4, out localGeometry, out material);
                return true;
            }

            if (markerName == GridDungeonMarkerNames.Fence) 
            {
                CreateWallGeometry(gridSize.x, 2, out localGeometry, out material);
                return true;
            }
            
            if (markerName == GridDungeonMarkerNames.WallHalf) 
            {
                CreateWallGeometry(gridSize.x, gridSize.y, out localGeometry, out material);
                ApplyOffset(new Vector3(0, -gridSize.y, 0), localGeometry);
                return true;
            }
            
            if (markerName == GridDungeonMarkerNames.Stair) 
            {
                CreateGroundGeometry(gridSize.x, gridSize.z, out localGeometry, out material);
                localGeometry.Vertices[1].y = gridSize.y;
                localGeometry.Vertices[2].y = gridSize.y;
                return true;
            }

            if (markerName == GridDungeonMarkerNames.Stair2X) 
            {
                CreateGroundGeometry(gridSize.x, gridSize.z, out localGeometry, out material);
                localGeometry.Vertices[1].y = gridSize.y * 2;
                localGeometry.Vertices[2].y = gridSize.y * 2;
                return true;
            }

            if (markerName == GridDungeonMarkerNames.WallSeparator || 
                markerName == GridDungeonMarkerNames.FenceSeparator || 
                markerName == GridDungeonMarkerNames.RoomWallSeparator)
            {
                var radius = gridSize.x * 0.1f;
                CreatePillarGeometry(radius, out localGeometry, out material);
                return true;
            }
            
            if (markerName == GridDungeonMarkerNames.WallHalfSeparator) 
            {
                var radius = gridSize.x * 0.1f;
                CreatePillarGeometry(radius, out localGeometry, out material);
                ApplyOffset(new Vector3(0, -gridSize.y, 0), localGeometry);
                return true;
            }

            return false;
        }
    }
}