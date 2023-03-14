//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using DungeonArchitect.Flow.Items;
using DungeonArchitect.Utils;

namespace DungeonArchitect.Flow.Domains.Layout
{
    [System.Serializable]
    public enum FlowLayoutGraphLinkType
    {
        Unconnected,
        Connected,
        OneWay
    }
    
    [System.Serializable]
    public class FlowLayoutGraphLinkState
    {
        //public bool Directional = false;
        //public bool OneWay = false;
        public FlowLayoutGraphLinkType type = FlowLayoutGraphLinkType.Unconnected;
        public List<FlowItem> items = new List<FlowItem>();

        public FlowLayoutGraphLinkState Clone()
        {
            var newState = new FlowLayoutGraphLinkState();
            newState.type = type;
            foreach (var item in items)
            {
                newState.AddItem(item.Clone());
            }
            return newState;
        }

        public void AddItem(FlowItem item)
        {
            items.Add(item);
        }

    }

    [System.Serializable]
    public class FlowLayoutGraphLink
    {
        public DungeonUID linkId;
        public DungeonUID source;
        public DungeonUID destination;
        public FlowLayoutGraphLinkState state = new FlowLayoutGraphLinkState();

        // If the source node was merged, the original unmerged node id would be here
        public DungeonUID sourceSubNode;
        
        // If the destination node was merged, the original unmerged node id would be here
        public DungeonUID destinationSubNode;
        
        public FlowLayoutGraphLink()
        {
            linkId = DungeonUID.NewUID();
        }

        public FlowLayoutGraphLink Clone()
        {
            var newLink = new FlowLayoutGraphLink();
            newLink.linkId = linkId;
            newLink.source = source;
            newLink.destination = destination;
            newLink.state = state.Clone();
            newLink.sourceSubNode = sourceSubNode;
            newLink.destinationSubNode = destinationSubNode;
            return newLink;
        }

        public void ReverseDirection()
        {
            MathUtils.Swap(ref source, ref destination);
            MathUtils.Swap(ref sourceSubNode, ref destinationSubNode);
        }
    }
}
