//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System;
using UnityEngine;
using System.Collections.Generic;
using DungeonArchitect.Graphs;

namespace DungeonArchitect.Themeing {
    /// <summary>
    /// The data-structure for serializing the theme graph to disk
    /// </summary>
	public class DungeonThemeData {
		public List<DungeonThemeItem> Props = new List<DungeonThemeItem>();

		public void BuildFromGraph(Graph graph) {
			Props.Clear();
			if (graph == null) {
				return;
			}
			var nodes = graph.Nodes.ToArray();
			Array.Sort (nodes, new LeftToRightNodeComparer ());

			foreach (var node in nodes) {
				if (node is VisualNode) {
					var visualNode = node as VisualNode;

					foreach (var meshParentNode in visualNode.GetParentNodes()) {
						if (meshParentNode is MarkerNode) {
							var markerNode = meshParentNode as MarkerNode;
							
							DungeonThemeItem item = null;
							if (visualNode is GameObjectNode) {
								var meshItem = new GameObjectDungeonThemeItem ();
								var goNode = visualNode as GameObjectNode;

								meshItem.Template = goNode.Template;

								item = meshItem;
							}
							else if (visualNode is GameObjectArrayNode) {
								var arrayPropData = new GameObjectArrayDungeonThemeItem ();
								var arrayNode = visualNode as GameObjectArrayNode;

								if (arrayNode == null || arrayNode.Templates == null) {
									arrayPropData.Templates = new GameObject[0];
								} else {
									var count = arrayNode.Templates.Length;
									arrayPropData.Templates = new GameObject[count];
									System.Array.Copy (arrayNode.Templates, arrayPropData.Templates, count);
								}
								item = arrayPropData;
							}
							else if (visualNode is SpriteNode) {
								var spriteItem = new SpriteDungeonThemeItem();
								var spriteNode = visualNode as SpriteNode;

								spriteItem.sprite = spriteNode.sprite;
								spriteItem.color = spriteNode.color;
								spriteItem.materialOverride = spriteNode.materialOverride;
								spriteItem.sortingLayerName = spriteNode.sortingLayerName;
								spriteItem.orderInLayer = spriteNode.orderInLayer;

								spriteItem.collisionType = spriteNode.collisionType;
								spriteItem.physicsMaterial = spriteNode.physicsMaterial;
								spriteItem.physicsOffset = spriteNode.physicsOffset;
								spriteItem.physicsSize = spriteNode.physicsSize;
								spriteItem.physicsRadius = spriteNode.physicsRadius;

								item = spriteItem;
							}
							else {
								// Unsupported visual node type
								continue;
							}

							// Set the common settings
							item.NodeId = visualNode.Id.ToString();
							item.AttachToSocket = markerNode.Caption;
							item.Affinity = visualNode.attachmentProbability;
							item.ConsumeOnAttach = visualNode.consumeOnAttach;
							item.Offset = visualNode.offset;
							item.StaticState = visualNode.IsStatic ? DungeonThemeItemStaticMode.ForceStatic : DungeonThemeItemStaticMode.Unchanged;
							item.affectsNavigation = visualNode.affectsNavigation;
							item.UseSelectionRule = visualNode.selectionRuleEnabled;
							item.SelectorRuleClassName = visualNode.selectionRuleClassName;
							item.UseTransformRule = visualNode.transformRuleEnabled;
							item.TransformRuleClassName = visualNode.transformRuleClassName;
                            item.useSpatialConstraint = visualNode.useSpatialConstraint;
                            item.spatialConstraint = visualNode.spatialConstraint;

							var emitterNodes = visualNode.GetChildNodes();
							foreach (var childNode in emitterNodes) {
								if (childNode is MarkerEmitterNode) {
									var emitterNode = childNode as MarkerEmitterNode;
									if (emitterNode.Marker != null) {
										PropChildSocketData childData = new PropChildSocketData();
										childData.Offset = emitterNode.offset;
										childData.SocketType = emitterNode.Marker.Caption;
										item.ChildSockets.Add (childData);
									}
								}
							}
							Props.Add(item);
						}
					}
				}
			}
		}
	}
	
	/// <summary>
	/// Sorts the nodes from left to right based on the X-axis.
    /// This is used for sorting the visual nodes for execution, 
    /// since they are executed from left to right
	/// </summary>
	public class LeftToRightNodeComparer : IComparer<GraphNode>
	{
		public int Compare(GraphNode a, GraphNode b)  
		{
			if (a.Bounds.x == b.Bounds.x) {
				return 0;
			}
			return (a.Bounds.x < b.Bounds.x) ? -1 : 1;
		}
	}
}
