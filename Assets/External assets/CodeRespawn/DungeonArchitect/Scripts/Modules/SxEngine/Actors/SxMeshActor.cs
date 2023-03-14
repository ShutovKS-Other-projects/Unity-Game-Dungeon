//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
namespace DungeonArchitect.SxEngine
{
    public class SxMeshActor : SxActor
    {
        public SxMeshComponent MeshComponent;
        
        public SxMeshActor()
        {
            MeshComponent = AddComponent<SxMeshComponent>();
        }

        public void SetMesh(SxMesh mesh)
        {
            MeshComponent.Mesh = mesh;
        }
        
        public void SetMaterial(SxMaterial material)
        {
            MeshComponent.Material = material;
        }
        
        public void SetMesh<T>() where T : SxMesh, new()
        {
            MeshComponent.Mesh = SxMeshRegistry.Get<T>();
        }

        public void SetMaterial<T>() where T : SxMaterial, new()
        {
            MeshComponent.Material = SxMaterialRegistry.Get<T>();
        }
    }
}