//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;

namespace DungeonArchitect.Graphs
{
	public class MarkerNode : GraphNode  {

		public string MarkerName
		{
			get => caption;
			set => caption = value;
		}
		
		public override void Initialize(string id, Graph graph) {
			base.Initialize(id, graph);
			UpdateName("MarkerNode_");
			
			Size = new Vector2(120, 50);
			
			if (inputPins == null) {
				inputPins = new List<GraphPin>();
			}
			
			if (outputPins == null) {
				outputPins = new List<GraphPin>();

				CreatePin(GraphPinType.Output,
				          new Vector2(60, 48),
				          new Rect(-40, -15, 80, 15),
				          new Vector2(0, 1));
			}
			
			if (caption == null) {
				caption = "Marker";
			}
		}


	}
}
