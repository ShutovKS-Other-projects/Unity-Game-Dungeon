//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;
using DungeonArchitect.Landscape;

namespace DungeonArchitect.Builders.Grid
{

	/// <summary>
	/// The type of the texture defined in the landscape paint settings.  
	/// This determines how the specified texture would be painted in the modified terrain
	/// </summary>
	public enum LandscapeTextureType
	{
		Room,
		Corridor,
		Cliff
	}

	/// <summary>
	/// Data-structure to hold the texture settings.  This contains enough information to paint the texture 
	/// on to the terrain
	/// </summary>
	[System.Serializable]
	public class LandscapeTexture
	{
		public LandscapeTextureType textureType;
		public TerrainLayer terrainLayer;
	}
	
    /// <summary>
    /// The terrain modifier that works with the grid based dungeon builder (DungeonBuilderGrid)
    /// It modifies the terrain by adjusting the height around the layout of the dungeon and painting 
    /// it based on the specified texture settings 
    /// </summary>
	public class LandscapeTransformerGrid : LandscapeTransformerBase
    {
		public LandscapeTexture[] textures;

		// The offset to apply on the terrain at the rooms and corridors. 
		// If 0, then it would touch the rooms / corridors so players can walk over it
		// Give a negative value if you want it to be below it (e.g. if you already have a ground mesh supported by pillars standing on this terrain)
		public float layoutLevelOffset = 0;

		public int smoothingDistance = 5;
		public AnimationCurve roomElevationCurve;
		public AnimationCurve corridorElevationCurve;

        public int roadBlurDistance = 6;
        public float corridorBlurThreshold = 0.5f;
        public float roomBlurThreshold = 0.5f;

        protected override void BuildTerrain(DungeonModel model) {

            if (model is GridDungeonModel && terrain != null)
            {
                var gridMode = model as GridDungeonModel;
                SetupTextures();
                UpdateHeights(gridMode);
                UpdateTerrainTextures(gridMode);
            }
		}

        Rectangle EncompassCellBounds(Rectangle cellBounds, Rectangle bounds)
        {
            int minX = Mathf.Min(bounds.Location.x, cellBounds.Location.x);
            int minZ = Mathf.Min(bounds.Location.z, cellBounds.Location.z);
            int maxX = Mathf.Max(bounds.Location.x + bounds.Size.x, cellBounds.Location.x + cellBounds.Size.x);
            int maxZ = Mathf.Max(bounds.Location.z + bounds.Size.z, cellBounds.Location.z + cellBounds.Size.z);

            return new Rectangle(minX, minZ, maxX - minX, maxZ - minZ);
        }

        protected override Rect GetDungeonBounds(DungeonModel model) {
            var gridConfig = GetComponent<GridDungeonConfig>();
            var gridModel = model as GridDungeonModel;
            var bounds = new Rectangle(0, 0, 0, 0);

            if (gridModel && gridConfig)
            {
                if (gridModel.Cells.Count > 0)
                {
                    bool first = true;
                    
                    foreach (var cell in gridModel.Cells)
                    {
                        if (first)
                        {
                            bounds = cell.Bounds;
                            first = false;
                            continue;
                        }

                        bounds = EncompassCellBounds(cell.Bounds, bounds);
                    }
                }
            }

            float expandX, expandY;
            {
                int expandByLogical = smoothingDistance * 2;
                LandscapeDataRasterizer.TerrainToWorldDistance(terrain, expandByLogical, expandByLogical, out expandX, out expandY);
            }

            var gridSize2D = new Vector2(gridConfig.GridCellSize.x, gridConfig.GridCellSize.z);
            var worldPos = new Vector2(bounds.X, bounds.Z) * gridSize2D;
            var worldSize = new Vector2(bounds.Width, bounds.Length) * gridSize2D;
            var result = new Rect(worldPos, worldSize);
            result.x -= expandX;
            result.y -= expandY;
            result.width += expandX * 2;
            result.height += expandY * 2;
            
            return result;
        }

