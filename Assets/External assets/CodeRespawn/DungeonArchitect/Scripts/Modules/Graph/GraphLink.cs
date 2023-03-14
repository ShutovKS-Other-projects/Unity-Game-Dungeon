//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Graphs
{
    /// <summary>
    /// A graph link is a directional connection between two graph nodes
    /// </summary>
    [System.Serializable]
    public class GraphLink : ScriptableObject
    {
        [SerializeField]
        int id;
        /// <summary>
        /// The ID of the link
        /// </summary>
        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                UpdateName();
            }
        }

        [SerializeField]
        GraphPin input;
        /// <summary>
        /// The input pin this link originates from
        /// </summary>
        public GraphPin Input
        {
            get
            {
                return input;
            }
            set
            {
                input = value;
            }
        }

        [SerializeField]
        GraphPin output;
        /// <summary>
        /// The output pin this link points to
        /// </summary>
        public GraphPin Output
        {
            get
            {
                return output;
            }
            set
            {
                output = value;
            }
        }

        [SerializeField]
        Graph graph;
        /// <summary>
        /// The graph this link belongs to
        /// </summary>
        public Graph Graph
        {
            get
            {
                return graph;
            }
            set
            {
                graph = value;
            }
        }

        public void OnEnable()
        {
            hideFlags = HideFlags.HideInHierarchy;
            UpdateName();
        }

        void UpdateName()
        {
            this.name = "Link_" + id;
        }


        /// <summary>
        /// Determines the spring strength of the link.  
        /// It reduces as it gets smaller to draw good looking link at any distance
        /// </summary>
        /// <returns></returns>
        public float GetTangentStrength()
        {
            var distance = (output.WorldPosition - input.WorldPosition).magnitude;
            var tangentStrength = Mathf.Min(output.TangentStrength, distance / 2.0f);
            return tangentStrength;
        }
    }
}