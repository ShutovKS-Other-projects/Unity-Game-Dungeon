//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Splatmap
{
    public class SplatRasterizer
    {
        public SplatRasterizer(SplatData splatData)
        {
            this.splatData = splatData;
        }

        private SplatData splatData;

        struct FloodFillPixelInfo
        {
            public FloodFillPixelInfo(IntVector2 currentPixel, IntVector2 sourcePixel, float startValue) {
                this.currentPixel = currentPixel;
                this.sourcePixel = sourcePixel;
                this.startValue = startValue;
            }

            public IntVector2 currentPixel;
            public IntVector2 sourcePixel;
            public float startValue;
            
            public float DistanceToSource()
            {
                return (sourcePixel - currentPixel).Distance();
            }
        }

        /// <summary>
        /// flood fills from every pixel whose value is are greater than 0.
        /// This creates a nice blur like effect. Useful for making thick lines from the generated map
        /// </summary>
        /// <param name="splatData"></param>
        /// <param name="decay"></param>
        public void DecayFloodFill(float decayMultiplier)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            int textureSize = splatData.Data.GetLength(0);
            var queue = new Queue<FloodFillPixelInfo>();
            for (int y = 0; y < textureSize; y++)
            {
                for (int x = 0; x < textureSize; x++)
                {
                    float value = splatData.Data[x, y];
                    if (value > 0.01f)
                    {
                        var info = new FloodFillPixelInfo();
                        info.currentPixel = new IntVector2(x, y);
                        info.sourcePixel = new IntVector2(x, y);
                        info.startValue = value;
                        queue.Enqueue(info);
                    }
                }
            }

            while (queue.Count > 0)
            {
                var front = queue.Dequeue();
                AddNeighbor(queue, ref front, textureSize, -1, 0, decayMultiplier);
                AddNeighbor(queue, ref front, textureSize, 1, 0, decayMultiplier);
                AddNeighbor(queue, ref front, textureSize, 0, 1, decayMultiplier);
                AddNeighbor(queue, ref front, textureSize, 0, -1, decayMultiplier);
            }

            sw.Stop();
            Debug.Log("Time elapsed: " + (sw.ElapsedMilliseconds / 1000.0f) + " s");
        }

        void AddNeighbor(Queue<FloodFillPixelInfo> queue, ref FloodFillPixelInfo info, int textureSize, int dx, int dy, float decayMultiplier)
        {
            FloodFillPixelInfo neighbor = info;
            neighbor.currentPixel.x += dx;
            neighbor.currentPixel.y += dy;
            if (neighbor.currentPixel.x < 0 || neighbor.currentPixel.x >= textureSize ||
                neighbor.currentPixel.y < 0 || neighbor.currentPixel.y >= textureSize)
            {
                // Out of bounds
                return;
            }

            var distance = neighbor.DistanceToSource();
            var weight = (neighbor.startValue - distance / textureSize * decayMultiplier);
            weight = Mathf.Clamp01(weight);
            if (weight <= 0)
            {
                return;
            }

            int nx = neighbor.currentPixel.x;
            int ny = neighbor.currentPixel.y;
            float existingWeight = splatData.Data[nx, ny];
            if (weight > existingWeight)
            {
                splatData.Data[nx, ny] = weight;
                queue.Enqueue(neighbor);
            }
        }


        int MapToTextureCoord(float value01, int lastIndex)
        {
            return Mathf.RoundToInt(lastIndex * value01);
        }

        void Swap(ref Vector2 a, ref Vector2 b)
        {
            Vector2 t = a;
            a = b;
            b = t;
        }

        public void ApplyCurve(AnimationCurve curve)
        {
            int textureSize = splatData.Data.GetLength(0);
            for (int y = 0; y < textureSize; y++)
            {
                for (int x = 0; x < textureSize; x++)
                {
                    float value = splatData.Data[x, y];
                    splatData.Data[x, y] = curve.Evaluate(value);
                }
            }
        }

        /// <summary>
        /// Draws a single pixel thick line. Values are normalized [0..1]
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void DrawLine(Vector2 start, Vector2 end, float value)
        {
            // Bresenham's line algorithm
            // https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm

            int texLastIndex = splatData.Data.GetLength(0) - 1;

            if (start.x > end.x) Swap(ref start, ref end);
            
            int x0 = MapToTextureCoord(start.x, texLastIndex);
            int y0 = MapToTextureCoord(start.y, texLastIndex);

            int x1 = MapToTextureCoord(end.x, texLastIndex);
            int y1 = MapToTextureCoord(end.y, texLastIndex);


            x0 = Mathf.Clamp(x0, 0, texLastIndex);
            y0 = Mathf.Clamp(y0, 0, texLastIndex);
            x1 = Mathf.Clamp(x1, 0, texLastIndex);
            y1 = Mathf.Clamp(y1, 0, texLastIndex);

            float slope = (y1 - y0) / (float)(x1 - x0);

            if (Mathf.Abs(x1 - x0) > Mathf.Abs(y1 - y0))
            {
                for (int x = x0; x <= x1; x++)
                {
                    float yf = slope * (x - x0) + y0;
                    int y = Mathf.RoundToInt(yf);
                    splatData.Data[x, y] = value;
                }
            }
            else
            {
                int sy = Mathf.Min(y0, y1);
                int ey = Mathf.Max(y0, y1);
                for (int y = sy; y <= ey; y++)
                {
                    float xf = (y - y0) / slope + x0;
                    int x = Mathf.RoundToInt(xf);
                    splatData.Data[x, y] = value;
                }
            }
        }

        public void DrawCircle(Vector2 center, float radius01, float value)
        {
            // Bresenham's circle algorithm
            // http://www.geeksforgeeks.org/bresenhams-circle-drawing-algorithm/

            int texLastIndex = splatData.Data.GetLength(0) - 1;

            int r = Mathf.RoundToInt(texLastIndex * Mathf.Clamp01(radius01));
            int cx = MapToTextureCoord(center.x, texLastIndex);
            int cy = MapToTextureCoord(center.y, texLastIndex);
            
            int x = 0;
            int y = r;

            int d = 3 - (2 * r);

            while (x <= y)
            {
                _DrawCircleOctants(cx, cy, x, y, value);
                x++;
                if (d < 0)
                {
                    d = d + (4 * x) + 6;
                }
                else
                {
                    d = d + 4 * (x - y) + 10;
                    y--;
                }
            }

        }

        void SetPixel(int x, int y, float value)
        {
            int texLastIndex = splatData.Data.GetLength(0) - 1;
            if (x >= 0 && x < texLastIndex && y >= 0 && y < texLastIndex)
            {
                splatData.Data[x, y] = value;
            }
        }
        void _DrawCircleOctants(int xc, int yc, int x, int y, float value)
        {
            SetPixel(xc + x, yc + y, value);
            SetPixel(xc - x, yc + y, value);
            SetPixel(xc + x, yc - y, value);
            SetPixel(xc - x, yc - y, value);
            SetPixel(xc + y, yc + x, value);
            SetPixel(xc - y, yc + x, value);
            SetPixel(xc + y, yc - x, value);
            SetPixel(xc - y, yc - x, value);
        }
    }
}
