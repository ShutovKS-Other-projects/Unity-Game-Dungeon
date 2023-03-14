//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Graphs;
using DungeonArchitect.RuntimeGraphs;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using DungeonArchitect.Graphs.Layouts;
using DungeonArchitect.Graphs.Layouts.Spring;
using RGraph = DungeonArchitect.RuntimeGraphs.RuntimeGraph<DungeonArchitect.Grammar.GrammarRuntimeGraphNodeData>;
using RNode = DungeonArchitect.RuntimeGraphs.RuntimeGraphNode<DungeonArchitect.Grammar.GrammarRuntimeGraphNodeData>;


namespace DungeonArchitect.Grammar
{
    public class GraphGrammarProcessorSettings
    {
        public int seed = 0;
        public bool runGraphGenerationScripts = false;
    }

    public class GraphGrammarProcessor 
    {
        public RuntimeGrammar Grammar { get; set; }

        System.Random random;
        public GraphGrammarProcessorSettings settings;

        public GraphGrammarProcessor(SnapFlowAsset asset, GraphGrammarProcessorSettings settings)
        {
            this.settings = settings;
            random = new System.Random(settings.seed);

            if (settings.runGraphGenerationScripts)
            {
                RunGraphGenerationScripts(asset);
            }

            // Build an optimized version of the grammar in memory to work with
            Grammar = RuntimeGrammar.Build(asset);

            // Find the start node
            GrammarNodeType startNodeType = null;
            foreach (var nodeTypeInfo in Grammar.NodeTypes)
            {
                if (nodeTypeInfo.nodeName == "S")
                {
                    startNodeType = nodeTypeInfo;
                    break;
                }
            }

            if (startNodeType != null)
            {
                var nodeData = new GrammarRuntimeGraphNodeData();
                nodeData.nodeType = startNodeType;
                nodeData.index = 0;
                RuntimeGraphBuilder.AddNode(nodeData, Grammar.ResultGraph);
            }
        }

        void RunGraphGenerationScripts(SnapFlowAsset asset)
        {
            foreach (var rule in asset.productionRules)
            {
                if (rule.LHSGraph.useProceduralScript)
                {
                    RunGraphGenerationScript(rule.LHSGraph, asset.nodeTypes);
                }
                foreach (var rhs in rule.RHSGraphs)
                {
                    if (rhs.graph.useProceduralScript)
                    {
                        RunGraphGenerationScript(rhs.graph, asset.nodeTypes);
                    }
                }
            }
        }

        void RunGraphGenerationScript(GrammarGraph graph, GrammarNodeType[] nodeTypes)
        {
            var scriptType = System.Type.GetType(graph.generatorScriptClass);
            if (scriptType != null)
            {
                IGrammarGraphBuildScript generatorScript = ScriptableObject.CreateInstance(scriptType) as IGrammarGraphBuildScript;
                if (generatorScript != null)
                {
                    var graphBuilder = new NonEditorGraphBuilder(graph);
                    GrammarGraphBuilder grammarBuilder = new GrammarGraphBuilder(graph, nodeTypes, graphBuilder);
                    grammarBuilder.ClearGraph();
                    generatorScript.Generate(grammarBuilder);

                    // Layout the generated graph
                    var config = new GraphLayoutSpringConfig();
                    var layout = new GraphLayoutSpring<GraphNode>(config);
                    var nodes = graph.Nodes.ToArray();
                    nodes = nodes.Where(n => !(n is CommentNode)).ToArray();
                    layout.Layout(nodes, new DefaultGraphLayoutNodeActions(graph));
                }
            }
        }

        public void Build()
        {
            var entryNode = Grammar.ExecutionGraph.EntryNode;
            if (Grammar.ExecutionGraph.EntryNode == null)
            {
                Debug.LogWarning("Entry node not found in execution graph");
                return;
            }

            var visited = new HashSet<RuntimeGraphNode<ExecutionRuntimeGraphNodeData>>();
            var node = (entryNode.Outgoing.Count > 0) ? entryNode.Outgoing[0] : null;

            while (node != null)
            {
                if (visited.Contains(node))
                {
                    break;
                }
                visited.Add(node);

                // Execute the node
                var rule = node.Payload.rule;
                int executionCount = GetExecutionCount(node.Payload);
                for (int i = 0; i < executionCount; i++)
                {
                    ApplyRule(Grammar.ResultGraph, rule);
                }

                // Move to the next node
                node = (node.Outgoing.Count > 0) ? node.Outgoing[0] : null;
            }

        }

