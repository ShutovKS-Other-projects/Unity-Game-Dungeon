//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;
using DungeonArchitect.Graphs;
using DungeonArchitect.SpatialConstraints;

namespace DungeonArchitect
{
	/// <summary>
	/// The data structure for a marker
	/// </summary>
	[System.Serializable]
	public class PropSocket {
		public int Id;
		public string SocketType;
		public Matrix4x4 Transform;
		public IntVector gridPosition;
		public int cellId;
		public bool markForDeletion = false;
		public List<PropSocket> childMarkers = new List<PropSocket>();
		public object metadata;

		public PropSocket() {}

		public PropSocket(PropSocket other)
		{
			Id = other.Id;
			SocketType = other.SocketType;
			Transform = other.Transform;
			gridPosition = other.gridPosition;
			cellId = other.cellId;
			markForDeletion = other.markForDeletion;
			childMarkers = other.childMarkers;
			metadata = other.metadata;
		}
        
		public override string ToString()
		{
			return SocketType;
		}
	}
}

namespace DungeonArchitect.Themeing
{
	/// <summary>
    /// Props can emit new sockets when they are inserted, to add more child props relative to them
	/// </summary>
	public class PropChildSocketData {
		public string SocketType;
		public Matrix4x4 Offset;
	}

	public enum DungeonThemeItemStaticMode
	{
		Unchanged,
		ForceStatic,
		ForceNonStatic,
	}

    /// <summary>
    /// The data structure to hold information about a single node in the asset file
    /// </summary>
	public abstract class DungeonThemeItem {
		/// <summary>
        /// The unique guid of the node that generated this prop
		/// </summary>
		public string NodeId;

		/// <summary>
        /// The socket to attach to
		/// </summary>
		public string AttachToSocket;

		/// <summary>
        /// The probability of attachment
		/// </summary>
		public float Affinity;

		/// <summary>
        /// Should this prop consume the node (i.e. stop further processing of the sibling nodes)
		/// </summary>
		public bool ConsumeOnAttach;

		/// <summary>
        /// The offset to apply from the node's marker position
		/// </summary>
		public Matrix4x4 Offset;

		/// <summary>
        /// The child socket markers emitted from this node
		/// </summary>
		public List<PropChildSocketData> ChildSockets = new List<PropChildSocketData>();

        /// <summary>
        /// Indicates if the object's static flag is to be set
        /// </summary>
        public DungeonThemeItemStaticMode StaticState = DungeonThemeItemStaticMode.Unchanged;

		/// <summary>
		/// Flag to indicate if this node's geometry affects the navmesh
		/// </summary>
		public bool affectsNavigation;

		/// <summary>
        /// Flag to indicate if a selection rule script is used to determine if this prop is selected for insertion
		/// </summary>
		public bool UseSelectionRule;

		/// <summary>
        /// The script to to determine if this prop is selected for insertion. This holds the class of type SelectorRule
		/// </summary>
		public string SelectorRuleClassName;
		
		/// <summary>
        /// Flag to indicate if a transformation rule script is used to determine the transform offset while inserting this mesh
		/// </summary>
		public bool UseTransformRule;

		/// <summary>
        /// The script that calculates the transform offset to be used when inserting this mesh. This holds a class of type TransformationRule
		/// </summary>
		public string TransformRuleClassName;

        /// <summary>
        /// Flag to indicate if spatial constraints are to be used
        /// </summary>
        public bool useSpatialConstraint = false;

        /// <summary>
        /// Spatial constraints lets you select a node based on nearby neighbor constraints
        /// </summary>
        public SpatialConstraintAsset spatialConstraint;

        /// <summary>
        /// If externally managed, the theme engine will not destroy the object 
        /// </summary>
        public bool externallyManaged = false;
    }

    /// <summary>
    /// Game Object node data asset attributes
    /// </summary>
	public class GameObjectDungeonThemeItem : DungeonThemeItem {
		// The template to instantiate
		public GameObject Template;
	}

	/// <summary>
	/// Game Object node data asset attributes
	/// </summary>
	public class GameObjectArrayDungeonThemeItem : DungeonThemeItem {
		// The template to instantiate
		public GameObject[] Templates;
	}

    /// <summary>
    /// Sprite node data asset attributes
    /// </summary>
    public class SpriteDungeonThemeItem : DungeonThemeItem
    {
		public Sprite sprite;
		public Color color;
		public Material materialOverride;
		public string sortingLayerName;
		public int orderInLayer;

		// Physics2D attributes
		public DungeonSpriteCollisionType collisionType;
		public PhysicsMaterial2D physicsMaterial;
		public Vector2 physicsOffset;
		public Vector2 physicsSize;
		public float physicsRadius;

	}
}
