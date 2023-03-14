//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.SxEngine
{
    public enum SxTextHAlign
    {
        Left, 
        Center,
        Right
    }

    public enum SxTextVAlign
    {
        Top,
        Center,
        Bottom
    }
    
    public struct SxTextComponentSettings {
        public Font Font;
        public Color Color;
        public float Scale;
        public SxTextHAlign HAlign;
        public SxTextVAlign VAlign;
        public float DepthBias;
    }
    public class SxTextComponent : SxActorComponent
    {
        private SxMesh mesh;
        private SxMaterial material;
        private string text = "";
        private SxTextComponentSettings settings;
        
        public string Text
        {
            get => text;
            set
            {
                text = value;
                if (settings.Font != null)
                {
                    settings.Font.RequestCharactersInTexture(text);
                }
                RebuildMesh();
            }
        }

        public void Initialize(SxTextComponentSettings settings)
        {
            this.settings = settings;
            Font.textureRebuilt += OnTextureRebuilt;

            if (settings.Font != null)
            {
                UpdateMaterial(settings.Font);
                RebuildMesh();
            }
        }

        void UpdateMaterial(Font font)
        {
            var unityMat = font.material;
            if (unityMat != null)
            {
                material = new SxUnityMaterial(unityMat);
                material.DepthBias = settings.DepthBias;
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            Font.textureRebuilt -= OnTextureRebuilt;
        }

        private void OnTextureRebuilt(Font font)
        {
            UpdateMaterial(font);
            RebuildMesh();
        }

        public override void Draw(SxRenderContext context, Matrix4x4 accumWorldTransform, SxRenderCommandList renderCommandList)
        {
            base.Draw(context, accumWorldTransform, renderCommandList);

            if (mesh != null && material != null)
            {
                renderCommandList.Add(new SxRenderCommand(accumWorldTransform, mesh, material));
            }
        }

        private void RebuildMesh()
        {
            mesh = null;
            if (material == null || text == null || text.Length == 0 || settings.Font == null)
            {
                return;
            }
            
            UpdateMaterial(settings.Font);

            var scale = settings.Scale / settings.Font.fontSize; 
            var vertices = new List<SxMeshVertex>();
            var pos = Vector3.zero;
            var textHeight = 0.0f;
            var textWidth = 0.0f;
            for (var i = 0; i < text.Length; i++)
            {
                CharacterInfo ch;
                settings.Font.GetCharacterInfo(text[i], out ch);
                var p0 = pos + new Vector3(ch.minX, ch.maxY, 0) * scale;
                var p1 = pos + new Vector3(ch.maxX, ch.maxY, 0) * scale;
                var p2 = pos + new Vector3(ch.maxX, ch.minY, 0) * scale;
                var p3 = pos + new Vector3(ch.minX, ch.minY, 0) * scale;
                
                var t0 = ch.uvTopLeft;
                var t1 = ch.uvTopRight;
                var t2 = ch.uvBottomRight;
                var t3 = ch.uvBottomLeft;

                vertices.Add(new SxMeshVertex(p0, settings.Color, t0));
                vertices.Add(new SxMeshVertex(p1, settings.Color, t1));
                vertices.Add(new SxMeshVertex(p2, settings.Color, t2));
                vertices.Add(new SxMeshVertex(p3, settings.Color, t3));

                pos += new Vector3(ch.advance * scale, 0, 0);

                textHeight = Mathf.Max(textHeight, ch.maxY * scale);
                textWidth += ch.advance * scale;
            }

            var offset = Vector3.zero;
            if (settings.VAlign == SxTextVAlign.Center)
            {
                offset.y -= textHeight * 0.5f;
            }
            else if (settings.VAlign == SxTextVAlign.Top)
            {
                offset.y -= textHeight;
            }

            if (settings.HAlign == SxTextHAlign.Center)
            {
                offset.x -= textWidth * 0.5f;
            }
            else if (settings.HAlign == SxTextHAlign.Right)
            {
                offset.x -= textWidth;
            }
            
            foreach (var vertex in vertices)
            {
                vertex.Position += offset;
            }
            
            mesh = new SxMesh();
            mesh.CreateSection(0, GL.QUADS, vertices.ToArray());
        }
    }
}