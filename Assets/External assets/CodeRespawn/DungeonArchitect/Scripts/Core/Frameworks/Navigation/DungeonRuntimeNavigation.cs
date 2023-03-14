//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;

namespace DungeonArchitect
{
    [System.Serializable]
    public enum DungeonNavMeshSourceType
    {
        MeshData,
        Collision
    }

    public class DungeonRuntimeNavigation : MonoBehaviour
    {
        /// <summary>
        /// Should dynamic navigation be created during runtime (For NPC AI)
        /// </summary>
        public bool enableRuntimeNavigation = false;

        public Vector3 boundsPadding = Vector3.zero;

        public bool bakeTerrain = true;
        public Terrain terrain;

        DungeonNavMeshSourceType meshSourceType = DungeonNavMeshSourceType.MeshData;

        // The size of the build bounds
        Bounds dungeonBounds;

        NavMeshData m_NavMesh;
        //AsyncOperation m_Operation;
        NavMeshDataInstance m_Instance;
        List<NavMeshBuildSource> meshSources = new List<NavMeshBuildSource>();

        public void BuildNavMesh()
        {
            DestroyNavMesh();

            if (enableRuntimeNavigation && Application.isPlaying)
            {
                m_NavMesh = new NavMeshData();
                m_Instance = NavMesh.AddNavMeshData(m_NavMesh);
                var dungeon = GetComponent<Dungeon>();
                if (!dungeon)
                {
                    Debug.LogError("DungeonRuntimeNavigation should be attached to a Dungeon prefab. Missing Dungeon Script in the game object");
                    return;
                }

                dungeonBounds = DungeonUtils.GetDungeonBounds(dungeon);
                dungeonBounds.size = dungeonBounds.size + boundsPadding;

                UpdateNavMesh(false);
            }
        }

        public void DestroyNavMesh()
        {
            // Unload navmesh and clear handle
            m_Instance.Remove();
            m_NavMesh = null;
            meshSources.Clear();

            dungeonBounds = new Bounds();
        }
        

        void CollectMeshSources() {
            meshSources.Clear();

            var dungeon = GetComponent<Dungeon>();
            if (!dungeon)
            {
                Debug.LogError("DungeonRuntimeNavigation should be attached to a Dungeon prefab. Missing Dungeon Script in the game object");
                return;
            }

            DungeonNavMeshSourceCollector.CollectSources(dungeon, meshSourceType, ref meshSources);

            if (bakeTerrain && terrain != null)
            {
                var source = new NavMeshBuildSource();
                source.shape = NavMeshBuildSourceShape.Terrain;
                source.sourceObject = terrain.terrainData;
                source.transform = terrain.transform.localToWorldMatrix;
                source.area = 0;
                meshSources.Add(source);
            }
        }

        void UpdateNavMesh(bool asyncUpdate = false)
        {
            CollectMeshSources();


            //NavMeshSourceTag.Collect(ref m_Sources);
            var defaultBuildSettings = NavMesh.GetSettingsByID(0);

            if (asyncUpdate)
            {
                /*m_Operation = */ NavMeshBuilder.UpdateNavMeshDataAsync(m_NavMesh, defaultBuildSettings, meshSources, dungeonBounds);
            }
            else
            {
                NavMeshBuilder.UpdateNavMeshData(m_NavMesh, defaultBuildSettings, meshSources, dungeonBounds);
            }
        }
        
        void OnDrawGizmosSelected()
        {
            if (!enableRuntimeNavigation) return;

            if (m_NavMesh)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(m_NavMesh.sourceBounds.center, m_NavMesh.sourceBounds.size);
            }
            
            Gizmos.color = Color.green;
            var center = dungeonBounds.center;
            var size = dungeonBounds.size;
            Gizmos.DrawWireCube(center, size);
        }


        void OnDisable()
        {
            DestroyNavMesh();
        }

    }

}