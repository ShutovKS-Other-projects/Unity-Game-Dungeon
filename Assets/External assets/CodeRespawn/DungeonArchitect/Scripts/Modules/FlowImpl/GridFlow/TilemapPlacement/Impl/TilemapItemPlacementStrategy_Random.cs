//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using DungeonArchitect.Flow.Domains.Tilemap;
using DungeonArchitect.Flow.Items;

namespace DungeonArchitect.Flow.Impl.GridFlow
{
    public class TilemapItemPlacementStrategyRandom : ITilemapItemPlacementStrategy
    {
        public bool PlaceItems(FlowItem item, FlowTilemapCell[] freeCells, TilemapItemPlacementSettings settings, TilemapItemPlacementStrategyContext context, ref int outFreeTileIndex, ref string errorMessage)
        {
            var freeCellIndexRef = new List<int>();
            for (int i = 0; i < freeCells.Length; i++) 
            {
                var freeCell = freeCells[i];
                var x = freeCell.TileCoord.x;
                var y = freeCell.TileCoord.y;
                var distanceCell = context.distanceField.distanceCells[x, y];
                if (!settings.avoidPlacingNextToDoors || distanceCell.DistanceFromDoor > 1)
                {
                    freeCellIndexRef.Add(i);
                }
            }

            if (freeCellIndexRef.Count == 0)
            {
                // Not enough free cells for placing the items
                errorMessage = "Insufficient free tiles";
                return false;
            }

            var freeCellTableIndex = context.random.Next(freeCellIndexRef.Count - 1);
            outFreeTileIndex = freeCellIndexRef[freeCellTableIndex];
            return true;
        }
    }
}
