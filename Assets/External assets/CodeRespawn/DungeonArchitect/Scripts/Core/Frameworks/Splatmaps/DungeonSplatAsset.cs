//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Splatmap
{
    public class DungeonSplatAsset : ScriptableObject
    {
        [SerializeField]
        public Texture2D[] splatTextures = new Texture2D[0];
    }
}
