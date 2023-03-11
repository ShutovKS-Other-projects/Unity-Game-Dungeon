//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;
using DungeonArchitect.Utils;

namespace DungeonArchitect.Builders.BSP
{

    public static class BSPDungeonMarkerNames
    {
        public static readonly string GroundRoom = "GroundRoom";
        public static readonly string GroundCorridor = "GroundCorridor";
        public static readonly string Door = "Door";
        public static readonly string WallRoom = "WallRoom";
        public static readonly string WallCorridor = "WallCorridor";
        public static readonly string WallSeparator = "WallSeparator";
    }

    // Non-serialized node class, unlike the serialized version. This is easier to work with but cannot be serialized.
    // These objects will finally be converted to the serialized version BSPNode and saved in the model.  
    // Use the BSPDungeonGraphQuery to traverse the serialized graph when loaded from disk
    class BSPNodeObject
    {
        public Rectangle bounds;
        public BSPNodeObject[] children = new BSPNodeObject[0];
        public BSPNodeObject parent;
        public DungeonUID id = DungeonUID.NewUID();
        public int depthFromRoot;
        public int padding;
		public bool horizontalSplit = false;
        //public bool customRoom;
		//public string customRoomId;
		public Color debugColor = Color.blue;
		public bool discarded = false;

		public List<BSPNodeObject> connectedRooms = new List<BSPNodeObject>();

        public NodeConnection[] subtreeLeafConnections = new NodeConnection[0];

        public Rectangle PaddedBounds
        {
            get
            {
                return Rectangle.ExpandBounds(bounds, -1 * padding);
            }
        }

        /// <summary>
        /// This function assumes that the cell is big enough to split. 
        /// Make sure to call CanSplit function before calling this
        /// </summary>
		public void Split(float splitRatio, System.Random random)
        {
			if (bounds.Width == bounds.Length) {
				horizontalSplit = random.NextFloat () < 0.5f;
			} 
			else {
				horizontalSplit = (bounds.Width > bounds.Length);
			}

			int totalSize = horizontalSplit ? bounds.Width : bounds.Length;
            
            int left = Mathf.RoundToInt(totalSize * splitRatio);
            int right = totalSize - left;

            var child0 = new BSPNodeObject();
            child0.parent = this;
            child0.padding = padding;
            child0.depthFromRoot = depthFromRoot + 1;

            var child1 = new BSPNodeObject();
            child1.parent = this;
            child1.padding = padding;
            child1.depthFromRoot = depthFromRoot + 1;

            var loc0 = bounds.Location;
            var size0 = bounds.Size;

            var loc1 = bounds.Location;
            var size1 = bounds.Size;

			if (horizontalSplit)
            {
                size0.x = left;

                loc1.x += left;
                size1.x = right;
            }
            else
            {
                size0.z = left;

                loc1.z += left;
                size1.z = right;
            }

            child0.bounds = new Rectangle(loc0, size0);
            child1.bounds = new Rectangle(loc1, size1);
            
            children = new BSPNodeObject[] { child0, child1 };
        }

        /// <summary>
        /// Check if it is required to split this node.  This happens if the size is greater than the max allowed room size.
        /// In that case a split is required
        /// </summary>
        /// <param name="maxSize"></param>
        /// <returns></returns>
        public bool MustSplit(int maxSize)
        {
            // Make sure that if we split the largest side, the two smaller sides are still big enough
            float largeSide = Mathf.Max(bounds.Width, bounds.Length);
            return largeSide > maxSize;
        }

        public bool CanSplit(int minSize)
        {
            // Make sure that if we split the largest side, the two smaller sides are still big enough
            float largeSide = Mathf.Max(bounds.Width, bounds.Length);
            return largeSide / 2 >= minSize;
        }

    }

	enum BSPNodeDirection {
		Left,
		Right,
		Top,
		Bottom
	}

	public class BSPDungeonBuilder : DungeonBuilder {

        BSPDungeonConfig bspConfig;
        BSPDungeonModel bspModel;

		new System.Random random;

