//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;
using DungeonArchitect;
using DungeonArchitect.Utils;

namespace DungeonArchitect
{
    public class MeshMerger : DungeonEventListener
    {
        class MergeMeshItem
        {
            public Mesh mesh;
            public Matrix4x4 transform;
        }

        public bool mergeMeshes = true;
        public float mergePatchSize = 60;
        public Transform mergedMeshParent;

        private int GetMaterialHash(Material[] materials)
        {
            if (materials == null) return 0;
            var builder = new System.Text.StringBuilder();
            foreach (var material in materials)
            {
                builder.Append(material.GetHashCode().ToString());
                builder.Append("_");
            }

            return builder.ToString().GetHashCode();
        }

        public override void OnPostDungeonBuild(Dungeon dungeon, DungeonModel model)
        {
            if (!mergeMeshes)
            {
                return;
            }

            mergePatchSize = Mathf.Max(mergePatchSize, 20);

            var sceneProvider = GetComponentInParent<DungeonSceneProvider>();
            var parentToMerge = sceneProvider.itemParent;

            var itemsByMaterialHash = new Dictionary<int, List<MergeMeshItem>>();
            var hashToMaterials = new Dictionary<int, Material[]>();

            var meshFilters = parentToMerge.GetComponentsInChildren<MeshFilter>();
            foreach (var meshFilter in meshFilters)
            {
                var gameObject = meshFilter.gameObject;
                if (!gameObject.isStatic)
                {
                    continue;
                }

                var renderer = gameObject.GetComponent<MeshRenderer>();

                if (renderer != null && renderer.sharedMaterials != null)
                {
                    var materialHash = GetMaterialHash(renderer.sharedMaterials);
                    if (!itemsByMaterialHash.ContainsKey(materialHash))
                    {
                        itemsByMaterialHash.Add(materialHash, new List<MergeMeshItem>());
                        hashToMaterials.Add(materialHash, renderer.sharedMaterials);
                    }

                    var item = new MergeMeshItem();
                    item.mesh = meshFilter.sharedMesh;
                    item.transform = Matrix4x4.TRS(gameObject.transform.position, gameObject.transform.rotation, gameObject.transform.localScale);
                    itemsByMaterialHash[materialHash].Add(item);
                }

                if (Application.isPlaying)
                {
                    GameObject.Destroy(renderer);
                    GameObject.Destroy(meshFilter);
                }
                else
                {
                    GameObject.DestroyImmediate(renderer);
                    GameObject.DestroyImmediate(meshFilter);
                }

            }

            // Clear out the dungeon node Id so they are not reused
            var sceneItemInfoList = GameObject.FindObjectsOfType<DungeonSceneProviderData>();
            foreach (var sceneItemInfo in sceneItemInfoList)
            {
                if (sceneItemInfo.dungeon == dungeon)
                {
                    sceneItemInfo.NodeId = "";
                }
            }

            foreach (var entry in itemsByMaterialHash)
            {
                var materialHash = entry.Key;
                if (!hashToMaterials.ContainsKey(materialHash))
                {
                    continue;
                }

                var materials = hashToMaterials[materialHash];
                var allItems = entry.Value;
                var patchItemMap = SpatialPartitionByPatchSize(allItems);
                foreach (var patchItemEntry in patchItemMap)
                {
                    var allPatchItems = patchItemEntry.Value;
                    var patchItemsWithinLimit = SplitPerVertexLimit(allPatchItems, 65532);
                    foreach (var patchItems in patchItemsWithinLimit)
                    {
                        var mergedMesh = CreateMergedMesh(patchItems);
                        var mergedHost = new GameObject();
                        mergedHost.isStatic = true;
                        var sceneData = mergedHost.AddComponent<DungeonSceneProviderData>();
                        sceneData.affectsNavigation = false;
                        sceneData.dungeon = dungeon;
                        sceneData.NodeId = System.Guid.NewGuid().ToString();

                        var mergedFilter = mergedHost.AddComponent<MeshFilter>();
                        mergedFilter.sharedMesh = mergedMesh;

                        var mergedRenderer = mergedHost.AddComponent<MeshRenderer>();
                        mergedRenderer.sharedMaterials = materials;

                        if (mergedMeshParent != null)
                        {
                            mergedHost.transform.parent = mergedMeshParent;
                        }
                    }
                }
            }
        }

        List<MergeMeshItem[]> SplitPerVertexLimit(List<MergeMeshItem> patchItems, int vertexLimit)
        {
            var result = new List<MergeMeshItem[]>();
            var currentList = new List<MergeMeshItem>();
            int vertexCount = 0;
            foreach (var item in patchItems)
            {
                if (item == null || item.mesh == null) continue;

                if (vertexCount + item.mesh.vertexCount >= vertexLimit)
                {
                    if (currentList.Count > 0)
                    {
                        result.Add(currentList.ToArray());
                    }

                    currentList = new List<MergeMeshItem>();
                    vertexCount = 0;
                }

                currentList.Add(item);
                vertexCount += item.mesh.vertexCount;
            }

            if (currentList.Count > 0)
            {
                result.Add(currentList.ToArray());
            }

            return result;
        }

        Dictionary<IntVector2, List<MergeMeshItem>> SpatialPartitionByPatchSize(List<MergeMeshItem> items)
        {
            var patches = new Dictionary<IntVector2, List<MergeMeshItem>>();
            foreach (var item in items)
            {
                var position = Matrix.GetTranslation(ref item.transform);
                var patchIndex = new IntVector2(
                    Mathf.FloorToInt(position.x / mergePatchSize),
                    Mathf.FloorToInt(position.z / mergePatchSize));

                if (!patches.ContainsKey(patchIndex))
                {
                    patches.Add(patchIndex, new List<MergeMeshItem>());
                }

                patches[patchIndex].Add(item);
            }

            return patches;
        }

        static Mesh CreateMergedMesh(MergeMeshItem[] items)
        {
            var vertices = new List<Vector3>();
            var uv = new List<Vector2>();
            var triangles = new List<int>();

            int baseIndex = 0;
            foreach (var item in items)
            {
                foreach (var localVertex in item.mesh.vertices)
                {
                    var worldVertex = item.transform.MultiplyPoint(localVertex);
                    vertices.Add(worldVertex);
                }

                foreach (var index in item.mesh.triangles)
                {
                    triangles.Add(baseIndex + index);
                }

                baseIndex += item.mesh.vertices.Length;

                uv.AddRange(item.mesh.uv);
            }

            var mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uv);
            mesh.SetTriangles(triangles.ToArray(), 0);
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}
