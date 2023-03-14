//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using System.Linq;
using DungeonArchitect.Flow.Exec;
using DungeonArchitect.Flow.Items;
using UnityEngine;

namespace DungeonArchitect.Flow.Domains.Layout.Tasks
{
    [System.Serializable]
    public enum LayoutFlowNodeHandler_SpawnItemMethod
    {
        RandomRange,
        LinearDifficulty,
        CurveDifficulty
    }
    
    public class LayoutBaseFlowTaskSpawnItems : FlowExecTask
    {
        public string[] paths = new string[] { "main" };

        public FlowGraphItemType itemType = FlowGraphItemType.Enemy;
        public string markerName = "";
        public FlowGraphItemCustomInfo customItemInfo = FlowGraphItemCustomInfo.Default;
        public int minCount = 1;
        public int maxCount = 4;
        public LayoutFlowNodeHandler_SpawnItemMethod spawnMethod = LayoutFlowNodeHandler_SpawnItemMethod.LinearDifficulty;
        public AnimationCurve spawnDistributionCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public float spawnDistributionVariance = 0.2f;
        public float minSpawnDifficulty = 0.0f;
        public float spawnProbability = 1.0f;

        public bool showDifficulty = false;
        public Color difficultyInfoColor = new Color(0, 0, 0.5f);

        class NodeInfo
        {
            public NodeInfo(FlowLayoutGraphNode node, float weight)
            {
                this.node = node;
                this.weight = weight;
            }

            public FlowLayoutGraphNode node;
            public float weight;
        }

        protected virtual bool Validate(FlowTaskExecContext context, FlowTaskExecInput input, ref string errorMessage, ref FlowTaskExecutionResult executionResult)
        {
            return true;
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
            if (!Validate(context, input, ref output.ErrorMessage, ref output.ExecutionResult))
            {
                return output;
            }
            
            var graph = output.State.GetState<FlowLayoutGraph>();

            var nodesByPath = new Dictionary<string, NodeInfo[]>();
            {
                var weights = FlowLayoutGraphUtils.CalculateWeights(graph, 1);
                var nodesByPathList = new Dictionary<string, List<NodeInfo>>();
                foreach (var entry in weights)
                {
                    var node = entry.Key;
                    var weight = entry.Value;

                    var pathName = node.pathName;
                    if (paths.Contains(pathName))
                    {
                        if (!nodesByPathList.ContainsKey(pathName))
                        {
                            nodesByPathList.Add(pathName, new List<NodeInfo>());
                        }
                        nodesByPathList[pathName].Add(new NodeInfo(node, weight));
                    }

                }

                // Sort the path list
                foreach (var entry in nodesByPathList)
                {
                    var pathName = entry.Key;
                    var pathList = entry.Value;
                    var sortedPath = pathList.OrderBy(info => info.weight).ToArray();
                    nodesByPath.Add(pathName, sortedPath);
                }
            }

            // Normalize the weights
            foreach (var entry in nodesByPath)
            {
                var pathName = entry.Key;
                var pathNodes = entry.Value;
                if (pathName.Length == 0) continue;

                float minWeight = float.MaxValue;
                float maxWeight = -float.MaxValue;
                foreach(var pathNode in pathNodes)
                {
                    minWeight = Mathf.Min(minWeight, pathNode.weight);
                    maxWeight = Mathf.Max(maxWeight, pathNode.weight);
                }

                foreach (var pathNode in pathNodes)
                {
                    if (Mathf.Abs(maxWeight - minWeight) > 1e-6f)
                    {
                        pathNode.weight = (pathNode.weight - minWeight) / (maxWeight - minWeight);
                    }
                    else
                    {
                        pathNode.weight = 1;
                    }
                }
            }

            foreach (var pathName in paths)
            {
                if (!nodesByPath.ContainsKey(pathName)) continue;
                NodeInfo[] pathNodes = nodesByPath[pathName];

                foreach (var pathNode in pathNodes)
                {
                    if (pathNode.weight < minSpawnDifficulty) continue;
                    int spawnCount = GetSpawnCount(context.Random, pathNode.weight);

                    for (int i = 0; i < spawnCount; i++)
                    {
                        var item = new FlowItem();
                        item.type = itemType;
                        item.markerName = markerName;
                        item.customInfo = customItemInfo;
                        pathNode.node.AddItem(item);
                        HandleItemSpawn(pathNode.node, item);
                    }
                }

                if (showDifficulty)
                {
                    EmitDebugInfo(pathNodes);
                }
            }

            output.ExecutionResult = FlowTaskExecutionResult.Success;
            return output;
        }

        protected virtual void HandleItemSpawn(FlowLayoutGraphNode node, FlowItem item)
        {
        }

        int GetSpawnCount(System.Random random, float weight)
        {
            weight = Mathf.Clamp01(weight);
            
            if (spawnMethod == LayoutFlowNodeHandler_SpawnItemMethod.CurveDifficulty && spawnDistributionCurve == null)
            {
                spawnMethod = LayoutFlowNodeHandler_SpawnItemMethod.LinearDifficulty;
            }

            int spawnCount = 0;
            if (spawnMethod == LayoutFlowNodeHandler_SpawnItemMethod.RandomRange)
            {
                spawnCount = random.Range(minCount, maxCount);
            }
            else if (spawnMethod == LayoutFlowNodeHandler_SpawnItemMethod.LinearDifficulty)
            {
                var v = random.Range(-spawnDistributionVariance, spawnDistributionVariance);
                var w = Mathf.Clamp01(weight + v);
                spawnCount = Mathf.RoundToInt(minCount + (maxCount - minCount) * w);
            }
            else if (spawnMethod == LayoutFlowNodeHandler_SpawnItemMethod.CurveDifficulty)
            {
                var v = random.Range(-spawnDistributionVariance, spawnDistributionVariance);
                var w = Mathf.Clamp01(weight + v);
                float t = spawnDistributionCurve.Evaluate(w);
                spawnCount = Mathf.RoundToInt(minCount + (maxCount - minCount) * t);
            }

            spawnProbability = Mathf.Clamp01(spawnProbability);
            if (random.NextFloat() > spawnProbability)
            {
                spawnCount = 0;
            }
            return spawnCount;
        }

        void EmitDebugInfo(NodeInfo[] nodes)
        {
            foreach (var nodeInfo in nodes)
            {
                var node = nodeInfo.node;
                var weight = nodeInfo.weight;

                var debugItem = new FlowItem();
                debugItem.type = FlowGraphItemType.Custom;
                debugItem.customInfo.text = weight.ToString("0.0");
                debugItem.customInfo.backgroundColor = difficultyInfoColor;
                node.AddItem(debugItem);
            }
        }
    }
}