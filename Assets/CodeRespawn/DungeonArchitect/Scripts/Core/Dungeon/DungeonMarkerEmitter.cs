//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect
{
    /// <summary>
    /// Marker Emitters let you emit your own markers anywhere in the map.  Implement this class and add it to the Dungeon object
    /// to add your own markers right after the dungeon layout is created
    /// </summary>
    public class DungeonMarkerEmitter : MonoBehaviour {
        /// <summary>
        /// Called by the dungeon object right after the dungeon is created
        /// </summary>
        /// <param name="builder">reference to the builder object used to build the dungeon</param>
        public virtual void EmitMarkers(DungeonBuilder builder)
        {
        }
    }
}