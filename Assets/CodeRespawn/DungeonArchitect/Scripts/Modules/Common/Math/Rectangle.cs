//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;

namespace DungeonArchitect
{
    /// <summary>
    /// Represents an integer rectangle
    /// </summary>
	[System.Serializable]
	public struct Rectangle {
		public Rectangle(int x, int z, int width, int length) {
			location = new IntVector(x, 0, z);
			size = new IntVector(width, 0, length);
		}
		public Rectangle(IntVector location, IntVector size) {
			this.location = location;
			this.size = size;
		}

		[SerializeField]
		IntVector location;
		public IntVector Location {
			get {
				return location;
			}
			set {
				location = value;
			}
		}
		
		[SerializeField]
		IntVector size;
		public IntVector Size {
			get {
				return size;
			}
			set {
				size = value;
			}
		}

		public int X {
			get { return Location.x; }
		}
		public int Z {
			get { return Location.z; }
		}
		public int Width {
			get { return Size.x; }
		}
		public int Length {
			get { return Size.z; }
		}

		public int Left { get { return X; } }
		public int Right { get { return X + Width; } }
		public int Back { get { return Z; } }
		public int Front { get { return Z + Length; } }

		public void SetY(int y) {
			location.y = y;
		}

		public IntVector Center() {
			return Location + Size / 2;
		}

		static Vector3 ToVector3(IntVector iv) {
			return new Vector3(iv.x, iv.y, iv.z);
		}

		public Vector3 CenterF() {
			return ToVector3(Location) + ToVector3(Size) / 2.0f;
		}

		public bool Contains(Rectangle rect) {
			return(X <= rect.X) &&
				((rect.X + rect.Width) <= (X + Width)) &&
					(Z <= rect.Z) &&
					((rect.Z + rect.Length) <= (Z + Length));
		}
		
		public bool Contains(IntVector Point) {
			return Contains(Point.x, Point.z);
		}
		
		public bool Contains(int x, int z) {
			return this.X <= x &&
				x < this.X + this.Width &&
					this.Z <= z &&
					z < this.Z + this.Length;
		}
		
		public static Rectangle Intersect(Rectangle a, Rectangle b) {
			int x1 = Mathf.Max(a.X, b.X);
			int x2 = Mathf.Min(a.X + a.Width, b.X + b.Width);
			int z1 = Mathf.Max(a.Z, b.Z);
			int z2 = Mathf.Min(a.Z + a.Length, b.Z + b.Length);
			
			if (x2 >= x1 && z2 >= z1) {
				return new Rectangle(x1, z1, x2 - x1, z2 - z1);
			}
			return new Rectangle();
		}
		
		public bool IntersectsWith(Rectangle rect) {
			return(rect.X < X + Width) &&
				(X < (rect.X + rect.Width)) &&
					(rect.Z < Z + Length) &&
					(Z < rect.Z + rect.Length);
		}

        

        public static Rectangle ExpandBounds(Rectangle rect, int distance)
        {
            
            var location = rect.Location;
            var size = rect.Size;

            location.x -= distance;
            location.z -= distance;

            size.x += distance * 2;
            size.z += distance * 2;

            var result = rect;
            result.location = location;
            result.size = size;

            return result;
        }

        public IntVector[] GetBorderPoints()
        {
            var result = new List<IntVector>();
            for (int dx = 0; dx < size.x; dx++)
            {
                var x = location.x + dx;
                var y = location.y;
                var z = location.z;
                var point = new IntVector(x, y, z);
                result.Add(point);

				if (size.z > 1) {
	                z = location.z + size.z - 1;
	                point = new IntVector(x, y, z);
	                result.Add(point);
				}
            }

            for (int dz = 1; dz < size.z - 1; dz++)
            {
                var x = location.x;
                var y = location.y;
                var z = location.z + dz;
                var point = new IntVector(x, y, z);
                result.Add(point);

				if (size.x > 1) {
	                x = location.x + size.x - 1;
	                point = new IntVector(x, y, z);
	                result.Add(point);
				}
            }

            return result.ToArray();
        }
	};

}
