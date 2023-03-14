//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;
using DungeonArchitect.Landscape;

namespace DungeonArchitect.Builders.SimpleCity
{

    /// <summary>
    /// The type of the texture defined in the landscape paint settings.  
    /// This determines how the specified texture would be painted in the modified terrain
    /// </summary>
    public enum SimpleCityLandscapeTextureType
    {
        Road,
        Park,
        CityWallPadding
    }

    /// <summary>
    /// Data-structure to hold the texture settings.  This contains enough information to paint the texture 
    /// on to the terrain
    /// </summary>
    [System.Serializable]
    public class SimpleCityLandscapeTexture
    {
        public SimpleCityLandscapeTextureType textureType;
        public TerrainLayer terrainLayer;
        public AnimationCurve curve;
    }

    [System.Serializable]
    public class SimpleCityFoliageEntry
    {
        public int grassIndex;
        public float density;
    }

    [System.Serializable]
    public class SimpleCityFoliageTheme
    {
        public SimpleCityLandscapeTextureType textureType = SimpleCityLandscapeTextureType.Park;
        public SimpleCityFoliageEntry[] foliageEntries;
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
        public SimpleCityLandscapeTexture[] textures;

        public SimpleCityFoliageTheme[] foliage;
        //SimpleCityFoliageTheme roadFoliage;
        //SimpleCityFoliageTheme openSpaceFoliage;

        public int roadBlurDistance = 6;
        public float corridorBlurThreshold = 0.5f;
        public float roomBlurThreshold = 0.5f;

        public float flatten = 1;

        public int blendingUnits = 6;
        public AnimationCurve smoothingCurve;

        protected override void BuildTerrain(DungeonModel model)
        {
            if (model is SimpleCityDungeonModel && terrain != null)
            {
                var cityModel = model as SimpleCityDungeonModel;
                SetupTextures();
                UpdateHeights(cityModel);
                UpdateTerrainTextures(cityModel);
            }
        }

        void UpdateHeights(SimpleCityDungeonModel model)
        {
            if (terrain == null || terrain.terrainData == null) return;

            var bounds = GetDungeonBounds(model, blendingUnits);
            var rasterizer = new LandscapeDataRasterizer(terrain, bounds);
            rasterizer.LoadData();
            var gridSize = model.Config.CellSize;

            var layoutBounds = GetDungeonBounds(model, 0);
            float y = transform.position.y;
            rasterizer.DrawCell(layoutBounds.x, layoutBounds.y, layoutBounds.width, layoutBounds.height, y, flatten);
            rasterizer.SmoothCell(layoutBounds.x, layoutBounds.y, layoutBounds.width - 1, layoutBounds.height - 1, y, blendingUnits, smoothingCurve, flatten);

            RemoveFoliageFromBaseLayout(model);

            rasterizer.SaveData();
        }

        void RemoveFoliageFromBaseLayout(SimpleCityDungeonModel model)
        {
            if (terrain == null || terrain.terrainData == null) return;
            var data = terrain.terrainData;

            var bounds = GetDungeonBounds(model, 0);
            int gx1, gy1, gx2, gy2;
            LandscapeDataRasterizer.WorldToTerrainCoord(terrain, bounds.x, bounds.y, out gx1, out gy1, RasterizerTextureSpace.DetailMap);
            LandscapeDataRasterizer.WorldToTerrainCoord(terrain, bounds.xMax, bounds.yMax, out gx2, out gy2, RasterizerTextureSpace.DetailMap);

            int sx = gx2 - gx1 + 1;
            int sy = gy2 - gy1 + 1;
            int[,] clearPatch = new int[sy, sx];
            for (int d = 0; d < data.detailPrototypes.Length; d++)
            {
                data.SetDetailLayer(gx1, gy1, d, clearPatch);
            }
        }

        protected override Rect GetDungeonBounds(DungeonModel model)
        {
            return GetDungeonBounds(model, blendingUnits);
        }

        private Rect GetDungeonBounds(DungeonModel model, int extraPadding)
        {
            var cityModel = model as SimpleCityDungeonModel;
            var cityConfig = cityModel.Config;

            Rect result = Rect.zero;

            if (cityModel && cityConfig)
            {
                int padding = cityConfig.cityWallPadding * 2 + extraPadding;
                var worldPadding2D = cityConfig.CellSize * padding;

                var worldSize2D = new Vector2();
                worldSize2D.x = cityModel.CityWidth * cityConfig.CellSize.x;
                worldSize2D.y = cityModel.CityHeight * cityConfig.CellSize.y;
                worldSize2D += worldPadding2D * 2;

                var basePosition3D = transform.position;
                var worldPosition2D = new Vector2(basePosition3D.x, basePosition3D.z);
                worldPosition2D -= worldPadding2D;

                result.position = worldPosition2D;
                result.size = worldSize2D;
            }

            return result;
        }

