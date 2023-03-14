//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using DungeonArchitect.Frameworks.Snap;
using UnityEngine;

namespace DungeonArchitect.Builders.Snap
{
    [System.Serializable]
    public struct SnapQueryModuleInfo
    {
        [SerializeField]
        public SnapModuleInstance instanceInfo;

        [SerializeField]
        public GameObject moduleGameObject;

        [SerializeField]
        public SnapQueryConnectionInfo[] connections;
    }

    [System.Serializable]
    public struct SnapQueryConnectionInfo
    {
        [SerializeField]
        public GameObject connectionGameObject;

        [SerializeField]
        public bool isDoor;
    }

    public class SnapQuery : DungeonEventListener
    {
        [HideInInspector]
        [SerializeField]
        public SnapQueryModuleInfo[] modules;

        public override void OnPostDungeonBuild(Dungeon dungeon, DungeonModel model)
        {
            var snapModel = model as SnapModel;
            // Build the module game object mapping
            var instanceMap = new Dictionary<string, SnapModuleInstance>();
            foreach (var instance in snapModel.modules)
            {
                instanceMap[instance.InstanceID] = instance;
            }

            var moduleList = new List<SnapQueryModuleInfo>();
            var dungeonItemList = FindObjectsOfType<DungeonSceneProviderData>();
            foreach (var dungeonItem in dungeonItemList)
            {
                var instanceId = dungeonItem.NodeId;
                if (!instanceMap.ContainsKey(instanceId)) continue;

                var moduleInfo = new SnapQueryModuleInfo();
                moduleInfo.instanceInfo = instanceMap[instanceId];
                moduleInfo.moduleGameObject = dungeonItem.gameObject;

                // Build the doors 
                var parent = dungeonItem.gameObject;
                var numChildren = parent.transform.childCount;
                var connections = new List<SnapQueryConnectionInfo>();
                for (int i = 0; i < numChildren; i++)
                {
                    var child = parent.transform.GetChild(i);
                    var connectionComponent = child.gameObject.GetComponent<SnapConnection>();
                    if (connectionComponent != null)
                    {
                        var connectionInfo = new SnapQueryConnectionInfo();
                        bool valid = true;
                        if (connectionComponent.doorObject.activeInHierarchy)
                        {
                            connectionInfo.connectionGameObject = connectionComponent.doorObject;
                            connectionInfo.isDoor = true;
                        }
                        else if (connectionComponent.wallObject.activeInHierarchy)
                        {
                            connectionInfo.connectionGameObject = connectionComponent.wallObject;
                            connectionInfo.isDoor = false;
                        }
                        else
                        {
                            valid = false;
                        }

                        if (valid)
                        {
                            connections.Add(connectionInfo);
                        }
                    }
                }
                moduleInfo.connections = connections.ToArray();

                moduleList.Add(moduleInfo);
            }
            modules = moduleList.ToArray();
        }

        public bool GetModuleInfo(Vector3 position, out SnapQueryModuleInfo outModule)
        {
            foreach (var module in modules)
            {
                if (module.instanceInfo.WorldBounds.Contains(position))
                {
                    outModule = module;
                    return true;
                }
            }
            outModule = new SnapQueryModuleInfo();
            return false;
        }

        public GameObject GetModuleGameObject(Vector3 position)
        {
            SnapQueryModuleInfo moduleInfo;
            if (GetModuleInfo(position, out moduleInfo))
            {
                return moduleInfo.moduleGameObject;
            }
            return null;
        }

        public GameObject[] GetModuleIncomingDoors(Vector3 position)
        {
            var doorObjects = new List<GameObject>();
            SnapQueryModuleInfo moduleInfo;
            if (GetModuleInfo(position, out moduleInfo))
            {
                var snapModel = GetComponent<SnapModel>();
                if (snapModel != null && moduleInfo.connections != null)
                {
                    foreach (var door in snapModel.connections)
                    {
                        if (door.ModuleBInstanceID == moduleInfo.instanceInfo.InstanceID)
                        {
                            var doorIndex = door.DoorBIndex;
                            if (doorIndex >= 0 && doorIndex < moduleInfo.connections.Length)
                            {
                                var connection = moduleInfo.connections[doorIndex];
                                if (connection.isDoor)
                                {
                                    doorObjects.Add(connection.connectionGameObject);
                                }
                            }
                        }
                    }
                }
            }
            
            return doorObjects.ToArray();
        }

        public GameObject[] GetModuleOutgoingDoors(Vector3 position)
        {
            var doorObjects = new List<GameObject>();
            SnapQueryModuleInfo moduleInfo;
            if (GetModuleInfo(position, out moduleInfo))
            {
                var snapModel = GetComponent<SnapModel>();
                if (snapModel != null && moduleInfo.connections != null)
                {
                    foreach (var door in snapModel.connections)
                    {
                        if (door.ModuleAInstanceID == moduleInfo.instanceInfo.InstanceID)
                        {
                            var doorIndex = door.DoorAIndex;
                            if (doorIndex >= 0 && doorIndex < moduleInfo.connections.Length)
                            {
                                var connection = moduleInfo.connections[doorIndex];
                                if (connection.isDoor)
                                {
                                    doorObjects.Add(connection.connectionGameObject);
                                }

                            }
                        }
                    }
                }
            }
            return doorObjects.ToArray();
        }

        public GameObject[] GetModuleDoors(Vector3 position)
        {
            var doors = new List<GameObject>();
            doors.AddRange(GetModuleIncomingDoors(position));
            doors.AddRange(GetModuleOutgoingDoors(position));
            return doors.ToArray();
        }
    }

}