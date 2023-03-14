//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Flow.Exec;
using UnityEngine;

namespace DungeonArchitect.Flow
{
    [System.Serializable]
    public class FlowAssetBase : ScriptableObject
    {
        [HideInInspector]
        [SerializeField]
        public FlowExecGraph execGraph;
    }
}