		/// <summary>
		/// Builds the dungeon layout.  In this method, you should build your dungeon layout and save it in your model file
		/// No markers should be emitted here.   (EmitMarkers function will be called later by the engine to do that)
		/// </summary>
		/// <param name="config">The builder configuration</param>
		/// <param name="model">The dungeon model that the builder will populate</param>
		public override void BuildDungeon(DungeonConfig config, DungeonModel model)
		{
			base.BuildDungeon(config, model);

			random = new System.Random((int)config.Seed);

			// We know that the dungeon prefab would have the appropriate config and models attached to it
			// Cast and save it for future reference
			bspConfig = config as BSPDungeonConfig;
            bspModel = model as BSPDungeonModel;
            bspModel.Config = bspConfig;

			// Generate the level layout and save it in a model.   No markers are emitted here. 
			GenerateLevelLayout();
		}

        public override void OnDestroyed() {
            base.OnDestroyed();
            if (model != null)
            {
                model.ResetModel();
            }
        }

		/// <summary>
		/// Override the builder's emit marker function to emit our own markers based on the layout that we built
		/// You should emit your markers based on the layout you have saved in the model generated previously
		/// When the user is designing the theme interactively, this function will be called whenever the graph state changes,
		/// so the theme engine can populate the scene (BuildDungeon will not be called if there is no need to rebuild the layout again)
		/// </summary>
		public override void EmitMarkers()
		{
			base.EmitMarkers();

			EmitLevelMarkers();
            
			ProcessMarkerOverrideVolumes();
		}

		void GenerateLevelLayout() {
            var rootNode = new BSPNodeObject();
            rootNode.bounds = new Rectangle(0, 0, bspConfig.dungeonWidth, bspConfig.dungeonLength);
            rootNode.padding = bspConfig.roomPadding;
            rootNode.depthFromRoot = 0;

            BuildDungeonGraph(rootNode);

            ConnectDoors(rootNode);


            GenerateCustomRooms(rootNode);

            DiscardExtraRooms(rootNode);

            SerializeGraph(rootNode);
        }
        
        void DebugRoomLayout(BSPNodeObject rootNode)
        {
			var edgeRooms = new List<BSPNodeObject>();
			FindBoundaryEdgeRooms(rootNode.children[1], BSPNodeDirection.Left, edgeRooms);

			foreach (var room in edgeRooms)
			{
				room.debugColor = Color.red;
			}
        }

        BSPNodeObject GetCornerSubtreeNode(BSPNodeObject node, bool left) {
            if (node.children == null || node.children.Length == 0)
            {
                return node;
            }

            var child = left ? node.children[0] : node.children[1];
            return GetCornerSubtreeNode(child, left);
        }
            
		void GenerateCustomRooms(BSPNodeObject rootNode)
		{
            
		}

        void DiscardExtraRooms(BSPNodeObject rootNode)
        {
            TraverseTree(rootNode, n => n.discarded = true);

            FlagConnectedLeafNodes(rootNode);

            int numNodes = 0;
            TraverseTree(rootNode, n => numNodes++);

            int maxTries = numNodes;
            int numTries = 0;

            while (ConnectActiveSubtrees(rootNode) && numTries <= maxTries)
            {
                numTries++;
            }

        }

        void FlagConnectedLeafNodes(BSPNodeObject node) 
        {
            if (node.depthFromRoot >= bspConfig.randomKillDepthStart)
            {
                return;
            }

            foreach (var connection in node.subtreeLeafConnections)
            {
                TraverseParentBranch(connection.Room0, n => n.discarded = false);
                TraverseParentBranch(connection.Room1, n => n.discarded = false);
            }

            foreach (var child in node.children)
            {
                FlagConnectedLeafNodes(child);
            }
        }

        bool ConnectActiveSubtrees(BSPNodeObject node) {
            bool stateModified = false;

            foreach (var child in node.children)
            {
                stateModified |= ConnectActiveSubtrees(child);
            }

            if (node.discarded)
            {
                return stateModified;
            }

            bool bothChildrenActive = (node.children.Length == 2 && !node.children[0].discarded && !node.children[1].discarded);

            if (bothChildrenActive)
            {
                foreach (var connection in node.subtreeLeafConnections)
                {
                    //connection.Room0.discarded = false;
                    //connection.Room1.discarded = false;
                    TraverseParentBranch(connection.Room0, n => {
                        if (n.discarded) {
                            n.discarded = false;
                            stateModified = true;
                        }
                    });
                    TraverseParentBranch(connection.Room1, n => {
                        if (n.discarded) {
                            n.discarded = false;
                            stateModified = true;
                        }
                    });
                }
            }

            return stateModified;
        }


