//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Utils
{
    /// <summary>
    /// Helper functions to draw debug information of the dungeon layout in the scene view 
    /// </summary>
    public class DebugDrawUtils
    {

        public static void DrawBounds(Rectangle bounds, Color color, Vector3 gridScale, bool mode2D)
        {
            var x0 = bounds.Left * gridScale.x;
            var x1 = bounds.Right * gridScale.x;
            var z0 = bounds.Front * gridScale.z;
            var z1 = bounds.Back * gridScale.z;
            var y = bounds.Location.y * gridScale.y;
            DrawLine(new Vector3(x0, y, z0), new Vector3(x1, y, z0), color, 0, false, mode2D);
            DrawLine(new Vector3(x1, y, z0), new Vector3(x1, y, z1), color, 0, false, mode2D);
            DrawLine(new Vector3(x1, y, z1), new Vector3(x0, y, z1), color, 0, false, mode2D);
            DrawLine(new Vector3(x0, y, z1), new Vector3(x0, y, z0), color, 0, false, mode2D);
        }

        public static void DrawBounds(Bounds bounds, Color color)
        {
            var min = bounds.min;
            var max = bounds.max;
            var x0 = min.x;
            var x1 = max.x;
            var y0 = min.y;
            var y1 = max.y;
            var z0 = min.z;
            var z1 = max.z;
            
            
            Debug.DrawLine(new Vector3(x0, y0, z0), new Vector3(x1, y0, z0), color, 0, false);
            Debug.DrawLine(new Vector3(x0, y1, z0), new Vector3(x1, y1, z0), color, 0, false);
            Debug.DrawLine(new Vector3(x0, y0, z1), new Vector3(x1, y0, z1), color, 0, false);
            Debug.DrawLine(new Vector3(x0, y1, z1), new Vector3(x1, y1, z1), color, 0, false);
            
            Debug.DrawLine(new Vector3(x0, y0, z0), new Vector3(x0, y1, z0), color, 0, false);
            Debug.DrawLine(new Vector3(x1, y0, z0), new Vector3(x1, y1, z0), color, 0, false);
            Debug.DrawLine(new Vector3(x0, y0, z1), new Vector3(x0, y1, z1), color, 0, false);
            Debug.DrawLine(new Vector3(x1, y0, z1), new Vector3(x1, y1, z1), color, 0, false);
            
            Debug.DrawLine(new Vector3(x0, y0, z0), new Vector3(x0, y0, z1), color, 0, false);
            Debug.DrawLine(new Vector3(x1, y0, z0), new Vector3(x1, y0, z1), color, 0, false);
            Debug.DrawLine(new Vector3(x0, y1, z0), new Vector3(x0, y1, z1), color, 0, false);
            Debug.DrawLine(new Vector3(x1, y1, z0), new Vector3(x1, y1, z1), color, 0, false);
        }

        public static void DrawPoint(Vector3 position, Color color, bool mode2D)
        {
            var start = position;
            var end = start + new Vector3(0, 0.2f, 0);
            DrawLine(start, end, color, 0, false, mode2D);
        }

        static Vector3 FlipFor2D(Vector3 v)
        {
            return new Vector3(v.x, v.z, v.y);
        }

        public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration, bool depthTest, bool mode2D)
        {
            if (mode2D)
            {
                start = FlipFor2D(start);
                end = FlipFor2D(end);
            }

            Debug.DrawLine(start, end, color, duration, depthTest);
        }
        static Vector3 GetPointOnCircle(float angle)
        {
            float radians = angle * Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(radians), 0, Mathf.Sin(radians));
        }

        public static void DrawCircle(Vector3 center, float radius, Color color)
        {
            DrawCircle(center, radius, color, 1.0f);
        }

        public static void DrawCircle(Vector3 center, float radius, Color color, float segmentMultiplier)
        {
            float perimeter = Mathf.PI * 2 * radius;
            float constSegmentMultiplier = 3;
            int segments = Mathf.RoundToInt(Mathf.Sqrt(perimeter) * segmentMultiplier * constSegmentMultiplier);
            segments = Mathf.Max(segments, 3);  // minimum 3 segments for a triangle

            float angle = 0;
            float angleSteps = 360.0f / segments;
            for (int i = 0; i <= segments; i++)
            {
                float nextAngle = angle + angleSteps;
                var start = center + GetPointOnCircle(angle) * radius;
                var end = center + GetPointOnCircle(nextAngle) * radius;
                angle = nextAngle;

                Debug.DrawLine(start, end, color);
            }
        }
    }
}
