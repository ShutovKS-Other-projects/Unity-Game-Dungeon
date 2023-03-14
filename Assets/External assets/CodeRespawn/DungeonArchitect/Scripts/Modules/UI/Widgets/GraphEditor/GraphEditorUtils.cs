//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect.Graphs;
using System.Collections.Generic;

namespace DungeonArchitect.UI.Widgets.GraphEditors
{
    public class GraphEditorUtils
    {
        /// <summary>
        /// Adds the node to the graph asset so it can be serialized to disk
        /// </summary>
        /// <param name="graph">The owning graph</param>
        /// <param name="link">The link to add to the graph</param>
        public static void AddToAsset(UIPlatform platform, Object assetObject, GraphLink link)
        {
            platform.AddObjectToAsset(link, assetObject);
        }

        /// <summary>
        /// Adds the node to the graph asset so it can be serialized to disk
        /// </summary>
        /// <param name="graph">The owning graph</param>
        /// <param name="node">The node to add to the graph</param>
        public static void AddToAsset(UIPlatform platform, Object assetObject, GraphNode node)
        {
            if (assetObject != null && node != null)
            {
                platform.AddObjectToAsset(node, assetObject);
                // Add all the pins in this node to the graph asset as well
                var pins = new List<GraphPin>();
                pins.AddRange(node.InputPins);
                pins.AddRange(node.OutputPins);
                foreach (var pin in pins)
                {
                    if (pin != null)
                    {
                        platform.AddObjectToAsset(pin, assetObject);
                    }
                }
            }
        }

    }

    /// <summary>
    /// Graph editor constants
    /// </summary>
    public class GraphEditorConstants
    {

        public static readonly Color PIN_COLOR = new Color(0.4f, 0.4f, 0.4f);
        public static readonly Color PIN_COLOR_HOVER = new Color(1, 0.6f, 0.0f);
        public static readonly Color PIN_COLOR_CLICK = new Color(1, 0.9f, 0.0f);

        public static readonly Color NODE_COLOR = new Color(0.2824f, 0.2824f, 0.2824f); //new Color(0.1f, 0.1f, 0.1f);
        public static readonly Color NODE_COLOR_SELECTED = new Color(.9f, 0.5f, 0.0f);

        public static readonly Color TEXT_COLOR = new Color(0.9f, 0.9f, 0.9f);
        public static readonly Color TEXT_COLOR_SELECTED = Color.white;

    }
}
