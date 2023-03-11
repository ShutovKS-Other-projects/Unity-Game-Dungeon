//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;

namespace DungeonArchitect.Builders.Isaac
{
    public class IsaacDungeonBuilder : DungeonBuilder
    {

        IsaacDungeonConfig isaacConfig;
        IsaacDungeonModel isaacModel;

        new System.Random random;


        List<IsaacRoom> rooms = new List<IsaacRoom>();
        List<IsaacDoor> doors = new List<IsaacDoor>();

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
            isaacConfig = config as IsaacDungeonConfig;
            isaacModel = model as IsaacDungeonModel;
            isaacModel.config = isaacConfig;

            // Generate the city layout and save it in a model.   No markers are emitted here. 
            GenerateLevelLayout();
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
            EmitLevelMarkers();
            ProcessMarkerOverrideVolumes();
        }

        struct LevelGrowthNode
        {
            public IsaacRoom room;
            public int moveDirection;
        }

        IntVector[] directions = new IntVector[] {
                new IntVector(1, 0, 0),
                new IntVector(0, 0, 1),
                new IntVector(-1, 0, 0),
                new IntVector(0, 0, -1),
            };

        void GenerateLevelLayout()
        {
            var queue = new Queue<LevelGrowthNode>();
            var visited = new HashSet<IntVector>();

            var roomFactory = new IsaacRoomFactory();
            rooms.Clear();
            doors.Clear();

            var start = new LevelGrowthNode();
            start.room = roomFactory.CreateRoom(IntVector.Zero);
            start.moveDirection = 0;
            rooms.Add(start.room);

            queue.Enqueue(start);
            visited.Add(start.room.position);

            var numRooms = random.Range(isaacConfig.minRooms, isaacConfig.maxRooms);
            bool isSpawnRoom = true;
            while (queue.Count > 0)
            {
                var top = queue.Dequeue();
                if (isSpawnRoom)
                {
                    // in the spawn room.  Spawn on all 4 sides
                    for (int d = 0; d < 4; d++)
                    {
                        AddNextRoomNode(roomFactory, queue, visited, numRooms, top.room, d, isaacConfig.spawnRoomBranchProbablity);
                    }
                    isSpawnRoom = false;
                }
                else
                {
                    // Grow forward
                    AddNextRoomNode(roomFactory, queue, visited, numRooms, top.room, top.moveDirection, isaacConfig.growForwardProbablity);

                    // Grow sideways
                    AddNextRoomNode(roomFactory, queue, visited, numRooms, top.room, (top.moveDirection + 1) % 4, isaacConfig.growSidewaysProbablity);
                    AddNextRoomNode(roomFactory, queue, visited, numRooms, top.room, (top.moveDirection + 3) % 4, isaacConfig.growSidewaysProbablity);
                }

                if (rooms.Count >= numRooms)
                {
                    break;
                }
            }

            // Generate the tile layout of the rooms
            var layoutBuilder = GetComponent<IsaacRoomLayoutBuilder>();
            foreach (var room in rooms)
            {
                GenerateRoomLayout(layoutBuilder, room);
            }

            isaacModel.rooms = rooms.ToArray();
            isaacModel.doors = doors.ToArray();
            rooms.Clear();
            doors.Clear();
        } 

        void AddNextRoomNode(IsaacRoomFactory roomFactory, 
                Queue<LevelGrowthNode> queue, HashSet<IntVector> visited, int maxRooms, 
                IsaacRoom parentRoom, int direction, float probability)
        {
            if (random.NextFloat() > probability) return;
            if (rooms.Count >= maxRooms) return;

            var nextPosition = parentRoom.position + directions[direction];
            if (!visited.Contains(nextPosition))
            {
                var nextRoom = roomFactory.CreateRoom(nextPosition);
                rooms.Add(nextRoom);
                var nextNode = new LevelGrowthNode();
                nextNode.room = nextRoom;
                nextNode.moveDirection = direction;
                queue.Enqueue(nextNode);
                visited.Add(nextPosition);

                // Create a door between the two rooms
                ConnectRoomsWithDoors(parentRoom, nextRoom);
            }
            else
            {
                // See if we can connect to the other room
                // first make sure we don't already have a connection between the two
                var nextRoom = GetRoomAt(nextPosition);
                if (!ContainsDoorBetween(parentRoom.roomId, nextRoom.roomId))
                {
                    float loopTest = random.NextFloat();
                    if (loopTest < isaacConfig.cycleProbability)
                    {
                        // Connect the two rooms together
                        if (nextRoom != null)
                        {
                            // Create a door between the two rooms
                            ConnectRoomsWithDoors(parentRoom, nextRoom);
                        }
                    }
                }
            }
        }


        void ConnectRoomsWithDoors(IsaacRoom roomA, IsaacRoom roomB)
        {
            // Create a door between the two rooms
            roomA.adjacentRooms.Add(roomB.roomId);
            roomB.adjacentRooms.Add(roomA.roomId);
            float doorPositionRatio = random.NextFloat();
            CreateDoor(roomA, roomB, doorPositionRatio);
        }

        IsaacRoom GetRoomAt(IntVector position)
        {
            foreach (var room in rooms)
            {
                if (room.position.Equals(position))
                {
                    return room;
                }
            }
            return null;
        }

        bool ContainsDoorBetween(int roomA, int roomB)
        {
            foreach (var door in doors)
            {
                if (door.roomA == roomA && door.roomB == roomB) return true;
                if (door.roomA == roomB && door.roomB == roomA) return true;
            }
            return false;
        }

