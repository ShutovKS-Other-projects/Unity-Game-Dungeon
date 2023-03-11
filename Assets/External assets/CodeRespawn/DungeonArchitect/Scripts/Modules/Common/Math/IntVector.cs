//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect
{
    /// <summary>
    /// Represent an integer vector
    /// </summary>
	[System.Serializable]
	public struct IntVector {
		[SerializeField]
		public int x;

		[SerializeField]
		public int y;

		[SerializeField]
		public int z;

        public IntVector(Vector3 v)
        {
            x = Mathf.RoundToInt(v.x);
            y = Mathf.RoundToInt(v.y);
            z = Mathf.RoundToInt(v.z);
        }

        public IntVector(int x, int y, int z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public void Set(int x, int y, int z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public float DistanceSq() {
			return x * x + y * y + z * z;
		}

		public float Distance() {
			return Mathf.Sqrt(x * x + y * y + z * z);
		}

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }

		public static IntVector operator+(IntVector a, IntVector b) {
			var result = new IntVector();
			result.x = a.x + b.x;
			result.y = a.y + b.y;
			result.z = a.z + b.z;
			return result;
		}
		public static IntVector operator-(IntVector a, IntVector b) {
			var result = new IntVector();
			result.x = a.x - b.x;
			result.y = a.y - b.y;
			result.z = a.z - b.z;
			return result;
		}
		public static IntVector operator*(IntVector a, IntVector b) {
			var result = new IntVector();
			result.x = a.x * b.x;
			result.y = a.y * b.y;
			result.z = a.z * b.z;
			return result;
		}
        public static Vector3 operator *(IntVector a, Vector3 b)
        {
            var result = new Vector3();
            result.x = a.x * b.x;
            result.y = a.y * b.y;
            result.z = a.z * b.z;
            return result;
        }
		public static IntVector operator/(IntVector a, IntVector b) {
			var result = new IntVector();
			result.x = a.x / b.x;
			result.y = a.y / b.y;
			result.z = a.z / b.z;
			return result;
		}
		
		public static IntVector operator+(IntVector a, int b) {
			var result = new IntVector();
			result.x = a.x + b;
			result.y = a.y + b;
			result.z = a.z + b;
			return result;
		}
		public static IntVector operator-(IntVector a, int b) {
			var result = new IntVector();
			result.x = a.x - b;
			result.y = a.y - b;
			result.z = a.z - b;
			return result;
		}
		public static IntVector operator*(IntVector a, int b) {
			var result = new IntVector();
			result.x = a.x * b;
			result.y = a.y * b;
			result.z = a.z * b;
			return result;
		}
		public static IntVector operator/(IntVector a, int b) {
			var result = new IntVector();
			result.x = a.x / b;
			result.y = a.y / b;
			result.z = a.z / b;
			return result;
        }
        public static bool operator==(IntVector a, IntVector b)
        {
            return a.x == b.x &&
                a.y == b.y &&
                a.z == b.z;
        }

        public static bool operator !=(IntVector a, IntVector b)
        {
            return !(a == b);
        }

        public override bool Equals(System.Object obj)
		{
			if (obj is IntVector) {
				var other = (IntVector)obj;
				return this.x == other.x &&
					this.y == other.y &&
					this.z == other.z;
			}
			return false;
		}
		public override int GetHashCode()
		{
			return (x ^ (y << 16)) ^ (z << 24);
		}

		public static Vector3 ToV3(IntVector iv) {
			return new Vector3(iv.x, iv.y, iv.z);
		}


        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", x, y, z);
        }

        public static readonly IntVector Zero = new IntVector(0, 0, 0);
	}
}
