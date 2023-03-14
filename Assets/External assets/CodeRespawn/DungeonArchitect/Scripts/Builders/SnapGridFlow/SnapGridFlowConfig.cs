//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Flow.Impl.SnapGridFlow;
using UnityEngine;

namespace DungeonArchitect.Builders.SnapGridFlow
{
    public class SnapGridFlowConfig : DungeonConfig
    {
        public SnapGridFlowAsset flowGraph;
        public SnapGridFlowModuleDatabase moduleDatabase;
        
        [Tooltip(@"If the flow graph cannot converge to a solution, retry again this many times.  Usually a dungeon converges within 1-10 tries, depending on how you've designed the flow graph")]
        public int numGraphRetries = 100;

        [Tooltip(@"Items spawned using the SpawnItem node will be placed under their individual room prefabs. This is useful for spawning NPCs under their respective rooms so they can be hidden when the room goes too far away (using visibility graph). However, you'll need to take care of de-parenting the player prefab and placing it back in the root so it doesn't get hidden when the spawn room is streamed out as you move in the dungeon. Check the sample for more info")]
        public bool spawnItemsUnderRoomPrefabs = false;

        [Tooltip(@"The max processing power allotted to the module resolver")]
        public int maxResolverFrames = 10000;
        
        [Tooltip(@"The system tries to not repeat a module within the last few room depths specified below")]
        public int nonRepeatingRooms = 3;
        
        public override bool HasValidConfig(ref string errorMessage)
        {
            if (flowGraph == null)
            {
                errorMessage = "Flow Graph asset is not assigned";
                return false;
            }
            
            if (moduleDatabase == null)
            {
                errorMessage = "Module Database asset is not assigned";
                return false;
            }

            return true;
        }
        
    }
}