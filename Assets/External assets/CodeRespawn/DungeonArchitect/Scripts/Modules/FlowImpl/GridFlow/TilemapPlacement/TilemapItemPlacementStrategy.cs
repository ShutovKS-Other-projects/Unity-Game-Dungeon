//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Flow.Domains;
using DungeonArchitect.Flow.Domains.Tilemap;
using DungeonArchitect.Flow.Items;

namespace DungeonArchitect.Flow.Impl.GridFlow
{
    public class TilemapItemPlacementStrategyContext
    {
        public FlowTilemap tilemap;
        public FlowTilemapDistanceField distanceField;
        public System.Random random;
    }

    public interface ITilemapItemPlacementStrategy
    {
        bool PlaceItems(FlowItem item, FlowTilemapCell[] freeCells, TilemapItemPlacementSettings settings, TilemapItemPlacementStrategyContext context, ref int outFreeTileIndex, ref string errorMessage);
    }

    [System.Serializable]
    public enum TilemapItemPlacementMethod
    {
        RandomTile,
        NearEdges,
        Script
    }

    [System.Serializable]
    public class TilemapItemPlacementSettings : IFlowDomainData
    {
        public TilemapItemPlacementMethod placementMethod = TilemapItemPlacementMethod.RandomTile;
        public bool avoidPlacingNextToDoors = true;
        public string placementScriptClass = "";
        public bool fallbackToRandomPlacement = true;

        public IFlowDomainData Clone()
        {
            var newObj = new TilemapItemPlacementSettings();
            newObj.placementMethod = placementMethod;
            newObj.avoidPlacingNextToDoors = avoidPlacingNextToDoors;
            newObj.placementScriptClass = placementScriptClass;
            newObj.fallbackToRandomPlacement = fallbackToRandomPlacement;
            return newObj;
        }
    }

    public class TilemapItemPlacementStrategyFactory
    {
        public static ITilemapItemPlacementStrategy Create(TilemapItemPlacementMethod method)
        {
            if (method == TilemapItemPlacementMethod.NearEdges)
            {
                return new TilemapItemPlacementStrategyNearEdge();
            }
            else if (method == TilemapItemPlacementMethod.Script)
            {
                return new TilemapItemPlacementStrategyScript();
            }
            else if (method == TilemapItemPlacementMethod.RandomTile)
            {
                return new TilemapItemPlacementStrategyRandom();
            }
            else
            {
                return new TilemapItemPlacementStrategyRandom();
            }
        }
    }

    public class TilemapItemPlacementStrategyUtils
    {
        public static bool Validate(TilemapItemPlacementSettings settings, ref string errorMessage)
        {
            if (settings.placementMethod == TilemapItemPlacementMethod.Script)
            {
                if (settings.placementScriptClass == null || settings.placementScriptClass.Length == 0)
                {
                    errorMessage = "Invalid script reference";
                    return false;
                }
            }
            return true;
        }
    }
}
