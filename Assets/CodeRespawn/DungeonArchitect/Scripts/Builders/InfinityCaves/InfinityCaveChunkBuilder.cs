//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect.Utils;

namespace DungeonArchitect.Builders.Infinity.Caves
{
	public static class InfinityCaveChunkMarkerNames
    {
        public static readonly string GroundBlock = "GroundBlock";
        public static readonly string WallBlock = "WallBlock";
        public static readonly string RockBlock = "RockBlock";
    }

	public class InfinityCaveChunkBuilder : DungeonBuilder {

        InfinityCaveChunkConfig chunkConfig;
        InfinityCaveChunkModel chunkModel;

		public override void BuildDungeon(DungeonConfig config, DungeonModel model)
		{
			base.BuildDungeon(config, model);

			chunkConfig = config as InfinityCaveChunkConfig;
			chunkModel = model as InfinityCaveChunkModel;
			chunkModel.Config = chunkConfig;

			// Generate the city layout and save it in a model.   No markers are emitted here. 
			GenerateLevelLayout();
		}
        
		public override void EmitMarkers()
		{
			base.EmitMarkers();

			EmitLevelMarkers();

			ProcessMarkerOverrideVolumes();
		}
        
		void GenerateLevelLayout()
        {
            int w = Mathf.RoundToInt(chunkConfig.chunkSize.x);
            int h = Mathf.RoundToInt(chunkConfig.chunkSize.z);

            var baseChunkCoord = MathUtils.ToIntVector(MathUtils.Divide(chunkConfig.chunkPosition, chunkConfig.chunkSize));
            var world = new bool[w * 3, h * 3];

            for (int cdx = -1; cdx <= 1; cdx++)
            {
                for (int cdz = -1; cdz <= 1; cdz++)
                {
                    var chunkCoord = baseChunkCoord + new IntVector(cdx, 0, cdz);
                    var chunkSeed = chunkCoord.GetHashCode();
                    var random = new System.Random(chunkSeed);
                    var sx = (cdx + 1) * w;
                    var sz = (cdz + 1) * h;
                    for (int x = sx; x < sx + w; x++)
                    {
                        for (int z = sz; z < sz + h; z++)
                        {
                            world[x, z] = random.NextFloat() < 0.5f;
                        }
                    }
                }
            }

            for (int i = 0; i < chunkConfig.iterations; i++)
            {
                world = ApplyAutomata(world);
            }

            chunkModel.tileStates = new MazeTileState[w, h];
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    bool occupied = world[w + x, h + y];
                    var cellType = occupied ? MazeTileState.Rock : MazeTileState.Empty;
                    if (occupied)
                    {
                        // Check if we have an empty space nearby so we can upgrade this to a wall
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            for (int dy = -1; dy <= 1; dy++)
                            {
                                //if (dx == 0 && dy == 0) continue;
                                var cx = w + x + dx;
                                var cy = h + y + dy;
                                var neighborEmpty = !world[cx, cy];
                                if (neighborEmpty)
                                {
                                    cellType = MazeTileState.Wall;
                                    break;
                                }
                            }
                            if (cellType == MazeTileState.Wall) break;
                        }
                    }
                    chunkModel.tileStates[x, y] = cellType;
                }
            }
        }

        bool[,] ApplyAutomata(bool[,] world)
        {
            var w = world.GetLength(0);
            var h = world.GetLength(1);

            var result = new bool[w, h];
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    var rocks = 0;
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            var cx = Mathf.Clamp(x + dx, 0, w - 1);
                            var cy = Mathf.Clamp(y + dy, 0, w - 1);
                            if (world[cx, cy])
                            {
                                rocks++;
                            }
                        }
                    }
                    result[x, y] = (rocks >= chunkConfig.neighborRocks);
                }
            }
            return result;
        }

		void EmitLevelMarkers()
        {
            var gridSize = new Vector3(chunkConfig.gridSize.x, 0, chunkConfig.gridSize.y);
            int w = Mathf.RoundToInt(chunkConfig.chunkSize.x);
            int h = Mathf.RoundToInt(chunkConfig.chunkSize.z);
            int sx = Mathf.RoundToInt(chunkConfig.chunkPosition.x);
            int sz = Mathf.RoundToInt(chunkConfig.chunkPosition.z);

            for (int x = 0; x < w; x++)
            {
                for (int z = 0; z < h; z++)
                {
                    var position = Vector3.Scale(new Vector3(sx + x + 0.5f, 0, sz + z + 0.5f), gridSize);
                    var markerTransform = Matrix4x4.TRS(position, Quaternion.identity, Vector3.one);
                    string markerName = "";
                    var state = chunkModel.tileStates[x, z];
                    if (state == MazeTileState.Rock)
                    {
                        markerName = InfinityCaveChunkMarkerNames.RockBlock;
                    }
                    else if (state == MazeTileState.Wall)
                    {
                        markerName = InfinityCaveChunkMarkerNames.WallBlock;
                    }
                    else if (state == MazeTileState.Empty)
                    {
                        markerName = InfinityCaveChunkMarkerNames.GroundBlock;
                    }
                    EmitMarker(markerName, markerTransform, new IntVector(sx + x, 0, sz + z), -1);
                }
            }
        } 

	}
}