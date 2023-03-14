//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;

namespace DungeonArchitect.Builders.SimpleCity
{
    public static class SimpleCityDungeonMarkerNames
    {
        public static readonly string House = "House";
        public static readonly string Park = "Park";
        public static readonly string Road_X = "Road_X";
        public static readonly string Road_T = "Road_T";
        public static readonly string Road_Corner = "Road_Corner";
        public static readonly string Road_S = "Road_S";
        public static readonly string Road_E = "Road_E";
        public static readonly string Road = "Road";

        public static readonly string CityWall = "CityWall";
        public static readonly string CityDoor = "CityDoor";
        public static readonly string CityGround = "CityGround";
        public static readonly string CornerTower = "CornerTower";
        public static readonly string CityWallPadding = "CityWallPadding";
    }

    public class SimpleCityDungeonBuilder : DungeonBuilder
    {
        SimpleCityDungeonConfig cityConfig;
        SimpleCityDungeonModel cityModel;

        new System.Random random;
        /// <summary>
        /// Builds the dungeon layout.  In this method, you should build your dungeon layout and save it in your model file
        /// No markers should be emitted here.   (EmitMarkers function will be called later by the engine to do that)
        /// </summary>
        /// <param name="config">The builder configuration</param>
        /// <param name="model">The dungeon model that the builder will populate</param>
        public override void BuildDungeon(DungeonConfig config, DungeonModel model)
        {
            base.BuildDungeon(config, model);

            random = new System.Random((int)config.Seed);

            // We know that the dungeon prefab would have the appropriate config and models attached to it
            // Cast and save it for future reference
            cityConfig = config as SimpleCityDungeonConfig;
            cityModel = model as SimpleCityDungeonModel;
            cityModel.Config = cityConfig;

            // Generate the city layout and save it in a model.   No markers are emitted here. 
            GenerateCityLayout();
        }

        /// <summary>
        /// Override the builder's emit marker function to emit our own markers based on the layout that we built
        /// You should emit your markers based on the layout you have saved in the model generated previously
        /// When the user is designing the theme interactively, this function will be called whenever the graph state changes,
        /// so the theme engine can populate the scene (BuildDungeon will not be called if there is no need to rebuild the layout again)
        /// </summary>
        public override void EmitMarkers()
        {
            base.EmitMarkers();
            EmitCityMarkers();
            EmitBoundaryMarkers();
            ProcessMarkerOverrideVolumes();
        }

        delegate void InsertHouseDelegate();

