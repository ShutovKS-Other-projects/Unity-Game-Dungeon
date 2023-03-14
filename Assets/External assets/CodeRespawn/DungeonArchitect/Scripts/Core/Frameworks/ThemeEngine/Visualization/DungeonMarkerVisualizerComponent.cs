//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Themeing
{   
    public class ThemeEditorVisMarkerGeometry
    {
        public Vector3[] Vertices;
        public Vector2[] UV;
        public int[] Indices;
    }
    
    public class ThemeEditorVisualizationState
    {
        public ThemeEditorVisMarkerGeometry LocalGeometry;
        public Matrix4x4[] MarkerTransforms;
        public Material Material;
    }

    public class DungeonMarkerVisualizerComponent : MonoBehaviour
    {
        void Awake()
        {
            if (Application.isPlaying)
            {
                Destroy(gameObject);
                return;
            }
        }

        public void Clear()
        {
            var meshFilter = gameObject.GetComponent<MeshFilter>();
            if (meshFilter == null)
            {
                meshFilter = gameObject.AddComponent<MeshFilter>();
            }

            meshFilter.mesh = new Mesh();
        }
        
        public void Build(ThemeEditorVisualizationState state)
        {
            var meshFilter = gameObject.GetComponent<MeshFilter>();
            if (meshFilter == null)
            {
                meshFilter = gameObject.AddComponent<MeshFilter>();
            }

            var meshRenderer = gameObject.GetComponent<MeshRenderer>();
            if (meshRenderer == null)
            {
                meshRenderer = gameObject.AddComponent<MeshRenderer>();            
            }

            var mesh = new Mesh();
            meshFilter.mesh = mesh;
            meshRenderer.material = state.Material;

            var vertices = new List<Vector3>();
            var uvs = new List<Vector2>();
            var indices = new List<int>();

            var localGeometry = state.LocalGeometry;
            
            foreach (var markerTransform in state.MarkerTransforms)
            {
                int baseIndex = vertices.Count;

                foreach (var localPosition in localGeometry.Vertices)
                {
                    var position = markerTransform.MultiplyPoint(localPosition);
                    vertices.Add(position);
                }
                
                uvs.AddRange(localGeometry.UV);
                
                foreach (var localIndex in localGeometry.Indices)
                {
                    indices.Add(baseIndex + localIndex);
                }
            }

            mesh.vertices = vertices.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.triangles = indices.ToArray();
        }
    }
}