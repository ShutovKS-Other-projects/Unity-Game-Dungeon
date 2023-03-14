//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;
using DungeonArchitect.Graphs;

namespace DungeonArchitect.Flow.Exec
{
    public class FlowExecGraph : Graph
    {
        [SerializeField]
        public FlowExecResultGraphNode resultNode;

        public override void OnEnable()
        {
            base.OnEnable();

            hideFlags = HideFlags.HideInHierarchy;
        }
    }



    public class FlowExecGraphUtils
    {
        public static FlowExecRuleGraphNode[] GetIncomingNodes(FlowExecRuleGraphNode node)
        {
            var result = new List<FlowExecRuleGraphNode>();
            var incomingNodes = GraphUtils.GetIncomingNodes(node);
            foreach (var incomingNode in incomingNodes)
            {
                var incomingExecNode = incomingNode as FlowExecRuleGraphNode;
                if (incomingExecNode != null)
                {
                    result.Add(incomingExecNode);
                }
            }
            return result.ToArray();
        }

    }
}