        /// <summary>
        /// Generate a layout and save it in the model
        /// </summary>
        void GenerateCityLayout()
        {
            cityConfig.roadWidth = Mathf.Max(1, cityConfig.roadWidth);
            var cityWidth = random.Range(cityConfig.minSize, cityConfig.maxSize);
            var cityLength = random.Range(cityConfig.minSize, cityConfig.maxSize);
            var roadWidth = cityConfig.roadWidth;

            cityModel.CityWidth = cityWidth;
            cityModel.CityHeight = cityLength;

            {
                cityModel.Cells = new SimpleCityCell[cityWidth, cityLength];

                for (int x = 0; x < cityWidth; x++)
                {
                    for (int z = 0; z < cityLength; z++)
                    {
                        var cell = new SimpleCityCell();
                        cell.Position = new IntVector(x, 0, z);
                        cell.CellType = SimpleCityCellType.House;
                        cell.Rotation = GetRandomRotation();
                        cityModel.Cells[x, z] = cell;
                    }
                }
            }


            // Build a road network by removing some houses 
            // First build roads along the edge of the map
            for (int x = 0; x < cityWidth; x++)
            {
                MakeRoad(x, 0, true);
                MakeRoad(x, cityLength - roadWidth, true);
            }
            for (int z = 0; z < cityLength; z++)
            {
                MakeRoad(0, z, false);
                MakeRoad(cityWidth - roadWidth, z, false);
            }

            // Create roads in-between
            for (int x = GetRandomBlockSize() + 1; x < cityWidth; x += GetRandomBlockSize() + 1)
            {
                if (cityWidth - x <= 2 * roadWidth) continue;
                for (int z = 0; z < cityLength; z++)
                {
                    MakeRoad(x, z, false);
                }
            }
            for (int z = GetRandomBlockSize() + 1; z < cityLength; z += GetRandomBlockSize() + 1)
            {
                if (cityLength - z <= 2 * roadWidth) continue;
                for (int x = 0; x < cityWidth; x++)
                {
                    MakeRoad(x, z, true);
                }
            }

			RemoveRoadEdges();
			

            // Insert bigger houses 
            for (int x = 0; x < cityWidth; x++)
            {
                for (int z = 0; z < cityLength; z++)
                {
                    foreach (var blockDimension in cityConfig.customBlockDimensions)
                    {
                        bool bProcess = random.NextFloat() < blockDimension.probability;
                        if (!bProcess) continue;

                        int BlockWidth = blockDimension.sizeX;
                        int BlockHeight = blockDimension.sizeZ;
                        
                        InsertHouseDelegate InsertHouse = delegate() {
                            if (CanContainBiggerHouse(x, z, BlockWidth, BlockHeight))
                            {
                                if (random.NextFloat() < cityConfig.biggerHouseProbability)
                                {
                                    InsertBiggerHouse(x, z, BlockWidth, BlockHeight, 0, blockDimension.markerName);
                                }
                            }
                        };


                        InsertHouseDelegate InsertHouse90 = delegate ()
                        {
                            // Try the 90 degrees rotated version
                            if (CanContainBiggerHouse(x, z, BlockHeight, BlockWidth))
                            {
                                if (random.NextFloat() < cityConfig.biggerHouseProbability)
                                {
                                    InsertBiggerHouse(x, z, BlockHeight, BlockWidth, 90, blockDimension.markerName);
                                }
                            }
                        };

                        if (random.NextFloat() < 0.5f)
                        {
                            InsertHouse();
                            InsertHouse90();
                        }
                        else
                        {
                            InsertHouse90();
                            InsertHouse();
                        }

                    }
                    
                }
            }


            for (int x = 0; x < cityWidth; x++)
            {
                for (int z = 0; z < cityLength; z++)
                {
                    var cell = cityModel.Cells[x, z];
                    if (cell.CellType == SimpleCityCellType.House)
                    {
                        FaceHouseTowardsRoad(cell);
                    }
                }
            }


            // Create padding cells

            var padding = cityConfig.cityWallPadding;
            var paddedCells = new List<SimpleCityCell>();

            for (int p = 1; p <= padding; p++)
            {
                var currentPadding = p;

                var sx = -currentPadding;
                var sz = -currentPadding;
                var ex = cityWidth + currentPadding - 1;
                var ez = cityLength + currentPadding - 1;

                // Fill it with city wall padding marker
                for (int x = sx; x < ex; x++)
                {
                    SimpleCityCellType cellType = SimpleCityCellType.CityWallPadding;
                    
                    paddedCells.Add(CreateCell(x, sz, cellType));
                    paddedCells.Add(CreateCell(x, ez, cellType));
                }

                for (int z = sz; z < ez; z++)
                {
                    SimpleCityCellType cellType = SimpleCityCellType.CityWallPadding;

                    paddedCells.Add(CreateCell(sx, z, cellType));
                    paddedCells.Add(CreateCell(ex, z, cellType));
                }
            }
            cityModel.WallPaddingCells = paddedCells.ToArray();
        }

