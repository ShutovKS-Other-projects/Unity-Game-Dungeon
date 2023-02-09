//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect.Utils;

namespace DungeonArchitect.Splatmap
{
    public class SplatData
    {
        float[,] data;
        public float this[IntVector2 coords]
        {
            get
            {
                return data[coords.x, coords.y];
            }
            set
            {
                data[coords.x, coords.y] = value;
            }
        }

        public float[,] Data
        {
            get { return data; }
        }

        public SplatData(int textureSize)
        {
            data = new float[textureSize, textureSize];
        }

        public void Write(Texture2D texture)
        {
            int numPixels = texture.width * texture.height;
            var pixels = new Color32[numPixels];

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    //float weight = x / (float)roadmap.width;
                    float weight = data[x, y];

                    byte alpha = MathUtils.ToByte(weight);
                    int index = y * texture.width + x;
                    var color = new Color32(0, 0, 0, alpha);
                    pixels[index] = color;
                }
            }

            texture.SetPixels32(pixels);
        }
    }
}
