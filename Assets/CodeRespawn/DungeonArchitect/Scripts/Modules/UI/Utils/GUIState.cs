//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.UI
{
    public class GUIState
    {
        Color color;
        Color backgroundColor;
        UIRenderer renderer;
        public GUIState(UIRenderer renderer)
        {
            this.renderer = renderer;
            Save();
        }

        public void Save()
        {
            if (renderer != null)
            {
                color = renderer.color;
                backgroundColor = renderer.backgroundColor;
            }
        }

        public void Restore()
        {
            if (renderer != null)
            {
                renderer.color = color;
                renderer.backgroundColor = backgroundColor;
            }
        }
    }
}
