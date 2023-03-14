//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections;
using System.Collections.Generic;
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Landscape;
using UnityEditor;
using UnityEngine;
using MathUtils = DungeonArchitect.Utils.MathUtils;

namespace DungeonArchitect.Builders.SnapGridFlow
{
        
	/// <summary>
	/// The type of the texture defined in the landscape paint settings.  
	/// This determines how the specified texture would be painted in the modified terrain
	/// </summary>
	public enum LandscapeTextureType
	{
		Room,
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
	public class LandscapeTransformerSGF : LandscapeTransformerBase
    {
		public LandscapeTexture[] textures;

		// The offset to apply on the terrain at the rooms and corridors. 
		// If 0, then it would touch the rooms / corridors so players can walk over it
		// Give a negative value if you want it to be below it (e.g. if you already have a ground mesh supported by pillars standing on this terrain)
		public float layoutLevelOffset = 0;

		public int smoothingDistance = 5;
		public AnimationCurve roomElevationCurve;
        public int roadBlurDistance = 6;
        public float roomBlurThreshold = 0.5f;

        private Vector3 chunkSize = Vector3.zero;
        private HashSet<Vector3Int> nodesToRasterize = new HashSet<Vector3Int>();
        private Vector3Int min = Vector3Int.zero;
        private Vector3Int max = Vector3Int.zero;
        private IntVector[] terrainBases;
        private float offsetY = 0;
        
        protected override void BuildTerrain(DungeonModel model) {

	        var sgfModel = model as SnapGridFlowModel;
	        if (terrain == null || sgfModel == null || sgfModel.layoutGraph == null) return;
            var sgfConfig = GetComponent<SnapGridFlowConfig>();
            if (sgfConfig == null || sgfConfig.moduleDatabase == null || sgfConfig.moduleDatabase.ModuleBoundsAsset == null) return;

            nodesToRasterize.Clear();
            chunkSize = sgfConfig.moduleDatabase.ModuleBoundsAsset.chunkSize;
            offsetY = sgfConfig.moduleDatabase.ModuleBoundsAsset.doorOffsetY;
            
            var graph = sgfModel.layoutGraph;
            if (graph == null || graph.Nodes.Count == 0) return;

            
            {
	            var occupiedNodes = new HashSet<Vector3Int>();
	            bool foundValid = false;
	            foreach (var node in graph.Nodes) 
	            {
		            if (!node.active) continue;
		            
		            FlowLayoutGraphNode[] subNodes = node.MergedCompositeNodes.Count == 0 ? new FlowLayoutGraphNode[]{ node } : node.MergedCompositeNodes.ToArray();
		            foreach (var subNode in subNodes)
		            {   
			            Vector3Int coord = new Vector3Int(
				            Mathf.RoundToInt(subNode.coord.x),
				            Mathf.RoundToInt(subNode.coord.y),
				            Mathf.RoundToInt(subNode.coord.z));

			            occupiedNodes.Add(coord);
			            
			            if (!foundValid)
			            {
				            min = coord;
				            max = coord;
				            foundValid = true;
			            }
			            else
			            {
				            min.x = Mathf.Min(min.x, coord.x);
				            min.y = Mathf.Min(min.y, coord.y);
				            min.z = Mathf.Min(min.z, coord.z);
			            
				            max.x = Mathf.Max(max.x, coord.x);
				            max.y = Mathf.Max(max.y, coord.y);
				            max.z = Mathf.Max(max.z, coord.z);
			            }
		            }
	            }

	            // Along the XZ plane, find the lowest Y coord for each point so we can rasterize the landscape on it
	            for (int x = min.x; x <= max.x; x++)
	            {
		            for (int z = min.z; z <= max.z; z++)
		            {
			            for (int y = min.y; y <= max.y; y++)
			            {
				            var coord = new Vector3Int(x, y, z);
				            if (occupiedNodes.Contains(coord))
				            {
					            nodesToRasterize.Add(coord);
					            break;
				            }
			            }
		            }
	            }
            }
            
            SetupTextures();
            UpdateHeights(sgfModel);
            //UpdateTerrainTextures();
		}

