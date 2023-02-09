//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;


namespace DungeonArchitect.Utils
{
    public class TexturedMaterialInstances
    {
        Shader shader = null;
        public TexturedMaterialInstances(Shader shader)
        {
            this.shader = shader;
        }

        Dictionary<Texture2D, Material> materialsByTexture = new Dictionary<Texture2D, Material>();
        public Material GetMaterial(Texture2D texture)
        {
            if (!materialsByTexture.ContainsKey(texture))
            {
                var material = new Material(shader);
                material.mainTexture = texture;

                materialsByTexture[texture] = material;
            }
            return materialsByTexture[texture];
        }
    }
}
