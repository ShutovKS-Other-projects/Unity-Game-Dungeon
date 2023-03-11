//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Items;
using DungeonArchitect.Graphs;
using UnityEngine;

namespace DungeonArchitect.Flow.Domains.Tilemap.Tooling
{
    public class FlowTilemapToolGraphNode : GraphNode
    {
        public int tileRenderSize = 12;
        public FlowTilemap Tilemap { get; private set; }
        public FlowLayoutGraph LayoutGraph { get; set; }
        public FlowLayoutGraphNode SelectedNode { get; set; }
        public FlowItem SelectedItem { get; set; }
        public bool RequestRecreatePreview { get; set; }

        public FlowTilemapToolGraphNode()
        {
            RequestRecreatePreview = false;
        }

        public override void Initialize(string id, Graph graph)
        {
            base.Initialize(id, graph);
        }

        public void SetTilemap(FlowTilemap tilemap)
        {
            this.Tilemap = tilemap;

            // Update the bounds
            var size = new Vector2(tilemap.Width, tilemap.Height) * tileRenderSize;
            bounds = new Rect(Vector2.zero, size);
        }
    }
}
