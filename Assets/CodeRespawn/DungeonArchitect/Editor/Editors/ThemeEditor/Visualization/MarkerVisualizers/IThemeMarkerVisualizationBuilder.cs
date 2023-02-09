//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Builders.BSP;
using DungeonArchitect.Builders.FloorPlan;
using DungeonArchitect.Builders.Grid;
using DungeonArchitect.Builders.GridFlow;
using DungeonArchitect.Builders.Isaac;
using DungeonArchitect.Builders.Mario;
using DungeonArchitect.Builders.Maze;
using DungeonArchitect.Builders.SimpleCity;
using DungeonArchitect.Editors.Visualization.Implementation;
using DungeonArchitect.Themeing;
using UnityEngine;

namespace DungeonArchitect.Editors.Visualization
{
    public interface IThemeMarkerVisualizationBuilder
    {
        bool Build(Dungeon dungeon, string markerName, out ThemeEditorVisMarkerGeometry localGeometry, out Material material);
    }

    public class ThemeMarkerVisualizationBuilderBase
    {
        protected void CreateGroundGeometry(float width, float length, out ThemeEditorVisMarkerGeometry localGeometry, out Material material)
        {
            localGeometry = ThemeMarkerVisualizationBuilderUtils.CreateGroundQuad(width, length);
            material = ThemeMarkerVisualizationBuilderUtils.GetGroundMaterial();
        }

        protected void CreateWallGeometry(float width, float height, out ThemeEditorVisMarkerGeometry localGeometry, out Material material)
        {
            localGeometry = ThemeMarkerVisualizationBuilderUtils.CreateWallQuad(width, height);
            material = ThemeMarkerVisualizationBuilderUtils.GetWallMaterial();
        }
        
        protected void CreatePillarGeometry(float radius, out ThemeEditorVisMarkerGeometry localGeometry, out Material material)
        {
            localGeometry = ThemeMarkerVisualizationBuilderUtils.CreateGroundQuad(radius * 2, radius * 2);
            material = ThemeMarkerVisualizationBuilderUtils.GetPillarMaterial();
        }

        protected void ApplyOffset(Vector3 offset, ThemeEditorVisMarkerGeometry localGeometry)
        {
            for (var i = 0; i < localGeometry.Vertices.Length; i++)
            {
                localGeometry.Vertices[i] = localGeometry.Vertices[i] + offset;
            }
        }
    }
    
    public static class ThemeMarkerVisualizationBuilderFactory
    {
        public static IThemeMarkerVisualizationBuilder Create(System.Type builderType)
        {
            if (builderType == typeof(GridDungeonBuilder))
            {
                return new GridThemeMarkerVisualizationBuilder();
            }
            if (builderType == typeof(GridFlowDungeonBuilder))
            {
                return new GridFlowThemeMarkerVisualizationBuilder();
            }
            if (builderType == typeof(SimpleCityDungeonBuilder))
            {
                return new SimpleCityThemeMarkerVisualizationBuilder();
            }
            if (builderType == typeof(FloorPlanBuilder))
            {
                return new FloorPlanThemeMarkerVisualizationBuilder();
            }
            if (builderType == typeof(BSPDungeonBuilder))
            {
                return new BSPThemeMarkerVisualizationBuilder();
            }
            if (builderType == typeof(MazeDungeonBuilder))
            {
                return new MazeThemeMarkerVisualizationBuilder();
            }
            if (builderType == typeof(MarioDungeonBuilder))
            {
                return new MarioThemeMarkerVisualizationBuilder();
            }
            if (builderType == typeof(IsaacDungeonBuilder))
            {
                return new IsaacThemeMarkerVisualizationBuilder();
            }
            
            return null;
        }
    }

    public static class ThemeMarkerVisualizationBuilderUtils
    {
        public static ThemeEditorVisMarkerGeometry CreateGroundQuad(float width, float length)
        {
            var geometry = CreateGroundQuad();
            var scale = new Vector3(width, 0, length) * 0.5f;
            for (var i = 0; i < geometry.Vertices.Length; i++)
            {
                geometry.Vertices[i] = Vector3.Scale(geometry.Vertices[i], scale);
            }

            return geometry;
        }

        public static ThemeEditorVisMarkerGeometry CreateWallQuad(float width, float height)
        {
            var geometry = CreateWallQuad();
            var scale = new Vector3(width * 0.5f, height, 0);
            for (var i = 0; i < geometry.Vertices.Length; i++)
            {
                geometry.Vertices[i] = Vector3.Scale(geometry.Vertices[i], scale);
            }

            return geometry;
        }
        
        public static Material GetGroundMaterial()
        {
            return Resources.Load<Material>("MarkerVisualizer/Materials/MatMarkerVisualizerBox");
        }
        
        public static Material GetWallMaterial()
        {
            return Resources.Load<Material>("MarkerVisualizer/Materials/MatMarkerVisualizerBox");
        }
        
        public static Material GetPillarMaterial()
        {
            return Resources.Load<Material>("MarkerVisualizer/Materials/MatMarkerVisualizerCircle");
        }

        static ThemeEditorVisMarkerGeometry CreateGroundQuad()
        {
            var geometry = new ThemeEditorVisMarkerGeometry();
            geometry.Vertices = new []
            {
                new Vector3(-1, 0, -1),
                new Vector3(1, 0, -1),
                new Vector3(1, 0, 1),
                new Vector3(-1, 0, 1)
            };

            geometry.UV = new[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1)
            };

            geometry.Indices = new[]
            {
                0, 2, 1,
                0, 3, 2
            };
            return geometry;
        }
        
        static ThemeEditorVisMarkerGeometry CreateWallQuad()
        {
            var geometry = new ThemeEditorVisMarkerGeometry();
            geometry.Vertices = new []
            {
                new Vector3(-1, 0, 0),
                new Vector3(1, 0, 0),
                new Vector3(1, 1, 0),
                new Vector3(-1, 1, 0)
            };

            geometry.UV = new[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1)
            };

            geometry.Indices = new[]
            {
                0, 2, 1,
                0, 3, 2
            };
            return geometry;
        }
    }
}