        protected override Rect GetDungeonBounds(DungeonModel model) {
            var sgfConfig = GetComponent<SnapGridFlowConfig>();
            var sgfModel = model as SnapGridFlowModel;

            var basePosition = GetBasePosition();
            var worldPos = new Vector2((min.x - 0.5f) * chunkSize.x, (min.z - 0.5f) * chunkSize.z) + new Vector2(basePosition.x, basePosition.z);
            var worldSize = new Vector2((max.x - min.x + 1) * chunkSize.x, (max.z - min.z + 1) * chunkSize.z);


            float expandX, expandY;
            {
                int expandByLogical = smoothingDistance * 2;
                LandscapeDataRasterizer.TerrainToWorldDistance(terrain, expandByLogical, expandByLogical, out expandX, out expandY);
            }

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

        Vector3 GetBasePosition()
        {
	        return transform.position; // + new Vector3(0, -offsetY, 0);
        }
        
		void UpdateHeights(SnapGridFlowModel model) {
			if (terrain == null || terrain.terrainData == null) return;
			var rasterizer = new LandscapeDataRasterizer(terrain, GetDungeonBounds(model));
			rasterizer.LoadData();

			var basePosition = GetBasePosition();
			
			// Raise the terrain
			foreach (var coord in nodesToRasterize)
			{
				var coordF = MathUtils.ToVector3(coord) - new Vector3(0.5f, 0, 0.5f);
				var worldPos = Vector3.Scale(coordF, chunkSize) + basePosition;
				var cellY = worldPos.y + layoutLevelOffset;
				rasterizer.DrawCell(worldPos.x, worldPos.z, chunkSize.x, chunkSize.z, cellY);
			}

            // Smooth the terrain
            ApplySmoothing(model, rasterizer);
            
			rasterizer.SaveData();
		}

        protected virtual void ApplySmoothing(SnapGridFlowModel model, LandscapeDataRasterizer rasterizer)
        {
	        var basePosition = GetBasePosition();
	        foreach (var coord in nodesToRasterize)
	        {
		        var coordF = MathUtils.ToVector3(coord) - new Vector3(0.5f, 0.5f, 0.5f);
		        var worldPos = Vector3.Scale(coordF, chunkSize) + basePosition;
		        var cellY = worldPos.y + layoutLevelOffset;
		        var curve = roomElevationCurve;
		        rasterizer.SmoothCell(worldPos.x, worldPos.z, chunkSize.x, chunkSize.z, cellY, smoothingDistance, curve);
	        }
        }

		void UpdateTerrainTextures() {
            if (terrain == null || terrain.terrainData == null) return;

			var data = terrain.terrainData;
			//var map = new float[data.alphamapWidth, data.alphamapHeight, numTextures];
            var map = data.GetAlphamaps(0, 0, data.alphamapWidth, data.alphamapHeight);
			UpdateBaseTexture(map);
			UpdateCliffTexture(map);
            RemoveFoliage();

			data.SetAlphamaps(0, 0, map);
		}

        void RemoveFoliage()
        {
            if (terrain == null || terrain.terrainData == null) return;
            var data = terrain.terrainData;

            var basePosition = GetBasePosition();
            foreach (var coord in nodesToRasterize)
            {
	            var coordF = MathUtils.ToVector3(coord) - new Vector3(0.5f, 0.5f, 0.5f);
	            var worldPos = Vector3.Scale(coordF, chunkSize) + basePosition;
                int gx1, gy1, gx2, gy2;
                LandscapeDataRasterizer.WorldToTerrainCoord(terrain, worldPos.x, worldPos.z, out gx1, out gy1, RasterizerTextureSpace.DetailMap);
                LandscapeDataRasterizer.WorldToTerrainCoord(terrain, worldPos.x + chunkSize.x, worldPos.z + chunkSize.z, out gx2, out gy2, RasterizerTextureSpace.DetailMap);

                int sx = gx2 - gx1 + 1;
                int sy = gy2 - gy1 + 1;
                int[,] clearPatch = new int[sy, sx];
                for (int d = 0; d < data.detailPrototypes.Length; d++)
                {
                    data.SetDetailLayer(gx1, gy1, d, clearPatch);
                }
            }
        }

		void UpdateBaseTexture(float[,,] map) {
			if (terrain == null || terrain.terrainData == null) return;
            var data = terrain.terrainData;

            int roomIndex = GetTextureIndex(LandscapeTextureType.Room);

            // Apply the room/corridor texture
            {
                var roomMap = new float[map.GetLength(0), map.GetLength(1)];
                var basePosition = GetBasePosition();
                foreach (var coord in nodesToRasterize)
                {
	                var coordF = MathUtils.ToVector3(coord) - new Vector3(0.5f, 0.5f, 0.5f);
	                var worldPos = Vector3.Scale(coordF, chunkSize) + basePosition;
					int gx1, gy1, gx2, gy2;
					LandscapeDataRasterizer.WorldToTerrainTextureCoord(terrain, worldPos.x, worldPos.z, out gx1, out gy1);
					LandscapeDataRasterizer.WorldToTerrainTextureCoord(terrain, worldPos.x + chunkSize.x, worldPos.z + chunkSize.z, out gx2, out gy2);
					for (var gx = gx1; gx <= gx2; gx++) {
						for (var gy = gy1; gy <= gy2; gy++) {
                            roomMap[gy, gx] = 1;
						}
					}
				}

                // Blur the layout data
                var filter = new BlurFilter(roadBlurDistance);
                roomMap = filter.ApplyFilter(roomMap);

                // Fill up the inner region with corridor index
                int numMaps = map.GetLength(2);
				for (var y = 0; y < data.alphamapHeight; y++) {
					for (var x = 0; x < data.alphamapWidth; x++) {
                        bool wroteData = false;
                        bool isRoom = (roomMap[y, x] > roomBlurThreshold);
                        if (isRoom && roomIndex >= 0)
                        {
                            map[y, x, roomIndex] = 1;
                            wroteData = true;
                        }

                        if (wroteData)
                        {
                            // Clear out other masks
                            for (int m = 0; m < numMaps; m++)
                            {
                                if (m == roomIndex)
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
