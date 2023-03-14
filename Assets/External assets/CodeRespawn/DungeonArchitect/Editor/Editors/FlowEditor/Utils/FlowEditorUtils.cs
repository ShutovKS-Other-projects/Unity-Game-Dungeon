//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Flow;
using UnityEngine;
using UnityEditor;
using DungeonArchitect.Graphs;
using DungeonArchitect.Flow.Exec;
using DungeonArchitect.UI.Widgets.GraphEditors;
using DungeonArchitect.UI;

namespace DungeonArchitect.Editors.Flow
{
    public class FlowEditorUtils
    {
        #region Asset Management
        public static void InitAsset(FlowAssetBase asset, UIPlatform platform)
        {
            if (asset == null) return;

            asset.execGraph = CreateAssetObject<FlowExecGraph>(asset);

            InitializeExecutionGraph(asset, platform);
        }

        private static T CreateAssetObject<T>(FlowAssetBase asset) where T : ScriptableObject
        {
            var obj = ScriptableObject.CreateInstance<T>();
            obj.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(obj, asset);
            return obj;
        }

        private static void InitializeExecutionGraph(FlowAssetBase asset, UIPlatform platform)
        {
            // Create an entry node in the execution graph
            var resultNode = CreateGraphNode<FlowExecResultGraphNode>(Vector2.zero, asset.execGraph, asset, platform);
            resultNode.Position = Vector2.zero;
            asset.execGraph.resultNode = resultNode;
            resultNode.task = CreateAssetObject<FlowExecTaskResult>(asset);
        }

        static T CreateGraphNode<T>(Vector2 position, Graph graph, FlowAssetBase asset, UIPlatform platform) where T : GraphNode
        {
            var node = GraphOperations.CreateNode(graph, typeof(T), null);
            GraphEditorUtils.AddToAsset(platform, asset, node);
            return node as T;
        }
        #endregion
        
    }
}
