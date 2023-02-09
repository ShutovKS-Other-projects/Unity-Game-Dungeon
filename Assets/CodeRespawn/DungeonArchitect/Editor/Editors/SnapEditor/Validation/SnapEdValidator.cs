//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Grammar;
using System.Collections.Generic;
using System.Linq;

namespace DungeonArchitect.Editors.SnapFlow
{
    public class SnapEdValidator
    {
        public static void Validate(SnapFlowAsset asset, DungeonFlowErrorList errorList)
        {
            if (asset != null && errorList != null && errorList.Errors != null)
            {
                Validate_NodeTypes(asset.nodeTypes, errorList);
                Validate_Rules(asset.productionRules, errorList);
                Validate_ExecutionGraph(asset.executionGraph, errorList);
            }
        }

        private static void Validate_NodeTypes(GrammarNodeType[] nodeTypesArray, DungeonFlowErrorList errorList)
        {
            var nodeTypes = new List<GrammarNodeType>(nodeTypesArray);
            if (nodeTypes.Count == 0)
            {
                errorList.Errors.Add(new DungeonFlowErrorEntry("Empty Node List", DungeonFlowErrorType.Error, new EmptyNodeListSnapEdValidatorAction()));
            }
            else
            {
                nodeTypes = nodeTypes.OrderBy(x => x.nodeName).ToList();

                for (int i = 0; i < nodeTypes.Count; i++)
                {
                    var nodeType = nodeTypes[i];
                    if (nodeType == null) continue;

                    if (nodeType.nodeName == "")
                    {
                        errorList.Errors.Add(new DungeonFlowErrorEntry("Empty node name", DungeonFlowErrorType.Error, new InvalidNodeTypeAction(nodeType)));
                    }

                    if (i > 0)
                    {
                        var previousNodeType = nodeTypes[i - 1];
                        if (previousNodeType != null && previousNodeType.nodeName == nodeType.nodeName)
                        {
                            string message = string.Format("Duplicate node name: '{0}'", nodeType.nodeName);
                            errorList.Errors.Add(new DungeonFlowErrorEntry(message, DungeonFlowErrorType.Error, new InvalidNodeTypeAction(nodeType)));
                        }
                    }
                }
            }
        }

        private static void Validate_Rules(GrammarProductionRule[] rulesArray, DungeonFlowErrorList errorList)
        {
            var rules = new List<GrammarProductionRule>(rulesArray);
            if (rules.Count == 0)
            {
                errorList.Errors.Add(new DungeonFlowErrorEntry("Empty Rule List", DungeonFlowErrorType.Error, new EmptyRuleListSnapEdValidatorAction()));
            }
            else
            {
                rules = rules.OrderBy(x => x.ruleName).ToList();

                for (int i = 0; i < rules.Count; i++)
                {
                    var rule = rules[i];
                    if (rule == null) continue;

                    if (rule.ruleName == "")
                    {
                        errorList.Errors.Add(new DungeonFlowErrorEntry("Empty rule name", DungeonFlowErrorType.Error, new InvalidProductionRuleAction(rule)));
                    }

                    // Validate the graphs
                    Validate_RuleGraphs(rule, rule.LHSGraph, "LHS", errorList);
                    if (rule.RHSGraphs.Count == 0)
                    {
                        string message = string.Format("No RHS graphs defined for rule: '{0}'", rule.ruleName);
                        errorList.Errors.Add(new DungeonFlowErrorEntry(message, DungeonFlowErrorType.Error, new MissingProductionRuleRHSAction(rule)));
                    }
                    else
                    {
                        for (int rhsIdx = 0; rhsIdx < rule.RHSGraphs.Count; rhsIdx++)
                        {
                            var rhsGraph = rule.RHSGraphs[rhsIdx].graph;
                            string graphName = string.Format("RHS({0})", rhsIdx + 1);
                            Validate_RuleGraphs(rule, rhsGraph, graphName, errorList);
                        }
                    }

                    if (i > 0)
                    {
                        var previousRule = rules[i - 1];
                        if (previousRule != null && previousRule.ruleName == rule.ruleName)
                        {
                            string message = string.Format("Duplicate rule name: '{0}'", rule.ruleName);
                            errorList.Errors.Add(new DungeonFlowErrorEntry(message, DungeonFlowErrorType.Error, new InvalidProductionRuleAction(rule)));
                        }
                    }
                }
            }
        }

        private static void Validate_RuleGraphs(GrammarProductionRule rule, GrammarGraph graph, string graphName, DungeonFlowErrorList errorList)
        {
            if (graph.Nodes.Count == 0)
            {
                string message = string.Format("Empty {0} graph for rule: '{1}'", graphName, rule.ruleName);
                errorList.Errors.Add(new DungeonFlowErrorEntry(message, DungeonFlowErrorType.Warning, new InvalidProductionGraphAction(rule, graph)));
            }

            // Check for deleted graph nodes
            foreach (var node in graph.Nodes)
            {
                if (node is GrammarTaskNode)
                {
                    var taskNode = node as GrammarTaskNode;
                    if (taskNode.NodeType == null)
                    {
                        string message = string.Format("Deleted node found in rule: '{0}', graph: '{0}'", rule.ruleName, graphName);
                        errorList.Errors.Add(new DungeonFlowErrorEntry(message, DungeonFlowErrorType.Error, new InvalidProductionGraphNodeAction(rule, node)));
                    }
                }
            }
        }

        private static void Validate_ExecutionGraph(GrammarExecGraph executionGraph, DungeonFlowErrorList errorList)
        {
            // Check if the entry node is connected
            {
                bool entryNodeConnected = false;
                foreach (var link in executionGraph.Links)
                {
                    if (link != null && link.Output != null && link.Output.Node != null)
                    {
                        if (link.Output.Node is GrammarExecEntryNode)
                        {
                            entryNodeConnected = true;
                        }
                    }
                }

                if (!entryNodeConnected)
                {
                    string message = "Execution graph's Entry Node is not connected";
                    errorList.Errors.Add(new DungeonFlowErrorEntry(message, DungeonFlowErrorType.Error, new InvalidExecutionGraphSnapEdValidatorAction()));
                }
            }

            // Check for deleted graph nodes
            foreach (var node in executionGraph.Nodes)
            {
                if (node is GrammarExecRuleNode)
                {
                    var execNode = node as GrammarExecRuleNode;
                    if (execNode.rule == null)
                    {
                        string message = "Deleted node found in execution graph";
                        errorList.Errors.Add(new DungeonFlowErrorEntry(message, DungeonFlowErrorType.Error, new InvalidExecutionGraphNodeAction(node)));
                    }
                }
            }

        }

    }
}
