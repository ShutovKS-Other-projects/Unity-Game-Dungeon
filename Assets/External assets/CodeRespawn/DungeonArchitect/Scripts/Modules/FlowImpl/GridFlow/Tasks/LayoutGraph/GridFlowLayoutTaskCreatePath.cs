//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Flow.Domains;
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Domains.Layout.Pathing;
using DungeonArchitect.Flow.Exec;
using DungeonArchitect.Flow.Domains.Layout.Tasks;
using DungeonArchitect.Flow.Impl.GridFlow.Constraints;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Flow.Impl.GridFlow.Tasks
{
    [FlowExecNodeInfo("Create Path", "Layout Graph/", 1020)]
    public class GridFlowLayoutTaskCreatePath : LayoutBaseFlowTaskCreatePath
    {
        [System.Serializable]
        public enum NodeConstraintType
        {
            None,
            Script
        }
        
        // Node position constraints
        public NodeConstraintType positionConstraintMode;
        // A ScriptableObject that implements ISGFLayoutNodePositionConstraint
        public string nodePositionConstraintScriptClassName;
        
        private readonly InstanceCache instanceCache = new InstanceCache();
        
        private Vector2Int FindGridSize(FlowLayoutGraph graph)
        {
            var gridSize = Vector2Int.zero;
            foreach (var node in graph.Nodes)
            {
                if (node != null)
                {
                    var coord = MathUtils.RoundToVector3Int(node.coord);
                    gridSize.x = Mathf.Max(gridSize.x, coord.x + 1);
                    gridSize.y = Mathf.Max(gridSize.y, coord.y + 1);
                }
            }
            return gridSize;
        }

        protected override IFlowLayoutNodeCreationConstraint CreateNodeCreationConstraint(FlowDomainExtensions domainExtensions, FlowLayoutGraph graph)
        {
            if (graph != null)
            {
                if (positionConstraintMode == NodeConstraintType.Script)
                {
                    // Try to instantiate the script
                    var scriptInstance = instanceCache.GetInstance(nodePositionConstraintScriptClassName) as IGridFlowLayoutNodePositionConstraint;
                    if (scriptInstance != null)
                    {
                        var gridSize = FindGridSize(graph);
                        return new GridFlowLayoutNodeConstraintProcessorScript(scriptInstance, gridSize);
                    }
                }
            }
            return new NullFlowLayoutNodeCreationConstraint();
        }
    }
}
