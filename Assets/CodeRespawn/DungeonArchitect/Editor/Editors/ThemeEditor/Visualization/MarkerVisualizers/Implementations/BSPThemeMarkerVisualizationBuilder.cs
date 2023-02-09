//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
 using DungeonArchitect.Builders.BSP;
 using DungeonArchitect.Themeing;
 using UnityEngine;
 
 namespace DungeonArchitect.Editors.Visualization.Implementation
 {
     public class BSPThemeMarkerVisualizationBuilder : ThemeMarkerVisualizationBuilderBase, IThemeMarkerVisualizationBuilder
     {
         public bool Build(Dungeon dungeon, string markerName, out ThemeEditorVisMarkerGeometry localGeometry, out Material material)
         {
             localGeometry = null;
             material = null;
             if (dungeon == null)
             {
                 return false;
             }
 
             var builder = dungeon.GetComponent<BSPDungeonBuilder>();
             var config = dungeon.GetComponent<BSPDungeonConfig>();
             var model = dungeon.GetComponent<BSPDungeonModel>();
             
             if (builder == null || config == null || model == null)
             {
                 return false;
             }
             
             var gridSize = config.gridSize;
             
             if (markerName == BSPDungeonMarkerNames.GroundRoom || markerName == BSPDungeonMarkerNames.GroundCorridor) 
             {
                 CreateGroundGeometry(gridSize.x, gridSize.y, out localGeometry, out material);
                 return true;
             }
             
             if (markerName == BSPDungeonMarkerNames.Door ||
                 markerName == BSPDungeonMarkerNames.WallRoom ||
                 markerName == BSPDungeonMarkerNames.WallCorridor) 
             {
                 var size = gridSize.x;
                 CreateWallGeometry(size, size, out localGeometry, out material);
                 return true;
             }
             
             if (markerName == BSPDungeonMarkerNames.WallSeparator)
             {
                 var radius = gridSize.x * 0.1f;
                 CreatePillarGeometry(radius, out localGeometry, out material);
                 return true;
             }

 
             return false;
         }
     }
 }