        void DiscardSubtree(BSPNodeObject node) {
            TraverseTree(node, n => n.discarded = true);
        }

        void TraverseTree(BSPNodeObject node, System.Action<BSPNodeObject> visit) {
            // traverse the children
            foreach (var child in node.children)
            {
                TraverseTree(child, visit);
            }

            visit(node);
        }

        void TraverseParentBranch(BSPNodeObject node, System.Action<BSPNodeObject> visit) {
            if (node == null)
            {
                return;
            }

            visit(node);

            TraverseParentBranch(node.parent, visit);
        }

        void ConnectDoors(BSPNodeObject node)
		{
			if (node.discarded || node.children == null) return;

			// Connect the children
			foreach (var child in node.children) {
				ConnectDoors(child);
			}

            // Connect the siblings
            if (node.children.Length == 2) {
                node.subtreeLeafConnections = ConnectPartitions(node.children [0], node.children [1], node.horizontalSplit);
            }

        }


        NodeConnection[] GetConnectionCandidates(BSPNodeObject[] leftRooms, BSPNodeObject[] rightRooms) {
            var connections = new List<NodeConnection>();

            foreach (var leftRoom in leftRooms)
            {
                foreach (var rightRoom in rightRooms)
                {
                    // Connect left and right together
                    var intersection = Rectangle.Intersect(leftRoom.bounds, rightRoom.bounds);
                    var minIntersection = bspConfig.roomPadding * 2;
                    if (intersection.Size.x > minIntersection || intersection.Size.z > minIntersection)
                    {
                        var connection = new NodeConnection(leftRoom, rightRoom, bspConfig.roomPadding);
                        connections.Add(connection);
                    }
                }
            }

            return connections.ToArray();
        }

        void Shuffle(List<BSPNodeObject> nodes) {
            for (int i = 0; i < nodes.Count; i++)
            {
                int j = random.Next() % nodes.Count;
                var temp = nodes[j];
                nodes[j] = nodes[i];
                nodes[i] = temp;
            }
        }

        NodeConnection[] ConnectPartitions(BSPNodeObject leftPartition, BSPNodeObject rightPartition, bool horizontalSplit) {
            var connections = new List<NodeConnection>();

			if (leftPartition.discarded || rightPartition.discarded)
			{
                return connections.ToArray();
			}
			
			var leftRooms = new List<BSPNodeObject>();
			var rightRooms = new List<BSPNodeObject>();

			if (horizontalSplit)
			{
				FindBoundaryEdgeRooms(leftPartition, BSPNodeDirection.Right, leftRooms);
				FindBoundaryEdgeRooms(rightPartition, BSPNodeDirection.Left, rightRooms);
			}
			else   // Vertical split
			{
				// Left = bottom partition
				// Right = top partition
				FindBoundaryEdgeRooms(leftPartition, BSPNodeDirection.Top, leftRooms);
				FindBoundaryEdgeRooms(rightPartition, BSPNodeDirection.Bottom, rightRooms);
			}

			// Connect the two rooms together
			if (leftRooms.Count == 0 || rightRooms.Count == 0)
            {
                return connections.ToArray();
			}

            Shuffle(leftRooms);
            Shuffle(rightRooms);

			bool roomsConnected = false;
			foreach (var leftRoom in leftRooms)
			{
				// First check if any of the right rooms are connected
				foreach (var rightRoom in rightRooms)
				{
					if (leftRoom.connectedRooms.Contains(rightRoom))
					{
						roomsConnected = true;
						break;
					}
				}

				foreach (var rightRoom in rightRooms)
				{
					if (leftRoom.connectedRooms.Contains(rightRoom))
					{
						// Already connected
						continue;
					}

					bool shouldConnectRooms = true;
					if (roomsConnected)
					{
						// rooms are already connected along this edge.  Check if can loop
						shouldConnectRooms = random.NextFloat() < bspConfig.loopingProbability;
					}

					if (shouldConnectRooms)
					{
						// Connect left and right together
						var intersection = Rectangle.Intersect(leftRoom.bounds, rightRoom.bounds);
						var minIntersection = bspConfig.roomPadding * 2;
						if (intersection.Size.x > minIntersection || intersection.Size.z > minIntersection)
						{
							// These two rooms can connect
							leftRoom.connectedRooms.Add(rightRoom);
							rightRoom.connectedRooms.Add(leftRoom);
							roomsConnected = true;

                            var connection = new NodeConnection(leftRoom, rightRoom, bspConfig.roomPadding);
                            connections.Add(connection);
						}
					}

				}
            }

            return connections.ToArray();
		}

