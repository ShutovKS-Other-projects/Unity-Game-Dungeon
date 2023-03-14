//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;
using DungeonArchitect.Graphs;
using DungeonArchitect.Utils;

namespace DungeonArchitect
{
    [ExecuteInEditMode]
    public class InfinityDungeon : MonoBehaviour
    {
        public Transform buildPositionTracker;
        public float buildDistance = 40;

        public List<Graph> dungeonThemes;
        public Transform parentGameObject;
        public GameObject chunkDungeonTemplate;

        [SerializeField]
        List<InfinityDungeonConfig> spawnedChunks = new List<InfinityDungeonConfig>();

        [SerializeField]
        [HideInInspector]
        bool dungeonBuilt = false;

        InfinityDungeonConfig config;

        private void Awake()
        {
            config = GetComponent<InfinityDungeonConfig>();
        }

        public void EditorUpdate()
        {
            spawnedChunks.RemoveAll(c => c == null || c.gameObject == null);
            foreach (var chunk in spawnedChunks)
            {
                var dungeon = chunk.gameObject.GetComponent<Dungeon>();
                dungeon.Update();
            }
        }

        private void Update()
        {
            if (dungeonBuilt)
            {
                UpdateChunks();
            }
        }

        public void BuildDungeon()
        {
            config = GetComponent<InfinityDungeonConfig>();

            DestroyDungeon();

            dungeonBuilt = true;
            UpdateChunks();
        }

        void UpdateChunks()
        {
            var position = (buildPositionTracker != null) ? buildPositionTracker.position : Vector3.zero;
            UpdateChunks(position);
        }

        void UpdateChunks(Vector3 buildPosition)
        {
            if (!dungeonBuilt)
            {
                return;
            }
            var buildExtents = new Vector3(buildDistance, buildDistance, buildDistance);
            var boundsMin = buildPosition - buildExtents;
            var boundsMax = buildPosition + buildExtents;

            // Convert to logical coords (divide by the grid size, if it exists)
            boundsMin = config.GetLogicalCoord(boundsMin);
            boundsMax = config.GetLogicalCoord(boundsMax);

            boundsMin = ClampOnBuildAxis(boundsMin);
            boundsMax = ClampOnBuildAxis(boundsMax);

            var size = config.chunkSize;
            size.x = Mathf.Max(size.x, 1);
            size.y = Mathf.Max(size.y, 1);
            size.z = Mathf.Max(size.z, 1);

            // convert to chunk coords
            boundsMin = MathUtils.V3FloorToInt(MathUtils.Divide(boundsMin, size));
            boundsMax = MathUtils.V3FloorToInt(MathUtils.Divide(boundsMax, size));

            var imin = new IntVector(boundsMin);
            var imax = new IntVector(boundsMax);

            var chunksToSpawn = new List<Vector3>();
            for (int x = imin.x; x <= imax.x; x++)
            {
                for (int y = imin.y; y <= imax.y; y++)
                {
                    for (int z = imin.z; z <= imax.z; z++)
                    {
                        var position = Vector3.Scale(new Vector3(x, y, z), config.chunkSize);
                        chunksToSpawn.Add(position);
                    }
                }
            }
            
            var chunksToDestroy = new List<InfinityDungeonConfig>();
            
            foreach (var existingChunk in spawnedChunks)
            {
                if (existingChunk == null) continue;
                if (chunksToSpawn.Contains(existingChunk.chunkPosition))
                {
                    // Already contains this chunk. No need to spawn
                    chunksToSpawn.Remove(existingChunk.chunkPosition);
                }
                else
                {
                    // Do not contain an entry for this chunk. We no longer need this chunk
                    chunksToDestroy.Add(existingChunk);
                }
            }

            // Destroy the existing chunks
            foreach (var chunkToDestroy in chunksToDestroy)
            {
                spawnedChunks.Remove(chunkToDestroy);

                if (chunkToDestroy.gameObject != null)
                {
                    var dungeon = chunkToDestroy.gameObject.GetComponent<Dungeon>();
                    if (dungeon != null)
                    {
                        dungeon.DestroyDungeon();
                    }
                    DungeonUtils.DestroyObject(chunkToDestroy.gameObject);
                }
            }

            // Build the new chunks
            foreach (var chunkToSpawn in chunksToSpawn)
            {
                var dungeon = BuildDungeonChunk(chunkToSpawn);
                var config = dungeon.GetComponent<InfinityDungeonConfig>();
                spawnedChunks.Add(config);
            }
        }

        Vector3 ClampOnBuildAxis(Vector3 p)
        {
            if (!config.BuildAlongX()) p.x = 0;
            if (!config.BuildAlongY()) p.y = 0;
            if (!config.BuildAlongZ()) p.z = 0;
            return p;
        }

        public Dungeon BuildDungeonChunk(Vector3 chunkPosition)
        {
            var dungeonObject = GameObject.Instantiate(chunkDungeonTemplate);
            dungeonObject.isStatic = true;

            if (parentGameObject != null)
            {
                dungeonObject.transform.parent = parentGameObject;
            }

            // Set up the parent for this object
            var sceneProvider = dungeonObject.GetComponent<DungeonSceneProvider>();
            sceneProvider.itemParent = dungeonObject;

            // copy the config
            var config = dungeonObject.GetComponent<InfinityDungeonConfig>();
            CopyDungeonConfig(config);

            // Setup the position for this chunk
            config.chunkPosition = chunkPosition;

            // Setup the position tracker
            var builder = dungeonObject.GetComponent<DungeonBuilder>();
            builder.asyncBuildStartPosition = buildPositionTracker;

            // Copy over the theme files
            var dungeon = dungeonObject.GetComponent<Dungeon>();
            dungeon.dungeonThemes = new List<Graph>(dungeonThemes);

            dungeon.Build();

            return dungeon;
        }

        public void DestroyDungeon()
        {
            foreach (var chunkConfig in spawnedChunks)
            {
                if (chunkConfig == null || chunkConfig.gameObject == null) continue;
                var dungeon = chunkConfig.gameObject.GetComponent<Dungeon>();
                if (dungeon != null)
                {
                    dungeon.DestroyDungeon();
                    DungeonUtils.DestroyObject(dungeon.gameObject);
                }
            }

            spawnedChunks.Clear();
            dungeonBuilt = false;
        }

        public void CopyDungeonConfig(InfinityDungeonConfig targetConfig)
        {
            var sourceConfig = GetComponent<InfinityDungeonConfig>();

            var fields = sourceConfig.GetType().GetFields();
            foreach (var field in fields)
            {
                field.SetValue(targetConfig, field.GetValue(sourceConfig));
            }
        }

    }
}
