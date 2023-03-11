//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.RuntimeGraphs;
using System.Collections.Generic;


namespace DungeonArchitect.Grammar
{
    public class GrammarRuntimeGraphNodeData
    {
        public GrammarNodeType nodeType;
        public int index;

        public override string ToString()
        {
            if (nodeType != null)
            {
                return nodeType.nodeName + ":" + index;
            }
            else
            {
                return "null";
            }
        }
    }

    public class GrammarRuntimeGraph : RuntimeGraph<GrammarRuntimeGraphNodeData>
    {
        public static GrammarRuntimeGraph BuildFrom(GrammarGraph graph)
        {
            var runtimeGraph = new GrammarRuntimeGraph();
            BuildFrom(graph, runtimeGraph);
            return runtimeGraph;
        }

        public static void BuildFrom(GrammarGraph graph, GrammarRuntimeGraph runtimeGraph)
        {
            runtimeGraph.Nodes.Clear();
            var buildHandlers = GrammarRuntimeGraphHandlers.Create();
            RuntimeGraphBuilder.Build(graph, runtimeGraph, buildHandlers);
        }
    }

    public class GrammarRuntimeGraphUtils
    {
        public static RuntimeGraphNode<GrammarRuntimeGraphNodeData> FindStartNode(GrammarRuntimeGraph graph)
        {
            if (graph.Nodes.Count == 0)
            {
                return null;
            }

            var visited = new HashSet<RuntimeGraphNode<GrammarRuntimeGraphNodeData>>();
            var startNode = graph.Nodes[0];

            while (true)
            {
                visited.Add(startNode);
                if (startNode.Incoming.Count == 0)
                {
                    break;
                }

                var nextNode = startNode.Incoming[0];
                if (visited.Contains(nextNode))
                {
                    break;
                }
                startNode = nextNode;
            }
            return startNode;
        }
    }

    public class GrammarRuntimeGraphHandlers : RuntimeGraphBuilderHandlers<GrammarRuntimeGraphNodeData>
    {
        public static GrammarRuntimeGraphHandlers Create()
        {
            var buildHandlers = new GrammarRuntimeGraphHandlers();
            buildHandlers.GetPayload = graphNode =>
            {
                if (graphNode is GrammarTaskNode)
                {
                    var taskNode = graphNode as GrammarTaskNode;

                    var data = new GrammarRuntimeGraphNodeData();
                    data.nodeType = taskNode.NodeType;
                    data.index = taskNode.executionIndex;
                    return data;
                }
                return new GrammarRuntimeGraphNodeData();
            };

            buildHandlers.CanCreateNode = graphNode =>
            {
                return (graphNode is GrammarTaskNode);
            };

            buildHandlers.NodeCreated = (graphNode, runtimeNode) =>
            {
            };

            return buildHandlers;
        }
    }

    public class ExecutionRuntimeGraphNodeData
    {
        public RuntimeGrammarProduction rule;
        public GrammarExecRuleRunMode runMode;
        public float runProbability;
        public int iterateCount;
        public int minIterateCount;
        public int maxIterateCount;
    }


    public class ExecutionRuntimeGraph : RuntimeGraph<ExecutionRuntimeGraphNodeData>
    {
        public RuntimeGraphNode<ExecutionRuntimeGraphNodeData> EntryNode { get; set; }

        public static ExecutionRuntimeGraph BuildFrom(GrammarExecGraph graph, Dictionary<GrammarProductionRule, RuntimeGrammarProduction> mapping)
        {
            var runtimeGraph = new ExecutionRuntimeGraph();
            var buildHandlers = ExecutionRuntimeGraphHandlers.Create(mapping);
            RuntimeGraphBuilder.Build(graph, runtimeGraph, buildHandlers);
            return runtimeGraph;
        }
    }

