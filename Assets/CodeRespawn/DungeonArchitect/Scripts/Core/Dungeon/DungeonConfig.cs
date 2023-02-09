//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect
{
    /// <summary>
    /// Base dungeon configuration.  Create your own implementation of this configuration based on your dungeon builder's needs
    /// </summary>
	public class DungeonConfig : MonoBehaviour {
        [Tooltip(@"Change this number to completely change the layout of your level")]
        public uint Seed = 0;

        public virtual bool HasValidConfig(ref string errorMessage)
        {
            return true;
        }

        public virtual bool IsMode2D()
        {
            return false;
        }
    }
}
