//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System;
using System.Collections.Generic;
using DungeonArchitect.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DungeonArchitect.SxEngine
{
    public struct SxRenderContext
    {
        public Vector3 CameraPosition;
    }

    public class SxRenderCommand : IComparable<SxRenderCommand>
    {
        public Matrix4x4 AccumWorldTransform; 
        public SxMesh Mesh;
        public SxMaterial Material;
        private float distanceSqToCam = 0;

        public SxRenderCommand(Matrix4x4 accumWorldTransform, SxMesh mesh, SxMaterial material)
        {
            AccumWorldTransform = accumWorldTransform;
            Mesh = mesh;
            Material = material;
        }

        public int CompareTo(SxRenderCommand b)
        {
            var a = this;
            var queueA = a.Material != null ? a.Material.RenderQueue : 0;
            var queueB = b.Material != null ? b.Material.RenderQueue : 0;
            if (queueA == queueB)
            {
                var depthBiasA = a.Material != null ? a.Material.DepthBias : 0;
                var depthBiasB = b.Material != null ? b.Material.DepthBias : 0;
                var da = a.distanceSqToCam + depthBiasA;
                var db = b.distanceSqToCam + depthBiasB;
                if (da == db) return 0;
                return da > db ? -1 : 1;
            }
            else
            {
                return queueA < queueB ? -1 : 1;
            }
        }

        public void UpdateDistanceToCam(Vector3 camLocation)
        {
            distanceSqToCam = (Matrix.GetTranslationDivW(ref AccumWorldTransform) - camLocation).sqrMagnitude;
        }
    }

    public class SxRenderCommandList
    {   
        private List<SxRenderCommand> renderCommands = new List<SxRenderCommand>();
        public SxRenderCommand[] Commands
        {
            get => renderCommands.ToArray();
        }
        
        public void Add(SxRenderCommand command)
        {
            renderCommands.Add(command);
        }

        public void Sort(Vector3 camLocation)
        {
            UpdateDistanceFromCam(camLocation);
            renderCommands.Sort();
        }

        private void UpdateDistanceFromCam(Vector3 camLocation)
        {
            foreach (var command in renderCommands)
            {
                command.UpdateDistanceToCam(camLocation);
            }
        }
    }
    
    public class SxRenderer 
    {
        class ClearState
        {
            public bool ClearDepth = false;
            public bool ClearColor = false;
            public Color Color = Color.black;
        }
        
        public RenderTexture Texture { get; private set; }
        public SxCamera Camera { get; } = new SxCamera();

        private ClearState clearState = new ClearState();

        public float FOV { get; } = 75;

        public delegate void DrawDelegate(SxRenderContext context);

        public event DrawDelegate Draw;

        public void SetClearState(bool clearDepth, bool clearColor, Color color)
        {
            clearState.ClearDepth = clearDepth;
            clearState.ClearColor = clearColor;
            clearState.Color = color;
        }

        public SxRenderContext CreateRenderContext()
        {
            return new SxRenderContext
            {
                CameraPosition = Camera.Location
            };
        }

        public float GetAspectRatio()
        {
            return Texture != null ? (float) Texture.width / Texture.height : 1.0f;
        }
        
        public void Render(Vector2 size, SxWorld world)
        {
            AcquireTexture(size);
            
            var oldRTT = RenderTexture.active; 
            RenderTexture.active = Texture;
            
            GL.PushMatrix();
            GL.LoadProjectionMatrix(Matrix4x4.Perspective(FOV, GetAspectRatio(), 0.1f, 100.0f));

            if (clearState.ClearColor || clearState.ClearDepth)
            {
                GL.Clear(clearState.ClearDepth, clearState.ClearColor, clearState.Color);
            }
            
            var context = CreateRenderContext();
            
            var renderCommandList = new SxRenderCommandList();
            world.Draw(context, renderCommandList);
            renderCommandList.Sort(context.CameraPosition);
            Render(renderCommandList, Camera.ViewMatrix);
            
            if (Draw != null)
            {                
                Draw.Invoke(context);
            }
            
            GL.PopMatrix();
            
            RenderTexture.active = oldRTT;
        }
        
        public void Release()
        {
            ReleaseTexture();
        }
        
        private void AcquireTexture(Vector2 size)
        {
            var width = Mathf.RoundToInt(size.x);
            var height = Mathf.RoundToInt(size.y);
            if (Texture != null && (Texture.width != width || Texture.height != height))
            {
                ReleaseTexture();
            }

            if (Texture == null)
            {
                Texture = new RenderTexture(Mathf.RoundToInt(size.x), Mathf.RoundToInt(size.y), 16, RenderTextureFormat.ARGB32);
                Texture.Create();
            }
        }

        private void ReleaseTexture()
        {
            Texture.Release();
            Object.DestroyImmediate(Texture);
            Texture = null;
        }
        
        class MergedMesh
        {
            public int DrawMode;
            public SxMaterial Material;
            public List<SxMeshVertex> Vertices = new List<SxMeshVertex>();
        }

        class MergeMeshList
        {
            public MergedMesh ActiveMesh;
            private List<MergedMesh> mergedMeshes = new List<MergedMesh>();
            public MergedMesh[] Meshes
            {
                get => mergedMeshes.ToArray();
            }
            
            public MergeMeshList()
            {
            }

            public void CreateNew()
            {
                ActiveMesh = new MergedMesh();
                mergedMeshes.Add(ActiveMesh);
            }
        }

        public void RenderDefault(SxRenderCommandList renderCommandList, Matrix4x4 viewMatrix)
        {
            foreach (var command in renderCommandList.Commands)
            {
                command.Material.Assign();
                GL.modelview = viewMatrix * command.AccumWorldTransform; 

                foreach (var entry in command.Mesh.Sections)
                {
                    var section = entry.Value;
                    GL.Begin(section.DrawMode);
                    
                    foreach (var vertex in section.Vertices)
                    {
                        GL.Color(vertex.Color);
                        GL.TexCoord(vertex.UV0);

                        var p = vertex.Position;
                        GL.Vertex3(p.x, p.y, p.z);
                    }
                    
                    GL.End();
                }
            }
        }

        public void RenderMerged(SxRenderCommandList renderCommandList, Matrix4x4 viewMatrix)
        {
            GL.modelview = viewMatrix;
            
            // Merge similar meshes
            var mergedMeshes = new MergeMeshList();
            foreach (var command in renderCommandList.Commands)
            {
                GenerateMergedMeshes(command, mergedMeshes);
            }

            // draw the merged meshes
            foreach (var mesh in mergedMeshes.Meshes)
            {
                mesh.Material.Assign();
                GL.Begin(mesh.DrawMode);
                foreach (var vertex in mesh.Vertices)
                {
                    GL.Color(vertex.Color);
                    GL.TexCoord(vertex.UV0);

                    var p = vertex.Position;
                    GL.Vertex3(p.x, p.y, p.z);

                }
                GL.End();
            }
        }
        
        public void Render(SxRenderCommandList renderCommandList, Matrix4x4 viewMatrix)
        {
            RenderDefault(renderCommandList, viewMatrix);
            //RenderMerged(renderCommandList, viewMatrix);
        }

        void GenerateMergedMeshes(SxRenderCommand command, MergeMeshList mergedMeshes)
        {
            if (command.Material == null || command.Mesh == null) return;
            
            foreach (var entry in command.Mesh.Sections)
            {
                var section = entry.Value;
                bool createNewMesh = mergedMeshes.ActiveMesh == null 
                                     || mergedMeshes.ActiveMesh.Material != command.Material
                                     || mergedMeshes.ActiveMesh.DrawMode != section.DrawMode;

                if (createNewMesh)
                {
                    mergedMeshes.CreateNew();
                    mergedMeshes.ActiveMesh.Material = command.Material;
                    mergedMeshes.ActiveMesh.DrawMode = section.DrawMode;
                }
                
                foreach (var vertex in section.Vertices)
                {
                    var mergedVertex = new SxMeshVertex();
                    mergedVertex.Color = vertex.Color;
                    mergedVertex.UV0 = vertex.UV0;

                    var p = command.AccumWorldTransform * MathUtils.ToVector4(vertex.Position, 1);
                    p /= p.w;
                    
                    mergedVertex.Position = p;
                    mergedMeshes.ActiveMesh.Vertices.Add(mergedVertex);
                }
            }
        }
    }
}


