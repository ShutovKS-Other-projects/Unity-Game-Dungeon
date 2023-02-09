//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Builders.Grid
{
    [System.Serializable]
    public enum GridDungeonWallType
    {
        WallsAsEdges,
        WallsAsTileBlocks
    }

    /// <summary>
    /// The dungeon configuration for the Grid builder
    /// </summary>
    public class GridDungeonConfig : DungeonConfig
    {
        /// <summary>
        /// This dungeon builder works on a grid based system and required modular mesh assets 
        /// to be placed on each cell (floors, walls, doors etc). This important field specifies the size
        /// of the cell to use. This size is determined by the art asset used in the dungeon theme 
        /// designed by the artist. In the demo, we have a floor mesh that is 400x400. The height
        /// of a floor is chosen to be 200 units as the stair mesh is 200 units high. Hence the
        /// defaults are set to 400x400x200. You should change this to the dimension of the modular
        /// asset your designer has created for the dungeon
        /// </summary>
        [Tooltip(@"This dungeon builder works on a grid based system and required modular mesh assets to be placed on each cell (floors, walls, doors etc). This important field specifies the size of the cell to use. This size is determined by the art asset used in the dungeon theme designed by the artist. In the demo, we have a floor mesh that is 400x400. The height of a floor is chosen to be 200 units as the stair mesh is 200 units high. Hence the defaults are set to 400x400x200. You should change this to the dimension of the modular asset your designer has created for the dungeon")]
        public Vector3 GridCellSize = new Vector3(4, 2, 4);

        /// <summary>
        /// Changing this number would completely change the layout of the dungeon. 
        /// This is the base random number seed that is used to build the dungeon. 
        /// There is a convenience function to randomize this value (button labeled R)
        /// </summary>
        [Tooltip(@"Changing this number would completely change the layout of the dungeon. This is the base random number seed that is used to build the dungeon. There is a convenience function to randomize this value (button labeled R)")]
        public int NumCells = 150;

        /// <summary>
        /// This is how small a cell size can be. While generation, a cell is either converted
        /// to a room, corridor or is discarded completely. The Cell width / height is randomly 
        /// chosen within this range
        /// </summary>
        [Tooltip(@"This is how small a cell size can be. While generation, a cell is either converted to a room, corridor or is discarded completely. The Cell width / height is randomly chosen within this range")]
        public int MinCellSize = 2;

        /// <summary>
        /// This is how big a cell size can be. While generation, a cell is either 
        /// converted to a room, corridor or is discarded completely.
        /// The Cell width / height is randomly chosen within this range
        /// </summary>
        [Tooltip(@"This is how big a cell size can be. While generation, a cell is either converted to a room, corridor or is discarded completely. The Cell width / height is randomly chosen within this range")]
        public int MaxCellSize = 5;

        /// <summary>
        ///  If a cell size exceeds past this limit, it is converted into a room. After cells are
        ///  promoted to rooms, all rooms are connected to each other through corridors
        ///  (either directly or indirectly. See spanning tree later)
        /// </summary>
        [Tooltip(@"If a cell size exceeds past this limit, it is converted into a room. After cells are promoted to rooms, all rooms are connected to each other through corridors (either directly or indirectly. See spanning tree later)")]
        public int RoomAreaThreshold = 15;

        /// <summary>
        /// The aspect ratio of the cells (width to height ratio). Keeping this value near 0 would 
        /// create square rooms. Bringing this close to 1 would create elongated / stretched rooms
        /// with a high width to height ratio
        /// </summary>
        [Tooltip(@"The aspect ratio of the cells (width to height ratio). Keeping this value near 0 would create square rooms. Bringing this close to 1 would create elongated / stretched rooms with a high width to height ratio")]
        public float RoomAspectDelta = 0.4f;

        /// <summary>
        /// The extra width to apply to one side of a corridor
        /// </summary>
        [Tooltip(@"The extra width to apply to one side of a corridor")]
        public int CorridorWidth = 1;
        
        /// <summary>
        /// Tweak this value to increase / reduce the height variations (and stairs)
        /// in your dungeon. A value close to 0 reduces the height variation and increases
        /// as you approach 1. Increasing this value to a higher level might create dungeons 
        /// with no place for proper stair placement since there is too much height variation.
        /// A value of 0.2 to 0.4 seems good
        /// </summary>
        [Tooltip(@"Tweak this value to increase / reduce the height variations (and stairs) in your dungeon. A value close to 0 reduces the height variation and increases as you approach 1. Increasing this value to a higher level might create dungeons  with no place for proper stair placement since there is too much height variation. A value of 0.2 to 0.4 seems good")]
        public float HeightVariationProbability = 0.2f;

        /// <summary>
        /// The number of logical floor units the dungeon height can vary. This determines how 
        /// high the dungeon's height can vary (e.g. max 2 floors high). Set this value depending 
        /// on the stair meshes you designer has created. In the sample demo, there are two stair
        /// meshes, one 200 units high (1 floor) and another 400 units high (2 floors).
        /// So the default is set to 2
        /// </summary>
        [Tooltip(@"The number of logical floor units the dungeon height can vary. This determines how  high the dungeon's height can vary (e.g. max 2 floors high). Set this value depending  on the stair meshes you designer has created. In the sample demo, there are two stair meshes, one 200 units high (1 floor) and another 400 units high (2 floors). So the default is set to 2")]
        public int MaxAllowedStairHeight = 1;

        /// <summary>
        /// Determines how many loops you would like to have in your dungeon. A value near 0 will create
        /// fewer loops creating linear dungeons. A value near 1 would create lots of loops, which would look unoriginal.
        /// Its good to allow a few loops so a value close to zero (like 0.2 should be good)
        /// </summary>
        [Tooltip(@"Determines how many loops you would like to have in your dungeon. A value near 0 will create fewer loops creating linear dungeons. A value near 1 would create lots of loops, which would look unoriginal. Its good to allow a few loops so a value close to zero (like 0.2 should be good)")]
        public float SpanningTreeLoopProbability = 0.15f;

        /// <summary>
        /// The generator would add stairs to make different areas of the dungeon accessible. However, we do not want too
        /// many stairs. For e.g., before adding a stair in a particular elevated area, the generator would check if this
        /// area is already accessible from a nearby stair. If so, it would not add it. This tolerance parameter determines
        /// how far to look for an existing path before we can add a stair. Play with this parameter if you see too many
        /// stairs close to each other, or too few
        /// </summary>
        [Tooltip(@"The generator would add stairs to make different areas of the dungeon accessible. However, we do not want too many stairs. For e.g., before adding a stair in a particular elevated area, the generator would check if this area is already accessible from a nearby stair. If so, it would not add it. This tolerance parameter determines how far to look for an existing path before we can add a stair. Play with this parameter if you see too many stairs close to each other, or too few")]
        public float StairConnectionTollerance = 6;

        /// <summary>
        /// Increase this value to remove nearby duplicate doors.  This value determines how many cell we can move to reach the two connected cells of a door if the door was removed
        /// </summary>
        [Tooltip(@"Increase this value to remove nearby duplicate doors.  This value determines how many cell we can move to reach the two connected cells of a door if the door was removed")]
        public float DoorProximitySteps = 3;
        
        /// <summary>
        /// The random number generator used in the dungeon generator does not use a uniform distribution.
        /// Instead it uses a normal distribution to get higher frequency of lower values and fewer higher values
        /// (and hence fewer room cells and a lot more corridor cells). Play with these parameters for different results
        /// </summary>
        [Tooltip(@"The random number generator used in the dungeon generator does not use a uniform distribution. Instead it uses a normal distribution to get higher frequency of lower values and fewer higher values (and hence fewer room cells and a lot more corridor cells). Play with these parameters for different results")]
        public float NormalMean = 0;

        /// <summary>
        /// The random number generator used in the dungeon generator does not use a uniform distribution.
        /// Instead it uses a normal distribution to get higher frequency of lower values and fewer higher values
        /// (and hence fewer room cells and a lot more corridor cells). Play with these parameters for different results
        /// </summary>
        [Tooltip(@"The random number generator used in the dungeon generator does not use a uniform distribution. Instead it uses a normal distribution to get higher frequency of lower values and fewer higher values (and hence fewer room cells and a lot more corridor cells). Play with these parameters for different results")]
        public float NormalStd = 0.3f;

        /// <summary>
        /// The radius within which to spawn the initial cells before they are separated.
        /// Keep to a low value like 10-15
        /// </summary>
        [Tooltip(@"The radius within which to spawn the initial cells before they are separated. Keep to a low value like 10-15")]
        public float InitialRoomRadius = 15;


        [Tooltip(@"Whether to treat walls as edges or as large tile blocks (like ground tiles)")]
        public GridDungeonWallType WallLayoutType = GridDungeonWallType.WallsAsEdges;

        /// <summary>
        /// __Internal
        /// </summary>
        public int FloorHeight = 0;

        public bool UseFastCellDistribution = false;

        public int CellDistributionWidth = 20;

        public int CellDistributionLength = 30;

        public bool Mode2D = false;
        
        public override bool IsMode2D()
        {
            return Mode2D;
        }
    }
}
