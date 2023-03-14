//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Builders.Snap
{

    [System.Serializable]
    public class SnapModuleInstance
    {
        /// <summary>
        /// The instance id of the module
        /// </summary>
        public string InstanceID;

        public Matrix4x4 WorldTransform;

        public Bounds WorldBounds;
    }

    [System.Serializable]
    public class SnapModuleConnection
    {
        /// <summary>
        /// The instance ID of the spawned module (See ModuleInstance structure)
        /// </summary>
        public string ModuleAInstanceID;

        /// <summary>
        /// The index of the door(see ModuleInfo structure)
        /// </summary>
        public int DoorAIndex;

        /// <summary>
        /// The instance ID of the spawned module (See ModuleInstance structure)
        /// </summary>
        public string ModuleBInstanceID;

        /// <summary>
        /// The index of the door(see ModuleInfo structure)
        /// </summary>
        public int DoorBIndex;
    }

    public class SnapModel : DungeonModel
    {
        [HideInInspector]
        public SnapModuleInstance[] modules;

        [HideInInspector]
        public SnapModuleConnection[] connections;


        public override void ResetModel()
        {
            modules = new SnapModuleInstance[0];
            connections = new SnapModuleConnection[0];
        }
    }

}