		void FindBoundaryEdgeRooms(BSPNodeObject node, BSPNodeDirection direction, List<BSPNodeObject> result) {
			if (node.discarded)
			{
				return;
			}

			bool hasChildren = (node.children != null && node.children.Length > 0);
			if (!hasChildren)
			{
				result.Add(node);
				return;
			}

			if (node.horizontalSplit)
			{
				if (direction == BSPNodeDirection.Left)
				{
					FindBoundaryEdgeRooms(node.children[0], direction, result);
				}
				else if (direction == BSPNodeDirection.Right)
				{
					FindBoundaryEdgeRooms(node.children[1], direction, result);
				}
				else
				{
					FindBoundaryEdgeRooms(node.children[0], direction, result);
					FindBoundaryEdgeRooms(node.children[1], direction, result);
				}
			}
			else  // Vertical split
			{
				if (direction == BSPNodeDirection.Bottom)
				{
					FindBoundaryEdgeRooms(node.children[0], direction, result);
				}
				else if (direction == BSPNodeDirection.Top)
				{
					FindBoundaryEdgeRooms(node.children[1], direction, result);
				}
				else
				{
					FindBoundaryEdgeRooms(node.children[0], direction, result);
					FindBoundaryEdgeRooms(node.children[1], direction, result);
				}
			}
		}

        void BuildDungeonGraph(BSPNodeObject node)
        {
            int targetMinRoomSize = bspConfig.minRoomSize + bspConfig.roomPadding * 2;
            int targetMaxRoomSize = bspConfig.maxRoomSize + bspConfig.roomPadding * 2;

            if (!node.CanSplit(targetMinRoomSize))
            {
                // Node is too small to split further
                return;
            }

            bool shouldSplit;
            if (node.MustSplit(targetMaxRoomSize))
            {
                shouldSplit = true;
            }
            else
            {
                // Check if the aspect ratio would be correct after a split

                // Use a probability to decide if we split
                shouldSplit = random.NextFloat() < bspConfig.smallerRoomProbability;
                
            }

            if (shouldSplit)
            {
                float splitRatio = 0.5f;
                bool unevenSplit = random.NextFloat() < bspConfig.unevenSplitProbability;
                if (unevenSplit)
                {
                    int sizeToSplit = Mathf.Max(node.bounds.Width, node.bounds.Length);

                    int allowedSplitDistance = sizeToSplit - 2 * targetMinRoomSize;
                    if (allowedSplitDistance > 0)
                    {
                        float allowedSplitRatio = allowedSplitDistance / (float)sizeToSplit;
                        var randomValue = random.NextFloat();    // get a random value between 0..1
                        randomValue = randomValue * 2 - 1;  // transform to -1..1

                        // From 0.5, we are going to move the split either to the left or right by half of the allowed ratio (-1..1 * halfAllowedSplitRatio)
                        splitRatio = 0.5f + randomValue * allowedSplitRatio / 2.0f;
                    }
                }
                node.Split(splitRatio, random);
            }

            // Process the children
            foreach (var child in node.children)
            {
                BuildDungeonGraph(child);
            }
        }
        