		void RemoveRoadEdge(int x, int z)
		{
			if (!IsStraightRoad(x, z)) {
				// Nothing to remove
				return;
			}

			var RoadsToRemove = new HashSet<IntVector>();
			RoadsToRemove.Add(new IntVector(x, 0, z));
			int index = x - 1;
			while (IsStraightRoad(index, z)) {
				RoadsToRemove.Add(new IntVector(index, 0, z));
				index--;
			}
			index = x + 1;
			while (IsStraightRoad(index, z)) {
				RoadsToRemove.Add(new IntVector(index, 0, z));
				index++;
			}

			index = z - 1;
			while (IsStraightRoad(x, index)) {
				RoadsToRemove.Add(new IntVector(x, 0, index));
				index--;
			}
			index = z + 1;
			while (IsStraightRoad(x, index)) {
				RoadsToRemove.Add(new IntVector(x, 0, index));
				index++;
			}

			foreach (IntVector Position in RoadsToRemove) {
				SimpleCityCell Cell = cityModel.Cells[Position.x, Position.z];
				Cell.CellType = SimpleCityCellType.House;
			}

		}

		bool IsStraightRoad(int x, int z) {
			if (GetCellType(x, z) != SimpleCityCellType.Road) {
				return false;
			}

			bool bTop = GetCellType(x, z - 1) == SimpleCityCellType.Road;
			bool bBottom = GetCellType(x, z + 1) == SimpleCityCellType.Road;
			bool bLeft = GetCellType(x - 1, z) == SimpleCityCellType.Road;
			bool bRight = GetCellType(x + 1, z) == SimpleCityCellType.Road;

			bool bHorizontal = bLeft && bRight;
			bool bVertical = bTop && bBottom;

			int Adjacent = 0;
			if (bTop) Adjacent++;
			if (bBottom) Adjacent++;
			if (bLeft) Adjacent++;
			if (bRight) Adjacent++;

			if (Adjacent != 2) return false;

			return bHorizontal || bVertical;
		}

		void RemoveRoadEdges() {
			int Width = cityModel.CityWidth;
			int Length = cityModel.CityHeight;

			for (int x = 0; x < Width; x++) {
				for (int z = 0; z < Length; z++) {
					if (IsStraightRoad(x, z)) {
						bool bRemove = random.NextFloat() < cityConfig.roadEdgeRemovalProbability;
						if (bRemove) {
							RemoveRoadEdge(x, z);
						}
					}
				}
			}
			/*
			// Remove the isolated road cells
			for (int x = 0; x < Width; x++) {
				for (int z = 0; z < Length; z++) {
					if (GetCellType(x, z) == SimpleCityCellType.Road) {
						int Adjacent = 0;
						if (GetCellType(x, z - 1) == SimpleCityCellType.Road) Adjacent++;
						if (GetCellType(x, z + 1) == SimpleCityCellType.Road) Adjacent++;
						if (GetCellType(x - 1, z) == SimpleCityCellType.Road) Adjacent++;
						if (GetCellType(x + 1, z) == SimpleCityCellType.Road) Adjacent++;
						if (Adjacent == 0) {
							// No adjacent roads connecting to this road cell. remove it
							SimpleCityCell Cell = demoModel.Cells[x, z];
							Cell.CellType = SimpleCityCellType.House;
						}
					}
				}
			}
			*/
		}

        SimpleCityCell CreateCell(int x, int z, SimpleCityCellType cellType)
        {
            var cell = new SimpleCityCell();
            cell.Position = new IntVector(x, 0, z);
            cell.CellType = cellType;
            cell.Rotation = Quaternion.identity;
            return cell;
        }

        SimpleCityCellType GetCellType(int x, int z) {
            if (x < 0 || x >= cityModel.Cells.GetLength(0) ||
                    z < 0 || z >= cityModel.Cells.GetLength(1)) {
                return SimpleCityCellType.Empty;
            }
            return cityModel.Cells[x, z].CellType;
        }

