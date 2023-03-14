//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect.Landscape;

namespace DungeonArchitect.Builders.CircularCity
{

    /// <summary>
    /// The type of the texture defined in the landscape paint settings.  
    /// This determines how the specified texture would be painted in the modified terrain
    /// </summary>
    public enum CircularCityLandscapeTextureType
    {
        Fill,
        Road,
        Park,
        CityWallPadding
    }

    /// <summary>
    /// Data-structure to hold the texture settings.  This contains enough information to paint the texture 
    /// on to the terrain
    /// </summary>
    [System.Serializable]
    public class CircularCityLandscapeTexture
    {
        public CircularCityLandscapeTextureType textureType;
        public Texture2D diffuse;
        public Texture2D normal;
        public float metallic = 0;
        public Vector2 size = new Vector2(15, 15);
        public Vector2 offset = Vector2.zero;
        public AnimationCurve curve;
    }

    [System.Serializable]
    public class CircularCityFoliageEntry
    {
        public int grassIndex;
        public float density;
    }

    [System.Serializable]
    public class CircularCityFoliageTheme
    {
        public CircularCityLandscapeTextureType textureType = CircularCityLandscapeTextureType.Park;
        public CircularCityFoliageEntry[] foliageEntries;
        public AnimationCurve curve;
        public float density;
    }


    /// <summary>
    /// The terrain modifier that works with the grid based dungeon builder (DungeonBuilderGrid)
    /// It modifies the terrain by adjusting the height around the layout of the dungeon and painting 
    /// it based on the specified texture settings 
    /// </summary>
    public class LandscapeTransformerCity : LandscapeTransformerBase
    {
        public CircularCityLandscapeTexture[] textures;

        public CircularCityFoliageTheme[] foliage;
        //CircularCityFoliageTheme roadFoliage;
        //CircularCityFoliageTheme openSpaceFoliage;

        public int roadBlurDistance = 6;
        public float corridorBlurThreshold = 0.5f;
        public float roomBlurThreshold = 0.5f;


        protected override void BuildTerrain(DungeonModel model)
        {
            if (model is CircularCityDungeonModel && terrain != null)
            {
                //var cityModel = model as CircularCityDungeonModel;
                //SetupTextures();
                //UpdateTerrainTextures(cityModel);
            }
        }
    }
}
