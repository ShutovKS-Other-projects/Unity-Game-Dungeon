//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace DungeonArchitect.Utils
{
    [StructLayout(LayoutKind.Explicit), Serializable]
    public struct DungeonUID : IComparable, 
        IComparable<DungeonUID>,
        IEquatable<DungeonUID>
    {
        [FieldOffset(0)] 
        public System.Guid Guid;

        [FieldOffset(0), SerializeField] private Int32 A;
        [FieldOffset(4), SerializeField] private Int32 B;
        [FieldOffset(8), SerializeField] private Int32 C;
        [FieldOffset(12), SerializeField] private Int32 D;

        public static DungeonUID NewUID()
        {
            return new DungeonUID()
            {
                Guid = System.Guid.NewGuid()
            };
        }

        public static readonly DungeonUID Empty = new DungeonUID()
        {
            Guid = System.Guid.Empty
        };

        public static bool operator==(DungeonUID a, DungeonUID b)
        {
            //return a.Guid == b.Guid;
            return a.A == b.A &&
                   a.B == b.B &&
                   a.C == b.C && 
                   a.D == b.D;
        }
        
        public static bool operator!=(DungeonUID a, DungeonUID b)
        {
            //return a.Guid != b.Guid;
            return a.A != b.A ||
                   a.B != b.B ||
                   a.C != b.C || 
                   a.D != b.D;
        }

        public bool Equals(DungeonUID other)
        {
            //return other.Guid.Equals(Guid);
            return A == other.A &&
                   B == other.B &&
                   C == other.C && 
                   D == other.D;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is DungeonUID)
            {
                var other = (DungeonUID)obj;
                return A == other.A &&
                       B == other.B &&
                       C == other.C && 
                       D == other.D;
            }

            return false;
        }

        public bool IsValid()
        {
            return Guid != System.Guid.Empty;
        }
        
        public int CompareTo(object obj)
        {
            if (obj == null) return -1;
            if (obj is DungeonUID)
            {
                return ((DungeonUID) obj).Guid.CompareTo(Guid);
            }

            if (obj is System.Guid)
            {
                return ((System.Guid) obj).CompareTo(Guid);
            }

            return -1;
        }

        public int CompareTo(DungeonUID other)
        {
            return other.Guid.CompareTo(Guid);
        }
        
        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }

        public override string ToString()
        {
            return Guid.ToString();
        }
    }
}