//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
namespace DungeonArchitect.Builders.Isaac
{
    public class IsaacBuilderUtils
    {

        public static IsaacRoomTile GetTileAt(int x, int z, IsaacRoomLayout layout)
        {
            if (x < 0 || x >= layout.Tiles.GetLength(0) || z < 0 || z >= layout.Tiles.GetLength(1))
            {
                var invalidTile = new IsaacRoomTile();
                invalidTile.tileType = IsaacRoomTileType.Empty;
                return invalidTile;
            }
            return layout.Tiles[x, z];
        }


        public static bool ContainsDoorAt(int x, int z, IsaacRoom room)
        {
            return room.doorPositions.Contains(new IntVector(x, 0, z));
        }

        public static IsaacRoom GetRoom(IsaacDungeonModel model, int roomId)
        {
            foreach (var room in model.rooms)
            {
                if (room.roomId == roomId)
                {
                    return room;
                }
            }
            return null;
        }
    }
}
