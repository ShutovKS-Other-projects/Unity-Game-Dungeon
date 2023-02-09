//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Flow.Domains;
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Domains.Layout.Pathing;
using DungeonArchitect.Flow.Domains.Layout.Tasks;
using DungeonArchitect.Flow.Exec;
using DungeonArchitect.Flow.Impl.SnapGridFlow.Constraints;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Flow.Impl.SnapGridFlow.Tasks
{
    [FlowExecNodeInfo("Create Path", "Layout Graph/", 1020)]
    public class SGFLayoutTaskCreatePath : LayoutBaseFlowTaskCreatePath, ISGFLayoutTaskPathBuilder
    {
        [System.Serializable]
        public enum NodeConstraintType
        {
            None,
            Script
        }
        
        public string[] snapModuleCategories = new string[] { "Room" };
        
        // Node position constraints
        public NodeConstraintType positionConstraintMode;
        // A ScriptableObject that implements ISGFLayoutNodePositionConstraint
        public string nodePositionConstraintScriptClassName;

        // Snap module category constraints
        public NodeConstraintType categoryConstraintMode;
        // A ScriptableObject that implements ISGFLayoutNodeCategoryConstraint
        public string categoryConstraintScriptClassName;
        
        private readonly InstanceCache instanceCache = new InstanceCache();
        
        public override FlowTaskExecOutput Execute(FlowTaskExecContext context, FlowTaskExecInput input)
        {
            FlowTaskExecOutput output;
            if (snapModuleCategories.Length == 0)
            {
                output = new FlowTaskExecOutput();
                output.ErrorMessage = "Missing Module Categories";
                output.ExecutionResult = FlowTaskExecutionResult.FailHalt;
                return output;
            }
            
            output = base.Execute(context, input);
            instanceCache.Clear();
            
            return output;
        }

        protected override void FinalizePath(FlowLayoutStaticGrowthState staticState, FlowLayoutSharedGrowthState sharedState, FlowLayoutGrowthState state)
        {
            base.FinalizePath(staticState, sharedState, state);
            
            // Extend the path nodes with the snap domain data
            for (var i = 0; i < state.Path.Count; i++)
            {
                var pathItem = state.Path[i];
                var node = staticState.GraphQuery.GetNode(pathItem.NodeId);
                if (node == null) continue;

                var sgfNodeGroupData = pathItem.userdata as SGFNodeGroupUserData;
                if (sgfNodeGroupData != null)
                {
                    node.SetDomainData(sgfNodeGroupData);
                }

                var snapNodeData = node.GetDomainData<FlowLayoutNodeSnapDomainData>();
                var pathLength = Mathf.Max(i + 1, minPathSize);
                snapNodeData.Categories = GetCategoriesAtNode(i, pathLength);
            }
        }

        public string[] GetSnapModuleCategories()
        {
            return snapModuleCategories;
        }

        public string[] GetCategoriesAtNode(int pathIndex, int pathLength)
        {
            if (categoryConstraintMode == NodeConstraintType.Script)
            {
                if (instanceCache != null && !string.IsNullOrWhiteSpace(categoryConstraintScriptClassName))
                {
                    var instance = instanceCache.GetInstance(categoryConstraintScriptClassName) as ISGFLayoutNodeCategoryConstraint;
                    if (instance != null)
                    {
                        return instance.GetModuleCategoriesAtNode(pathIndex, pathLength);
                    }
                }
            }
            return snapModuleCategories;
        }

        private SnapGridFlowModuleDatabase GetModuleDatabase(FlowDomainExtensions domainExtensions)
        {
            var extension = domainExtensions.GetExtension<SnapGridFlowDomainExtension>();
            return extension.ModuleDatabase;
        }
        
        protected override FlowLayoutNodeGroupGenerator CreateNodeGroupGenerator(FlowDomainExtensions domainExtensions, FlowLayoutGraph graph)
        {
            var moduleDatabase = GetModuleDatabase(domainExtensions);
            if (moduleDatabase == null)
            {
                return new NullFlowLayoutNodeGroupGenerator();
            }
            else
            {
                return new SnapFlowLayoutNodeGroupGenerator(moduleDatabase, this);
            }
        }

        protected override IFlowLayoutGraphConstraints CreateGraphConstraint(FlowDomainExtensions domainExtensions, FlowLayoutGraph graph)
        {
            var moduleDatabase = GetModuleDatabase(domainExtensions);
            if (moduleDatabase == null)
            {
                return new NullFlowLayoutGraphConstraints();
            }
            else
            {             
                return new SnapFlowLayoutGraphConstraints(moduleDatabase, this);   
            }
        }

        private Vector3Int FindGridSize(FlowLayoutGraph graph)
        {
            var gridSize = Vector3Int.zero;
            foreach (var node in graph.Nodes)
            {
                if (node != null)
                {
                    var coord = MathUtils.RoundToVector3Int(node.coord);
                    gridSize.x = Mathf.Max(gridSize.x, coord.x + 1);
                    gridSize.y = Mathf.Max(gridSize.y, coord.y + 1);
                    gridSize.z = Mathf.Max(gridSize.z, coord.z + 1);
                }
            }
            return gridSize;
        }

        protected override IFlowLayoutNodeCreationConstraint CreateNodeCreationConstraint(FlowDomainExtensions domainExtensions, FlowLayoutGraph graph)
        {           if (graph != null)
            {
                if (positionConstraintMode == NodeConstraintType.Script)
                {
                    // Try to instantiate the script
                    var scriptInstance = instanceCache.GetInstance(nodePositionConstraintScriptClassName) as ISGFLayoutNodePositionConstraint;
                    if (scriptInstance != null)
                    {
                        var gridSize = FindGridSize(graph);
                        return new SGFLayoutNodeConstraintProcessorScript(scriptInstance, gridSize);
                    }
                }
            }
            return new NullFlowLayoutNodeCreationConstraint();
        }
    }
}