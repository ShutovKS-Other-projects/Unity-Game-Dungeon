//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;
using DungeonArchitect.SpatialConstraints;

namespace DungeonArchitect.Graphs
{
	
	[System.Serializable]
	public class VisualNode : PlaceableNode {
		/// <summary>
		/// Indicates if the game object created from this visual node is set to static
		/// If you are spawning NPCs or other dynamic objects, uncheck this
		/// </summary>
		public bool IsStatic = false;

		/// <summary>
		/// Indicates of the geometry in this node contributes to the navigation mesh
		/// You should enable this only if necessary to improve navmesh generation performance
		/// </summary>
		public bool affectsNavigation = true;

		/// <summary>
		/// Indicates if the selection rule is enabled.  The selection rule will not run if this is disabled
		/// </summary>
		public bool selectionRuleEnabled = false;

		/// <summary>
		/// The class name of the selection rule. 
		/// Selection rules let you specify behavior logic for selecting your nodes
		/// </summary>
		public string selectionRuleClassName;

		/// <summary>
		/// Indicates if the transform rule is enabled.  The transform rule will not run if this is disabled
		/// </summary>
		public bool transformRuleEnabled = false;

		/// <summary>
		/// The class name of the transformation rule.  
		/// Transform rules let you specify behavior logic to apply the offset on the nodes
		/// </summary>
		public string transformRuleClassName;

        /// <summary>
        /// Flag to indicate if spatial constraints are to be used
        /// </summary>
        public bool useSpatialConstraint = false;

        /// <summary>
        /// Spatial constraints lets you select a node based on nearby neighbor constraints
        /// </summary>
        [SerializeField]
        public SpatialConstraintAsset spatialConstraint;
        
		public override void Initialize(string id, Graph graph) {
			base.Initialize(id, graph);
			Size = new Vector2(120, 120);
			bool createInputPins = false;
			bool createOutputPins = false;
			
			if (inputPins == null) {
				inputPins = new List<GraphPin>();
				createInputPins = true;
			}
			if (outputPins == null) {
				outputPins = new List<GraphPin>();
				createOutputPins = true;
			}
			
			if (createInputPins) {
				// Create an input pin on the top
				CreatePin(GraphPinType.Input,
				          new Vector2(60, 0),
				          new Rect(-40, 0, 80, 15),
				          new Vector2(0, -1));
			}
			
			if (createOutputPins) {
				// Create an output pin at the bottom
				CreatePin(GraphPinType.Output,
				          new Vector2(60, 120),
				          new Rect(-40, -15, 80, 15),
				          new Vector2(0, 1));
			}
            

        }
		
		public override void CopyFrom(GraphNode node)
		{
			base.CopyFrom(node);
			
			var visualNode = node as VisualNode;
			if (visualNode == null) return;
			
			IsStatic = visualNode.IsStatic;
			affectsNavigation = visualNode.affectsNavigation;
			selectionRuleEnabled = visualNode.selectionRuleEnabled;
			selectionRuleClassName = visualNode.selectionRuleClassName;
			transformRuleEnabled = visualNode.transformRuleEnabled;
			transformRuleClassName = visualNode.transformRuleClassName;
		}
        
    }

}