        void CreateDoor(IsaacRoom roomA, IsaacRoom roomB, float ratio)
        {
            var door = new IsaacDoor();
            door.roomA = roomA.roomId;
            door.roomB = roomB.roomId;
            door.ratio = ratio;
            doors.Add(door);

            // Create the door tile
            var roomWidth = isaacConfig.roomWidth;
            var roomHeight = isaacConfig.roomHeight;
            bool horizontal = (roomA.position.z - roomB.position.z) == 0; // Are the two room horizontal
            var size = horizontal ? isaacConfig.roomHeight : isaacConfig.roomWidth;
            var location1D = Mathf.FloorToInt(size * door.ratio);

            var leftRoom = roomA;
            var rightRoom = roomB;
            if (horizontal && leftRoom.position.x > rightRoom.position.x)
            {
                // Swap
                leftRoom = roomB;
                rightRoom = roomA;
            }
            else if (!horizontal && leftRoom.position.z > rightRoom.position.z)
            {
                // Swap
                leftRoom = roomB;
                rightRoom = roomA;
            }


            IntVector leftRoomDoor;
            IntVector rightRoomDoor;

            if (horizontal)
            {
                leftRoomDoor = new IntVector(roomWidth, 0, location1D);
                rightRoomDoor = new IntVector(-1, 0, location1D);
            }
            else
            {
                leftRoomDoor = new IntVector(location1D, 0, roomHeight);
                rightRoomDoor = new IntVector(location1D, 0, -1);
            }

            leftRoom.doorPositions.Add(leftRoomDoor);
            rightRoom.doorPositions.Add(rightRoomDoor);
        }

        bool IsWall(int x, int z, IsaacRoomLayout layout)
        {
            var center = IsaacBuilderUtils.GetTileAt(x, z, layout);
            if (center.tileType == IsaacRoomTileType.Floor) return false;
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dz = -1; dz <= 1; dz++)
                {
                    if (dx == 0 && dz == 0) continue;
                    var cell = IsaacBuilderUtils.GetTileAt(x + dx, z + dz, layout);
                    if (cell.tileType == IsaacRoomTileType.Floor)
                    {
                        // Contains an adjacent floor tile
                        return true;
                    }
                }
            }
            return false;
        }

        void GenerateRoomLayout(IsaacRoomLayoutBuilder layoutBuilder, IsaacRoom room)
        {
            IsaacRoomLayout layout;
            if (layoutBuilder == null)
            {
                layout = GenerateEmptyRoomLayout();
            }
            else
            {
                layout = layoutBuilder.GenerateLayout(room, random, isaacConfig.roomWidth, isaacConfig.roomHeight);
            }
            room.layout = layout;
        }

        IsaacRoomLayout GenerateEmptyRoomLayout()
        {
            var layout = new IsaacRoomLayout();
            layout.InitializeTiles(isaacConfig.roomWidth, isaacConfig.roomHeight, IsaacRoomTileType.Floor);
            return layout;
        }
        
        void EmitLevelMarkers()
        {
            var tileSize = new Vector3(isaacConfig.tileSize.x, 0, isaacConfig.tileSize.y);
            var roomSizeWorld = new IntVector(isaacConfig.roomWidth, 0, isaacConfig.roomHeight) * tileSize;
            var roomPadding = new Vector3(isaacConfig.roomPadding.x, 0, isaacConfig.roomPadding.y);
            foreach (var room in isaacModel.rooms)
            {
                var roomBasePosition = room.position * (roomSizeWorld + roomPadding);
                var roomWidth = room.layout.Tiles.GetLength(0);
                var roomHeight = room.layout.Tiles.GetLength(1);
                
                for (int x = -1; x < roomWidth + 1; x++)
                {
                    for (int z = -1; z < roomHeight + 1; z++)
                    {
                        var tilePosition = new IntVector(x, 0, z);
                        var tileOffset = tilePosition * tileSize;
                        var markerPosition = roomBasePosition + tileOffset;
                        var transformation = Matrix4x4.TRS(markerPosition, Quaternion.identity, Vector3.one);
                        var tile = IsaacBuilderUtils.GetTileAt(x, z, room.layout);
                        if (tile.tileType == IsaacRoomTileType.Floor)
                        {
                            EmitMarker(IsaacDungeonMarkerNames.Ground, transformation, tilePosition, room.roomId);
                        }
                        else if (IsaacBuilderUtils.ContainsDoorAt(x, z, room))
                        {
                            EmitMarker(IsaacDungeonMarkerNames.ST_DOOR2D, transformation, tilePosition, room.roomId);
                            EmitMarker(IsaacDungeonMarkerNames.Door, transformation, tilePosition, room.roomId);
                        }
                        else if (IsWall(x, z, room.layout))
                        {
                            EmitMarker(IsaacDungeonMarkerNames.ST_WALL2D, transformation, tilePosition, room.roomId);
                            EmitMarker(IsaacDungeonMarkerNames.Wall, transformation, tilePosition, room.roomId);
                        }
                    }
                }
                
            }
        }
    }

    class IsaacRoomFactory
    {
        int idCounter = 0;

        public IsaacRoom CreateRoom(IntVector position)
        {
            var room = new IsaacRoom();
            room.roomId = idCounter++;
            room.position = position;
            return room;
        }

    }
    
    public static class IsaacDungeonMarkerNames {
        public static readonly string Ground = "Ground";
        public static readonly string Door = "Door";
        public static readonly string Wall = "Wall";
        
        public static readonly string ST_DOOR2D = "Door2D";
        public static readonly string ST_WALL2D = "Wall2D";
    }
}