        int GetExecutionCount(ExecutionRuntimeGraphNodeData data)
        {
            switch (data.runMode)
            {
                case GrammarExecRuleRunMode.Iterate:
                    return data.iterateCount;

                case GrammarExecRuleRunMode.IterateRange:
                    return random.Range(data.minIterateCount, data.maxIterateCount);

                case GrammarExecRuleRunMode.RunWithProbability:
                    return random.NextFloat() <= data.runProbability ? 1 : 0;

                case GrammarExecRuleRunMode.RunOnce:
                default:
                    return 1;
            }
        }

        WeightedGrammarRuntimeGraph GetRandomGraph(WeightedGrammarRuntimeGraph[] graphs)
        {
            if (graphs.Length == 0) return null;
            if (graphs.Length == 1) return graphs[0];

            float totalWeights = 0;
            foreach (var graph in graphs)
            {
                totalWeights += graph.Weight;
            }

            float selectionValue = random.Range(0.0f, totalWeights);
            foreach (var graph in graphs)
            {
                if (selectionValue <= graph.Weight)
                {
                    return graph;
                }
                selectionValue -= graph.Weight;
            }
            return null;
        }

        void ApplyRule(RGraph dataGraph, RuntimeGrammarProduction rule)
        {
            var matches = GraphPatternMatcher.Match(dataGraph, rule.LHS);

            foreach (var match in matches)
            {
                var lhs = rule.LHS;
                var rhs = GetRandomGraph(rule.RHSList);
                if (lhs == null || rhs == null) continue;

                ApplyMatch(dataGraph, lhs, rhs, match.PatternToDataNode);
            }
        }

        Dictionary<int, RNode> CreateNodeByIndexMap(RGraph graph)
        {
            var result = new Dictionary<int, RNode>();
            foreach (var node in graph.Nodes)
            {
                result.Add(node.Payload.index, node);
            }
            return result;
        }

        void ApplyMatch(RGraph dataGraph, RGraph lhs, RGraph rhs, Dictionary<RNode, RNode> LHSToDataNode)
        {
            // Break the links
            {
                var matchedDataNodes = LHSToDataNode.Values.ToArray();
                foreach (var dataNode in matchedDataNodes)
                {
                    var outgoingNodes = new List<RNode>(dataNode.Outgoing);
                    foreach (var outgoingNode in outgoingNodes)
                    {
                        if (matchedDataNodes.Contains(outgoingNode))
                        {
                            // Break this link
                            dataNode.BreakLinkTo(outgoingNode);
                        }
                    }
                }
            }

            var LHSNodesByIndex = CreateNodeByIndexMap(lhs);
            var RHSNodesByIndex = CreateNodeByIndexMap(rhs);

            var RHStoDataNode = new Dictionary<RNode, RNode>();

            // Delete unused nodes 
            {
                var dataNodeDeletionList = new List<RNode>(LHSToDataNode.Values);
                foreach (var entry in RHSNodesByIndex)
                {
                    var indexToRetain = entry.Key;
                    var rhsNode = entry.Value;
                    if (LHSNodesByIndex.ContainsKey(indexToRetain))
                    {
                        var lhsNodeToRetain = LHSNodesByIndex[indexToRetain];
                        if (LHSToDataNode.ContainsKey(lhsNodeToRetain))
                        {
                            var dataNodeToRetain = LHSToDataNode[lhsNodeToRetain];
                            dataNodeDeletionList.Remove(dataNodeToRetain);

                            // Update the caption if this is a wildcard node
                            if (!rhsNode.Payload.nodeType.wildcard)
                            {
                                dataNodeToRetain.Payload.nodeType = rhsNode.Payload.nodeType;
                            }

                            RHStoDataNode.Add(rhsNode, dataNodeToRetain);
                        }
                    }
                }

                // Now delete the unused nodes
                foreach (var dataNodeToDelete in dataNodeDeletionList)
                {
                    dataGraph.RemoveNode(dataNodeToDelete);
                }
            }

            // Add new nodes
            {
                var newRhsIndices = new List<int>(RHSNodesByIndex.Keys);
                foreach (var lhsEntry in LHSNodesByIndex)
                {
                    newRhsIndices.Remove(lhsEntry.Key);
                }

                foreach (var rhsIndexToCreate in newRhsIndices)
                {
                    var rhsNode = RHSNodesByIndex[rhsIndexToCreate];
                    var dataNode = new RNode(dataGraph);

                    var payload = new GrammarRuntimeGraphNodeData();
                    payload.nodeType = rhsNode.Payload.nodeType;

                    dataNode.Payload = payload;
                    dataGraph.Nodes.Add(dataNode);

                    RHStoDataNode.Add(rhsNode, dataNode);
                }
            }

            // Make the links
            {
                foreach (var entry in RHSNodesByIndex)
                {
                    var rhsNode = entry.Value;
                    foreach (var outgoingRHS in rhsNode.Outgoing)
                    {
                        var dataNodeStart = RHStoDataNode[rhsNode];
                        var dataNodeEnd = RHStoDataNode[outgoingRHS];
                        dataNodeStart.MakeLinkTo(dataNodeEnd);
                    }
                }
            }
        }
    }

