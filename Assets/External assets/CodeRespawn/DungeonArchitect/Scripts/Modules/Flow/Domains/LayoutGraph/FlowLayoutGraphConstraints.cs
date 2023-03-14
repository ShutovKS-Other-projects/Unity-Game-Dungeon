//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Flow.Domains.Layout.Pathing;

namespace DungeonArchitect.Flow.Domains.Layout
{
    public class FFAGConstraintsLink
    {
        public FFAGConstraintsLink(FlowLayoutGraphNode node, FlowLayoutGraphNode incomingNode, FlowLayoutGraphLink link)
        {
            this.Node = node;
            this.IncomingNode = incomingNode;
            this.Link = link;
        }

        public FlowLayoutGraphNode Node;
        public FlowLayoutGraphNode IncomingNode;
        public FlowLayoutGraphLink Link;
    }
    
    public interface IFlowLayoutGraphConstraints
    {
        bool IsValid(FlowLayoutGraphQuery graphQuery, FlowLayoutGraphNode node, FlowLayoutGraphNode[] incomingNodes);
        bool IsValid(FlowLayoutGraphQuery graphQuery, FlowLayoutPathNodeGroup group, int pathIndex, int pathLength, FFAGConstraintsLink[] incomingNodes);
    }

    public class NullFlowLayoutGraphConstraints : IFlowLayoutGraphConstraints
    {
        public bool IsValid(FlowLayoutGraphQuery graphQuery, FlowLayoutGraphNode node, FlowLayoutGraphNode[] incomingNodes)
        {
            return true;
        }

        public bool IsValid(FlowLayoutGraphQuery graphQuery, FlowLayoutPathNodeGroup group, int pathIndex, int pathLength, FFAGConstraintsLink[] incomingNodes)
        {
            return true;
        }
    }
}
