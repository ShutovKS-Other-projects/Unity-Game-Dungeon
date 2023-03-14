//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect
{
    /// <summary>
    /// Represent an integer vector
    /// </summary>
	[System.Serializable]
    public struct IntVector2
    {
        [SerializeField]
        public int x;

        [SerializeField]
        public int y;

        public IntVector2(Vector3 v)
        {
            x = Mathf.RoundToInt(v.x);
            y = Mathf.RoundToInt(v.y);
        }

        public IntVector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public void Set(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int ManhattanDistance()
        {
            return x + y;
        }
        public float DistanceSq()
        {
            return x * x + y * y;
        }

        public IntVector2 Abs()
        {
            return new IntVector2(Mathf.Abs(x), Mathf.Abs(y));
        }

        public float Distance()
        {
            return Mathf.Sqrt(x * x + y * y);
        }

        public Vector2 ToVector2()
        {
            return new Vector2(x, y);
        }

        public static IntVector2 operator +(IntVector2 a, IntVector2 b)
        {
            var result = new IntVector2();
            result.x = a.x + b.x;
            result.y = a.y + b.y;
            return result;
        }
        public static IntVector2 operator -(IntVector2 a, IntVector2 b)
        {
            var result = new IntVector2();
            result.x = a.x - b.x;
            result.y = a.y - b.y;
            return result;
        }
        public static IntVector2 operator *(IntVector2 a, IntVector2 b)
        {
            var result = new IntVector2();
            result.x = a.x * b.x;
            result.y = a.y * b.y;
            return result;
        }
        public static Vector3 operator *(IntVector2 a, Vector3 b)
        {
            var result = new Vector3();
            result.x = a.x * b.x;
            result.y = a.y * b.y;
            return result;
        }
        public static IntVector2 operator /(IntVector2 a, IntVector2 b)
        {
            var result = new IntVector2();
            result.x = a.x / b.x;
            result.y = a.y / b.y;
            return result;
        }

        public static IntVector2 operator +(IntVector2 a, int b)
        {
            var result = new IntVector2();
            result.x = a.x + b;
            result.y = a.y + b;
            return result;
        }
        public static IntVector2 operator -(IntVector2 a, int b)
        {
            var result = new IntVector2();
            result.x = a.x - b;
            result.y = a.y - b;
            return result;
        }
        public static IntVector2 operator *(IntVector2 a, int b)
        {
            var result = new IntVector2();
            result.x = a.x * b;
            result.y = a.y * b;
            return result;
        }
        public static IntVector2 operator /(IntVector2 a, int b)
        {
            var result = new IntVector2();
            result.x = a.x / b;
            result.y = a.y / b;
            return result;
        }

        public override bool Equals(System.Object obj)
        {
            if (obj is IntVector2)
            {
                var other = (IntVector2)obj;
                return this.x == other.x &&
                    this.y == other.y;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return (x ^ (y << 16));
        }

        public static Vector2 ToV2(IntVector2 iv)
        {
            return new Vector2(iv.x, iv.y);
        }

        public static readonly IntVector2 Zero = new IntVector2(0, 0);
    }
}
