//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using DungeonArchitect.SxEngine;
using UnityEngine;

namespace DungeonArchitect.Flow.Domains.Layout.Tooling.Graph3D
{

    [System.Serializable]
    class FlowLayoutGraphUnityVisualizerObject
    {
        [SerializeField]
        public GameObject gameObject;
        
        [SerializeField]
        private MeshFilter meshFilter;

        [SerializeField]
        private MeshRenderer meshRenderer;

        public void Build(SxRenderCommand command, Transform parent, ISxSceneNode node)
        {
            Destroy();

            gameObject = new GameObject();
            gameObject.transform.parent = parent;
            gameObject.transform.localPosition = node.WorldTransform.Positon;
            gameObject.transform.localRotation = node.WorldTransform.Rotation;
            gameObject.transform.localScale = node.WorldTransform.Scale;

            if (command == null || command.Mesh == null) return;
            var material = command.Material != null ? command.Material.UnityMaterial : null;
            if (command.Material == null) return;

            meshFilter = gameObject.AddComponent<MeshFilter>();
            meshRenderer = gameObject.AddComponent<MeshRenderer>();

            var mesh = new Mesh();
            meshFilter.sharedMesh = mesh;
            meshRenderer.sharedMaterial = material;

            var vertices = new List<Vector3>();
            var uvs = new List<Vector2>();
            var colors = new List<Color>();
            var triangles = new List<int>();

            foreach (var sectionEntry in command.Mesh.Sections)
            {
                var section = sectionEntry.Value;
                if (section == null || section.Vertices.Length == 0) continue;

                if (section.DrawMode != GL.QUADS) continue;

                for (var i = 0; i + 3 < section.Vertices.Length; i += 4)
                {
                    var baseIndex = vertices.Count;
                    var p0 = section.Vertices[i + 0];
                    var p1 = section.Vertices[i + 1];
                    var p2 = section.Vertices[i + 2];
                    var p3 = section.Vertices[i + 3];

                    var quad = new SxMeshVertex[] {p0, p1, p2, p3};
                    foreach (var vert in quad)
                    {
                        var worldPosition = vert.Position;
                        vertices.Add(worldPosition);
                        uvs.Add(vert.UV0);
                        colors.Add(vert.Color);
                    }

                    triangles.Add(baseIndex + 0);
                    triangles.Add(baseIndex + 1);
                    triangles.Add(baseIndex + 2);

                    triangles.Add(baseIndex + 0);
                    triangles.Add(baseIndex + 2);
                    triangles.Add(baseIndex + 3);
                }
            }

            mesh.vertices = vertices.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.colors = colors.ToArray();
            mesh.triangles = triangles.ToArray();

        }

        public void Destroy()
        {
            // Destroy, if already created
            if (gameObject != null)
            {
                DungeonUtils.DestroyObject(gameObject);
            }

            meshFilter = null;
            meshRenderer = null;
            gameObject = null;
        }
    }
    
    public class FlowLayoutGraphUnityVisualizer
    {
        private List<FlowLayoutGraphUnityVisualizerObject> subObjects = new List<FlowLayoutGraphUnityVisualizerObject>();
        
        public GameObject Build(SxWorld world) 
        {
            var gameObject = new GameObject("SnapGridFlowDebugVisualizer");

            var context = new SxRenderContext();
            var nodeTransforms = new Dictionary<ISxSceneNode, Transform>();
            BuildRecursive(context, world.RootNode, gameObject.transform, Matrix4x4.identity, new HashSet<ISxSceneNode>(), 0, nodeTransforms);

            return gameObject;
        }
        
        void BuildRecursive(SxRenderContext context, ISxSceneNode node, Transform parent, Matrix4x4 incomingWorldTransform, HashSet<ISxSceneNode> visited, int depth, Dictionary<ISxSceneNode, Transform> nodeTransforms)
        {
            if (visited.Contains(node))
            {
                return;
            }

            visited.Add(node);

            var accumulatedWorldTransform = incomingWorldTransform * node.WorldTransform.Matrix;

            var renderCommandList = new SxRenderCommandList();
            node.Draw(context, accumulatedWorldTransform, renderCommandList);
            
            // Build the game object
            var subObject = new FlowLayoutGraphUnityVisualizerObject();
            var renderCommand = renderCommandList.Commands.Length == 1 ? renderCommandList.Commands[0] : null; 
            subObject.Build(renderCommand, parent, node);
            subObject.gameObject.transform.parent = parent;
            nodeTransforms.Add(node, subObject.gameObject.transform);

            var camAligners = new List<FlowLayoutCamAlignerBase>();
            
            if (node is SxLayoutNodeActor)
            {
                var camAligner = subObject.gameObject.AddComponent<FlowLayoutNodeCamAligner>();
                camAligners.Add(camAligner);
            }
            else if (node is SxLayoutLinkActor)
            {
                var linkActor = node as SxLayoutLinkActor;
                if (nodeTransforms.ContainsKey(linkActor.StartActor) && nodeTransforms.ContainsKey(linkActor.EndActor))
                {
                    var startTransform = nodeTransforms[linkActor.StartActor];
                    var endTransform = nodeTransforms[linkActor.EndActor];
                
                    var camAligner = subObject.gameObject.AddComponent<FlowLayoutLinkCamAligner>();
                    camAligner.startTransform = startTransform;
                    camAligner.endTransform = endTransform;
                    camAligner.thickness = linkActor.Settings.Thickness;
                    camAligner.startRadius = linkActor.Settings.StartRadius;
                    camAligner.endRadius = linkActor.Settings.EndRadius;
                    camAligner.oneWay = linkActor.Settings.OneWay;
                    camAligners.Add(camAligner);
                }
            }
            else if (node is SxLayoutItemActor && depth == 1)
            {
                var camAligner = subObject.gameObject.AddComponent<FlowLayoutNodeCamAligner>();
                camAligners.Add(camAligner);
            }
            
            
            // Iterate the children
            foreach (var childNode in node.Children)
            {
                BuildRecursive(context, childNode, subObject.gameObject.transform, accumulatedWorldTransform, visited, depth + 1, nodeTransforms);
            }
            
            foreach (var camAligner in camAligners)
            {
                camAligner.AlignToCam();
            }
        }
    }
}