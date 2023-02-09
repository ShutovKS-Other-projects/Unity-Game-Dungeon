//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using System;
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Impl.SnapGridFlow;
using UnityEngine;

namespace DungeonArchitect.Builders.SnapGridFlow
{
    public class SnapGridFlowModel : DungeonModel
    {
        [HideInInspector]
        [NonSerialized]
        public FlowLayoutGraph layoutGraph;
        
        [HideInInspector]
        [NonSerialized]
        public SgfModuleNode[] snapModules;
        
        public override void ResetModel()
        {
            layoutGraph = null;
            snapModules = new SgfModuleNode[0];
        }
    }
}