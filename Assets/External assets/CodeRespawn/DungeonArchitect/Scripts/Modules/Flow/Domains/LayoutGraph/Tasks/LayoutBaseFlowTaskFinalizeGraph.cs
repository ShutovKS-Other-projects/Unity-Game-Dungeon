//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using DungeonArchitect.Flow.Exec;
using DungeonArchitect.Flow.Items;
using UnityEngine;

namespace DungeonArchitect.Flow.Domains.Layout.Tasks
{
    public class LayoutBaseFlowTaskFinalizeGraph : FlowExecTask
    {
        public bool debugDraw = false;
        public int oneWayDoorPromotionWeight = 0;


        struct ItemInfo
        {
            public ItemInfo(FlowItem item, FlowLayoutGraphNode node, FlowLayoutGraphLink link)
            {
                this.item = item;
                this.node = node;
                this.link = link;
            }

            public object GetParent()
            {
                if (node == null) return link;
                return node;
            }

            public FlowItem item;
            public FlowLayoutGraphNode node;
            public FlowLayoutGraphLink link;
        }

        public override FlowTaskExecOutput Execute(FlowTaskExecContext context, FlowTaskExecInput input)
        {
            var output = new FlowTaskExecOutput();
            if (input.IncomingTaskOutputs.Length == 0)
            {
                output.ErrorMessage = "Missing Input";
                output.ExecutionResult = FlowTaskExecutionResult.FailHalt;
                return output;
            }

            if (input.IncomingTaskOutputs.Length > 1)
            {
                output.ErrorMessage = "Only one input allowed";
                output.ExecutionResult = FlowTaskExecutionResult.FailHalt;
                return output;
            }
            
            output.State = input.CloneInputState();
            var graph = output.State.GetState<FlowLayoutGraph>();

            if (graph == null)
            {
                output.ErrorMessage = "Missing graph input";
                output.ExecutionResult = FlowTaskExecutionResult.FailHalt;
                return output;
            }

            var weights = FlowLayoutGraphUtils.CalculateWeights(graph, 10);
            if (debugDraw)
            {
                EmitDebugInfo(graph, weights);
            }

            // Make the links one directional if the difference in the source/dest nodes is too much
            foreach (var link in graph.Links)
            {
                if (link.state.type == FlowLayoutGraphLinkType.Unconnected) continue;
                var source = graph.GetNode(link.source);
                var dest = graph.GetNode(link.destination);
                if (source == null || dest == null) continue;
                if (!source.active || !dest.active) continue;

                int weightDiff = (weights[source] + 1) - weights[dest];
                if (weightDiff > oneWayDoorPromotionWeight)
                {
                    link.state.type = FlowLayoutGraphLinkType.OneWay;
                }
            }

            // Remove undirected links
            var links = graph.Links.ToArray();
            foreach (var link in links)
            {
                if (link.state.type == FlowLayoutGraphLinkType.Unconnected)
                {
                    graph.RemoveLink(link);
                }
            }

            output.ExecutionResult = FlowTaskExecutionResult.Success;
            return output;
        }

        protected IntVector2 GetNodeCoord(FlowLayoutGraphNode node)
        {
            var coordF = node.coord;
            return new IntVector2(Mathf.RoundToInt(coordF.x), Mathf.RoundToInt(coordF.y));
        }



        private void EmitDebugInfo(FlowLayoutGraph graph, Dictionary<FlowLayoutGraphNode, int> weights)
        {
            foreach (var entry in weights)
            {
                var node = entry.Key;
                var weight = entry.Value;

                var debugItem = new FlowItem();
                debugItem.type = FlowGraphItemType.Custom;
                debugItem.customInfo.itemType = "debug";
                debugItem.customInfo.text = weight.ToString();
                debugItem.customInfo.backgroundColor = new Color(0, 0, 0.3f);
                node.AddItem(debugItem);
            }
        }

        /*
        private bool ResolveKeyLocks(FlowLayoutGraph graph, ItemInfo keyInfo, ItemInfo lockInfo)
        {
            var keyItem = keyInfo.item;
            var lockItem = lockInfo.item;
            var lockNode = lockInfo.node;
            if (lockNode == null) return false;


            var incomingLinks = (from link in graph.GetIncomingLinks(lockNode)
                                where link.state.Type != FlowLayoutGraphLinkType.Unconnected
                                select link).ToArray();

            var outgoingLinks = (from link in graph.GetOutgoingLinks(lockNode)
                                 where link.state.Type != FlowLayoutGraphLinkType.Unconnected
                                 select link).ToArray();

            bool canLockIncoming = true;
            bool canLockOutgoing = true;
            if (incomingLinks.Length == 0)
            {
                canLockIncoming = false;
            }

            if (outgoingLinks.Length == 0)
            {
                canLockOutgoing = false;
            }

            var lockParent = lockInfo.GetParent();
            var keyParent = keyInfo.GetParent();
            if (lockParent == keyParent && lockParent != null)
            {
                canLockIncoming = false;
            }

            if (!canLockIncoming && !canLockOutgoing)
            {
                return false;
            }

            keyItem.referencedItemIds.Remove(lockItem.itemId);
            lockNode.Items.Remove(lockItem);

            FlowLayoutGraphLink[] linksToLock;
            if (canLockIncoming && canLockOutgoing)
            {
                // We can lock either the incoming or outgoing.  Choose the one that requires less links to be locked
                if (incomingLinks.Length == outgoingLinks.Length)
                {
                    linksToLock = incomingLinks;
                }
                else
                {
                    linksToLock = outgoingLinks.Length < incomingLinks.Length ? outgoingLinks : incomingLinks;
                }
            }
            else
            {
                linksToLock = canLockOutgoing ? outgoingLinks : incomingLinks;
            }
            foreach (var link in linksToLock)
            {
                var linkLock = lockItem.Clone();
                linkLock.itemId = System.Guid.NewGuid();
                link.state.AddItem(linkLock);
                keyItem.referencedItemIds.Add(linkLock.itemId);
            }

            return true;
        }
        */
    }
}