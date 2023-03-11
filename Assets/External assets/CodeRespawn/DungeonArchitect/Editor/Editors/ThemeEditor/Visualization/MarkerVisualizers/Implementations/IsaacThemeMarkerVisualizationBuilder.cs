//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Builders.Isaac;
using DungeonArchitect.Themeing;
using UnityEngine;
 
 namespace DungeonArchitect.Editors.Visualization.Implementation
 {
     public class IsaacThemeMarkerVisualizationBuilder : ThemeMarkerVisualizationBuilderBase, IThemeMarkerVisualizationBuilder
     {
         public bool Build(Dungeon dungeon, string markerName, out ThemeEditorVisMarkerGeometry localGeometry, out Material material)
         {
             localGeometry = null;
             material = null;
             if (dungeon == null)
             {
                 return false;
             }
 
             var builder = dungeon.GetComponent<IsaacDungeonBuilder>();
             var config = dungeon.GetComponent<IsaacDungeonConfig>();
             var model = dungeon.GetComponent<IsaacDungeonModel>();
             
             if (builder == null || config == null || model == null)
             {
                 return false;
             }
             
             var gridSize = config.tileSize;
             
             if (markerName == IsaacDungeonMarkerNames.Ground ||
                 markerName == IsaacDungeonMarkerNames.Door ||
                 markerName == IsaacDungeonMarkerNames.Wall) 
             {
                 CreateGroundGeometry(gridSize.x, gridSize.y, out localGeometry, out material);
                 return true;
             }
             
             return false;
         }
     }
 }
