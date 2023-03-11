//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Builders.Mario;
using DungeonArchitect.Themeing;
using UnityEngine;

namespace DungeonArchitect.Editors.Visualization.Implementation
{
    public class MarioThemeMarkerVisualizationBuilder : ThemeMarkerVisualizationBuilderBase, IThemeMarkerVisualizationBuilder
    {
        public bool Build(Dungeon dungeon, string markerName, out ThemeEditorVisMarkerGeometry localGeometry, out Material material)
        {
            localGeometry = null;
            material = null;
            if (dungeon == null)
            {
                return false;
            }

            var builder = dungeon.GetComponent<MarioDungeonBuilder>();
            var config = dungeon.GetComponent<MarioDungeonConfig>();
            var model = dungeon.GetComponent<MarioDungeonModel>();

            if (builder == null || config == null || model == null)
            {
                return false;
            }

            var gridSize = config.gridSize;

            if (markerName == MarioDungeonMarkerNames.Ground ||
                markerName == MarioDungeonMarkerNames.BackgroundGround ||
                markerName == MarioDungeonMarkerNames.BackgroundCeiling ||
                markerName == MarioDungeonMarkerNames.Corridor)
            {
                CreateGroundGeometry(gridSize.x, gridSize.z, out localGeometry, out material);
                return true;
            }

            if (markerName == MarioDungeonMarkerNames.WallFront ||
                markerName == MarioDungeonMarkerNames.WallBack ||
                markerName == MarioDungeonMarkerNames.WallSide ||
                markerName == MarioDungeonMarkerNames.BackgroundWall)
            {
                CreateWallGeometry(gridSize.x, gridSize.y, out localGeometry, out material);
                return true;
            }

            if (markerName == MarioDungeonMarkerNames.Stair)
            {
                CreateGroundGeometry(gridSize.x, gridSize.z, out localGeometry, out material);
                localGeometry.Vertices[1].y = gridSize.y;
                localGeometry.Vertices[2].y = gridSize.y;
                return true;
                
            }
            return false;
        }
    }
}