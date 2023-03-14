//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using DungeonArchitect.Flow.Impl.SnapGridFlow.Components;
using UnityEngine;

namespace DungeonArchitect.Flow.Impl.SnapGridFlow
{   
    
    // Snap Grid Flow Module Database
    [System.Serializable]
    public class SgfModuleDatabaseConnectionInfo
    {
        public int ConnectionIndex = -1;
        public Matrix4x4 Transform = Matrix4x4.identity;
        public string Category = "";
    };

    [System.Serializable]
    public class SgfModuleDatabasePlaceableMarkerInfo
    {
        public PlaceableMarker placeableMarkerTemplate;
        public int count;
    }
    
    [System.Serializable]
    public class SgfModuleDatabaseItem : ISerializationCallbackReceiver {
        [SerializeField]
        public SnapGridFlowModule ModulePrefab;
        [SerializeField]
        public string Category = "Room";
        [SerializeField]
        public bool allowRotation = true;
        // How often do you want this to be selected?  0.0 for least preference, 1.0 for most preference.  Specify a value from 0.0 to 1.0 
        [SerializeField]
        public float SelectionWeight = 1.0f;
        
        [HideInInspector]
        public Bounds ModuleBounds;
        [HideInInspector]
        public Vector3Int NumChunks = new Vector3Int(1, 1, 1);
        [HideInInspector]
        public SgfModuleDatabaseConnectionInfo[] Connections;
        [HideInInspector]
        public SgfModuleAssembly[] RotatedAssemblies;   // 4 Cached module assemblies rotated in 90 degree CW steps
        [HideInInspector]
        public SgfModuleDatabasePlaceableMarkerInfo[] AvailableMarkers;
        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            if (Category.Length == 0 && ModulePrefab == null && !allowRotation && SelectionWeight == 0)
            {
                Category = "Room";
                allowRotation = true;
                SelectionWeight = 1;
            }
        }
    };

    public class SnapGridFlowModuleDatabase : ScriptableObject
    {
        public SnapGridFlowModuleBounds ModuleBoundsAsset;
        public SgfModuleDatabaseItem[] Modules;

        public SgfModuleDatabaseItem[] GetCategoryModules(string category)
        {
            var result = new List<SgfModuleDatabaseItem>();
            
            foreach (var moduleItem in Modules)
            {
                if (moduleItem.Category == category)
                {
                    result.Add(moduleItem);
                }
            }

            return result.ToArray();
        }
    }
}