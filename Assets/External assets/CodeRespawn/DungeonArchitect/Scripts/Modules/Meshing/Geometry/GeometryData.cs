using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Meshing.Common
{
    public struct GeometryVertex
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector2 uv;
        public float ao;
    }

    public class GeometryData 
    {
        public GeometryVertex[] vertices = new GeometryVertex[0];
        public int[] indices = new int[0];
    }
}
