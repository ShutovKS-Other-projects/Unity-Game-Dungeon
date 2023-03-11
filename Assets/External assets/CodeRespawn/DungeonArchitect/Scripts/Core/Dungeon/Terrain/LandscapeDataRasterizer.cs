//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Landscape
{
    public enum RasterizerTextureSpace
    {
        HeightMap,
        AlphaMap,
        DetailMap
    }

    /// <summary>
    /// Manages the landscape data and performs various rasterization algorithms (draw cells, lines etc)
    /// </summary>
    public class LandscapeDataRasterizer
    {
        Terrain terrain;
        float[,] heights;
        bool[,] lockedCells;

        Rect worldBounds;
        Vector2Int heightmapFrameStart;
        Vector2Int heightmapFrameSize;

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="terrain">The terrain object to modify</param>
        /// <param name="elevation">The prefered ground level elevation</param>
        public LandscapeDataRasterizer(Terrain terrain, Rect worldBounds)
        {
            this.terrain = terrain;
            this.worldBounds = worldBounds;

            int x1, y1, x2, y2;
            WorldToTerrainCoord(terrain, worldBounds.x, worldBounds.y, out x1, out y1, RasterizerTextureSpace.HeightMap);
            WorldToTerrainCoord(terrain, worldBounds.xMax, worldBounds.yMax, out x2, out y2, RasterizerTextureSpace.HeightMap);

            heightmapFrameStart = new Vector2Int(x1, y1);
            heightmapFrameSize = new Vector2Int(x2 - x1 + 1, y2 - y1 + 1);
        }

        /// <summary>
        /// Loads the data from the terrain into memory for modification
        /// </summary>
        public void LoadData()
        {
            var data = terrain.terrainData;

            var fx = heightmapFrameStart.x;
            var fy = heightmapFrameStart.y;
            var fw = heightmapFrameSize.x;
            var fh = heightmapFrameSize.y;

            heights = data.GetHeights(fx, fy, fw, fh);

            lockedCells = new bool[fh, fw];

            for (int ix = 0; ix < fw; ix++)
            {
                for (int iy = 0; iy < fh; iy++)
                {
                    lockedCells[iy, ix] = false;
                }
            }
        }

        /// <summary>
        /// Gets the elevation in normalized space
        /// </summary>
        /// <param name="worldElevation"></param>
        /// <returns></returns>
        float GetElevation(float worldElevation)
        {
            var resolution = terrain.terrainData.size.y;
            return (worldElevation - terrain.transform.position.y) / resolution;
        }

        /// <summary>
        /// Gets the height of the terrain at the specified world space
        /// </summary>
        /// <param name="terrain">The terrain object</param>
        /// <param name="worldX">X cooridnate in world space</param>
        /// <param name="worldZ">Z cooridnate in world space</param>
        /// <returns>The Y height of the terrain at the specified location</returns>
        public static float GetHeight(Terrain terrain, float worldX, float worldZ)
        {
            int gx, gy;
            LandscapeDataRasterizer.WorldToTerrainCoord(terrain, worldX, worldZ, out gx, out gy);
            var height = terrain.terrainData.GetHeight(gx, gy);

            return height + terrain.transform.position.y;
        }

        public static void WorldToTerrainDistance(Terrain terrain, float worldDistX, float worldDistZ, out int terrainDistX, out int terrainDistZ)
        {
            var terrainSize = terrain.terrainData.size;
            var data = terrain.terrainData;

            var terrainWidth = data.heightmapResolution;
            var terrainHeight = data.heightmapResolution;

            var multiplierX = (terrainWidth - 1) / terrainSize.x;
            var multiplierZ = (terrainHeight - 1) / terrainSize.z;

            terrainDistX = Mathf.RoundToInt(worldDistX * multiplierX);
            terrainDistZ = Mathf.RoundToInt(worldDistZ * multiplierZ);
        }

        public static void TerrainToWorldDistance(Terrain terrain, int terrainDistX, int terrainDistZ, out float worldDistX, out float worldDistZ)
        {
            var terrainSize = terrain.terrainData.size;
            var data = terrain.terrainData;

            var terrainWidth = data.heightmapResolution;
            var terrainHeight = data.heightmapResolution;

            worldDistX = Mathf.RoundToInt(terrainDistX * terrainSize.x / (terrainWidth - 1));
            worldDistZ = Mathf.RoundToInt(terrainDistZ * terrainSize.z / (terrainHeight - 1));
        }

        /// <summary>
        /// Converts the world coordinate to internal terrain coordinate where the data is loaded
        /// </summary>
        /// <param name="terrain">The terrain to query</param>
        /// <param name="x">x coordinate in world coordinate</param>
        /// <param name="y">z coordinate in world coordinate</param>
        /// <param name="gx">x cooridnate in the 2D terrain height data coordinate</param>
        /// <param name="gy">y cooridnate in the 2D terrain height data coordinate</param>
        public static void WorldToTerrainCoord(Terrain terrain, float x, float y, out int gx, out int gy)
        {
            WorldToTerrainCoord(terrain, x, y, out gx, out gy, RasterizerTextureSpace.HeightMap);
        }

        /// <summary>
        /// Converts the world coordinate to internal terrain coordinate where the data is loaded
        /// </summary>
        /// <param name="terrain">The terrain to query</param>
        /// <param name="x">x coordinate in world coordinate</param>
        /// <param name="y">z coordinate in world coordinate</param>
        /// <param name="gx">x cooridnate in the 2D terrain height data coordinate</param>
        /// <param name="gy">y cooridnate in the 2D terrain height data coordinate</param>
        public static void WorldToTerrainCoord(Terrain terrain, float x, float y, out int gx, out int gy, RasterizerTextureSpace textureSpace)
        {
            var terrainSize = terrain.terrainData.size;
            var data = terrain.terrainData;

            var terrainWidth = 0;
            var terrainHeight = 0;

            if (textureSpace == RasterizerTextureSpace.HeightMap)
            {
                terrainWidth = data.heightmapResolution;
                terrainHeight = data.heightmapResolution;
            }
            else if (textureSpace == RasterizerTextureSpace.AlphaMap)
            {
                terrainWidth = data.alphamapWidth;
                terrainHeight = data.alphamapHeight;
            }
            else if (textureSpace == RasterizerTextureSpace.DetailMap)
            {
                terrainWidth = data.detailWidth;
                terrainHeight = data.detailHeight;
            }

            var multiplierX = (terrainWidth - 1) / terrainSize.x;
            var multiplierZ = (terrainHeight - 1) / terrainSize.z;

            var offset = new Vector2();
            offset.x = -terrain.transform.position.x;
            offset.y = -terrain.transform.position.z;
            var xf = (x + offset.x) * multiplierX;
            var yf = (y + offset.y) * multiplierZ;

            gx = Mathf.RoundToInt(xf);
            gy = Mathf.RoundToInt(yf);
        }

        /// <summary>
        /// Converts the world coordinate to terrain texture coordinate
        /// </summary>
        /// <param name="terrain">The terrain to query</param>
        /// <param name="x">x coordinate in world coordinate</param>
        /// <param name="y">z coordinate in world coordinate</param>
        /// <param name="tx">x cooridnate in the 2D terrain texture data coordinate</param>
        /// <param name="ty">y cooridnate in the 2D terrain texture data coordinate</param>
        public static void WorldToTerrainTextureCoord(Terrain terrain, float x, float y, out int tx, out int ty)
        {
            var terrainSize = terrain.terrainData.size;
            var data = terrain.terrainData;

            var terrainWidth = data.alphamapWidth;
            var terrainHeight = data.alphamapHeight;

            var multiplierX = (terrainWidth - 1) / terrainSize.x;
            var multiplierZ = (terrainHeight - 1) / terrainSize.z;

            var offset = new Vector2();
            offset.x = -terrain.transform.position.x;
            offset.y = -terrain.transform.position.z;
            var xf = (x + offset.x) * multiplierX;
            var yf = (y + offset.y) * multiplierZ;

            tx = Mathf.RoundToInt(xf);
            ty = Mathf.RoundToInt(yf);
        }

        /// <summary>
        /// Converts the world coordinate to terrain texture coordinate
        /// </summary>
        /// <param name="terrain">The terrain to query</param>
        /// <param name="x">x coordinate in world coordinate</param>
        /// <param name="y">z coordinate in world coordinate</param>
        /// <param name="tx">x cooridnate in the 2D terrain texture data coordinate</param>
        /// <param name="ty">y cooridnate in the 2D terrain texture data coordinate</param>
        public static void WorldToTerrainDetailCoord(Terrain terrain, float x, float y, out int tx, out int ty)
        {
            var terrainSize = terrain.terrainData.size;
            var data = terrain.terrainData;

            var terrainWidth = data.detailWidth;
            var terrainHeight = data.detailHeight;

            var multiplierX = (terrainWidth - 1) / terrainSize.x;
            var multiplierZ = (terrainHeight - 1) / terrainSize.z;

            var offset = new Vector2();
            offset.x = -terrain.transform.position.x;
            offset.y = -terrain.transform.position.z;
            var xf = (x + offset.x) * multiplierX;
            var yf = (y + offset.y) * multiplierZ;

            tx = Mathf.RoundToInt(xf);
            ty = Mathf.RoundToInt(yf);
        }


        public void DrawCell(float x, float y, float w, float h, float elevation)
        {
            DrawCell(x, y, w, h, elevation, 1);
        }

        /// <summary>
        /// Rasterizes the terrain height along the specified world cooridnate with the specified elevation height
        /// </summary>
        /// <param name="x">x cooridnate in world space</param>
        /// <param name="y">z coordinate in world space</param>
        /// <param name="w">width in world space</param>
        /// <param name="h">height in world space</param>
        /// <param name="elevation">The elevation to set in the specified bounds</param>
        public void DrawCell(float x, float y, float w, float h, float elevation, float flatten)
        {
            var data = terrain.terrainData;
            var terrainWidth = data.heightmapResolution;
            var terrainHeight = data.heightmapResolution;

            var fx = heightmapFrameStart.x;
            var fy = heightmapFrameStart.y;
            var fw = heightmapFrameSize.x;
            var fh = heightmapFrameSize.y;

            flatten = Mathf.Clamp01(flatten);

            int x1, y1, x2, y2;
            WorldToTerrainCoord(terrain, x, y, out x1, out y1);
            WorldToTerrainCoord(terrain, x + w, y + h, out x2, out y2);

            for (int ix = x1; ix <= x2; ix++)
            {
                for (int iy = y1; iy <= y2; iy++)
                {
                    if (ix < 0 || ix >= terrainWidth || iy < 0 || iy >= terrainHeight)
                    {
                        // Out of terrain boundaries. Ignore
                        continue;
                    }
                    int tx = Mathf.Clamp(ix - fx, 0, fw - 1);
                    int ty = Mathf.Clamp(iy - fy, 0, fh - 1);
                    var normalizedElevation = GetElevation(elevation);
                    float e0 = heights[ty, tx];
                    float e1 = normalizedElevation;
                    float e = Mathf.Lerp(e0, e1, flatten);
                    heights[ty, tx] = e;
                    lockedCells[ty, tx] = true;
                }
            }
        }

        public void SmoothCell(float x, float y, float w, float h, float elevation, int smoothingDistance, AnimationCurve smoothingCurve)
        {
            SmoothCell(x, y, w, h, elevation, smoothingDistance, smoothingCurve, 1);
        }

        /// <summary>
        /// Applies a smoothing blur filter based on the user-defined smoothing curve 
        /// </summary>
        /// <param name="x">x cooridnate in world space</param>
        /// <param name="y">z coordinate in world space</param>
        /// <param name="w">width in world space</param>
        /// <param name="h">height in world space</param>
        /// <param name="elevation">The elevation to set in the specified bounds</param>
        /// <param name="smoothingDistance">The distance to apply the smoothing transition on.  For e.g. if the distance it 5, the smoothing would occur over 5 units</param>
        /// <param name="smoothingCurve">The user defined curve to control the steepness of cliffs</param>
        public void SmoothCell(float x, float y, float w, float h, float elevation, int smoothingDistance, AnimationCurve smoothingCurve, float flatten)
        {
            var fx = heightmapFrameStart.x;
            var fy = heightmapFrameStart.y;
            var fw = heightmapFrameSize.x;
            var fh = heightmapFrameSize.y;

            flatten = Mathf.Clamp01(flatten);

            int x1, y1, x2, y2;
            WorldToTerrainCoord(terrain, x, y, out x1, out y1);
            WorldToTerrainCoord(terrain, x + w, y + h, out x2, out y2);
            var bounds = new Rectangle(x1, y1, x2 - x1, y2 - y1);
            var startElevation = GetElevation(elevation);
            for (int i = 1; i <= smoothingDistance; i++)
            {
                bounds = Rectangle.ExpandBounds(bounds, 1);
                var borderPoints = bounds.GetBorderPoints();
                foreach (var borderPoint in borderPoints)
                {
                    var ix = borderPoint.x;
                    var iy = borderPoint.z;
                    int tx = Mathf.Clamp(ix - fx, 0, fw - 1);
                    int ty = Mathf.Clamp(iy - fy, 0, fh - 1);

                    if (lockedCells[ty, tx])
                    {
                        continue;
                    }

                    var endElevation = heights[ty, tx];
                    var frameFlatten = (float)(smoothingDistance - i) / (smoothingDistance);
                    //frameSmooth = Mathf.Clamp01(smoothingCurve.Evaluate(frameSmooth));
                    frameFlatten = Remap(frameFlatten, 0, 1, 0, flatten, true);
                    var cellElevation = endElevation + (startElevation - endElevation) * frameFlatten;
                    heights[ty, tx] = cellElevation;
                }
            }
        }

        float Remap(float value, float oldMin, float oldMax, float newMin, float newMax, bool clamp)
        {
            var ratio = (value - oldMin) / (oldMax - oldMin);
            if (clamp)
            {
                ratio = Mathf.Clamp01(ratio);
            }
            return newMin + (newMax - newMin) * ratio;
        }


        /// <summary>
        /// Saves the data in memory back into the terrain. This modifies the terrain object
        /// </summary>
        public void SaveData()
        {
            var fx = heightmapFrameStart.x;
            var fy = heightmapFrameStart.y;

            terrain.terrainData.SetHeights(fx, fy, heights);
        }
    }
}