        void EmitLevelMarkers()
        {
            var gridSize3D = new Vector3(bspConfig.gridSize.x, 0, bspConfig.gridSize.y);
            foreach (var node in bspModel.nodes)
            {
                if (node.discarded) continue;
                // Draw the ground tiles
                if (node.children.Length == 0)
                {
                    var paddedBounds = node.paddedBounds;
                    for (int x = 0; x < paddedBounds.Size.x; x++)
                    {
                        for (int z = 0; z < paddedBounds.Size.z; z++)
                        {
                            Vector3 position = Vector3.Scale(new Vector3(paddedBounds.Location.x + x + 0.5f, 0, paddedBounds.Location.z + z + 0.5f), gridSize3D);
                            
                            var transform = Matrix4x4.TRS(position, Quaternion.identity, Vector3.one);
                            EmitMarker(BSPDungeonMarkerNames.GroundRoom, transform, new IntVector(x, 0, z), -1);
                        }
                    }
                }
            }

            var doorPositions = new HashSet<IntVector>();

            foreach (var connection in bspModel.connections)
            {
                var offset = connection.doorFacingX ? new Vector3(0, 0, 0.5f) : new Vector3(0.5f, 0, 0);
                var pos0 = connection.doorPosition0;
                var pos1 = connection.doorPosition1;
                var pos0F = pos0.ToVector3();
                var pos1F = pos1.ToVector3();

                // Emit the doors
                {
                    Vector3 worldPos0 = Vector3.Scale(pos0F + offset, gridSize3D);
                    Vector3 worldPos1 = Vector3.Scale(pos1F + offset, gridSize3D);
                    float angle0 = connection.doorFacingX ? 90 : 0;
                    float angle1 = connection.doorFacingX ? 270 : 180;

                    Matrix4x4 transform;

                    transform = Matrix4x4.TRS(worldPos0, Quaternion.Euler(0, angle0, 0), Vector3.one);
                    EmitMarker(BSPDungeonMarkerNames.Door, transform, pos0, -1);

                    transform = Matrix4x4.TRS(worldPos1, Quaternion.Euler(0, angle1, 0), Vector3.one);
                    EmitMarker(BSPDungeonMarkerNames.Door, transform, pos1, -1);

                    doorPositions.Add(pos0);
                    doorPositions.Add(pos1);
                }

                int x0 = Mathf.Min(pos0.x, pos1.x);
                int x1 = Mathf.Max(pos0.x, pos1.x);
                int z0 = Mathf.Min(pos0.z, pos1.z);
                int z1 = Mathf.Max(pos0.z, pos1.z);

                if (x0 == x1) z1--;
                if (z0 == z1) x1--;

                // Draw the corridor ground tiles
                for (int x = x0; x <= x1; x++)
                {
                    for (int z = z0; z <= z1; z++)
                    {
                        var doorGroundPosition = Vector3.Scale(new Vector3(x + 0.5f, 0, z + 0.5f), gridSize3D);
                        var transform = Matrix4x4.TRS(doorGroundPosition, Quaternion.identity, Vector3.one);
                        EmitMarker(BSPDungeonMarkerNames.GroundCorridor, transform, new IntVector(x, 0, z), -1);
                    }
                }

                // Draw the corridor walls
                if (x0 == x1)
                {
                    for (int z = z0; z <= z1; z++)
                    {
                        var worldPos = Vector3.Scale(new Vector3(x0, 0, z + 0.5f), gridSize3D);
                        var transform = Matrix4x4.TRS(worldPos, Quaternion.Euler(0, 90, 0), Vector3.one);
                        EmitMarker(BSPDungeonMarkerNames.WallCorridor, transform, new IntVector(x0, 0, z), -1);

                        worldPos = Vector3.Scale(new Vector3(x0 + 1, 0, z + 0.5f), gridSize3D);
                        transform = Matrix4x4.TRS(worldPos, Quaternion.Euler(0, 270, 0), Vector3.one);
                        EmitMarker(BSPDungeonMarkerNames.WallCorridor, transform, new IntVector(x1, 0, z), -1);
                    }
                }
                else
                {
                    for (int x = x0; x <= x1; x++)
                    {
                        var worldPos = Vector3.Scale(new Vector3(x + 0.5f, 0, z0), gridSize3D);
                        var transform = Matrix4x4.TRS(worldPos, Quaternion.Euler(0, 0, 0), Vector3.one);
                        EmitMarker(BSPDungeonMarkerNames.WallCorridor, transform, new IntVector(x, 0, z0), -1);

                        worldPos = Vector3.Scale(new Vector3(x + 0.5f, 0, z0 + 1), gridSize3D);
                        transform = Matrix4x4.TRS(worldPos, Quaternion.Euler(0, 180, 0), Vector3.one);
                        EmitMarker(BSPDungeonMarkerNames.WallCorridor, transform, new IntVector(x, 0, z1), -1);
                    }
                }
            }

            foreach (var node in bspModel.nodes)
            {
                if (node.discarded || node.children.Length > 0)
                {
                    continue;
                }

                var loc = node.paddedBounds.Location;
                var size = node.paddedBounds.Size;
                int x0 = loc.x;
                int x1 = loc.x + size.x;
                int z0 = loc.z;
                int z1 = loc.z + size.z;

                // Emit he walls 
                {
                    for (int x = x0; x < x1; x++)
                    {
                        if (!doorPositions.Contains(new IntVector(x, 0, z0)))
                        {
                            var worldPos = Vector3.Scale(new Vector3(x + 0.5f, 0, z0), gridSize3D);
                            var transform = Matrix4x4.TRS(worldPos, Quaternion.Euler(0, 0, 0), Vector3.one);
                            EmitMarker(BSPDungeonMarkerNames.WallRoom, transform, new IntVector(x, 0, z0), -1);
                        }

                        if (!doorPositions.Contains(new IntVector(x, 0, z1)))
                        {
                            var worldPos = Vector3.Scale(new Vector3(x + 0.5f, 0, z1), gridSize3D);
                            var transform = Matrix4x4.TRS(worldPos, Quaternion.Euler(0, 180, 0), Vector3.one);
                            EmitMarker(BSPDungeonMarkerNames.WallRoom, transform, new IntVector(x, 0, z1), -1);
                        }
                    }

                    for (int z = z0; z < z1; z++)
                    {

                        if (!doorPositions.Contains(new IntVector(x0, 0, z)))
                        {
                            var worldPos = Vector3.Scale(new Vector3(x0, 0, z + 0.5f), gridSize3D);
                            var transform = Matrix4x4.TRS(worldPos, Quaternion.Euler(0, 90, 0), Vector3.one);
                            EmitMarker(BSPDungeonMarkerNames.WallRoom, transform, new IntVector(x0, 0, z), -1);
                        }

                        if (!doorPositions.Contains(new IntVector(x1, 0, z)))
                        {
                            var worldPos = Vector3.Scale(new Vector3(x1, 0, z + 0.5f), gridSize3D);
                            var transform = Matrix4x4.TRS(worldPos, Quaternion.Euler(0, 270, 0), Vector3.one);
                            EmitMarker(BSPDungeonMarkerNames.WallRoom, transform, new IntVector(x1, 0, z), -1);
                        }
                    }
                }

                // Emit the wall separators
                {
                    for (int x = x0; x <= x1; x++)
                    {
                        var worldPos = Vector3.Scale(new Vector3(x, 0, z0), gridSize3D);
                        var transform = Matrix4x4.TRS(worldPos, Quaternion.Euler(0, 0, 0), Vector3.one);
                        EmitMarker(BSPDungeonMarkerNames.WallSeparator, transform, new IntVector(x, 0, z0), -1);

                        worldPos = Vector3.Scale(new Vector3(x, 0, z1), gridSize3D);
                        transform = Matrix4x4.TRS(worldPos, Quaternion.Euler(0, 0, 0), Vector3.one);
                        EmitMarker(BSPDungeonMarkerNames.WallSeparator, transform, new IntVector(x, 0, z1), -1);
                    }

                    for (int z = z0 + 1; z <= z1 - 1; z++)
                    {
                        var worldPos = Vector3.Scale(new Vector3(x0, 0, z), gridSize3D);
                        var transform = Matrix4x4.TRS(worldPos, Quaternion.Euler(0, 0, 0), Vector3.one);
                        EmitMarker(BSPDungeonMarkerNames.WallSeparator, transform, new IntVector(x0, 0, z), -1);

                        worldPos = Vector3.Scale(new Vector3(x1, 0, z), gridSize3D);
                        transform = Matrix4x4.TRS(worldPos, Quaternion.Euler(0, 0, 0), Vector3.one);
                        EmitMarker(BSPDungeonMarkerNames.WallSeparator, transform, new IntVector(x1, 0, z), -1);
                    }
                }
            }

        }

