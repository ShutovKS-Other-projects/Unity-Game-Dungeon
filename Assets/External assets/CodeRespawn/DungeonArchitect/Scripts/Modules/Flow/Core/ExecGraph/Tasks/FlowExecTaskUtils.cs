//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;

namespace DungeonArchitect.Flow.Exec
{
    public class FlowExecTaskUtils 
    {
        public static FlowExecTaskState[] GetIncomingStates(FlowExecRuleGraphNode currentNode, FlowExecNodeOutputRegistry nodeOutputRegistry)
        {
            var incomingStates = new List<FlowExecTaskState>();
            var incomingNodes = FlowExecGraphUtils.GetIncomingNodes(currentNode);
            foreach (var incomingNode in incomingNodes)
            {
                var incomingExecState = nodeOutputRegistry.Get(incomingNode.Id);
                if (incomingExecState != null)
                {
                    incomingStates.Add(incomingExecState.State);
                }
            }
            return incomingStates.ToArray();
        }
    }
}
