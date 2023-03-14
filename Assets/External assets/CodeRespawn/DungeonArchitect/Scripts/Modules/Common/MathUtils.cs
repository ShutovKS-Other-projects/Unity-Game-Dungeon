//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Utils
{
    /// <summary>
    /// Various math utility functions
    /// </summary>
    public class MathUtils
    {
        /// <summary>
        /// Copies the rectangle object
        /// </summary>
        /// <param name="other">The object to copy</param>
        /// <returns>The copied object</returns>
        public static Rectangle Copy(Rectangle other)
        {
            return new Rectangle(other.X, other.Z, other.Width, other.Length);
        }

        /// <summary>
        /// Divides two vector3 objects
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>The divided vector</returns>
		public static Vector3 Divide(Vector3 a, Vector3 b) {
			return new Vector3(
				a.x / b.x,
				a.y / b.y,
				a.z / b.z
				);
		}

        /// <summary>
        /// Converts an IntVector to a Vector3
        /// </summary>
        /// <param name="v">the input int vector</param>
        /// <returns></returns>
		public static Vector3 ToVector3(IntVector v) {
			return new Vector3(v.x, v.y, v.z);
		}
        
        public static Vector3 ToVector3(Vector3Int v) {
	        return new Vector3(v.x, v.y, v.z);
        }
        
        public static Vector4 ToVector4(Vector3 v, float w) {
	        return new Vector4(v.x, v.y, v.z, w);
        }

        /// <summary>
        /// Converts the world coordinates to grid coordinates
        /// </summary>
        /// <param name="WorldCoord">The world cooridnate</param>
        /// <param name="GridCellSize">The grid cell size</param>
        /// <returns>The resulting grid coordinate</returns>
		public static IntVector WorldToGrid(Vector3 WorldCoord, Vector3 GridCellSize) {
			return ToIntVector(Divide (WorldCoord, GridCellSize));
		}

        /// <summary>
        /// Converts the grid coordinate to world coordinate
        /// </summary>
        /// <param name="GridCellSize">The grid cell size</param>
        /// <param name="v">The input grid coordinate</param>
        /// <returns>The resulting world coordinate</returns>
        public static Vector3 GridToWorld(Vector3 GridCellSize, IntVector v)
        {
			return GridToWorld(GridCellSize, ToVector3(v));
		}

        /// <summary>
        /// Converts the grid coordinate to world coordinate
        /// </summary>
        /// <param name="GridCellSize">The grid cell size</param>
        /// <param name="v">The input grid coordinate</param>
        /// <returns>The resulting world coordinate</returns>
        public static Vector3 GridToWorld(Vector3 GridCellSize, Vector3 v)
        {
			return Vector3.Scale (GridCellSize, v);
		}

        /// <summary>
        /// Converts an IntVector to a Vector3, with the XYZ components floored
        /// </summary>
        /// <param name="v">The input Vector3 to convert</param>
        /// <returns>The corresponding IntVector, floored in each component</returns>
		public static IntVector ToIntVector(Vector3 v) {
			return new IntVector(
				Mathf.FloorToInt(v.x),
				Mathf.FloorToInt(v.y),
				Mathf.FloorToInt(v.z)
				);
		}

        public static Vector3Int FloorToVector3Int(Vector3 v) {
	        return new Vector3Int(
		        Mathf.FloorToInt(v.x),
		        Mathf.FloorToInt(v.y),
		        Mathf.FloorToInt(v.z)
	        );
        }

        public static Vector3Int RoundToVector3Int(Vector3 v) {
	        return new Vector3Int(
		        Mathf.RoundToInt(v.x),
		        Mathf.RoundToInt(v.y),
		        Mathf.RoundToInt(v.z)
	        );
        }

        
        public static Vector2Int RoundToVector2Int(Vector3 v) {
	        return new Vector2Int(
		        Mathf.RoundToInt(v.x),
		        Mathf.RoundToInt(v.y)
	        );
        }
        
        public static Vector3 V3FloorToInt(Vector3 v)
        {
            return new Vector3(
                Mathf.FloorToInt(v.x),
                Mathf.FloorToInt(v.y),
                Mathf.FloorToInt(v.z));
        }
        public static Vector3 V3RoundToInt(Vector3 v)
        {
            return new Vector3(
                Mathf.RoundToInt(v.x),
                Mathf.RoundToInt(v.y),
                Mathf.RoundToInt(v.z));
        }

        public static Vector3 ComponentMin(Vector3 a, Vector3 b)
        {
	        return new Vector3(
		        Mathf.Min(a.x, b.x),
		        Mathf.Min(a.y, b.y),
		        Mathf.Min(a.z, b.z)
	        );
        }

        public static Vector3 ComponentMax(Vector3 a, Vector3 b)
        {
	        return new Vector3(
		        Mathf.Max(a.x, b.x),
		        Mathf.Max(a.y, b.y),
		        Mathf.Max(a.z, b.z)
	        );
        }

        /// <summary>
        /// Rounds to an IntVector, with the XYZ components rounded to the nearest int
        /// </summary>
        /// <param name="v">The input Vector3 to convert</param>
        /// <returns>The rounded IntVector</returns>
		public static IntVector RoundToIntVector(Vector3 v) {
			return new IntVector(
				Mathf.RoundToInt(v.x),
				Mathf.RoundToInt(v.y),
				Mathf.RoundToInt(v.z)
				);
		}

        /// <summary>
        /// Snaps the position to the nearest grid cell location
        /// </summary>
        /// <param name="position">The position to snap</param>
        /// <param name="gridCellSize">The size of the grid cell</param>
        /// <returns>The snapped position</returns>
		public static Vector3 SnapToGrid(Vector3 position, Vector3 gridCellSize) {
			return SnapToGrid(position, gridCellSize, true);
		}

        /// <summary>
        /// Snaps the position to the nearest grid cell location
        /// </summary>
        /// <param name="position">The position to snap</param>
        /// <param name="gridCellSize">The size of the grid cell</param>
        /// <param name="useRounding">Flag to indicate if rounding is to be used.  Uses floor if false</param>
        /// <returns>The snapped position</returns>
        public static Vector3 SnapToGrid(Vector3 position, Vector3 gridCellSize, bool useRounding)
        {
			Vector3 gridPosition;
			if (useRounding) {
				gridPosition = new Vector3(
					Mathf.RoundToInt(position.x / gridCellSize.x),
					Mathf.RoundToInt(position.y / gridCellSize.y),
					Mathf.RoundToInt(position.z / gridCellSize.z));
			} else {
				gridPosition = new Vector3(
					Mathf.FloorToInt(position.x / gridCellSize.x),
					Mathf.FloorToInt(position.y / gridCellSize.y),
					Mathf.FloorToInt(position.z / gridCellSize.z));
				//gridPosition += Vector3.one;
			}
			return Vector3.Scale(gridPosition, gridCellSize);
		}

        /// <summary>
        /// Checks if the two rectangles intersect
        /// </summary>
        /// <param name="outer">The outer rect</param>
        /// <param name="inner">The inner rect</param>
        /// <returns>True if they intersect, false otherwise</returns>
		public static bool Intersects(Rect a, Rect b) {
            bool notIntersecting = (a.xMin > b.xMax || a.xMax < b.xMin || a.yMin > b.yMax || a.yMax < b.yMin);
            return !notIntersecting;
		}
		
        public static Vector2 ClosestPointOnRect(Rect rect, Vector2 p)
        {
            var result = new Vector2();
            result.x = Mathf.Clamp(p.x, rect.xMin, rect.xMax);
            result.y = Mathf.Clamp(p.y, rect.yMin, rect.yMax);
            return result;
        }


        public static Rect ExpandRect(Rect bounds, float amount)
        {
            var rect = new Rect(bounds);
            rect.x -= amount;
            rect.y -= amount;
            rect.width += amount * 2;
            rect.height += amount * 2;
            return rect;
        }

        public static Vector2 ClampToRect(Vector2 position, Rect bounds)
        {
            var result = position;
            result.x = Mathf.Clamp(result.x, bounds.x, bounds.x + bounds.width);
            result.y = Mathf.Clamp(result.y, bounds.y, bounds.y + bounds.height);
            return result;
        }

		/// <summary>
		/// Flips the coordinates for 2D mode
		/// </summary>
		/// <param name="bounds">Bounds.</param>
		public static void FlipYZ(ref Bounds bounds) {
			bounds.size = FlipYZ (bounds.size);
			var center = FlipYZ (bounds.center);
			center.y = 0;
			bounds.center = center;
		}

		/// <summary>
		/// Flips the coordinates for 2D mode
		/// </summary>
		/// <returns>The Y.</returns>
		/// <param name="bounds">Bounds.</param>
		public static Vector3 FlipYZ(Vector3 bounds) {
			var z = bounds.z;
			bounds.z = bounds.y;
			bounds.y = z;
			return bounds;
		}

		/// <summary>
		/// Flips the coordinates for 2D mode
		/// </summary>
		/// <returns>The Y.</returns>
		/// <param name="bounds">Bounds.</param>
		public static IntVector FlipYZ(IntVector bounds) {
			var z = bounds.z;
			bounds.z = bounds.y;
			bounds.y = z;
			return bounds;
		}

        public static void Abs(ref Vector3 v)
        {
            v.x = Mathf.Abs(v.x);
            v.y = Mathf.Abs(v.y);
            v.z = Mathf.Abs(v.z);
        }

		/// <summary>
		/// Flag to indicate an invalid location
		/// </summary>
		public static readonly int INVALID_LOCATION = -1000000;


		public static void Swap<T>(ref T a, ref T b)
		{
			T t = a;
			a = b;
			b = t;
		}
        
        public static void Shuffle<T>(List<T> Array, System.Random Random)
        {
            int Count = Array.Count;
            for (int i = 0; i < Count; i++)
            {
                int j = Random.Range(0, Count - 1);
                T Temp = Array[i];
                Array[i] = Array[j];
                Array[j] = Temp;
            }
        }

        public static void Shuffle<T>(T[] Array, System.Random Random)
        {
            int Count = Array.Length;
            for (int i = 0; i < Count; i++)
            {
                int j = Random.Range(0, Count - 1);
                T Temp = Array[i];
                Array[i] = Array[j];
                Array[j] = Temp;
            }
        }

        public static int[] GetShuffledIndices(int Count, System.Random Random)
        {
            var Indices = new List<int>();
            for (int i = 0; i < Count; i++)
            {
                Indices.Add(i);
            }

            Shuffle(Indices, Random);
            return Indices.ToArray();
        }

        public static Bounds TransformBounds(Matrix4x4 transform, Bounds bounds)
        {
            var vertices = new Vector3[8];
            var center = bounds.center;
            var extents = bounds.extents;

            vertices[0] = center + new Vector3(extents.x, extents.y, extents.z);
            vertices[1] = center + new Vector3(extents.x, extents.y, -extents.z);
            vertices[2] = center + new Vector3(extents.x, -extents.y, extents.z);
            vertices[3] = center + new Vector3(extents.x, -extents.y, -extents.z);
            vertices[4] = center + new Vector3(-extents.x, extents.y, extents.z);
            vertices[5] = center + new Vector3(-extents.x, extents.y, -extents.z);
            vertices[6] = center + new Vector3(-extents.x, -extents.y, extents.z);
            vertices[7] = center + new Vector3(-extents.x, -extents.y, -extents.z);

            for (int i = 0; i < 8; i++)
            {
                vertices[i] = transform.MultiplyPoint3x4(vertices[i]);
            }

            var newBounds = new Bounds(vertices[0], Vector3.zero);
            for (int i = 1; i < 8; i++) { 
                newBounds.Encapsulate(vertices[i]);
            }
            
            return newBounds;
        }

        public static Bounds TransformBoundsX(Matrix4x4 transform, Bounds localBounds)
        {

            var center = transform * localBounds.center;
            
            // transform the local extents' axes
            var extents = localBounds.extents;
            var axisX = transform * new Vector3(extents.x, 0, 0);
            var axisY = transform * new Vector3(0, extents.y, 0);
            var axisZ = transform * new Vector3(0, 0, extents.z);

            // sum their absolute value to get the world extents
            extents.x = Mathf.Abs(axisX.x) + Mathf.Abs(axisY.x) + Mathf.Abs(axisZ.x);
            extents.y = Mathf.Abs(axisX.y) + Mathf.Abs(axisY.y) + Mathf.Abs(axisZ.y);
            extents.z = Mathf.Abs(axisX.z) + Mathf.Abs(axisY.z) + Mathf.Abs(axisZ.z);

            return new Bounds { center = center, extents = extents };
        }

        public static bool V3Equals(Vector3 a, Vector3 b)
        {
            return V3Equals(a, b, 1e-6f);
        }

        public static bool V3Equals(Vector3 a, Vector3 b, float threshold)
        {
            return Vector3.Magnitude(a - b) < threshold;
        }

        public static byte ToByte(float value01)
        {
            return (byte)Mathf.RoundToInt(Mathf.Clamp01(value01) * 255);
        }

        public static Vector3 ReflectVector(Vector3 direction, Vector3 normal)
        {
	        return direction - 2 * Vector3.Dot(direction, normal) * normal;
        }
    }
}
