//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.SxEngine
{
    public class SxMeshComponent : SxActorComponent
    {
        public SxMesh Mesh;
        public SxMaterial Material
        {
            get => _material;
            set => _material = value;
        }
        
        private SxMaterial _material;
        
        public override void Draw(SxRenderContext context, Matrix4x4 accumWorldTransform, SxRenderCommandList renderCommandList)
        {
            base.Draw(context, accumWorldTransform, renderCommandList);
            
            if (Material == null)
            {
                Material = SxMaterialRegistry.Get<SxDefaultMaterial>();
            }
            
            if (Mesh != null)
            {
                renderCommandList.Add(new SxRenderCommand(accumWorldTransform, Mesh, Material));
            }
        }
    }
}