        void SetupTextures()
        {
            if (terrain == null || terrain.terrainData == null) return;
            var data = terrain.terrainData;

            // Add the specified terrain layers on the terrain data, if they have not been added already
            {
                var targetLayers = new List<TerrainLayer>(data.terrainLayers);
                foreach (var texture in textures)
                {
                    if (!targetLayers.Contains(texture.terrainLayer))
                    {
                        targetLayers.Add(texture.terrainLayer);
                    }
                }

                data.terrainLayers = targetLayers.ToArray();
            }
        }

        void UpdateTerrainTextures(SimpleCityDungeonModel model)
        {
            if (terrain == null || terrain.terrainData == null) return;

            var data = terrain.terrainData;
            var map = data.GetAlphamaps(0, 0, data.alphamapWidth, data.alphamapHeight);
            UpdateBaseTexture(model, map);

            data.SetAlphamaps(0, 0, map);
        }
        

        void UpdateBaseTexture(SimpleCityDungeonModel model, float[,,] map)
        {
            if (terrain == null || terrain.terrainData == null) return;
            var data = terrain.terrainData;
            
            var activeTextureTypes = new SimpleCityLandscapeTextureType[] {
                SimpleCityLandscapeTextureType.Park,
                SimpleCityLandscapeTextureType.Road,
                SimpleCityLandscapeTextureType.CityWallPadding,
            };

            var activeCellTypes = new SimpleCityCellType[] {
                SimpleCityCellType.Park,
                SimpleCityCellType.Road,
                SimpleCityCellType.CityWallPadding,
            };

            var dataMaps = new List<float[,]>();
            for (int i = 0; i < activeTextureTypes.Length; i++)
            {
                dataMaps.Add(new float[map.GetLength(0), map.GetLength(1)]);
            }
            
            var gridSize2D = model.Config.CellSize;
            var gridSize = new Vector3(gridSize2D.x, 0, gridSize2D.y);
            var cells = new List<SimpleCityCell>();
            foreach (var cell in model.Cells)
            {
                cells.Add(cell);
            }
            cells.AddRange(model.WallPaddingCells);

            var basePosition = transform.position;

            foreach (var cell in cells)
            {
                var locationGrid = cell.Position;
                var location = basePosition + locationGrid * gridSize - gridSize / 2.0f;
                var size = gridSize;
                int gx1, gy1, gx2, gy2;
                LandscapeDataRasterizer.WorldToTerrainTextureCoord(terrain, location.x, location.z, out gx1, out gy1);
                LandscapeDataRasterizer.WorldToTerrainTextureCoord(terrain, location.x + size.x, location.z + size.z, out gx2, out gy2);
                for (int i = 0; i < activeTextureTypes.Length; i++)
                {
                    //SimpleCityLandscapeTextureType activeTexType = activeTextureTypes[i];
                    SimpleCityCellType activeCellType = activeCellTypes[i];
                    //int textureIndex = GetTextureIndex(activeTexType);
                    var dataMap = dataMaps[i];

                    for (var gx = gx1; gx <= gx2; gx++)
                    {
                        for (var gy = gy1; gy <= gy2; gy++)
                        {
                            dataMap[gy, gx] = (cell.CellType == activeCellType) ? 1 : 0;
                        }
                    }
                }
            }

            // Blur the layout data
            var filter = new BlurFilter(roadBlurDistance);
            for (int i = 0; i < dataMaps.Count; i++) 
            {
                dataMaps[i] = filter.ApplyFilter(dataMaps[i]);
            }

            int numMaps = map.GetLength(2);

            for (int i = 0; i < dataMaps.Count; i++)
            {
                var dataMap = dataMaps[i];
                int textureIndex = GetTextureIndex(activeTextureTypes[i]);
                if (textureIndex < 0) continue;
                for (var y = 0; y < data.alphamapHeight; y++)
                {
                    for (var x = 0; x < data.alphamapWidth; x++)
                    {
                        float value = dataMap[y, x];
                        if (value > 0)
                        {
                            map[y, x, textureIndex] = value;
                            float remaining = 1 - dataMap[y, x];
                            float sum = 0;
                            for (int m = 0; m < numMaps; m++)
                            {
                                if (m != textureIndex)
                                {
                                    sum += map[y, x, m];
                                }
                            }
                            if (sum > 0)
                            {
                                for (int m = 0; m < numMaps; m++)
                                {
                                    if (m != textureIndex)
                                    {
                                        map[y, x, m] = map[y, x, m] / sum * remaining;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Normalize
            for (var y = 0; y < data.alphamapHeight; y++)
            {
                for (var x = 0; x < data.alphamapWidth; x++)
                {
                    // Apply the curves
                    for (int t = 0; t < textures.Length; t++)
                    {
                        var curve = textures[t].curve;
                        if (curve != null && curve.keys.Length > 0)
                        {
                            map[y, x, t] = curve.Evaluate(map[y, x, t]);
                        }
                    }

                    float sum = 0;
                    for (int t = 0; t < textures.Length; t++)
                    {
                        sum += map[y, x, t];
                    }

                    for (int t = 0; t < textures.Length; t++)
                    {
                        map[y, x, t] /= sum;
                    }
                }
            }

            for (int layer = 0; layer < data.detailPrototypes.Length; layer++)
            {
                var foliageMap = data.GetDetailLayer(0, 0, data.detailWidth, data.detailHeight, layer);

                for (int x = 0; x < data.detailWidth; x++)
                {
                    float nx = x / (float)(data.detailWidth - 1);
                    int sampleX = Mathf.RoundToInt(nx * (data.alphamapWidth - 1));
                    for (int y = 0; y < data.detailHeight; y++)
                    {
                        float ny = y / (float)(data.detailHeight - 1);
                        int sampleY = Mathf.RoundToInt(ny * (data.alphamapHeight - 1));

                        bool bIsValid = false;
                        float influence = 0;
                        foreach (var foliageTheme in foliage)
                        {
                            var textureIndex = GetTextureIndex(foliageTheme.textureType);
                            if (textureIndex < 0) continue;
                            float[,] paintMap = dataMaps[textureIndex];
                            bIsValid |= paintMap[sampleY, sampleX] > 0;

                            foreach (var entry in foliageTheme.foliageEntries)
                            {
                                if (entry.grassIndex == layer)
                                {
                                    
                                    float mapData = map[sampleY, sampleX, textureIndex];
                                    bIsValid |= mapData > 0;

                                    if (foliageTheme.curve != null && foliageTheme.curve.length > 0)
                                    {
                                        mapData = foliageTheme.curve.Evaluate(mapData);
                                    }
                                    float alpha = mapData * entry.density * foliageTheme.density;
                                    influence += alpha;
                                }
                            }
                        }

                        if (bIsValid)
                        {
                            int value = Mathf.FloorToInt(influence);
                            float frac = influence - value;
                            if (Random.value < frac) value++;
                            foliageMap[y, x] = value;
                        }
                    }
                }

                data.SetDetailLayer(0, 0, layer, foliageMap);
            }

            /*
            // Update foliage
            foreach (var foliageTheme in foliage)
            {
                var textureIndex = GetTextureIndex(foliageTheme.textureType);
                if (textureIndex < 0) continue;
                foreach (var entry in foliageTheme.foliageEntries)
                {
                    int layer = entry.grassIndex;
                    var foliageMap = data.GetDetailLayer(0, 0, data.detailWidth, data.detailHeight, layer);
                    for (int x = 0; x < data.detailWidth; x++)
                    {
                        float nx = x / (float)(data.detailWidth - 1);
                        int sampleX = Mathf.RoundToInt(nx * (data.alphamapWidth - 1));
                        for (int y = 0; y < data.detailHeight; y++)
                        {
                            float ny = y / (float)(data.detailHeight - 1);
                            int sampleY = Mathf.RoundToInt(ny * (data.alphamapHeight - 1));

                            float alpha = map[sampleY, sampleX, textureIndex] * entry.density * foliageTheme.density;
                            int value = Mathf.FloorToInt(alpha);
                            float frac = alpha - value;
                            if (Random.value < frac) value++;
                            foliageMap[y, x] = value;
                        }
                    }

                    data.SetDetailLayer(0, 0, layer, foliageMap);
                }
            }
            */
        }

        /// <summary>
        /// Returns the index of the landscape texture.  -1 if not found
        /// </summary>
        /// <returns>The texture index. -1 if not found</returns>
        /// <param name="textureType">Texture type.</param>
        int GetTextureIndex(SimpleCityLandscapeTextureType textureType)
        {
            if (terrain == null || terrain.terrainData == null) return -1;
            var data = terrain.terrainData;
            for (int i = 0; i < textures.Length; i++)
            {
                if (textures[i].textureType == textureType)
                {
                    return System.Array.IndexOf(data.terrainLayers, textures[i].terrainLayer);
                }
            }
            return -1;	// Doesn't exist
        }

    }
}
