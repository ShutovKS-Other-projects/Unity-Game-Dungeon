//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using DungeonArchitect.Flow.Items;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Flow.Domains.Layout.Pathing
{
    public interface IFlowLayoutNodeCreationConstraint
    {
        bool CanCreateNodeAt(FlowLayoutGraphNode node, int totalPathLength, int currentPathPosition);
    }

    public class NullFlowLayoutNodeCreationConstraint : IFlowLayoutNodeCreationConstraint
    {
        public bool CanCreateNodeAt(FlowLayoutGraphNode node, int totalPathLength, int currentPathPosition)
        {
            return true;
        }
    }
    
    public class FlowLayoutStaticGrowthState {
        public FlowLayoutGraph Graph;
        public FlowLayoutGraphQuery GraphQuery;
        public FlowLayoutGraphNode HeadNode = null;
        public List<FlowLayoutGraphNode> SinkNodes = new List<FlowLayoutGraphNode>();
        public System.Random Random;
        public int MinPathSize;
        public int MaxPathSize;
        public Color NodeColor;
        public string PathName;
        public string StartNodePathNameOverride = "";
        public string EndNodePathNameOverride = "";
        public IFlowLayoutGraphConstraints GraphConstraint;
        public FlowLayoutNodeGroupGenerator NodeGroupGenerator;
        public IFlowLayoutNodeCreationConstraint NodeCreationConstraint;
    };

    public class FlowLayoutGrowthStatePathItem {
        public DungeonUID NodeId;
        public DungeonUID PreviousNodeId;
        public object userdata;

        public FlowLayoutGrowthStatePathItem Clone()
        {
            var clone = new FlowLayoutGrowthStatePathItem();
            clone.NodeId = NodeId;
            clone.PreviousNodeId = PreviousNodeId;
            clone.userdata = userdata;  // TODO: clone?
            return clone;
        }
    };

    public enum EFlowLayoutGrowthErrorType
    {
        None,
        GraphConstraint,
        NodeConstraint,
        EmptyNodeGroup,
        CannotMerge,
    }
    
    public class FlowLayoutGrowthState {
        public List<FlowLayoutGrowthStatePathItem> Path = new List<FlowLayoutGrowthStatePathItem>();
        public HashSet<DungeonUID> Visited = new HashSet<DungeonUID>();
        public List<FlowLayoutGraphNodeGroup> NodeGroups = new List<FlowLayoutGraphNodeGroup>();
        public FlowLayoutGraphNode TailNode = null;

        public FlowLayoutGrowthState Clone()
        {
            var clone = new FlowLayoutGrowthState();
            clone.Visited = new HashSet<DungeonUID>(Visited);
            clone.TailNode = TailNode;
            
            foreach (var path in Path)
            {
                clone.Path.Add(path.Clone());
            }
            
            foreach (var group in NodeGroups)
            {
                clone.NodeGroups.Add(group.Clone());
            }
            
            return clone;
        }
    };

    public class FlowLayoutSharedGrowthState
    {
        public FFAGConstraintsLink LinkFromHead;
        public FFAGConstraintsLink LinkToTail;
        public EFlowLayoutGrowthErrorType LastError = EFlowLayoutGrowthErrorType.None;
    }

    class FlowLayoutGraphPathUtils {
    
        //static bool GrowPath(const UFlowAbstractNode* CurrentNode, const FFlowAGStaticGrowthState& StaticState, FFlowAGGrowthState& OutState);
        public static void FinalizePath(FlowLayoutStaticGrowthState staticState, FlowLayoutSharedGrowthState sharedState, FlowLayoutGrowthState state)
        {
            var path = state.Path;

            if (path.Count == 0) {
                return;
            }
            
            // Create merged node groups
            foreach (var groupInfo in state.NodeGroups) {
                CreateMergedCompositeNode(staticState.Graph, staticState.GraphQuery, groupInfo);
            }
            staticState.GraphQuery.Rebuild();
            
            FlowLayoutGraph graph = staticState.GraphQuery.GetGraph();
            var childToParentMap = new Dictionary<DungeonUID, DungeonUID>(); // [ChildNodeId -> ParentNodeId]
            foreach (var parentNode in graph.Nodes) {
                if (parentNode.MergedCompositeNodes.Count > 1) {
                    foreach (var childNode in parentNode.MergedCompositeNodes) {
                        childToParentMap[childNode.nodeId] = parentNode.nodeId;
                    }
                }
            }

            var pathLength = path.Count;
            for (int i = 0; i < pathLength; i++) {
                var pathItem = path[i];
                var origNodeId = pathItem.NodeId;
                var origPrevNodeId = pathItem.PreviousNodeId;
                if (childToParentMap.ContainsKey(pathItem.NodeId))
                {
                    pathItem.NodeId = childToParentMap[pathItem.NodeId];
                }

                if (childToParentMap.ContainsKey(pathItem.PreviousNodeId))
                {
                    pathItem.PreviousNodeId = childToParentMap[pathItem.PreviousNodeId];
                }
                
                FlowLayoutGraphNode pathNode = staticState.GraphQuery.GetNode(pathItem.NodeId);
                if (pathNode == null) continue;
                pathNode.active = true;
                pathNode.color = staticState.NodeColor;
                pathNode.pathIndex = i;
                pathNode.pathLength = pathLength;

                string pathName;
                if (i == 0 && staticState.StartNodePathNameOverride.Length > 0) {
                    pathName = staticState.StartNodePathNameOverride;
                }
                else if (i == path.Count - 1 && staticState.EndNodePathNameOverride.Length > 0) {
                    pathName = staticState.EndNodePathNameOverride;
                }
                else {
                    pathName = staticState.PathName;
                }
                pathNode.pathName = pathName;
                

                // Link the path nodes
                if (i > 0) {
                    var linkSrc = pathItem.PreviousNodeId;
                    var linkDst = pathItem.NodeId;
                    var linkSrcSub = origPrevNodeId;
                    var linkDstSub = origNodeId;
                    
                    var possibleLinks = staticState.Graph.GetLinks(linkSrc, linkDst, true);
                    foreach (var possibleLink in possibleLinks) {
                        if (possibleLink == null) continue;
                        if (possibleLink.source == linkSrc && possibleLink.destination == linkDst) {
                            bool bValid = (!possibleLink.sourceSubNode.IsValid() || possibleLink.sourceSubNode == linkSrcSub);
                            bValid &= (!possibleLink.destinationSubNode.IsValid() || possibleLink.destinationSubNode == linkDstSub);

                            // Found the correct link
                            if (bValid) {
                                possibleLink.state.type = FlowLayoutGraphLinkType.Connected;
                                break;
                            }
                        }
                        else if (possibleLink.source == linkDst && possibleLink.destination == linkSrc) {
                            bool bValid = (!possibleLink.sourceSubNode.IsValid() || possibleLink.sourceSubNode == linkDstSub);
                            bValid &= (!possibleLink.destinationSubNode.IsValid() || possibleLink.destinationSubNode == linkSrcSub);

                            // Found the correct link
                            if (bValid) {
                                possibleLink.state.type = FlowLayoutGraphLinkType.Connected;
                                possibleLink.ReverseDirection();
                                break;
                            }
                        }
                    }
                }
            }

            // Setup the start / end links
            if (staticState.HeadNode != null)
            {
                Debug.Assert(sharedState.LinkFromHead != null);
                var linkSrc = sharedState.LinkFromHead.IncomingNode.nodeId;
                var linkDst = sharedState.LinkFromHead.Node.nodeId;
                
                FlowLayoutGraphLink headLink = null;
                bool reverse = false;
                foreach (var link in staticState.Graph.Links)
                {
                    if ((link.source == linkSrc || link.sourceSubNode == linkSrc) &&
                        (link.destination == linkDst || link.destinationSubNode == linkDst))
                    {
                        headLink = link;
                        break;
                    }
                    
                    if ((link.source == linkDst || link.sourceSubNode == linkDst) &&
                        (link.destination == linkSrc || link.destinationSubNode == linkSrc))
                    {
                        headLink = link;
                        reverse = true;
                        break;
                    }

                }

                if (headLink != null)
                {
                    headLink.state.type = FlowLayoutGraphLinkType.Connected;
                    if (reverse)
                    {
                        headLink.ReverseDirection();
                    }
                }
            }

            // Find the end node, if any so that it can merge back to the specified branch (specified in variable EndOnPath)
            if (state.TailNode != null) {
                Debug.Assert(sharedState.LinkToTail != null);
                var linkSrc = sharedState.LinkToTail.IncomingNode.nodeId;
                var linkDst = sharedState.LinkToTail.Node.nodeId;
                
                FlowLayoutGraphLink tailLink = null;
                bool reverse = false;
                foreach (var link in staticState.Graph.Links)
                {
                    if ((link.source == linkSrc || link.sourceSubNode == linkSrc) &&
                        (link.destination == linkDst || link.destinationSubNode == linkDst))
                    {
                        tailLink = link;
                        break;
                    }
                    
                    if ((link.source == linkDst || link.sourceSubNode == linkDst) &&
                        (link.destination == linkSrc || link.destinationSubNode == linkSrc))
                    {
                        tailLink = link;
                        reverse = true;
                        break;
                    }

                }

                if (tailLink != null)
                {
                    tailLink.state.type = FlowLayoutGraphLinkType.Connected;
                    if (reverse)
                    {
                        tailLink.ReverseDirection();
                    }
                }
            }
        }
        
        static FlowLayoutGraphNode CreateMergedCompositeNode(FlowLayoutGraph graph, FlowLayoutGraphQuery graphQuery, FlowLayoutGraphNodeGroup nodeGroup)
        {
            if (nodeGroup.GroupNodes.Count <= 1) {
                return null;
            }
    
            var subNodes = new HashSet<FlowLayoutGraphNode>();
            var subNodeIds = new HashSet<DungeonUID>();
            var subItems = new HashSet<FlowItem>();

            var previewLocation = Vector3.zero;
            var coord = Vector3.zero;
            foreach (var subNodeId in nodeGroup.GroupNodes) {
                FlowLayoutGraphNode subNode = graphQuery.GetNode(subNodeId);
                if (subNode == null) continue;
                subNodes.Add(subNode);
                subNodeIds.Add(subNodeId);
                foreach (var item in subNode.items)
                {
                    subItems.Add(item);
                }
                coord += subNode.coord;
                previewLocation += subNode.position;
            }
            var numSubNodes = subNodes.Count;
            if (numSubNodes > 0) {
                coord /= numSubNodes;
                previewLocation /= numSubNodes;

                FlowLayoutGraphNode newNode = graph.CreateNode();
                newNode.active = true;
                newNode.items = new List<FlowItem>(subItems);
                newNode.coord = coord;
                newNode.position = previewLocation;
                newNode.MergedCompositeNodes = new List<FlowLayoutGraphNode>(subNodes);

                // Remove all the sub nodes from the graph 
                foreach (FlowLayoutGraphNode subNode in subNodes) {
                    graph.Nodes.Remove(subNode);
                }

                foreach (FlowLayoutGraphLink link in graph.Links) {
                    if (subNodeIds.Contains(link.source)) {
                        link.sourceSubNode = link.source;
                        link.source = newNode.nodeId;
                    }
                    if (subNodeIds.Contains(link.destination)) {
                        link.destinationSubNode = link.destination;
                        link.destination = newNode.nodeId;
                    }
                }

                var filteredLinks = new List<FlowLayoutGraphLink>();
                foreach (var link in graph.Links)
                {
                    if (link.source != link.destination)
                    {
                        filteredLinks.Add(link);
                    }
                }

                graph.Links = filteredLinks;
                return newNode;
            }

            return null;
        }
    };

    public class FlowLayoutPathNodeGroup 
    {
        public bool IsGroup = false;
        public float Weight = 1.0f;
        public List<DungeonUID> GroupNodes = new List<DungeonUID>();        // The list of nodes that belong to this node
        public List<DungeonUID> GroupEdgeNodes = new List<DungeonUID>();     // The list of nodes on the edge of the group (so they can connect to other nodes)

        public object userdata;
    };


    public abstract class FlowLayoutNodeGroupGenerator
    {
        public abstract FlowLayoutPathNodeGroup[] Generate(FlowLayoutGraphQuery graphQuery, FlowLayoutGraphNode currentNode, int pathIndex, int pathLength, System.Random random, HashSet<DungeonUID> visited);
        
        public virtual int GetMinNodeGroupSize() { return 1; }
    }

    public class NullFlowLayoutNodeGroupGenerator : FlowLayoutNodeGroupGenerator
    {
        public override FlowLayoutPathNodeGroup[] Generate(FlowLayoutGraphQuery graphQuery, FlowLayoutGraphNode currentNode, int pathIndex, int pathLength, System.Random random, HashSet<DungeonUID> visited)
        {
            if (currentNode == null)
            {
                return new FlowLayoutPathNodeGroup[0];
            }

            var group = new FlowLayoutPathNodeGroup();
            group.IsGroup = false;
            group.GroupNodes.Add(currentNode.nodeId);
            group.GroupEdgeNodes.Add(currentNode.nodeId);
            return new FlowLayoutPathNodeGroup[] { group };
        }
    }
    
    class FlowLayoutPathStackFrame {
        public FlowLayoutGraphNode CurrentNode;
        public FlowLayoutGraphNode IncomingNode;
        public FlowLayoutGraphLink IncomingLink;
        public FlowLayoutGrowthState State = new FlowLayoutGrowthState();
    };

    class FFlowLayoutPathingSystemResult
    {
        public FFlowLayoutPathingSystemResult()
        {
        }

        public FFlowLayoutPathingSystemResult(FlowLayoutGrowthState state, FlowLayoutStaticGrowthState staticState, FlowLayoutSharedGrowthState sharedState)
        {
            this.State = state;
            this.StaticState = staticState;
            this.SharedState = sharedState;
        }

        public FlowLayoutGrowthState State;
        public FlowLayoutStaticGrowthState StaticState;
        public FlowLayoutSharedGrowthState SharedState;
    }
    
    class FlowPathGrowthSystem : StackSystem<FlowLayoutPathStackFrame, FlowLayoutStaticGrowthState, FlowLayoutSharedGrowthState, FFlowLayoutPathingSystemResult>
    {
        public FlowPathGrowthSystem(FlowLayoutStaticGrowthState staticState) : base(staticState)
        {
        }
    }

    /**
     * Maintains a list of growth systems and runs them in parallel till the first solution is found.
     * This also avoids a single solution from getting stuck and taking a very long time,
     * as multiple paths are being explored at the same time
    */
    class FFlowAgPathingSystem
    {
        public bool FoundResult
        {
            get => foundResult;
        }

        public bool Timeout
        {
            get => timeout;
        }

        public FFlowLayoutPathingSystemResult Result
        {
            get => result;
        }

        public FFlowAgPathingSystem(long maxFramesToProcess)
        {
            this.maxFramesToProcess = maxFramesToProcess;
        }

        public void RegisterGrowthSystem(FlowLayoutGraphNode startNode, FlowLayoutStaticGrowthState staticState, int count = 1)
        {
            Debug.Assert(count > 0);

            for (int i = 0; i < count; i++)
            {
                var initFrame = new FlowLayoutPathStackFrame();
                initFrame.CurrentNode = startNode;
                initFrame.IncomingNode = null;
                var growthSystem = new FlowPathGrowthSystem(staticState);
                growthSystem.Initialize(initFrame);
                growthSystems.Add(growthSystem);
            }
        }

        public void Execute(int numParallelSearches)
        {
            numParallelSearches = Mathf.Max(numParallelSearches, 1);

            frameCounter = 0;
            for (int i = 0; i < growthSystems.Count; i += numParallelSearches)
            {
                var startIdx = i;
                var endIdx = Mathf.Min(i + numParallelSearches - 1, growthSystems.Count - 1);
                ExecuteImpl(startIdx, endIdx);

                if (foundResult || timeout)
                {
                    break;
                }
            }
        }

        public EFlowLayoutGrowthErrorType GetLastError()
        {
            foreach (var growthSystem in growthSystems)
            {
                if (growthSystem != null && growthSystem.SharedState.LastError != EFlowLayoutGrowthErrorType.None)
                {
                    return growthSystem.SharedState.LastError;
                }
            }

            return EFlowLayoutGrowthErrorType.None;
        }
        
        private void ExecuteImpl(int startIdx, int endIdx)
        {
            bool running = true;
            while (running && !timeout && !foundResult)
            {
                running = false;
                for (int i = startIdx; i <= endIdx; i++)
                {
                    var growthSystem = growthSystems[i];
                    if (growthSystem.Running)
                    {
                        growthSystem.ExecuteStep(FlowLayoutPathStackGrowthTask.Execute);

                        running |= growthSystem.Running;
                        if (growthSystem.FoundResult)
                        {
                            foundResult = true;
                            result = growthSystem.Result;
                            break;
                        }

                        frameCounter++;
                        if (frameCounter >= maxFramesToProcess)
                        {
                            timeout = true;
                            break;
                        }
                    }
                }
            }
        }


        private List<FlowPathGrowthSystem> growthSystems = new List<FlowPathGrowthSystem>();
        private bool foundResult = false;
        private bool timeout = false;
        private long frameCounter = 0;
        private long maxFramesToProcess = 0;
        private FFlowLayoutPathingSystemResult result;
    }
}
