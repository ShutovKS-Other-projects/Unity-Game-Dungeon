//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;

namespace DungeonArchitect.Graphs
{
    /// <summary>
    /// Represents a graph node in the theme graph.  This is the base class for all graph nodes
    /// </summary>
    [System.Serializable]
    public class GraphNode : ScriptableObject
    {
        [SerializeField]
        [HideInInspector]
		protected string id;
        /// <summary>
        /// The ID of the graph node
        /// </summary>
        public string Id
        {
            get
            {
                return id;
            }
			set { id = value; }
        }

        [SerializeField]
        [HideInInspector]
        protected string caption;
        /// <summary>
        /// The caption label of the node. It is up to the implementation to draw this label, if needed
        /// </summary>
        public string Caption
        {
            get
            {
                return caption;
            }
            set
            {
                caption = value;
            }
        }

        [SerializeField]
        [HideInInspector]
        protected Rect bounds = new Rect(10, 10, 120, 120);
        /// <summary>
        /// The bounds of the node
        /// </summary>
        public Rect Bounds
        {
            get
            {
                return bounds;
            }
            set
            {
                bounds = value;
            }
        }

        [SerializeField]
        [HideInInspector]
        protected bool canBeDeleted = true;
        public bool CanBeDeleted
        {
            get { return canBeDeleted; }
        }

        [SerializeField]
        [HideInInspector]
        protected bool canBeSelected = true;
        public bool CanBeSelected
        {
            get { return canBeSelected; }
        }

        [SerializeField]
        [HideInInspector]
        protected bool canBeMoved = true;
        public bool CanBeMoved
        {
            get { return canBeMoved; }
        }

        [SerializeField]
        [HideInInspector]
        protected bool selected = false;
        /// <summary>
        /// Flag to indicate if the node has been selected
        /// </summary>
        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                if (canBeSelected)
                {
                    selected = value;
                }
                else
                {
                    selected = false;
                }
            }
        }

        /// <summary>
        /// The size of the node
        /// </summary>
        public Vector2 Size
        {
            get { return bounds.size; }
            set
            {
                bounds.size = value;
            }
        }

        /// <summary>
        /// The position of the node
        /// </summary>
        public Vector2 Position
        {
            get { return bounds.position; }
            set
            {
                bounds.position = value;
            }
        }

        [SerializeField]
        [HideInInspector]
        protected int zIndex;
        /// <summary>
        /// The Z-index of the node.  It determines if the node is on top of other nodes
        /// </summary>
        public int ZIndex
        {
            get
            {
                return zIndex;
            }
            set
            {
                zIndex = value;
            }
        }

        [SerializeField]
        [HideInInspector]
        protected List<GraphPin> inputPins;
        /// <summary>
        /// List of input pins owned by this node
        /// </summary>
        public GraphPin[] InputPins
        {
            get
            {
                return inputPins != null ? inputPins.ToArray() : new GraphPin[0];
            }
        }

        [SerializeField]
        [HideInInspector]
        protected List<GraphPin> outputPins;
        /// <summary>
        /// List of output pins owned by this node
        /// </summary>
        public GraphPin[] OutputPins
        {
            get
            {
                return outputPins != null ? outputPins.ToArray() : new GraphPin[0];
            }
        }

        /// <summary>
        /// Gets the first output pin. Returns null if no output pins are defined
        /// </summary>
        public GraphPin OutputPin
        {
            get
            {
                if (outputPins == null || outputPins.Count == 0) return null;
                return outputPins[0];
            }
        }

        /// <summary>
        /// Gets the first input pin. Returns null if no input pins are defined
        /// </summary>
        public GraphPin InputPin
        {
            get
            {
                if (inputPins == null || inputPins.Count == 0) return null;
                return inputPins[0];
            }
        }

        [SerializeField]
        [HideInInspector]
        protected Graph graph;

        /// <summary>
        /// The graph that owns this node
        /// </summary>
        public Graph Graph
        {
            get
            {
                return graph;
            }
        }

        public virtual void OnEnable()
        {
            hideFlags = HideFlags.HideInHierarchy;
        }

		public virtual void Initialize(string id, Graph graph)
        {
            this.id = id;
            this.graph = graph;
        }

        /// <summary>
        /// Called when the node is copied.  
        /// The implementations should implement copy here (e.g. deep / shallow copy depending on implementation)
        /// </summary>
        /// <param name="node"></param>
        public virtual void CopyFrom(GraphNode node)
        {
            if (node != null)
            {
                caption = node.Caption;
                this.bounds = node.Bounds;
            }
        }

        protected void UpdateName(string prefix)
        {
            this.name = prefix + id;
        }

        private bool dragging = false;
        public bool Dragging
        {
            get { return dragging; }
            set { dragging = value; }
        }

        /// <summary>
        /// Creates a pin with the specified configuration
        /// </summary>
        /// <param name="pinType">The type of pin (input / output)</param>
        /// <param name="position">The position of the pin, relative to the node bounds</param>
        /// <param name="boundsOffset">The bounds of the pin, relative to the position</param>
        /// <param name="tangent">The tangent of the pin.  Links connected to the pin would come out from this direction</param>
        protected GraphPin CreatePin(GraphPinType pinType, Vector2 position, Rect boundsOffset, Vector2 tangent)
        {
            return CreatePinOfType<GraphPin>(pinType, position, boundsOffset, tangent);
        }

        protected T CreatePinOfType<T>(GraphPinType pinType, Vector2 position, Rect boundsOffset, Vector2 tangent) where T : GraphPin
        {
            var pin = CreateInstance<T>();
            pin.PinType = pinType;
            pin.Node = this;
            pin.Position = position;
            pin.BoundsOffset = boundsOffset;
            pin.Tangent = tangent;
            if (pinType == GraphPinType.Input)
            {
                pin.name = this.name + "_InputPin";
                if (inputPins == null)
                {
                    inputPins = new List<GraphPin>();
                }
                inputPins.Add(pin);
            }
            else
            {
                pin.name = this.name + "_OutputPin";
                if (outputPins == null)
                {
                    outputPins = new List<GraphPin>();
                }
                outputPins.Add(pin);
            }
            return pin;
        }

        /// <summary>
        /// Gets the list of parent graph nodes
        /// </summary>
        /// <returns>List of parent graph nodes</returns>
        public GraphNode[] GetParentNodes()
        {
            var parents = new List<GraphNode>();
            if (InputPins.Length > 0)
            {
                foreach (var link in InputPins[0].GetConntectedLinks())
                {
                    if (link != null && link.Output != null && link.Output.Node != null)
                    {
                        parents.Add(link.Output.Node);
                    }
                }
            }
            return parents.ToArray();
        }

        /// <summary>
        /// Gets the list of child nodes
        /// </summary>
        /// <returns>List of child nodes</returns>
        public GraphNode[] GetChildNodes()
        {
            var children = new List<GraphNode>();
            if (OutputPins.Length > 0)
            {
                foreach (var link in OutputPins[0].GetConntectedLinks())
                {
                    if (link != null && link.Input != null && link.Input.Node != null)
                    {
                        children.Add(link.Input.Node);
                    }
                }
            }
            return children.ToArray();
        }

        /// <summary>
        /// Moves the node by the specified delta
        /// </summary>
        /// <param name="delta">The delta offset to move the node by</param>
        public void DragNode(Vector2 delta)
        {
            Position += delta;
        }
        
    }
}
