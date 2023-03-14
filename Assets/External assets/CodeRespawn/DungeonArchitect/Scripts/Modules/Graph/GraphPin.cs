//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;

namespace DungeonArchitect.Graphs
{
    /// <summary>
    /// The graph pin type
    /// </summary>
    public enum GraphPinType
    {
        Input,
        Output,
        Unknown
    }

    /// <summary>
    /// The state of the mouse input on a pin
    /// </summary>
    public enum GraphPinMouseState
    {
        Hover,
        Clicked,
        None
    }

    /// <summary>
    /// A pin is used to connect a link to a node
    /// </summary>
    [System.Serializable]
    public class GraphPin : ScriptableObject
    {
        GraphPinMouseState clickState = GraphPinMouseState.None;
        /// <summary>
        /// The state of the mouse input on this pin
        /// </summary>
        public GraphPinMouseState ClickState
        {
            get
            {
                return clickState;
            }
            set
            {
                clickState = value;
            }
        }

        [SerializeField]
        GraphPinType pinType;

        /// <summary>
        /// The type of this pin
        /// </summary>
        public GraphPinType PinType
        {
            get
            {
                return pinType;
            }
            set
            {
                pinType = value;
            }
        }

        public delegate void OnPinLinksDestroyed(GraphPin pin);

        /// <summary>
        /// Notifies whenever the pin is destroyed
        /// </summary>
        public event OnPinLinksDestroyed PinLinksDestroyed;

        /// <summary>
        /// The node this pin belongs to
        /// </summary>
        [SerializeField]
        GraphNode node;

        /// <summary>
        /// The owning graph node
        /// </summary>
        public GraphNode Node
        {
            get
            {
                return node;
            }
            set
            {
                node = value;
            }
        }

        [SerializeField]
        Vector2 position = new Vector2();
        /// <summary>
        /// The position of the graph pin, relative to the owning node's position
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

        /// <summary>
        /// The world position of the pin
        /// </summary>
        public Vector2 WorldPosition
        {
            get
            {
                if (node != null)
                {
                    return position + node.Position;
                }
                else
                {
                    return position;
                }
            }
        }

        [SerializeField]
        Rect boundsOffset = new Rect(0, 0, 20, 20);
        /// <summary>
        /// The bounds of the pin, relative to the node's position
        /// </summary>
        public Rect BoundsOffset
        {
            get
            {
                return boundsOffset;
            }
            set
            {
                boundsOffset = value;
            }
        }

        [SerializeField]
        Vector2 tangent = new Vector2();
        /// <summary>
        /// The tangent of the pin.  Links connected to this pin would come in or out from this direction
        /// </summary>
        public Vector2 Tangent
        {
            get
            {
                return tangent;
            }
            set
            {
                tangent = value;
            }
        }

        [SerializeField]
        float tangentStrength = 50;
        /// <summary>
        /// The spring strength of the link connected to this pin
        /// </summary>
        public float TangentStrength
        {
            get
            {
                return tangentStrength;
            }
            set
            {
                tangentStrength = value;
            }
        }

        public void OnEnable()
        {
            hideFlags = HideFlags.HideInHierarchy;
        }

        /// <summary>
        /// Gets all the links connected to this pin
        /// </summary>
        /// <returns>The connected links.</returns>
        public GraphLink[] GetConntectedLinks()
        {
            var result = new List<GraphLink>();
            if (node != null && node.Graph != null)
            {
                foreach (var link in node.Graph.Links)
                {
                    if (link == null)
                    {
                        continue;
                    }
                    if (link.Input == this || link.Output == this)
                    {
                        result.Add(link);
                    }
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// Checks if a point is inside the pin
        /// </summary>
        /// <param name="worldPoint">The point to test in world coordinates</param>
        /// <returns>true, if inside the bounds of this pin, false otherwise</returns>
        public virtual bool ContainsPoint(Vector2 worldPoint)
        {
            return GetWorldBounds().Contains(worldPoint);
        }

        /// <summary>
        /// Gets the bounds of the pin, in world coordinates
        /// </summary>
        /// <returns>The bounds of the pin, in world coordinates</returns>
        Rect GetWorldBounds()
        {
            var position = this.WorldPosition + boundsOffset.position;
            var bounds = new Rect(position.x, position.y, boundsOffset.size.x, boundsOffset.size.y);
            return bounds;
        }

        /// <summary>
        /// Gets the bounds of the pin, relative to the node position
        /// </summary>
        /// <returns>The bounds of the pin, relative to the node position</returns>
        public Rect GetBounds()
        {
            var position = this.Position + boundsOffset.position;
            var bounds = new Rect(position.x, position.y, boundsOffset.size.x, boundsOffset.size.y);
            return bounds;
        }

        public void NotifyPinLinksDestroyed()
        {
            if (PinLinksDestroyed != null)
            {
                PinLinksDestroyed(this);
            }
        }

        bool requestLinkDeletionInitiated = false;
        public bool RequestLinkDeletionInitiated
        {
            get { return requestLinkDeletionInitiated; }
            set { requestLinkDeletionInitiated = value; }
        }

    }
}
