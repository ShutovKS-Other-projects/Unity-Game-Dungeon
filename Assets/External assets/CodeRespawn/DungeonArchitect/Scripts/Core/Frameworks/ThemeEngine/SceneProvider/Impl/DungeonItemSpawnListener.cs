//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect
{
    /// <summary>
    /// Subclass this on your script and drop it on your dungeon game object to 
    /// start inserting your own custom data into the spawned dungeon items
    /// </summary>
    public class DungeonItemSpawnListener : MonoBehaviour
    {
        public virtual void SetMetadata(GameObject dungeonItem, DungeonNodeSpawnData spawnData) { }
    }
}
