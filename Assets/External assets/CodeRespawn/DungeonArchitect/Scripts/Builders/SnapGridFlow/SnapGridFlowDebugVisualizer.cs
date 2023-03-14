//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Domains.Layout.Tooling.Graph3D;
using DungeonArchitect.SxEngine;
using UnityEngine;

namespace DungeonArchitect.Builders.SnapGridFlow.DebugVisuals
{
    [ExecuteInEditMode]
    public class SnapGridFlowDebugVisualizer : DungeonEventListener
    {
        private SxWorld world;
        public float offsetY = 3;
        public float nodeRadius = 1.5f;
        
        public override void OnPostDungeonBuild(Dungeon dungeon, DungeonModel model)
        {
            var debugDraw = (dungeon != null) ? dungeon.debugDraw : false;

            if (debugDraw)
            {
                var sgfModel = model as SnapGridFlowModel;
                BuildVisualization(sgfModel.layoutGraph, dungeon);
            }
        }

        public override void OnDungeonDestroyed(Dungeon dungeon)
        {
            DestroyVisualization(dungeon);
            
            if (world != null)
            {
                world.Clear();
            }
        }
        
        void BuildVisualization(FlowLayoutGraph graph, Dungeon dungeon)
        {
            if (graph == null) return;
            var t = dungeon.transform;
            
            // Update the position of the nodes
            {
                var sgfConfig = GetComponent<SnapGridFlowConfig>();
                if (sgfConfig.moduleDatabase != null && sgfConfig.moduleDatabase.ModuleBoundsAsset != null)
                {
                    var chunkSize = sgfConfig.moduleDatabase.ModuleBoundsAsset.chunkSize;
                    foreach (var node in graph.Nodes)
                    {
                        var nodePos = Vector3.Scale(node.coord, chunkSize) + new Vector3(0, offsetY, 0);
                        node.position = t.TransformPoint(nodePos);
                        foreach (var subNode in node.MergedCompositeNodes)
                        {
                            subNode.position = Vector3.Scale(subNode.coord, chunkSize) + new Vector3(0, offsetY, 0);
                        }
                    }
                }
            }

            world = new SxWorld();
            var buildSettings = SxLayout3DWorldBuilder.BuildSettings.Create();
            buildSettings.MergedNodeMaterial = SxMaterialRegistry.Get<SxFlowMergedNodeMaterialZWrite>();
            buildSettings.ItemMaterial = SxMaterialRegistry.Get<SxFlowItemMaterialZWrite>();

            var renderSettings = new FlowLayout3DRenderSettings(nodeRadius);
            SxLayout3DWorldBuilder.Build(world, graph, buildSettings, renderSettings);

            DestroyVisualization(dungeon);
            
            var visualizer = new FlowLayoutGraphUnityVisualizer();
            var visualizerGameObject = visualizer.Build(world);
            var debugComponent = visualizerGameObject.AddComponent<SnapGridFlowDebugComponent>();
            debugComponent.dungeon = dungeon;
        }

        void DestroyVisualization(Dungeon dungeon)
        {
            var debugComponents = FindObjectsOfType<SnapGridFlowDebugComponent>();
            var gameObjectsToDestroy = new List<GameObject>();
            foreach (var debugComponent in debugComponents)
            {
                if (debugComponent == null) continue;
                if (debugComponent.dungeon == dungeon)
                {
                    gameObjectsToDestroy.Add(debugComponent.gameObject);
                }
            }
            
            foreach (var obj in gameObjectsToDestroy)
            {
                if (obj == null) continue;
                DungeonUtils.DestroyObject(obj);
            }
        }
        
    }
}