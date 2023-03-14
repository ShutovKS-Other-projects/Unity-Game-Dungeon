//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace DungeonArchitect.Graphs
{
    /// <summary>
    /// An ID provider for graph objects
    /// </summary>
    [Serializable]
    public class IndexCounter
    {
        [SerializeField]
        int index = 0;

        public int GetNext()
        {
            index++;
            return index;
        }
    }
    
    /// <summary>
    /// Theme Graph data structure holds all the theme nodes and their connections
    /// </summary>
    [Serializable]
    public class Graph : ScriptableObject
    {
        [SerializeField]
        IndexCounter indexCounter;
        public DungeonArchitect.Graphs.IndexCounter IndexCounter
        {
            get { return indexCounter; }
        }

        [SerializeField]
        IndexCounter topZIndex;

        [SerializeField]
        List<GraphNode> nodes;
        /// <summary>
        /// List of graph nodes
        /// </summary>
        public List<GraphNode> Nodes
        {
            get
            {
                return nodes;
            }
        }

        [SerializeField]
        List<GraphLink> links;

        /// <summary>
        /// List of graph links connecting the nodes
        /// </summary>
        public List<GraphLink> Links
        {
            get
            {
                return links;
            }
        }

        /// <summary>
        /// The z index of the top most node
        /// </summary>
        public IndexCounter TopZIndex
        {
            get
            {
                return topZIndex;
            }
        }
        
        public virtual void OnEnable()
        {
            //hideFlags = HideFlags.HideAndDontSave;
            if (IndexCounter == null)
            {
                indexCounter = new IndexCounter();
            }
            if (topZIndex == null)
            {
                topZIndex = new IndexCounter();
            }
            if (nodes == null)
            {
                nodes = new List<GraphNode>();
            }
            if (links == null)
            {
                links = new List<GraphLink>();
            }

            // Remove any null nodes
            for (int i = 0; i < nodes.Count; )
            {
                if (nodes[i] == null)
                {
                    nodes.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }

        /// <summary>
        /// Gets the node by it's id
        /// </summary>
        /// <param name="id">The ID of the node</param>
        /// <returns>The retrieved node.  null if node with this id doesn't exist</returns>
        public GraphNode GetNode(string id)
        {
            var result = from node in Nodes
                         where node.Id == id
                         select node;

            return (result.Count() > 0) ? result.Single() : null;
        }

        /// <summary>
        /// Get all nodes of the specified type
        /// </summary>
        /// <typeparam name="T">The type of nodes to retrieve. Should be a subclass of GraphNode</typeparam>
        /// <returns>List of all the nodes of the specified type</returns>
        public T[] GetNodes<T>() where T : GraphNode
        {
            var targetNodes = new List<T>();
            foreach (var node in nodes)
            {
                if (node is T)
                {
                    targetNodes.Add(node as T);
                }
            }
            return targetNodes.ToArray();
        }
                
    }

}