        void SerializeGraph(BSPNodeObject rootNode)
        {
            var serializedNodes = new List<BSPNode>();
            var serializedConnections = new List<BSPNodeConnection>();

            SerializeGraph(rootNode, serializedNodes, serializedConnections);

            bspModel.nodes = serializedNodes.ToArray();
            bspModel.connections = serializedConnections.ToArray();

            bspModel.rootNode = rootNode.id;
        }

        void SerializeGraph(BSPNodeObject node, List<BSPNode> serializedNodes, List<BSPNodeConnection> serializedConnections)
        {
			if (node == null)
			{
				return;
			}

            var serializedNode = new BSPNode();
            serializedNode.id = node.id;
            serializedNode.bounds = node.bounds;
            serializedNode.paddedBounds = node.PaddedBounds;
            serializedNode.depthFromRoot = node.depthFromRoot;
			serializedNode.debugColor = node.debugColor;
			serializedNode.discarded = node.discarded;
            
            if (node.parent != null)
            {
                serializedNode.parent = node.parent.id;
            }

            var childIds = new List<DungeonUID>();
            foreach (var child in node.children)
            {
				if (child != null)
				{
					childIds.Add(child.id);
				}
            }
            serializedNode.children = childIds.ToArray();
            
			var connectedIds = new List<DungeonUID>();
			foreach (var connectedRoom in node.connectedRooms)
			{
				connectedIds.Add(connectedRoom.id);
			}
			serializedNode.connectedRooms = connectedIds.ToArray();

            var subtreeLeafConnections = new List<BSPNodeConnection>();
            foreach (var connection in node.subtreeLeafConnections)
            {
                var serializedConnection = new BSPNodeConnection();
                serializedConnection.room0 = connection.Room0.id;
                serializedConnection.room1 = connection.Room1.id;
                serializedConnection.doorPosition0 = connection.DoorPosition0;
                serializedConnection.doorPosition1 = connection.DoorPosition1;
                serializedConnection.doorFacingX = connection.DoorFacingX;
                subtreeLeafConnections.Add(serializedConnection);

                if (!connection.Room0.discarded && !connection.Room1.discarded)
                {
                    serializedConnections.Add(serializedConnection);
                }
            }
            serializedNode.subtreeLeafConnections = subtreeLeafConnections.ToArray();
            
			serializedNodes.Add(serializedNode);

            // Serialize the children
            foreach (var child in node.children)
            {
                SerializeGraph(child, serializedNodes, serializedConnections);
            }
        }