        void FaceHouseTowardsRoad(SimpleCityCell cell) {
            int x = cell.Position.x;
            int z = cell.Position.z;

            bool roadLeft = GetCellType(x - 1, z) == SimpleCityCellType.Road;
            bool roadRight = GetCellType(x + 1, z) == SimpleCityCellType.Road;
            bool roadTop = GetCellType(x, z - 1) == SimpleCityCellType.Road;
            bool roadBottom = GetCellType(x, z + 1) == SimpleCityCellType.Road;
            
            if (!roadLeft && !roadRight && !roadTop && !roadBottom) {
                // No roads nearby. promote to park
                cell.CellType = SimpleCityCellType.Park;
                cell.Rotation = Quaternion.Euler(0, 90 * (random.Next() % 4), 0);
                return;
            }

            float angle = 0;
            if (roadLeft) angle = 0;
            else if (roadRight) angle = 180;
            else if (roadTop) angle = 270;
            else if (roadBottom) angle = 90;

            cell.Rotation = Quaternion.Euler(0, angle, 0);
        }

        /// <summary>
        /// Make sure the 2x2 grid is occupied by 4 1x1 houses, so we can replace theme with a single bigger house
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        bool CanContainBiggerHouse(int x, int z, int w, int h)
        {
            int cityWidth = cityModel.Cells.GetLength(0);
            int cityLength = cityModel.Cells.GetLength(1);
            for (int dx = 0; dx < w; dx++)
            {
                for (int dz = 0; dz < h; dz++)
                {
                    if (x + dx >= cityWidth || z + dz >= cityLength)
                    {
                        return false;
                    }

                    var cell = cityModel.Cells[x + dx, z + dz];
                    if (cell.CellType != SimpleCityCellType.House)
                    {
                        return false;
                    }
                }
            }

            // The house can fit in this area.
            // Make sure this house is connected to the road
            bool connectedToRoad = IsConnectedToRoad(x, z, w, h);
            return connectedToRoad ;
        }
        
        bool IsConnectedToRoad(int x, int z, int w, int h)
        {
            int cityWidth = cityModel.Cells.GetLength(0);
            int cityLength = cityModel.Cells.GetLength(1);

            var samplePoints = new List<IntVector>();
            for (int dx = 0; dx < w; dx++)
            {
                int[] dz = new int[] { z - 1, z + h };
                for (int dzi = 0; dzi < 2; dzi++)
                {
                    int xx = x + dx;
                    int zz = dz[dzi];
                    samplePoints.Add(new IntVector(xx, 0, zz));
                }
            }
            
            for (int dz = 0; dz < w; dz++)
            {
                int[] dx = new int[] { x - 1, x + w };
                for (int dxi = 0; dxi < 2; dxi++)
                {
                    int xx = dx[dxi];
                    int zz = z + dz;
                    samplePoints.Add(new IntVector(xx, 0, zz));
                }
            }

            foreach (var samplePoint in samplePoints)
            {
                int xx = samplePoint.x;
                int zz = samplePoint.z;
                if (xx < 0 || xx >= cityWidth || zz < 0 || zz >= cityLength)
                {
                    continue;
                }

                var cell = cityModel.Cells[xx, zz];
                if (cell.CellType == SimpleCityCellType.Road)
                {
                    // Connected to a road
                    return true;
                }
            }
            
            // No adjacent road cells found
            return false;
        }
        /// <summary>
        /// Replaces the 4 1x1 smaller houses with a single 2x2 bigger house.  Assumes that there are 4 houses in x,z to x+1,z+1
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        void InsertBiggerHouse(int x, int z, int w, int h, float Angle, string markerName)
        {
            for (int dx = 0; dx < w; dx++)
            {
                for (int dz = 0; dz < h; dz++)
                {
                    var cell = cityModel.Cells[x + dx, z + dz];
                    if (dx == 0 && dz == 0)
                    {
                        cell.CellType = SimpleCityCellType.UserDefined;
                        cell.Rotation = Quaternion.Euler(0, Angle, 0);
                        cell.BlockSize = new Vector3(w, 0, h);
                        cell.MarkerNameOverride = markerName;
                    }
                    else
                    {
                        // Make these cells empty, as they will be occupied by the bigger house and we don't want any markers here
                        cell.CellType = SimpleCityCellType.Empty;
                    }
                }
            }
        }

