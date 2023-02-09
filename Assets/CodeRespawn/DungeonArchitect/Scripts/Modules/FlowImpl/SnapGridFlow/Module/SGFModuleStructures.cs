//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System;
using System.Collections.Generic;
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Impl.SnapGridFlow.Components;
using DungeonArchitect.Frameworks.Snap;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Flow.Impl.SnapGridFlow
{
    [System.Serializable]
    public class SgfModuleDoor
    {
        // The transform of the door relative to the module
        public Matrix4x4 LocalTransform;

        // The module that hosts this door
        [NonSerialized] 
        public SgfModuleNode Owner;

        // The other door that is connected to this door
        public SgfModuleDoor ConnectedDoor;

        // The spawned door component
        public SnapConnection SpawnedDoor;

        public SgfModuleAssemblySideCell CellInfo;
    };

    [System.Serializable]
    public class SgfModuleNode {
        public DungeonUID ModuleInstanceId;
        public Matrix4x4 WorldTransform = Matrix4x4.identity;
        
        [NonSerialized]
        public SgfModuleDatabaseItem ModuleDBItem;

        [NonSerialized]
        public FlowLayoutGraphNode LayoutNode;
        
        // The spawned module prefab
        public SnapGridFlowModule SpawnedModule;

        // The doors in this module
        public SgfModuleDoor[] Doors = new SgfModuleDoor[0];

        public HashSet<SgfModuleDoor> Incoming = new HashSet<SgfModuleDoor>();
        public HashSet<SgfModuleDoor> Outgoing = new HashSet<SgfModuleDoor>();

        public Bounds GetModuleBounds()
        {
            return MathUtils.TransformBounds(WorldTransform, ModuleDBItem.ModuleBounds);
        }
    };
}