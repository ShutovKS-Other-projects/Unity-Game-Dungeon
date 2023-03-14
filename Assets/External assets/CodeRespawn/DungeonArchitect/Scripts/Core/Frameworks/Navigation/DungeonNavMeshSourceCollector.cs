//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace DungeonArchitect
{
    public class DungeonNavMeshSourceCollector
    {

        static NavMeshBuildSource CreateMeshSource(Mesh mesh, Matrix4x4 transform)
        {
            var source = new NavMeshBuildSource();
            source.shape = NavMeshBuildSourceShape.Mesh;
            source.sourceObject = mesh;
            source.transform = transform;
            source.area = 0;
            return source;
        }

        public static void CollectSources(Dungeon dungeon, DungeonNavMeshSourceType MeshSourceType, ref List<NavMeshBuildSource> sources) 
        {
            sources.Clear();

            if (dungeon == null) return;
            
            var components = GameObject.FindObjectsOfType<DungeonSceneProviderData>();
            foreach (var component in components)
            {
                if (component.dungeon == dungeon && component.affectsNavigation)
                {
                    var gameObject = component.gameObject;
                    if (MeshSourceType == DungeonNavMeshSourceType.Collision)
                    {
                        var colliders = gameObject.GetComponentsInChildren<Collider>();
                        foreach (var collider in colliders)
                        {
                            if (collider is MeshCollider)
                            {
                                var meshCollider = collider as MeshCollider;
                                NavMeshBuildSource source = CreateMeshSource(meshCollider.sharedMesh, meshCollider.transform.localToWorldMatrix);
                                sources.Add(source);
                            }
                            else
                            {
                                var source = new NavMeshBuildSource();
                                source.component = collider;
                                source.transform = collider.transform.localToWorldMatrix;
                                if (collider is BoxCollider) source.shape = NavMeshBuildSourceShape.Box;
                                else if (collider is SphereCollider) source.shape = NavMeshBuildSourceShape.Sphere;
                                else if (collider is CapsuleCollider) source.shape = NavMeshBuildSourceShape.Capsule;
                                sources.Add(source);
                            }
                        }

                    }
                    else if (MeshSourceType == DungeonNavMeshSourceType.MeshData)
                    {
                        var meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
                        foreach (var meshFilter in meshFilters)
                        {
                            if (meshFilter == null || meshFilter.sharedMesh == null) continue;
                            NavMeshBuildSource source = CreateMeshSource(meshFilter.sharedMesh, meshFilter.transform.localToWorldMatrix);
                            sources.Add(source);
                        }
                    }
                }
            }

        }

    }

}
