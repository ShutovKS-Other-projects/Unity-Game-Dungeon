//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect.Graphs;

namespace DungeonArchitect
{
    /// <summary>
    /// Dungeon layout that lies within this volumes bounds picks up the theme set in this volume
    /// </summary>
    public class ThemeOverrideVolume : Volume
    {
        public Graph overrideTheme;

        /// <summary>
        /// Uses the base theme's markers if the overriden them doesn't have any game objects for a marker
        /// </summary>
        public bool useBaseThemeForMissingMarkers = false;

        void Awake()
        {
            COLOR_WIRE = new Color(0.1f, 0.5f, 1, 1);
            COLOR_SOLID_DESELECTED = new Color(0, 0.5f, 1, 0.0f);
            COLOR_SOLID = new Color(0, 0.5f, 1, 0.1f);
        }
    }
}
