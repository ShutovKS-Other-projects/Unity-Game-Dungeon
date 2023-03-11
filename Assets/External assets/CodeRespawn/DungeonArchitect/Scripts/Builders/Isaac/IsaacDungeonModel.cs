//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System;
using System.Collections.Generic;

namespace DungeonArchitect.Builders.Isaac
{
    [Serializable]
    public class IsaacRoom
    {
        [HideInInspector]
        public int roomId;

        [HideInInspector]
        public IntVector position;

        [HideInInspector]
        public IsaacRoomLayout layout;

        [HideInInspector]
        public List<int> adjacentRooms = new List<int>();

        [HideInInspector]
        public List<IntVector> doorPositions = new List<IntVector>();
    }

    [Serializable]
    public class IsaacDoor
    {
        [HideInInspector]
        public int roomA;

        [HideInInspector]
        public int roomB;

        [HideInInspector]
        public float ratio = 0.5f;
    }

    [Serializable]
    public class IsaacRoomLayout
    {
        [HideInInspector]
        public IsaacRoomTile[,] Tiles = new IsaacRoomTile[0, 0];

        public void InitializeTiles(int width, int height, IsaacRoomTileType tileType)
        {
            Tiles = new IsaacRoomTile[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    var tile = new IsaacRoomTile();
                    tile.tileType = tileType;
                    Tiles[x, z] = tile;
                }
            }
        }
    }

    [Serializable]
    public class IsaacRoomTile
    {
        public IsaacRoomTileType tileType;
    }

    public enum IsaacRoomTileType
    {
        Floor,
        Door,
        Empty
    }

    public class IsaacDungeonModel : DungeonModel
    {
        [HideInInspector]
        public IsaacDungeonConfig config;

        [HideInInspector]
        public IsaacRoom[] rooms;

        [HideInInspector]
        public IsaacDoor[] doors;
    }
}