        void SetupTextures() {
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

		void UpdateHeights(GridDungeonModel model) {
			if (terrain == null || terrain.terrainData == null) return;
			var rasterizer = new LandscapeDataRasterizer(terrain, GetDungeonBounds(model));
			rasterizer.LoadData();
			var gridSize = model.Config.GridCellSize;

			// Raise the terrain
			foreach (var cell in model.Cells) {
				var locationGrid = cell.Bounds.Location;
				var location = locationGrid * gridSize;
				var size = cell.Bounds.Size * gridSize;
				var cellY = location.y + layoutLevelOffset;
				rasterizer.DrawCell(location.x, location.z, size.x, size.z, cellY);
			}

            // Smooth the terrain
            ApplySmoothing(model, rasterizer);
            
			rasterizer.SaveData();
		}

        protected virtual void ApplySmoothing(GridDungeonModel model, LandscapeDataRasterizer rasterizer)
        {
            var gridSize = model.Config.GridCellSize;
            foreach (var cell in model.Cells)
            {
                var locationGrid = cell.Bounds.Location;
                var location = locationGrid * gridSize;
                var size = cell.Bounds.Size * gridSize;
                var cellY = location.y + layoutLevelOffset;
                var curve = (cell.CellType == CellType.Room) ? roomElevationCurve : corridorElevationCurve;
                rasterizer.SmoothCell(location.x, location.z, size.x, size.z, cellY, smoothingDistance, curve);
            }
        }

		void UpdateTerrainTextures(GridDungeonModel model) {
            if (terrain == null || terrain.terrainData == null) return;

			var data = terrain.terrainData;
			//var map = new float[data.alphamapWidth, data.alphamapHeight, numTextures];
            var map = data.GetAlphamaps(0, 0, data.alphamapWidth, data.alphamapHeight);
			UpdateBaseTexture(model, map);
			UpdateCliffTexture(map);
            RemoveFoliage(model);

			data.SetAlphamaps(0, 0, map);
		}

        void RemoveFoliage(GridDungeonModel model)
        {
            if (terrain == null || terrain.terrainData == null) return;
            var data = terrain.terrainData;
            var gridSize = model.Config.GridCellSize;

            foreach (var cell in model.Cells)
            {
                if (cell.CellType == CellType.Unknown) continue;

                var bounds = cell.Bounds;
                var locationGrid = bounds.Location;
                var location = locationGrid * gridSize;
                var size = bounds.Size * gridSize;
                int gx1, gy1, gx2, gy2;
                LandscapeDataRasterizer.WorldToTerrainCoord(terrain, location.x, location.z, out gx1, out gy1, RasterizerTextureSpace.DetailMap);
                LandscapeDataRasterizer.WorldToTerrainCoord(terrain, location.x + size.x, location.z + size.z, out gx2, out gy2, RasterizerTextureSpace.DetailMap);

                int sx = gx2 - gx1 + 1;
                int sy = gy2 - gy1 + 1;
                int[,] clearPatch = new int[sy, sx];
                for (int d = 0; d < data.detailPrototypes.Length; d++)
                {
                    data.SetDetailLayer(gx1, gy1, d, clearPatch);
                }
            }
        }

		void UpdateBaseTexture(GridDungeonModel model, float[,,] map) {
			if (terrain == null || terrain.terrainData == null) return;
            var data = terrain.terrainData;

            int corridorIndex = GetTextureIndex(LandscapeTextureType.Corridor);
            int roomIndex = GetTextureIndex(LandscapeTextureType.Room);

            // Apply the room/corridor texture
            {
				var gridSize = model.Config.GridCellSize;
                var roomMap = new float[map.GetLength(0), map.GetLength(1)];
                var corridorMap = new float[map.GetLength(0), map.GetLength(1)];
                foreach (var cell in model.Cells)
                {
					var bounds = cell.Bounds;
					var locationGrid = bounds.Location;
					var location = locationGrid * gridSize;
					var size = bounds.Size * gridSize;
					int gx1, gy1, gx2, gy2;
					LandscapeDataRasterizer.WorldToTerrainTextureCoord(terrain, location.x, location.z, out gx1, out gy1);
					LandscapeDataRasterizer.WorldToTerrainTextureCoord(terrain, location.x + size.x, location.z + size.z, out gx2, out gy2);
					for (var gx = gx1; gx <= gx2; gx++) {
						for (var gy = gy1; gy <= gy2; gy++) {
                            if (cell.CellType == CellType.Unknown) continue;
                            if (cell.CellType == CellType.Room)
                            {
                                roomMap[gy, gx] = 1;
                            }
                            else
                            {
                                corridorMap[gy, gx] = 1;
                            }
						}
					}
				}

                // Blur the layout data
                var filter = new BlurFilter(roadBlurDistance);
                roomMap = filter.ApplyFilter(roomMap);
                corridorMap = filter.ApplyFilter(corridorMap);

                // Fill up the inner region with corridor index
                int numMaps = map.GetLength(2);
				for (var y = 0; y < data.alphamapHeight; y++) {
					for (var x = 0; x < data.alphamapWidth; x++) {
                        bool wroteData = false;
                        bool isCorridor = (corridorMap[y, x] > corridorBlurThreshold);
						if (isCorridor) {
                            if (roomIndex >= 0)
                            {
                                map[y, x, roomIndex] = 0;
                            }
                            if (corridorIndex >= 0)
                            {
                                map[y, x, corridorIndex] = 1;
                                wroteData = true;
                            }
                        }
                    
                        bool isRoom = (roomMap[y, x] > roomBlurThreshold);
                        if (isRoom)
                        {
                            if (corridorIndex >= 0)
                            {
                                map[y, x, corridorIndex] = 0;
                            }
                            if (roomIndex >= 0)
                            {
                                map[y, x, roomIndex] = 1;
                                wroteData = true;
                            }
                        }

                        if (wroteData)
                        {
                            // Clear out other masks
                            for (int m = 0; m < numMaps; m++)
                            {
                                if (m == corridorIndex || m == roomIndex)
                                {
                                    continue;
                                }

                                map[y, x, m] = 0;
                            }
                        }
                    }
				}
			}
		}

		void UpdateCliffTexture(float[,,] map) {
			if (terrain == null) return;
			int cliffIndex = GetTextureIndex(LandscapeTextureType.Cliff);
			if (cliffIndex < 0) return;
			
			var data = terrain.terrainData;
			
			// For each point on the alphamap...
			for (var y = 0; y < data.alphamapHeight; y++) {
				for (var x = 0; x < data.alphamapWidth; x++) {
					// Get the normalized terrain coordinate that
					// corresponds to the the point.
					var normX = x * 1.0f / (data.alphamapWidth - 1);
					var normY = y * 1.0f / (data.alphamapHeight - 1);
					
					// Get the steepness value at the normalized coordinate.
					var angle = data.GetSteepness(normX, normY);
					
					// Steepness is given as an angle, 0..90 degrees. Divide
					// by 90 to get an alpha blending value in the range 0..1.
					var frac = angle / 90.0f;
					frac *= 2;
					frac = Mathf.Clamp01(frac);
					var cliffRatio = frac;
					var nonCliffRatio = 1 - frac;
					
					for (int t = 0; t < textures.Length; t++) {
						if (t == cliffIndex) {
							map[y, x, t] = cliffRatio;
						} else {
							map[y, x, t] *= nonCliffRatio;
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Returns the index of the landscape texture.  -1 if not found
		/// </summary>
		/// <returns>The texture index. -1 if not found</returns>
		/// <param name="textureType">Texture type.</param>
		int GetTextureIndex(LandscapeTextureType textureType) {
            if (terrain == null || terrain.terrainData == null) return -1;
            var data = terrain.terrainData;
            for (int i = 0; i < textures.Length; i++) {
				if (textures[i].textureType == textureType) {
                    return System.Array.IndexOf(data.terrainLayers, textures[i].terrainLayer);
				}
			}
			return -1;	// Doesn't exist
		}

	}
}