        public override void DebugDraw()
        {
            if (!bspModel) return;

            var gridSize3D = new Vector3(bspConfig.gridSize.x, 0, bspConfig.gridSize.y);
			var graphQuery = bspModel.CreateGraphQuery();
            var discardedColor = new Color(0, 0, 0, 0.35f);
            var debugTextItems = new List<DebugTextItem>();
            int debugDoorIndex = 0;
            foreach (var node in bspModel.nodes)
            {
                if (!node.discarded) 
                {
                    //continue;
                }
                //DebugDrawUtils.DrawBounds(node.bounds, Color.green, gridSize3D, false);

                // Draw the room
                if (node.children.Length == 0)
                {
                    // only render leaf nodes
                    var paddedBounds = node.paddedBounds;
                    var color = node.discarded ? discardedColor : node.debugColor;
                    DebugDrawUtils.DrawBounds(paddedBounds, color, gridSize3D, false);

                }

                var connectionColor = Color.red;

                bool renderedDoors = false;
				// Draw the connected rooms
				//foreach (var connectedNodeId in node.connectedRooms)
                foreach (var leafConnection in node.subtreeLeafConnections)
                {
                    var room0 = graphQuery.GetNode(leafConnection.room0);
                    var room1 = graphQuery.GetNode(leafConnection.room1);
					//var connectedNode = graphQuery.GetNode(connectedNodeId);
					var intersection = Rectangle.Intersect(room0.bounds, room1.bounds);
					var center = intersection.Center();
                    var centerF = IntVector.ToV3(center);
                    var centerWorld = Vector3.Scale(centerF, gridSize3D);
					var offsetStart = Vector3.zero;
					var offsetEnd = Vector3.zero;
					var padding = bspConfig.roomPadding;

                    var doorOffset0 = Vector3.zero;
                    var doorOffset1 = Vector3.zero;

                    var textOffset = Vector3.zero;

					if (intersection.Size.x > 0)
					{
						offsetStart.z -= padding;
						offsetEnd.z += padding;

                        doorOffset0.x -= 0.0f;
                        doorOffset1.x += 1.0f;

                        textOffset.x -= 1;
                        textOffset.z += 1.0f;
					}
					else
					{
						offsetStart.x -= padding;
                        offsetEnd.x += padding;

                        doorOffset0.z -= 0.0f;
                        doorOffset1.z += 1.0f;

                        textOffset.x -= 0.25f;
					}

                    offsetStart = Vector3.Scale(offsetStart, gridSize3D);
                    offsetEnd = Vector3.Scale(offsetEnd, gridSize3D);

                    doorOffset0 = Vector3.Scale(doorOffset0, gridSize3D);
                    doorOffset1 = Vector3.Scale(doorOffset1, gridSize3D);

                    textOffset = Vector3.Scale(textOffset, gridSize3D);

                    bool discarded = (room0.discarded || room1.discarded);
                    var doorColor = discarded ? discardedColor : connectionColor;
                    Debug.DrawLine(centerWorld + offsetStart + doorOffset0, centerWorld + offsetEnd + doorOffset0, doorColor);
                    Debug.DrawLine(centerWorld + offsetStart + doorOffset1, centerWorld + offsetEnd + doorOffset1, doorColor);

                    if (!discarded)
                    {
                        var debugText = new DebugTextItem();
                        debugText.position = centerWorld + textOffset;
                        debugText.color = Color.black;
                        debugText.message = "" + (char)('A' + debugDoorIndex);
                        debugTextItems.Add(debugText);

                        renderedDoors = true;
                    }
				}

                if (renderedDoors)
                {
                    debugDoorIndex++;
                }
            }

            var debugText3D = GetComponent<DebugText3D>();
            if (debugText3D != null)
            {
                debugText3D.items = debugTextItems.ToArray();
            }
        }
    }

