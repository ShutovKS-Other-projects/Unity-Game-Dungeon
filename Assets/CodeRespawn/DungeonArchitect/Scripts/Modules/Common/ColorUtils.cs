//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Utils
{
    public class ColorUtils
    {
        public static Color BrightenColor(Color color, float saturationMultiplier, float brightnessMultiplier)
        {
            float H, S, V;
            Color.RGBToHSV(color, out H, out S, out V);
            S *= saturationMultiplier;
            V = Mathf.Clamp01(V * brightnessMultiplier);

            return Color.HSVToRGB(H, S, V);
        }
    }
}