    class GraphPatternMatch
    {
        public Dictionary<RNode, RNode> PatternToDataNode = new Dictionary<RNode, RNode>();
    }

    class GraphPatternMatcher
    {
        public static GraphPatternMatch[] Match(RGraph dataGraph, RGraph patternGraph)
        {
            if (patternGraph.Nodes.Count == 0)
            {
                return new GraphPatternMatch[0];
            }

            var matches = new List<GraphPatternMatch>();
            var unmatchedNodes = new HashSet<RNode>(dataGraph.Nodes);
            while(true)
            {
                bool foundMatch = false;

                foreach (var dataNode in dataGraph.Nodes)
                {
                    if (!unmatchedNodes.Contains(dataNode))
                    {
                        // Already processed
                        continue;
                    }

                    var match = new GraphPatternMatch();
                    var visited = new HashSet<RNode>();
                    if (MatchRecursive(dataNode, patternGraph.Nodes[0], unmatchedNodes, match, visited))
                    {
                        matches.Add(match);
                        foundMatch = true;

                        // Remove the matched nodes from the list so they are not matched again
                        foreach (var entry in match.PatternToDataNode)
                        {
                            var matchedDataNode = entry.Value;
                            unmatchedNodes.Remove(matchedDataNode);
                        }
                    }
                }
                if (!foundMatch)
                {
                    break;
                }
            }
            return matches.ToArray();
        }

        static bool IsNodeDataEqual(RNode a, RNode b)
        {
            if (a.Payload.nodeType == null || b.Payload.nodeType == null) return false;
            if (a.Payload.nodeType.wildcard || b.Payload.nodeType.wildcard) return true;
            return a.Payload.nodeType == b.Payload.nodeType;
        }

        static bool TraverseChildren(List<RNode> dataChildNodes, List<RNode> patternChildNodes,
                HashSet<RNode> unmatchedNodes, GraphPatternMatch match, HashSet<RNode> visited)
        {
            if (patternChildNodes.Count == 0)
            {
                return true;
            }

            bool foundAllChlidPaths = true;
            // Traverse to children
            foreach (var patternNode in patternChildNodes)
            {
                if (patternNode == null) continue;
                if (visited.Contains(patternNode)) continue;

                bool foundPath = false;
                foreach (var dataNode in dataChildNodes)
                {
                    if (visited.Contains(dataNode)) continue;
                    if (!IsNodeDataEqual(patternNode, dataNode)) continue;

                    if (MatchRecursive(dataNode, patternNode, unmatchedNodes, match, visited))
                    {
                        foundPath = true;
                        break;
                    }
                }

                if (!foundPath)
                {
                    foundAllChlidPaths = false;
                    break;
                }
            }

            return foundAllChlidPaths;
        }

        private static bool MatchRecursive(RNode dataNode, RNode patternNode, HashSet<RNode> unmatchedNodes, GraphPatternMatch match, HashSet<RNode> visited)
        {
            if (dataNode == null || patternNode == null || !IsNodeDataEqual(dataNode, patternNode) || !unmatchedNodes.Contains(dataNode))
            {
                return false;
            }

            visited.Add(dataNode);
            visited.Add(patternNode);

            match.PatternToDataNode.Add(patternNode, dataNode);

            bool success = TraverseChildren(dataNode.Incoming, patternNode.Incoming, unmatchedNodes, match, visited)
                && TraverseChildren(dataNode.Outgoing, patternNode.Outgoing, unmatchedNodes, match, visited);

            if (!success)
            {
                visited.Remove(dataNode);
                visited.Remove(patternNode);
                match.PatternToDataNode.Remove(patternNode);
            }

            return success;
        }
    }

}