    class NodeConnection {
        BSPNodeObject room0;
        public BSPNodeObject Room0
        {
            get
            {
                return room0;
            }
        }

        BSPNodeObject room1;
        public BSPNodeObject Room1
        {
            get
            {
                return room1;
            }
        }
        
        bool doorFacingX;
        public bool DoorFacingX
        {
            get { return doorFacingX; }
        }

        IntVector doorPosition0;
        public IntVector DoorPosition0
        {
            get { return doorPosition0; }
            set { doorPosition0 = value; }
        }

        IntVector doorPosition1;
        public IntVector DoorPosition1
        {
            get { return doorPosition1; }
            set { doorPosition1 = value; }
        }

        public NodeConnection(BSPNodeObject room0, BSPNodeObject room1, int padding) {
            this.room0 = room0;
            this.room1 = room1;

            var intersection = Rectangle.Intersect(room0.bounds, room1.bounds);
            var center = intersection.Center();
            if (intersection.Size.x > 0)
            {
                doorPosition0 = center + new IntVector(0, 0, padding);
                doorPosition1 = center - new IntVector(0, 0, padding);
                doorFacingX = false;
            }
            else
            {
                doorPosition0 = center + new IntVector(padding, 0, 0);
                doorPosition1 = center - new IntVector(padding, 0, 0);
                doorFacingX = true;
            }
        }
    }
}