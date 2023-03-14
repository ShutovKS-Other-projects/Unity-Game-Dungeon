//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using DungeonArchitect.Flow.Impl.SnapGridFlow;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Builders.SnapGridFlow
{
    [System.Serializable]
    public struct SGFQueryModuleInfo
    {
        [SerializeField] 
        public DungeonUID ModuleInstanceId;

        [SerializeField] 
        public Bounds bounds;
    }

    public class SnapGridFlowQuery : DungeonEventListener
    {

        [HideInInspector]
        public SGFQueryModuleInfo[] modules;

        private SnapGridFlowModel sgfModel;
        
        public override void OnPostDungeonBuild(Dungeon dungeon, DungeonModel model)
        {
            sgfModel = model as SnapGridFlowModel;
            if (sgfModel == null)
            {
                return;
            }

            var moduleInfoList = new List<SGFQueryModuleInfo>();
            foreach (var node in sgfModel.snapModules)
            {
                var module = node.SpawnedModule;
                
                var info = new SGFQueryModuleInfo();
                info.ModuleInstanceId = node.ModuleInstanceId;
                {
                    var moduleBounds = module.moduleBounds;
                    var boxSize = Vector3.Scale(moduleBounds.chunkSize, MathUtils.ToVector3(module.numChunks));
                    var extent = boxSize * 0.5f;
                    var center = extent;
                    var localBounds = new Bounds(center, boxSize);
                    var localToWorld = module.transform.localToWorldMatrix;
                    info.bounds = MathUtils.TransformBounds(localToWorld, localBounds);
                }
                moduleInfoList.Add(info);
            }

            modules = moduleInfoList.ToArray();
        }

        public bool IsValid()
        {
            return modules != null && modules.Length > 0;
        }
        
        SnapGridFlowModel GetModel()
        {
            if (sgfModel == null)
            {
                sgfModel = GetComponent<SnapGridFlowModel>();
            }

            return sgfModel;
        }
        
        public SgfModuleNode GetRoomNodeAtLocation(Vector3 position)
        {
            var instanceId = DungeonUID.Empty;
            foreach (var info in modules)
            {
                var bounds = info.bounds;
                if (bounds.Contains(position))
                {
                    instanceId = info.ModuleInstanceId;
                    break;
                }
            }

            if (instanceId == DungeonUID.Empty)
            {
                return null;
            }

            var model = GetModel();
            if (model == null || model.snapModules == null)
            {
                return null;
            }

            foreach (var node in model.snapModules)
            {
                if (node.ModuleInstanceId == instanceId)
                {
                    return node;
                }
            }

            return null;
        }
        
        public SgfModuleDoor[] GetDoorsInRoomNode(Vector3 position)
        {
            var roomNode = GetRoomNodeAtLocation(position);
            if (roomNode == null || roomNode.SpawnedModule == null)
            {
                return null;
            }

            return roomNode.Doors;
        }
        
        public GameObject GetRoomGameObject(Vector3 position)
        {
            var roomNode = GetRoomNodeAtLocation(position);
            if (roomNode == null || roomNode.SpawnedModule == null)
            {
                return null;
            }

            return roomNode.SpawnedModule.gameObject;
        }

        
    }
}