//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Linq;
using System.Text;
using System.Collections.Generic;
using MarkerChildMap = System.Collections.Generic.Dictionary<DungeonArchitect.Graphs.MarkerNode, System.Collections.Generic.List<DungeonArchitect.Graphs.MarkerNode>>;

namespace DungeonArchitect.Graphs
{
    public class ThemeGraphSchema : GraphSchema
    {
        public override bool CanCreateLink(GraphPin output, GraphPin input, out string errorMessage)
        {
            errorMessage = "";
            if (output == null || input == null)
            {
                errorMessage = "Invalid connection";
                return false;
            }
            if (output.PinType != GraphPinType.Output || input.PinType != GraphPinType.Input)
            {
                errorMessage = "Not Allowed";
                return false;
            }

            var sourceNode = output.Node;
            var destNode = input.Node;

            bool valid = (sourceNode is MarkerNode && destNode is VisualNode) ||
                         (sourceNode is VisualNode && destNode is MarkerEmitterNode);

            if (!valid)
            {
                errorMessage = "Not Allowed";
                return false;
            }

            // Check for loops. We dont allow loops in the graph as they do not make sense and would also cause an infinite loop in code
            var cyclePath = new List<MarkerNode>();
            if (ContainsLoops(output, input, ref cyclePath))
            {
                errorMessage = "Not Allowed. Contains Loop: " + CombineMarkerNames(cyclePath);
                return false;
            }

            
            return base.CanCreateLink(output, input, out errorMessage);
        }
        
        
        static string CombineMarkerNames(List<MarkerNode> markerNodes)
        {
            var builder = new StringBuilder();
            foreach (var markerNode in markerNodes)
            {
                if (builder.Length > 0)
                {
                    builder.Append(" > ");
                }
                builder.Append(markerNode.Caption);
            }
            return builder.ToString();
        }

        static bool ContainsLoops(GraphPin a, GraphPin b, ref List<MarkerNode> cyclePath)
        {
            cyclePath.Clear();
            if (a == null || b == null) return false;
            var graph = a.Node.Graph;
            var markerNodes = graph.GetNodes<MarkerNode>();

            var markerChildMap = new MarkerChildMap();
            foreach (var markerNode in markerNodes)
            {
                var emitters = GetEmitters(markerNode, a, b);
                var outMarkers = new List<MarkerNode>();
                foreach (var emitter in emitters)
                {
                    if (emitter.Marker != null)
                    {
                        outMarkers.Add(emitter.Marker);
                    }
                }
                markerChildMap.Add(markerNode, outMarkers);
            }

            if (FindCycles(ref markerChildMap, ref cyclePath))
            {
                return true;
            }

            // TODO: Implement me.  Important to avoid infinite loops later in the pipeline, if this graph has a loop
            return false;
        }

        static bool FindCycles(ref MarkerChildMap markerChildMap, ref List<MarkerNode> cyclePath)
        {
            var visited = new HashSet<MarkerNode>();
            foreach (var markerNode in markerChildMap.Keys)
            {
                if (visited.Contains(markerNode))
                {
                    // Already processed
                    // TODO: check if we need this as it would never happen
                    continue;
                }

                var traversePath = new List<MarkerNode>();
                traversePath.Add(markerNode);
                if (CheckCycleDFS(ref markerChildMap, ref traversePath))
                {
                    cyclePath = traversePath;
                    return true;
                }
            }
            return false;
        }

        static bool CheckCycleDFS(ref MarkerChildMap markerChildMap, ref List<MarkerNode> traversePath)
        {
            var topMarker = traversePath.Last();
            if (!markerChildMap.ContainsKey(topMarker)) return false;

            var childMarkers = markerChildMap[topMarker];
            foreach (var childMarker in childMarkers)
            {

                if (traversePath.Contains(childMarker))
                {
                    // Cycle detected
                    traversePath.Add(childMarker);
                    return true;
                }

                traversePath.Add(childMarker);
                var containsCycle = CheckCycleDFS(ref markerChildMap, ref traversePath);
                if (containsCycle)
                {
                    return true;
                }

                // Remove the last element since we are done processing it
                traversePath.RemoveAt(traversePath.Count - 1);
            }
            return false;
        }

        static GraphNode[] GetOutgoingNodes(GraphNode node, GraphPin a, GraphPin b)
        {
            var result = new List<GraphNode>();
            var outPin = node.OutputPin;
            if (outPin != null)
            {
                foreach (var link in outPin.GetConntectedLinks())
                {
                    var inPin = link.Input;
                    if (inPin != null)
                    {
                        var nextNode = inPin.Node;
                        if (nextNode != null)
                        {
                            result.Add(nextNode);
                        }
                    }
                }
            }

            if (a != null && b != null)
            {
                if (outPin == a && b.Node != null)
                {
                    result.Add(b.Node);
                }
                if (outPin == b && a.Node != null)
                {
                    result.Add(a.Node);
                }
            }

            return result.ToArray();
        }

        static MarkerEmitterNode[] GetEmitters(MarkerNode markerNode, GraphPin a, GraphPin b)
        {
            var emitters = new List<MarkerEmitterNode>();

            var meshNodes = GetOutgoingNodes(markerNode, a, b);
            foreach (var meshNode in meshNodes)
            {
                var emitterNodes = GetOutgoingNodes(meshNode, a, b);
                foreach (var emitterNode in emitterNodes)
                {
                    if (emitterNode is MarkerEmitterNode)
                    {
                        emitters.Add(emitterNode as MarkerEmitterNode);
                    }
                }
            }

            return emitters.ToArray();
        }

    }
}