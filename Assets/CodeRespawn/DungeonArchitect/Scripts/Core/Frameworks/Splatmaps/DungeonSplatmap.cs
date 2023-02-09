//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Splatmap
{
    [System.Serializable]
    public struct DungeonSplatmapTextureInfo
    {
        [SerializeField]
        public string id;

        [SerializeField]
        public TextureFormat textureFormat;

        [SerializeField]
        public int textureSize;
    }

    public class DungeonSplatmap : MonoBehaviour
    {
        public DungeonSplatmapTextureInfo[] textures;
        public DungeonSplatAsset splatmap;
    }
}