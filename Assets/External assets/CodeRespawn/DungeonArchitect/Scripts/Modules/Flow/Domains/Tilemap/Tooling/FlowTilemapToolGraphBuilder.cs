//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Items;
using DungeonArchitect.Graphs;

namespace DungeonArchitect.Flow.Domains.Tilemap.Tooling
{
    public class FlowTilemapToolBuildContext
    {
        public FlowTilemap tilemap;
        public FlowLayoutGraph LayoutGraph;
        public GraphBuilder graphBuilder;

        public FlowLayoutGraphNode selectedNode;
        public FlowItem selectedItem;
    }

    public class FlowTilemapToolGraphBuilder
    {
        public static void Build(FlowTilemapToolBuildContext context)
        {
            if (context.graphBuilder == null)
            {
                return;
            }

            context.graphBuilder.DestroyAllNodes();

            if (context.tilemap == null)
            {
                return;
            }

            var previewNode = context.graphBuilder.CreateNode<FlowTilemapToolGraphNode>();
            if (previewNode != null)
            {
                previewNode.SetTilemap(context.tilemap);
                previewNode.LayoutGraph = context.LayoutGraph;
                previewNode.SelectedNode = context.selectedNode;
                previewNode.SelectedItem = context.selectedItem;
            }

        }
    }
}
