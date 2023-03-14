//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Linq;
using DungeonArchitect.Flow.Exec;
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Domains.Layout.Tasks;
using DungeonArchitect.Flow.Items;

namespace DungeonArchitect.Flow.Impl.GridFlow.Tasks
{

    [FlowExecNodeInfo("Finalize Graph", "Layout Graph/", 1050)]
    public class GridFlowLayoutTaskFinalizeGraph : LayoutBaseFlowTaskFinalizeGraph
    {
        public bool generateCaves = true;
        public bool generateCorridors = true;
        public int maxEnemiesPerCaveNode = 3;

        public override FlowTaskExecOutput Execute(FlowTaskExecContext context, FlowTaskExecInput input)
        {
            var result = base.Execute(context, input);
            if (result.ExecutionResult == FlowTaskExecutionResult.Success)
            {
                var graph = result.State.GetState<FlowLayoutGraph>();
                AssignRoomTypes(graph, context.Random);
            }

            return result;
        }

        void AssignRoomTypes(FlowLayoutGraph graph, System.Random random)
        {
            foreach (var node in graph.Nodes)
            {
                var tilemapDomain = node.GetDomainData<GridFlowTilemapDomainData>();
                tilemapDomain.RoomType = GetNodeRoomType(graph, node);
            }

            // Make another pass and force assign rooms where a link requires a door
            foreach (var link in graph.Links)
            {
                bool containsLock = link.state.items.Count(i => i.type == FlowGraphItemType.Lock) > 0;
                if (containsLock || link.state.type == FlowLayoutGraphLinkType.OneWay)
                {
                    // We need atleast one room type that supports doors (rooms and corridors)
                    var nodeA = graph.GetNode(link.source);
                    var nodeB = graph.GetNode(link.destination);

                    var domainDataA = nodeA.GetDomainData<GridFlowTilemapDomainData>();
                    var domainDataB = nodeB.GetDomainData<GridFlowTilemapDomainData>();
                    
                    var containsDoorA = (domainDataA.RoomType == GridFlowLayoutNodeRoomType.Room || domainDataA.RoomType == GridFlowLayoutNodeRoomType.Corridor);
                    var containsDoorB = (domainDataB.RoomType == GridFlowLayoutNodeRoomType.Room || domainDataB.RoomType == GridFlowLayoutNodeRoomType.Corridor);
                    if (!containsDoorA && !containsDoorB)
                    {
                        // promote one of them to a room
                        var nodeToPromote = (random.NextFloat() < 0.5f) ? domainDataA : domainDataB;
                        nodeToPromote.RoomType = GridFlowLayoutNodeRoomType.Room;
                    }
                }
            }
        }

        GridFlowLayoutNodeRoomType GetNodeRoomType(FlowLayoutGraph graph, FlowLayoutGraphNode node)
        {
            var incoming = graph.GetIncomingLinks(node).ToArray();
            var outgoing = graph.GetOutgoingLinks(node).ToArray();
            int numEnemies = node.items.Count(i => i.type == FlowGraphItemType.Enemy);
            
            {
                int numKeys = node.items.Count(i => i.type == FlowGraphItemType.Key);
                int numBonus = node.items.Count(i => i.type == FlowGraphItemType.Bonus);
                bool hasEntrance = node.items.Count(i => i.type == FlowGraphItemType.Entrance) > 0;
                bool hasExit = node.items.Count(i => i.type == FlowGraphItemType.Exit) > 0;

                if (hasEntrance || hasExit || numKeys > 0 || numBonus > 0)
                {
                    return GridFlowLayoutNodeRoomType.Room;
                }
            }

            var roomType = CalculateRoomType(graph, incoming, outgoing, numEnemies);
            if (roomType == GridFlowLayoutNodeRoomType.Corridor && !generateCorridors)
            {
                roomType = GridFlowLayoutNodeRoomType.Cave;
            }
            if (roomType == GridFlowLayoutNodeRoomType.Cave && !generateCaves)
            {
                roomType = GridFlowLayoutNodeRoomType.Room;
            }

            return roomType;
        }

        GridFlowLayoutNodeRoomType CalculateRoomType(FlowLayoutGraph graph, FlowLayoutGraphLink[] incoming,
            FlowLayoutGraphLink[] outgoing, int numEnemies)
        {
            if (incoming.Length == 1 && outgoing.Length == 1 && numEnemies == 0) 
            {
                // make sure the incoming and outgoing are in the same line
                var incomingNode = graph.GetNode(incoming[0].source);
                var outgoingNode = graph.GetNode(outgoing[0].destination);
                var coordIn = GetNodeCoord(incomingNode);
                var coordOut = GetNodeCoord(outgoingNode);

                var sameLine = (coordIn.x == coordOut.x || coordIn.y == coordOut.y);
                if (sameLine)
                {
                    return GridFlowLayoutNodeRoomType.Corridor;
                }
            }

            return numEnemies <= maxEnemiesPerCaveNode
                ? GridFlowLayoutNodeRoomType.Cave
                : GridFlowLayoutNodeRoomType.Room;
        }

    }
}
