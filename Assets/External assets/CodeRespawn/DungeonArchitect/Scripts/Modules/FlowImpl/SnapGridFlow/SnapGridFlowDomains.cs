//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System;
using DungeonArchitect.Flow.Domains;
using DungeonArchitect.Flow.Impl.SnapGridFlow.Tasks;

namespace DungeonArchitect.Flow.Impl.SnapGridFlow
{
    public class SnapGridFlowLayoutGraph3DDomain : IFlowDomain
    {
        public Type[] SupportedTasks { get; } = supportedTypes;
        public string DisplayName { get; } = "Layout Graph";
        
        private static readonly Type[] supportedTypes = new Type[]
        {
            typeof(SGFLayoutTaskCreateGrid),
            typeof(SGFLayoutTaskCreatePath),
            typeof(SGFLayoutTaskCreateMainPath),
            typeof(SGFLayoutTaskSpawnItems),
            typeof(SGFLayoutTaskCreateKeyLock),
            typeof(SGFLayoutTaskFinalizeGraph),
            typeof(SGFLayoutTaskExpandGridSize),
            typeof(SGFLayoutTaskAddPadding),
        };
    }
}