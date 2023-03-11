//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Flow.Domains.Layout.Pathing;
using DungeonArchitect.Flow.Exec;

namespace DungeonArchitect.Flow.Domains.Layout.Tasks
{
    public abstract class LayoutBaseFlowTaskPathBuilderBase : FlowExecTask
    {
        protected virtual void FinalizePath(FlowLayoutStaticGrowthState staticState, FlowLayoutSharedGrowthState sharedState, FlowLayoutGrowthState state)
        {
            FlowLayoutGraphPathUtils.FinalizePath(staticState, sharedState, state);
        }


        protected virtual FlowLayoutNodeGroupGenerator CreateNodeGroupGenerator(FlowDomainExtensions domainExtensions, FlowLayoutGraph graph)
        {
            return new NullFlowLayoutNodeGroupGenerator();
        }

        protected virtual IFlowLayoutGraphConstraints CreateGraphConstraint(FlowDomainExtensions domainExtensions, FlowLayoutGraph graph)
        {
            return new NullFlowLayoutGraphConstraints();
        }

        protected virtual IFlowLayoutNodeCreationConstraint CreateNodeCreationConstraint(FlowDomainExtensions domainExtensions, FlowLayoutGraph graph)
        {
            return new NullFlowLayoutNodeCreationConstraint();
        }
    }
}