    public class ExecutionRuntimeGraphHandlers : RuntimeGraphBuilderHandlers<ExecutionRuntimeGraphNodeData>
    {
        public static ExecutionRuntimeGraphHandlers Create(Dictionary<GrammarProductionRule, RuntimeGrammarProduction> mapping)
        {
            var buildHandlers = new ExecutionRuntimeGraphHandlers();
            buildHandlers.GetPayload = graphNode =>
            {
                if (graphNode is GrammarExecRuleNode)
                {
                    var ruleNode = graphNode as GrammarExecRuleNode;
                    if (mapping.ContainsKey(ruleNode.rule))
                    {
                        var payload = new ExecutionRuntimeGraphNodeData();
                        payload.rule = mapping[ruleNode.rule];
                        payload.runMode = ruleNode.runMode;
                        payload.runProbability = ruleNode.runProbability;
                        payload.iterateCount = ruleNode.iterateCount;
                        payload.minIterateCount = ruleNode.minIterateCount;
                        payload.maxIterateCount = ruleNode.maxIterateCount;
                        return payload;
                    }
                }
                return new ExecutionRuntimeGraphNodeData();
            };

            buildHandlers.CanCreateNode = graphNode =>
            {
                return (graphNode is GrammarExecRuleNode) || (graphNode is GrammarExecEntryNode);
            };

            buildHandlers.NodeCreated = (graphNode, runtimeNode) =>
            {
                if (graphNode is GrammarExecEntryNode)
                {
                    var execRuntimeGraph = runtimeNode.Graph as ExecutionRuntimeGraph;
                    execRuntimeGraph.EntryNode = runtimeNode;
                }
            };
            return buildHandlers;
        }
    }

    public class WeightedGrammarRuntimeGraph : GrammarRuntimeGraph
    {
        public float Weight = 1.0f;
    }

    public class RuntimeGrammarProduction
    {
        public GrammarProductionRule rule;
        public GrammarRuntimeGraph LHS;
        public WeightedGrammarRuntimeGraph[] RHSList;
    }

    public class RuntimeGrammar
    {
        public GrammarRuntimeGraph ResultGraph;
        public ExecutionRuntimeGraph ExecutionGraph;
        public RuntimeGrammarProduction[] Rules;
        public GrammarNodeType[] NodeTypes;

        public static RuntimeGrammar Build(SnapFlowAsset flowAsset)
        {
            if (flowAsset == null)
            {
                return null;
            }
            var grammar = new RuntimeGrammar();
            grammar.NodeTypes = flowAsset.nodeTypes;
            grammar.ResultGraph = new GrammarRuntimeGraph();

            // Build the rules
            var ruleMapping = new Dictionary<GrammarProductionRule, RuntimeGrammarProduction>();
            int numRules = flowAsset.productionRules.Length;
            grammar.Rules = new RuntimeGrammarProduction[numRules];
            for (int i = 0; i < numRules; i++)
            {
                var rule = flowAsset.productionRules[i];
                var runtimeRule = new RuntimeGrammarProduction();
                grammar.Rules[i] = runtimeRule;
                ruleMapping.Add(rule, runtimeRule);

                runtimeRule.rule = rule;
                runtimeRule.LHS = GrammarRuntimeGraph.BuildFrom(rule.LHSGraph);
                int numRHS = rule.RHSGraphs.Count;
                runtimeRule.RHSList = new WeightedGrammarRuntimeGraph[numRHS];
                for (int r = 0; r < numRHS; r++)
                {
                    var rhsGraph = new WeightedGrammarRuntimeGraph();
                    GrammarRuntimeGraph.BuildFrom(rule.RHSGraphs[r].graph, rhsGraph);
                    rhsGraph.Weight = rule.RHSGraphs[r].weight;
                    runtimeRule.RHSList[r] = rhsGraph;
                }
            }

            grammar.ExecutionGraph = ExecutionRuntimeGraph.BuildFrom(flowAsset.executionGraph, ruleMapping);

            return grammar;
        }
    }
}
