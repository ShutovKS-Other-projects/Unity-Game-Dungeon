//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Flow.Domains.Tilemap;
using DungeonArchitect.Flow.Items;
using UnityEngine;

namespace DungeonArchitect.Flow.Impl.GridFlow
{
    public class TilemapItemPlacementStrategyScript : ITilemapItemPlacementStrategy
    {
        public bool PlaceItems(FlowItem item, FlowTilemapCell[] freeCells, TilemapItemPlacementSettings settings,
                TilemapItemPlacementStrategyContext context, ref int outFreeTileIndex, ref string errorMessage)
        {
            if (settings.placementScriptClass != null && settings.placementScriptClass.Length > 0)
            {
                var type = System.Type.GetType(settings.placementScriptClass);
                if (type != null)
                {
                    var script = ScriptableObject.CreateInstance(type) as ITilemapItemPlacementStrategy;
                    if (script != null)
                    {
                        return script.PlaceItems(item, freeCells, settings, context, ref outFreeTileIndex, ref errorMessage);
                    }
                }
            }

            errorMessage = "Invalid script reference";
            return false;
        }

    }
}
