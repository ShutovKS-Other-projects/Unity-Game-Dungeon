//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;

namespace DungeonArchitect.Graphs
{

	public class MarkerEmitterNode : PlaceableNode {
		[SerializeField]
		MarkerNode marker;

		public MarkerNode Marker {
			get {
				return marker;
			}
			set {
				marker = value;
			}
		}

		public override void Initialize(string id, Graph graph) {
			base.Initialize(id, graph);
			UpdateName("MarkerEmitterNode_");
			
			Size = new Vector2(120, 50);
			
			if (inputPins == null) {
				inputPins = new List<GraphPin>();
				
				CreatePin(GraphPinType.Input,
				          new Vector2(60, -2),
				          new Rect(-40, 0, 80, 15),
				          new Vector2(0, -1));
			}
			
			if (outputPins == null) {
				outputPins = new List<GraphPin>();
			}
			
			if (caption == null) {
				caption = "MarkerEmitter";
			}
		}
	}

}
