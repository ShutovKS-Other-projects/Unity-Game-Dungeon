//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.SxEngine
{
    public class SxMeshVertex
    {
        public Vector3 Position { get; set; } = Vector3.zero;
        public Color Color { get; set; } = Color.white;
        public Vector2 UV0 { get; set; } = Vector2.zero;

        public SxMeshVertex()
        {
        }

        public SxMeshVertex(Vector3 position)
        {
            this.Position = position;
        }
        
        public SxMeshVertex(Vector3 position, Color color)
        {
            this.Position = position;
            this.Color = color;
        }
        
        public SxMeshVertex(Vector3 position, Color color, Vector2 uv0)
        {
            this.Position = position;
            this.Color = color;
            this.UV0 = uv0;
        }
    }

    public class SxMeshSection
    {
        public int DrawMode { get; set; } = GL.LINES;
        public SxMeshVertex[] Vertices { get; set; } = new SxMeshVertex[0];
    }

    public class SxMesh
    {
        public Dictionary<int, SxMeshSection> Sections = new Dictionary<int, SxMeshSection>();
        
        public void CreateSection(int sectionIndex, int drawMode, SxMeshVertex[] vertices)
        {
            ClearSection(sectionIndex);

            var section = new SxMeshSection();
            section.DrawMode = drawMode;
            section.Vertices = vertices;
            Sections.Add(sectionIndex, section);
        }

        public void ClearSection(int sectionIndex)
        {
            Sections.Remove(sectionIndex);
        }

        public void ClearAllSections()
        {
            Sections.Clear();
        }
    }
    
    public class SxMeshRegistry
    {
        private static Dictionary<System.Type, SxMesh> cache = new Dictionary<System.Type, SxMesh>();

        public static SxMesh Get<T>() where T : SxMesh, new()
        {
            if (cache.ContainsKey(typeof(T)))
            {
                return cache[typeof(T)];
            }

            var mesh = new T();
            cache.Add(typeof(T), mesh);
            return mesh;
        }
    }
}