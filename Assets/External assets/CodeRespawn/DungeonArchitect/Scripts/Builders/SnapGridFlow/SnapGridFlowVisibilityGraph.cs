//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using DungeonArchitect.Flow.Impl.SnapGridFlow;
using DungeonArchitect.Utils;
using DungeonArchitect.Visibility;
using DungeonArchitect.Visibility.Impl;
using UnityEngine;

namespace DungeonArchitect.Builders.SnapGridFlow
{
    public class SnapGridFlowVisibilityGraph : DungeonEventListener
    {
        public int visibilityDepth = 1;
        public Transform[] trackedObjects;
        
        private VisibilityGraph visibilityGraph = new VisibilityGraph();


        void Update()
        {
            UpdateVisibility();
        }
        
        private void UpdateVisibility() {
            if (trackedObjects.Length == 0)
            {
                // Disable the updates when we don't have any objects to track
                return;
            }
            
            var trackedPositions = new List<Vector3>();
            if (trackedObjects != null)
            {
                foreach (var trackedObject in trackedObjects)
                {
                    if (trackedObject != null)
                    {
                        trackedPositions.Add(trackedObject.position);
                    }
                }
            }

            visibilityGraph.UpdateVisibility(trackedPositions.ToArray());
        }

        private void BuildVisibilityGraph(SnapGridFlowModel model)
        {
            visibilityGraph.Clear();
            visibilityGraph.VisibilityDepth = visibilityDepth;
            
            if (model != null && model.snapModules != null && model.layoutGraph != null)
            {
                var modules = new Dictionary<DungeonUID, SgfModuleNode>();
                var visibilityNodes = new Dictionary<DungeonUID, VisibilityGraphNode>();
                
                foreach (var moduleInfo in model.snapModules)
                {
                    if (moduleInfo == null || moduleInfo.SpawnedModule == null) continue;
                    modules[moduleInfo.ModuleInstanceId] = moduleInfo;
                    
                    var visibilityNode = new GameObjectVisibilityGraphNode(moduleInfo.SpawnedModule.gameObject);
                    visibilityGraph.RegisterNode(visibilityNode);
                    visibilityNodes[moduleInfo.ModuleInstanceId] = visibilityNode;
                }
                
                foreach (var link in model.layoutGraph.Links)
                {
                    if (visibilityNodes.ContainsKey(link.source) && visibilityNodes.ContainsKey(link.destination))
                    {
                        var source = visibilityNodes[link.source];
                        var dest = visibilityNodes[link.destination];
                        
                        source.AddConnection(dest);
                        dest.AddConnection(source);
                    }
                }
            }
        }

        public override void OnPostDungeonBuild(Dungeon dungeon, DungeonModel model)
        {
            BuildVisibilityGraph(model as SnapGridFlowModel);
        }

        public override void OnDungeonDestroyed(Dungeon dungeon)
        {
            visibilityGraph.Clear();
        }
    }
}