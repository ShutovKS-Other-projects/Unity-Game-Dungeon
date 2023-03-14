//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Flow.Impl.GridFlow;
using UnityEngine;

namespace DungeonArchitect.Builders.GridFlow
{
    public class GridFlowDungeonConfig : DungeonConfig
    {
        public GridFlowAsset flowAsset;
        public Vector3 gridSize = new Vector3(4, 4, 4);
        
        [Tooltip(@"If the flow graph cannot converge to a solution, retry again this many times.  Usually a dungeon converges within 1-10 tries, depending on how you've designed the flow graph")]
        public int numGraphRetries = 100;
        
        public bool Mode2D = false;

        // Advanced properties
        [Tooltip(@"If using Walls as Edges, rotates the walls by 180 along Y to make your grid builder themes work consistently with this grid flow builder")]
        public bool flipEdgeWalls = false;
        
        public override bool IsMode2D()
        {
            return Mode2D;
        }
        
        public override bool HasValidConfig(ref string errorMessage)
        {
            if (flowAsset == null)
            {
                errorMessage = "Flow Asset is not assign in the configuration";
                return false;
            }
            return true;
        }

    }
}