        /// <summary>
        /// Turns a house cell into a road
        /// </summary>
        /// <param name="cell"></param>
        void MakeRoad(int x, int z, bool horizontal)
        {
            var dx = horizontal ? 0 : 1;
            var dz = horizontal ? 1 : 0;
            for (int d = 0; d < cityConfig.roadWidth; d++)
            {
                var ix = x + d * dx;
                var iz = z + d * dz;
                ix = Mathf.Clamp(ix, 0, cityModel.CityWidth - 1);
                iz = Mathf.Clamp(iz, 0, cityModel.CityHeight - 1);
                var cell = cityModel.Cells[ix, iz];
                cell.CellType = SimpleCityCellType.Road;
                cell.Rotation = Quaternion.identity;
            }
        }

        /// <summary>
        /// Emit marker points so that the theme can decorate the scene layout that we just built
        /// </summary>
        void EmitCityMarkers()
        {
            var basePosition = transform.position;
            var cells = cityModel.Cells;
            var width = cells.GetLength(0);
            var length = cells.GetLength(1);
            var cellSize = new Vector3(cityConfig.CellSize.x, 0, cityConfig.CellSize.y);

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < length; z++)
                {
                    var cell = cells[x, z];
                    string markerName = "Unknown";

                    Quaternion rotation = Quaternion.identity;
                    var worldPosition = cell.Position * cellSize + basePosition;

                    if (cell.CellType == SimpleCityCellType.House)
                    {
                        markerName = SimpleCityDungeonMarkerNames.House;
                        rotation = cell.Rotation;
                    }
                    else if (cell.CellType == SimpleCityCellType.UserDefined)
                    {
                        markerName = cell.MarkerNameOverride;
                        worldPosition += Vector3.Scale(cell.BlockSize / 2.0f - new Vector3(0.5f, 0, 0.5f), cellSize);
                        rotation = cell.Rotation;
                    }
                    else if (cell.CellType == SimpleCityCellType.Park)
                    {
                        markerName = SimpleCityDungeonMarkerNames.Park;
                        rotation = cell.Rotation;
                    }
                    else if (cell.CellType == SimpleCityCellType.Road)
                    {
                        float angle = 0;
                        markerName = RoadBeautifier.GetRoadMarkerName(x, z, cells, out angle);
                        rotation = Quaternion.Euler(0, angle, 0);
                    }

                    var markerTransform = Matrix4x4.TRS(worldPosition, rotation, Vector3.one);
                    EmitMarker(markerName, markerTransform, cell.Position, -1);

                    // Emit the generic road marker
                    if (cell.CellType == SimpleCityCellType.Road)
                    {
                        EmitMarker(SimpleCityDungeonMarkerNames.Road, markerTransform, cell.Position, -1);
                    }
                }
            }
        }

        void EmitBoundaryMarkers()
        {
            var config = cityModel.Config;
            var cells = cityModel.Cells;

            var padding = config.cityWallPadding;
            var doorSize = config.cityDoorSize;

            var width = cells.GetLength(0);
            var length = cells.GetLength(1);

            var cellSize = new Vector3(config.CellSize.x, 0, config.CellSize.y);
            for (int p = 1; p <= padding; p++)
            {

                var currentPadding = p;

                var sx = -currentPadding;
                var sz = -currentPadding;
                var ex = width + currentPadding - 1;
                var ez = length + currentPadding - 1;

                if (currentPadding == padding)
                {
                    var halfDoorSize = doorSize / 2.0f;
                    // Insert markers along the 4 wall sides
                    for (float x = sx; x < ex; x++)
                    {
                        if ((int)x == (int)((sx + ex) / 2 - halfDoorSize))
                        {
                            EmitDoorMarker(cellSize, x + halfDoorSize, sz, 0);
                            EmitDoorMarker(cellSize, x + halfDoorSize, ez, 180);
                            x += halfDoorSize;
                            continue;
                        }
                        EmitWallMarker(cellSize, x + 0.5f, sz, 0);
                        EmitWallMarker(cellSize, x + 0.5f, ez, 180);
                    }

                    for (float z = sz; z < ez; z++)
                    {
                        if ((int)z == (int)((sz + ez) / 2 - halfDoorSize))
                        {
                            EmitDoorMarker(cellSize, sx, z + halfDoorSize, 90);
                            EmitDoorMarker(cellSize, ex, z + halfDoorSize, 270);
                            z += halfDoorSize;
                            continue;
                        }
                        EmitWallMarker(cellSize, sx, z + 0.5f, 90);
                        EmitWallMarker(cellSize, ex, z + 0.5f, 270);
                    }


                    EmitMarkerAt(cellSize, SimpleCityDungeonMarkerNames.CornerTower, sx, sz, 0);
                    EmitMarkerAt(cellSize, SimpleCityDungeonMarkerNames.CornerTower, ex + 0.5f, sz, 0);
                    EmitMarkerAt(cellSize, SimpleCityDungeonMarkerNames.CornerTower, sx, ez + 0.5f, 0);
                    EmitMarkerAt(cellSize, SimpleCityDungeonMarkerNames.CornerTower, ex + 0.5f, ez + 0.5f, 0);
                }
                else
                {
                    // Fill it with city wall padding marker
                    for (float x = sx; x < ex; x++)
                    {
                        EmitMarkerAt(cellSize, SimpleCityDungeonMarkerNames.CityWallPadding, x + 0.5f, sz, 0);
                        EmitMarkerAt(cellSize, SimpleCityDungeonMarkerNames.CityWallPadding, x + 0.5f, ez + 0.5f, 180);
                    }

                    for (float z = sz; z < ez; z++)
                    {
                        EmitMarkerAt(cellSize, SimpleCityDungeonMarkerNames.CityWallPadding, sx, z + 0.5f, 90);
                        EmitMarkerAt(cellSize, SimpleCityDungeonMarkerNames.CityWallPadding, ex + 0.5f, z + 0.5f, 270);
                    }
                }
            }

            // Emit a ground marker since the city builder doesn't emit any ground.  
            // The theme can add a plane here if desired (won't be needed if building on a landscape)
            EmitGroundMarker(width, length, cellSize);

        }

        void EmitWallMarker(Vector3 cellSize, float x, float z, float angle)
        {
            EmitMarkerAt(cellSize, SimpleCityDungeonMarkerNames.CityWall, x, z, angle);
        }

        void EmitDoorMarker(Vector3 cellSize, float x, float z, float angle)
        {
            EmitMarkerAt(cellSize, SimpleCityDungeonMarkerNames.CityDoor, x, z, angle);
        }

        void EmitGroundMarker(int sizeX, int sizeZ, Vector3 cellSize)
        {
            var position = Vector3.Scale(new Vector3(sizeX, 0, sizeZ) / 2.0f, cellSize) + transform.position;
            var scale = new Vector3(sizeX, 1, sizeZ);
            var trans = Matrix4x4.TRS(position, Quaternion.identity, scale);
            EmitMarker(SimpleCityDungeonMarkerNames.CityGround, trans, IntVector.Zero, -1);
        }

        void EmitMarkerAt(Vector3 cellSize, string markerName, float x, float z, float angle)
        {
            var worldPosition = Vector3.Scale(new Vector3(x, 0, z), cellSize) + transform.position;
            var rotation = Quaternion.Euler(0, angle, 0);
            var transformation = Matrix4x4.TRS(worldPosition, rotation, Vector3.one);
            var gridPosition = new IntVector((int)x, 0, (int)z); // Optionally provide where this marker is in the grid position
            EmitMarker(markerName, transformation, gridPosition, -1);
        }

        Quaternion GetRandomRotation()
        {
            // Randomly rotate in steps of 90
            var angle = random.Next(0, 4) * 90;
            return Quaternion.Euler(0, angle, 0);
        }

        int GetRandomBlockSize()
        {
            return random.Next(cityConfig.minBlockSize, cityConfig.maxBlockSize + 1);
        }
    }
}