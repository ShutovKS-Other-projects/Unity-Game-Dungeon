//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect
{
    /// <summary>
    /// Negation volumes remove procedural geometries from the scene that lie with it's bounds
    /// </summary>
    [ExecuteInEditMode]
    public class NegationVolume : Volume
    {
        public bool inverse = false;

        void Awake()
        {
            COLOR_WIRE = new Color(1, 0.5f, 0, 1);
            COLOR_SOLID_DESELECTED = new Color(1, 0.5f, 0, 0.0f);
            COLOR_SOLID = new Color(1, 0.5f, 0, 0.1f);
        }
    }
}
