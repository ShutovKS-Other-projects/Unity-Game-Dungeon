//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;


namespace DungeonArchitect
{
    [System.Serializable]
    public class MarkerReplacementEntry
    {
        public string fromMarker;
        public string toMarker;
    }

    /// <summary>
    /// This volume replaces any specified markers found in the scene before theming is applied to it. 
    /// This helps in having more control over the generated dungeon, e.g. remove / add doors, walls etc
    /// </summary>
    [ExecuteInEditMode]
    public class MarkerReplaceVolume : Volume
    {
        void Awake()
        {
            COLOR_WIRE = new Color(1, 0.25f, 0.5f, 1);
            COLOR_SOLID_DESELECTED = new Color(1, 0.25f, 0.5f, 0.0f);
            COLOR_SOLID = new Color(1, 0.25f, 0.5f, 0.1f);
        }

        public MarkerReplacementEntry[] replacements;
    }
}