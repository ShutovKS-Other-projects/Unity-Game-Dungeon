//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Landscape
{
    [System.Serializable]
    public class DLCacheBounds
    {
        [HideInInspector]
        [SerializeField]
        public int x1;

        [HideInInspector]
        [SerializeField]
        public int x2;

        [HideInInspector]
        [SerializeField]
        public int y1;

        [HideInInspector]
        [SerializeField]
        public int y2;
    }

    /// <summary>
    /// Before building a dungeon, we find the bounds of the dungeon and save
    /// the heightmap data within that bounds before we modify the landscape
    /// This way, the next rebuild of dungeon with another layout would first 
    /// restore this data (thereby removing the older layout that was baked in the landscape)
    /// and apply the new layout on the landscape
    /// </summary>
    [System.Serializable]
    public class DungeonLandscapeRestorationCache : ScriptableObject
    {
        [HideInInspector]
        [SerializeField]
        float[] dataHeights;

        [HideInInspector]
        [SerializeField]
        DLCacheBounds boundsHeight = new DLCacheBounds();

        [HideInInspector]
        [SerializeField]
        float[] dataAlphamap;

        [HideInInspector]
        [SerializeField]
        int numAlphaMaps = 0;

        [HideInInspector]
        [SerializeField]
        DLCacheBounds boundsAlpha = new DLCacheBounds();

        [HideInInspector]
        [SerializeField]
        int[] dataDetails;

        [HideInInspector]
        [SerializeField]
        int numDetailMaps = 0;

        [HideInInspector]
        [SerializeField]
        DLCacheBounds boundsDetails = new DLCacheBounds();

        static void SerializeArray(float[,] array, out float[] result)
        {
            int sx = array.GetLength(1);
            int sy = array.GetLength(0);

            result = new float[sx * sy];
            for (int y = 0; y < sy; y++)
            {
                for (int x = 0; x < sx; x++)
                {
                    int idx = y * sx + x;
                    result[idx] = array[y, x];
                }
            }
        }

        static void SerializeArray(float[,,] array, int numAlphaMaps, out float[] result)
        {
            int sx = array.GetLength(1);
            int sy = array.GetLength(0);
            int sm = numAlphaMaps;

            result = new float[sx * sy * sm];
            for (int m = 0; m < sm; m++)
            {
                for (int y = 0; y < sy; y++)
                {
                    for (int x = 0; x < sx; x++)
                    {
                        int idx = (sx * sy) * m + sx * y + x;
                        result[idx] = array[y, x, m];
                    }
                }
            }
        }

        static void DeserializeArray(float[] array, int sx, int sy, out float[,] result)
        {
            result = new float[sy, sx];
            if (array.Length != sx * sy)
            {
                Debug.LogError("Invalid array deserialization");
                return;
            }

            for (int y = 0; y < sy; y++)
            {
                for (int x = 0; x < sx; x++)
                {
                    int idx = y * sx + x;
                    result[y, x] = array[idx];
                }
            }
        }

        static void DeserializeArray(float[] array, int sx, int sy, int sm, int desiredSM, out float[,,] result)
        {
            if (array.Length != sx * sy * sm)
            {
                Debug.LogError("Invalid array deserialization");
                result = new float[0, 0, 0];
                return;
            }

            desiredSM = Mathf.Max(sm, desiredSM);
            result = new float[sy, sx, desiredSM];

            for (int m = 0; m < sm; m++)
            {
                for (int y = 0; y < sy; y++)
                {
                    for (int x = 0; x < sx; x++)
                    {
                        int idx = (sx * sy) * m + sx * y + x;
                        result[y, x, m] = array[idx];
                    }
                }
            }
        }

        public void SaveLandscapeData(Terrain terrain, Rect worldBounds)
        {
            var data = terrain.terrainData;

            // Save the heights
            {
                LandscapeDataRasterizer.WorldToTerrainCoord(terrain, worldBounds.x, worldBounds.y, out boundsHeight.x1, out boundsHeight.y1);
                LandscapeDataRasterizer.WorldToTerrainCoord(terrain, worldBounds.x + worldBounds.width, worldBounds.y + worldBounds.height, out boundsHeight.x2, out boundsHeight.y2);

                int cacheWidth = boundsHeight.x2 - boundsHeight.x1;
                int cacheHeight = boundsHeight.y2 - boundsHeight.y1;
                float[,] heights = data.GetHeights(boundsHeight.x1, boundsHeight.y1, cacheWidth, cacheHeight);
                SerializeArray(heights, out dataHeights);
            }

            // Save the alpha maps
            {
                LandscapeDataRasterizer.WorldToTerrainCoord(terrain, worldBounds.x, worldBounds.y, out boundsAlpha.x1, out boundsAlpha.y1, RasterizerTextureSpace.AlphaMap);
                LandscapeDataRasterizer.WorldToTerrainCoord(terrain, worldBounds.x + worldBounds.width, worldBounds.y + worldBounds.height, out boundsAlpha.x2, out boundsAlpha.y2, RasterizerTextureSpace.AlphaMap);

                int cacheWidth = boundsAlpha.x2 - boundsAlpha.x1;
                int cacheHeight = boundsAlpha.y2 - boundsAlpha.y1;
                float[,,] maps = data.GetAlphamaps(boundsAlpha.x1, boundsAlpha.y1, cacheWidth, cacheHeight);
                numAlphaMaps = data.terrainLayers.Length;

                SerializeArray(maps, numAlphaMaps, out dataAlphamap);
            }

            // Save the details map (foliage)
            {
                LandscapeDataRasterizer.WorldToTerrainCoord(terrain, worldBounds.x, worldBounds.y, out boundsDetails.x1, out boundsDetails.y1, RasterizerTextureSpace.DetailMap);
                LandscapeDataRasterizer.WorldToTerrainCoord(terrain, worldBounds.x + worldBounds.width, worldBounds.y + worldBounds.height, out boundsDetails.x2, out boundsDetails.y2, RasterizerTextureSpace.DetailMap);

                int cacheWidth = boundsDetails.x2 - boundsDetails.x1;
                int cacheHeight = boundsDetails.y2 - boundsDetails.y1;

                numDetailMaps = data.detailPrototypes.Length;
                int arraySize = numDetailMaps * cacheWidth * cacheHeight;
                dataDetails = new int[arraySize];

                int index = 0;
                for (int d = 0; d < numDetailMaps; d++)
                {
                    int[,] map = data.GetDetailLayer(boundsDetails.x1, boundsDetails.y1, cacheWidth, cacheHeight, d);
                    for (int y = 0; y < cacheHeight; y++)
                    {
                        for (int x = 0; x < cacheWidth; x++)
                        {
                            dataDetails[index] = map[y, x];
                            index++;
                        }
                    }
                }
            }
        }

        public void RestoreLandscapeData(Terrain terrain, Rect worldBounds)
        {
            if (terrain == null || terrain.terrainData == null) return;

            var data = terrain.terrainData;

            // Restore the heightmap
            if (dataHeights != null)
            {
                float[,] heights;
                DeserializeArray(dataHeights, boundsHeight.x2 - boundsHeight.x1, boundsHeight.y2 - boundsHeight.y1, out heights);
                data.SetHeights(boundsHeight.x1, boundsHeight.y1, heights);
            }

            // Restore the alpha maps (paint data)
            if (dataAlphamap != null)
            {
                int w = boundsAlpha.x2 - boundsAlpha.x1;
                int h = boundsAlpha.y2 - boundsAlpha.y1;
                int numLayers = terrain.terrainData.terrainLayers.Length;

                float[,,] maps;
                DeserializeArray(dataAlphamap, w, h, numAlphaMaps, numLayers, out maps);
                if (data.alphamapLayers == maps.GetLength(2))
                {
                    data.SetAlphamaps(boundsAlpha.x1, boundsAlpha.y1, maps);
                }
                else
                {
                    Debug.LogError("Cannot restore old landscape layout. The landscape settings have since been changed");
                }
            }

            // Restore the details map (foliage)
            {
                LandscapeDataRasterizer.WorldToTerrainCoord(terrain, worldBounds.x, worldBounds.y, out boundsDetails.x1, out boundsDetails.y1, RasterizerTextureSpace.DetailMap);
                LandscapeDataRasterizer.WorldToTerrainCoord(terrain, worldBounds.x + worldBounds.width, worldBounds.y + worldBounds.height, out boundsDetails.x2, out boundsDetails.y2, RasterizerTextureSpace.DetailMap);

                int cacheWidth = boundsDetails.x2 - boundsDetails.x1;
                int cacheHeight = boundsDetails.y2 - boundsDetails.y1;

                int index = 0;
                for (int d = 0; d < numDetailMaps; d++)
                {
                    int[,] map = new int[cacheHeight, cacheWidth];
                    for (int y = 0; y < cacheHeight; y++)
                    {
                        for (int x = 0; x < cacheWidth; x++)
                        {
                            map[y, x] = dataDetails[index];
                            index++;
                        }
                    }
                    data.SetDetailLayer(boundsDetails.x1, boundsDetails.y1, d, map);
                }
            }
        }
    }

}
