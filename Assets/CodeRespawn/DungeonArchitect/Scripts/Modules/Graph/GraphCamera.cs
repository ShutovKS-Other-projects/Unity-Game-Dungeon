//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System;
using UnityEngine;

namespace DungeonArchitect.Graphs
{
    /// <summary>
    /// A camera that manages the graph editor's viewport
    /// </summary>
    [Serializable]
    public class GraphCamera
    {
        float maxAllowedZoom = 6.0f;
        public float MaxAllowedZoom
        {
            get { return maxAllowedZoom; }
            set { maxAllowedZoom = value; }
        }

        [SerializeField]
        Vector2 position = Vector2.zero;
        /// <summary>
        /// Position of the camera
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        public Vector2 ScreenOffset = Vector2.zero;

        [SerializeField]
		float zoomLevel = 1;
        /// <summary>
        /// Zoom scale of the graph camera
        /// </summary>
        public float ZoomLevel
        {
            get
            {
				return zoomLevel;
            }
            set
            {
				zoomLevel = value;
            }
        }

        /// <summary>
        /// Pan the camera along the specified delta value
        /// </summary>
        /// <param name="x">Delta value to move along the X value</param>
        /// <param name="y">Delta value to move along the Y value</param>
        public void Pan(int x, int y)
        {
            Pan(new Vector2(x, y));
        }

        /// <summary>
        /// Pan the camera along the specified delta value
        /// </summary>
        /// <param name="delta">The delta offset to move the camera to</param>
        public void Pan(Vector2 delta)
        {
			position += delta * zoomLevel;
        }

        /// <summary>
        /// Handles the user mouse and keyboard input 
        /// </summary>
        /// <param name="e"></param>
        public void HandleInput(Event e)
        {
			// Handle zooming
			if (e.type == EventType.ScrollWheel) {
				// Grab the original position under the mouse so we can restore it after the zoom
				var originalGraphPosition = ScreenToWorld(e.mousePosition);

				float zoomMultiplier = 0.1f;
				zoomMultiplier *= Mathf.Sign(e.delta.y);
				zoomLevel = Mathf.Clamp(zoomLevel * (1 + zoomMultiplier), 1, maxAllowedZoom);

				var newGraphPosition = ScreenToWorld (e.mousePosition);
				position += originalGraphPosition - newGraphPosition;
			}

            // Handle pan
            int dragButton1 = 1;
            int dragButton2 = 2;
            if (e.type == EventType.MouseDrag && (e.button == dragButton1 || e.button == dragButton2))
            {
                if (e.delta.magnitude < 150)
                {
                    Pan(-e.delta);
                }
            }
        }

        /// <summary>
        /// Converts world coordinates (in the graph view) into Screen coordinates (relative to the editor window)
        /// </summary>
        /// <param name="worldCoord">The world cooridnates of the graph view</param>
        /// <returns>The screen cooridnates relative to the editor window</returns>
        public Vector2 WorldToScreen(Vector2 worldCoord)
        {
            return (worldCoord - position) / zoomLevel + ScreenOffset;
        }

        /// <summary>
        /// Converts the Screen coordinates (of the editor window) into the graph's world coordinate
        /// </summary>
        /// <param name="screenCoord"></param>
        /// <returns>The world coordinates in the graph view</returns>
        public Vector2 ScreenToWorld(Vector2 screenCoord)
        {
            screenCoord -= ScreenOffset;

            return screenCoord * zoomLevel + position; 
        }

        /// <summary>
        /// Converts world coordinates (in the graph view) into Screen coordinates (relative to the editor window)
        /// </summary>
        /// <param name="worldCoord">The world cooridnates of the graph view</param>
        /// <returns>The screen cooridnates relative to the editor window</returns>
        public Rect WorldToScreen(Rect worldCoord)
        {
            var screen = worldCoord;
            screen.position = WorldToScreen(worldCoord.position);
            screen.size = worldCoord.size / zoomLevel;
            return screen;
        }

        /// <summary>
        /// Converts the Screen coordinates (of the editor window) into the graph's world coordinate
        /// </summary>
        /// <param name="screenCoord"></param>
        /// <returns>The world coordinates in the graph view</returns>
        public Rect ScreenToWorld(Rect screenCoord)
        {
            var world = screenCoord;
            world.position = ScreenToWorld(screenCoord.position);
            world.size = screenCoord.size * ZoomLevel;
            return world;
        }



        /// <summary>
        /// Moves the camera so most of the nodes are visible
        /// </summary>
        /// <param name="graph">The graph to query</param>
        /// <param name="editorBounds">The bounds of the editor window</param>
        public void FocusOnBestFit(Graph graph, Rect editorBounds)
        {
            if (graph == null) return;
            if (graph.Nodes.Count > 0)
            {
                var graphVisibleSize = editorBounds.size * zoomLevel;
                Vector2 average = Vector2.zero;
                foreach (var node in graph.Nodes)
                {
                    if (node == null) continue;
                    average += node.Bounds.center;
                }
                average /= graph.Nodes.Count;
                position = average - graphVisibleSize / 2.0f;
            }
            else
            {
                position = Vector3.zero;
                zoomLevel = 1.0f;
            }
        }

        /// <summary>
        /// Moves the camera to the specified node
        /// </summary>
        /// <param name="node">The node to focus on</param>
        /// <param name="editorBounds">The bounds of the editor window</param>
        public void FocusOnNode(GraphNode node, Rect editorBounds)
		{
			var graphVisibleSize = editorBounds.size * zoomLevel;
            var nodePosition = node.Bounds.center;
			position = nodePosition - graphVisibleSize / 2.0f;
        }
    }
}
