//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
 using DungeonArchitect.Builders.Maze;
 using DungeonArchitect.Themeing;
 using UnityEngine;
 
 namespace DungeonArchitect.Editors.Visualization.Implementation
 {
     public class MazeThemeMarkerVisualizationBuilder : ThemeMarkerVisualizationBuilderBase, IThemeMarkerVisualizationBuilder
     {
         public bool Build(Dungeon dungeon, string markerName, out ThemeEditorVisMarkerGeometry localGeometry, out Material material)
         {
             localGeometry = null;
             material = null;
             if (dungeon == null)
             {
                 return false;
             }
 
             var builder = dungeon.GetComponent<MazeDungeonBuilder>();
             var config = dungeon.GetComponent<MazeDungeonConfig>();
             var model = dungeon.GetComponent<MazeDungeonModel>();
             
             if (builder == null || config == null || model == null)
             {
                 return false;
             }
             
             var gridSize = config.gridSize;
             
             if (markerName == MazeDungeonMarkerNames.GroundBlock || markerName == MazeDungeonMarkerNames.WallBlock) 
             {
                 CreateGroundGeometry(gridSize.x, gridSize.y, out localGeometry, out material);
                 return true;
             }
             
             return false;
         }
     }
 }
