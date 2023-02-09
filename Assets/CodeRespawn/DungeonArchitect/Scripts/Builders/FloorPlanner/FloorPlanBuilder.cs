//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;
using DungeonArchitect.Utils;

namespace DungeonArchitect.Builders.FloorPlan
{
    public static class FloorPlanMarkerNames
    {
        public static readonly string Ground = "Ground";
        public static readonly string Ceiling = "Ceiling";
        public static readonly string Wall = "Wall";
        public static readonly string Door = "Door";
        public static readonly string BuildingWall = "BuildingWall";
    }

    public class FloorPlanBuilder : DungeonBuilder
    {
        FloorPlanConfig floorPlanConfig;
        FloorPlanModel floorPlanModel;

        FloorChunkDB ChunkDB;
        FloorDoorManager DoorManager;
        HashSet<int> Visited = new HashSet<int>();

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
            floorPlanConfig = config as FloorPlanConfig;
            floorPlanModel = model as FloorPlanModel;
            floorPlanModel.Config = floorPlanConfig;

            // Generate the floor plan layout and save it in a model.   No markers are emitted here. 
            BuildLayout();

            markers.Clear();
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
            EmitBuildingMarkers();
            ProcessMarkerOverrideVolumes();
        }

        void BuildLayout() {

            ChunkDB = new FloorChunkDB();
            DoorManager = new FloorDoorManager();
            Visited.Clear();
	        random = new System.Random((int)floorPlanConfig.Seed);

	        var userDefinedSplits = new List<Vector2>();
	        var dungeonPosition = transform.position;
	        foreach (var pointTool  in floorPlanConfig.manualHallwayPoints)
	        {
		        if (pointTool == null) continue;
		        var worldPosition = pointTool.transform.position - dungeonPosition;
		        var localPoint = new Vector2(worldPosition.x / floorPlanConfig.GridSize.x, worldPosition.z / floorPlanConfig.GridSize.z);
		        userDefinedSplits.Add(localPoint);
	        }
	        
	        int NumFloors = Mathf.RoundToInt(floorPlanConfig.BuildingSize.y);
	        for (int y = 0; y < NumFloors; y++) {
		        // Build the hallways and the intermediate floor chunks (which will hold the rooms)
		        List<FloorChunk> FloorChunks = new List<FloorChunk>();
		        {
			        FloorChunk InitialFloor = ChunkDB.Create();
			        InitialFloor.Bounds.Location = new IntVector(0, y, 0);
			        InitialFloor.Bounds.Size = MathUtils.ToIntVector(floorPlanConfig.BuildingSize);

			        Stack<FloorChunk> Stack = new Stack<FloorChunk>();
			        Stack.Push(InitialFloor);
			        while (Stack.Count > 0) {
				        FloorChunk Top = Stack.Pop();
				        float Area = Top.Area();
				        if (Area <= 0) continue;
				        if (Top.Area() <= floorPlanConfig.MinRoomChunkArea) {
					        FloorChunks.Add(Top);
				        }
				        else {
					        // Needs to be split further
					        FloorChunk Left = ChunkDB.Create();
					        FloorChunk Right = ChunkDB.Create();
					        FloorChunk Hallway = ChunkDB.Create();

					        bool userManualSplit = false;
					        if (userDefinedSplits.Count > 0)
					        {
						        var localPosition = userDefinedSplits[0];
						        if (Top.Bounds.Contains(Mathf.FloorToInt(localPosition.x), Mathf.FloorToInt(localPosition.y)))
						        {
							        userManualSplit = true;
						        }
					        }

					        if (userManualSplit)
					        {
						        var localPosition = userDefinedSplits[0];
						        userDefinedSplits.RemoveAt(0);

						        float ratio = 0;
						        if (Top.Bounds.Size.x > Top.Bounds.Size.z)
						        {
							        ratio = localPosition.x - Top.Bounds.X;
						        }
						        else
						        {
							        ratio = localPosition.y - Top.Bounds.Z;
						        }
						        
						        SplitChunk(Top, ratio, Left, Right, Hallway);
					        }
					        else
					        {
						        SplitChunk(Top, Left, Right, Hallway);
					        }

					        ChunkDB.Register(Hallway);

					        Stack.Push(Left);
					        Stack.Push(Right);
				        }
			        }
		        }

		        // Split the floor chunks (space between the hallways) to create rooms
		        foreach (FloorChunk Chunk in FloorChunks) {
			        Stack<FloorChunk> Stack = new Stack<FloorChunk>();
			        Stack.Push(Chunk);
                    int MinRoomSize = Mathf.Max(1, floorPlanConfig.MinRoomSize);
                    int MaxRoomSize = Mathf.Max(1, floorPlanConfig.MaxRoomSize);
                    MaxRoomSize = Mathf.Max(MinRoomSize, MaxRoomSize);

                    int MinArea = MinRoomSize * MinRoomSize;
			        int MaxArea = MaxRoomSize * MaxRoomSize;

			        while (Stack.Count > 0) {
				        FloorChunk Top = Stack.Pop();
				        bool bRequiresSplit = true;
				        int Area = Top.Area();
				        int Length = Top.GetLength();
				        if (Length > MaxRoomSize) {
					        // Length is too big. force a split
					        bRequiresSplit = true;
				        }
				        else if (Area <= MinArea) {
					        // This room is too small and should not be split
					        bRequiresSplit = false;
				        }
				        else if (Area <= MaxArea) {
					        float SplitProbability = (Area - MinArea) / (MaxArea - MinArea);
					        SplitProbability += floorPlanConfig.RoomSplitProbabilityOffset;
					        if (Chunk.GetLength() >= Chunk.GetWidth() * 2) {
						        SplitProbability += floorPlanConfig.RoomSplitProbabilityOffset;
					        }
					        bRequiresSplit = (random.Next() < SplitProbability);
				        }

				        if (bRequiresSplit) {
					        FloorChunk Left = ChunkDB.Create();
					        FloorChunk Right = ChunkDB.Create();
					        SplitChunk(Top, Left, Right);
					        Stack.Push(Left);
					        Stack.Push(Right);
				        }
				        else {
					        ChunkDB.Register(Top);
				        }
			        }
		        }

                // TODO: Add support for floor plan specific volumes
                // ...
	        }

	        ChunkDB.CacheChunkPositions();

	        for (int y = 0; y < floorPlanConfig.BuildingSize.y; y++) {
		        CreateDoors(y);
            }

            // Save the data to the model
            floorPlanModel.Chunks = ChunkDB.GetAllChunks();
        }
        
        void SplitChunk(FloorChunk Chunk, FloorChunk OutLeft, FloorChunk OutRight, FloorChunk OutHallway)
        {
            int HallWidth = floorPlanConfig.HallWidth;
	        int Length = Chunk.GetLength();
	        int RemainingLength = Length - HallWidth;
	        int MinChunkLength = floorPlanConfig.MinRoomSize;
	        int LengthLeft = MinChunkLength + random.Next(0, Mathf.Max(0, RemainingLength - MinChunkLength * 2 + 1));
	        int LengthRight = RemainingLength - LengthLeft;
	        SplitChunk(Chunk, LengthLeft, LengthRight, OutLeft, OutRight, OutHallway);
        }

        void SplitChunk(FloorChunk Chunk, float ratio, FloorChunk OutLeft, FloorChunk OutRight, FloorChunk OutHallway)
        {
	        int HallWidth = floorPlanConfig.HallWidth;
	        int Length = Chunk.GetLength();
	        int RemainingLength = Length - HallWidth;
	        int MinChunkLength = floorPlanConfig.MinRoomSize;
	        //int LengthLeft = MinChunkLength + random.Next(0, Mathf.Max(0, RemainingLength - MinChunkLength * 2 + 1));
	        int LengthLeft = Mathf.FloorToInt(ratio);
	        LengthLeft = Mathf.Max(MinChunkLength, LengthLeft);
	        int LengthRight = RemainingLength - LengthLeft;
	        
	        SplitChunk(Chunk, LengthLeft, LengthRight, OutLeft, OutRight, OutHallway);
        }

        void SplitChunk(FloorChunk Chunk, int LengthLeft, int LengthRight, FloorChunk OutLeft, FloorChunk OutRight, FloorChunk OutHallway)
        {
	        int HallWidth = floorPlanConfig.HallWidth;
	        OutLeft.Bounds = Chunk.Bounds;
	        OutLeft.ChunkType = FloorChunkType.Room;
	        OutLeft.SetLength(LengthLeft);

	        OutHallway.Bounds = Chunk.Bounds;
	        OutHallway.ChunkType = FloorChunkType.Hall;
	        OutHallway.OffsetAlongLength(LengthLeft);
	        OutHallway.SetLength(HallWidth);

	        OutRight.Bounds = Chunk.Bounds;
	        OutRight.ChunkType = FloorChunkType.Room;
	        OutRight.OffsetAlongLength(LengthLeft + HallWidth);
	        OutRight.SetLength(LengthRight);
	        
        }

        void SplitChunk(FloorChunk Chunk, FloorChunk OutLeft, FloorChunk OutRight)
        {
            int MinRoomLength = floorPlanConfig.MinRoomSize;
	        int Length = Chunk.GetLength();
            int LengthLeft = MinRoomLength + random.Next(0, Mathf.Max(0, Length - MinRoomLength * 2));
	        int LengthRight = Length - LengthLeft;
	        OutLeft.Bounds = Chunk.Bounds;
	        OutLeft.ChunkType = FloorChunkType.Room;
	        OutLeft.SetLength(LengthLeft);
	
	        OutRight.Bounds = Chunk.Bounds;
	        OutRight.OffsetAlongLength(LengthLeft);
	        OutRight.ChunkType = FloorChunkType.Room;
	        OutRight.SetLength(LengthRight);
        }
        void EmitMarkerAt(Vector3 WorldLocation, string MarkerName, Quaternion Rotation)
        {
	        Vector3 BasePosition = gameObject.transform.position;
	        Vector3 MarkerPosition = BasePosition + WorldLocation;
            Matrix4x4 Transform = Matrix4x4.TRS(MarkerPosition, Rotation, Vector3.one);
	        EmitMarker(MarkerName, Transform, IntVector.Zero, -1);
        }
        void EmitMarkerAt(Vector3 WorldLocation, string MarkerName, float Angle)
        {
	        EmitMarkerAt(WorldLocation, MarkerName, Quaternion.Euler(0, Angle, 0));
        }

	    bool VolumeEncompassesPoint(DungeonArchitect.Volume volume, IntVector GridPoint) {
            // TODO: Implement me
            return false;
        }
	    void GetVolumeCells(DungeonArchitect.Volume volume, int y, List<IntVector> OutCells) {
            OutCells.Clear();
            // TODO: Implement me
        }

        
        //////////////////////////// Door connection //////////////////////////
        static int GetChunkDoorConnectionScore(FloorChunk Chunk) {
	        if (Chunk == null) return -1000;
	        if (Chunk.bReachable) return -500;
	        if (!Chunk.bConnectDoors) return -1000;
	        if (Chunk.ChunkType == FloorChunkType.Hall) return 1000;
	        if (Chunk.ChunkType == FloorChunkType.Room) return 500;
	        return 0;
        }

        public class FloorIslandNode {
	        public FloorIslandNode() {
                IslandId = -1;
                Location = IntVector.Zero;
            }
	        public int IslandId;
	        public FloorChunk Chunk;
	        public IntVector Location;
        };

        void FloodFill(IntVector StartLocation, int IslandId, HashSet<IntVector> Visited, List<FloorIslandNode> IslandNodes, FloorChunkDB ChunkDB) {

	        FloorChunk PreferedChunk = ChunkDB.GetChunkAt(StartLocation);
	        if (!PreferedChunk.bConnectDoors) {
		        // We don't want doors here
		        return;
	        }
	        if (PreferedChunk == null) {
		        return;
	        }

	        Queue<IntVector> Queue = new Queue<IntVector>();
	        Queue.Enqueue(StartLocation);

	        while (Queue.Count > 0) {
		        IntVector Location = Queue.Dequeue();

		        FloorChunk CurrentChunk = ChunkDB.GetChunkAt(Location);
		        if (CurrentChunk != PreferedChunk) {
			        continue;
		        }

		        // Create a node here
		        FloorIslandNode Node = new FloorIslandNode();
		        Node.IslandId = IslandId;
		        Node.Chunk = CurrentChunk;
		        Node.Location = Location;

		        IslandNodes.Add(Node);

		        // Add the neighbors to the queue
		        List<IntVector> Neighbors = new List<IntVector>();
		        Neighbors.Add(Location + new IntVector(-1, 0, 0));
		        Neighbors.Add(Location + new IntVector(1, 0, 0));
		        Neighbors.Add(Location + new IntVector(0, 0, 1));
		        Neighbors.Add(Location + new IntVector(0, 0, -1));

		        foreach (IntVector Neighbor in Neighbors) {
			        if (Visited.Contains(Neighbor)) {
				        continue;
			        }
			        FloorChunk NeighborChunk = ChunkDB.GetChunkAt(Neighbor);
			        if (NeighborChunk != null && NeighborChunk.Id == CurrentChunk.Id) {
				        Queue.Enqueue(Neighbor);
				        Visited.Add(Neighbor);
			        }
		        }
	        }
        }

        class FloorIslandAdjacency {
	        public FloorIslandNode A;
	        public FloorIslandNode B;
        };

        class IslandNodePriorityPredicate : IComparer<int> {
	        public IslandNodePriorityPredicate(Dictionary<int, FloorChunk> InIslandToChunkMap) {
                IslandToChunkMap = InIslandToChunkMap; 
            }
            public int Compare(int IslandA, int IslandB)  
            {
                FloorChunk ChunkA = IslandToChunkMap.ContainsKey(IslandA) ? IslandToChunkMap[IslandA] : null;
		        FloorChunk ChunkB = IslandToChunkMap.ContainsKey(IslandB) ? IslandToChunkMap[IslandB] : null;
                
                int ScoreA = GetChunkDoorConnectionScore(ChunkA);
                int ScoreB = GetChunkDoorConnectionScore(ChunkB);
                if (ScoreA == ScoreB) return 0;
                return ScoreA < ScoreB ? 1 : -1;
            }

	        Dictionary<int, FloorChunk> IslandToChunkMap;

        };

        void ConnectIslandRecursive(int IslandId, Dictionary<int, List<FloorIslandAdjacency>> AdjacencyByIslands, 
		        HashSet<int> IslandVisited, System.Random random, FloorDoorManager DoorManager, Dictionary<int, FloorChunk> IslandToChunkMap) {

	        if (IslandVisited.Contains(IslandId)) {
		        return;
	        }
	        IslandVisited.Add(IslandId);

	        if (!AdjacencyByIslands.ContainsKey(IslandId)) {
		        // No adjacent islands
		        return;
	        }

	        List<FloorIslandAdjacency> AdjacentNodes = AdjacencyByIslands[IslandId];
	        HashSet<int> AdjacentIslands = new HashSet<int>();
	        foreach (FloorIslandAdjacency AdjacentNode in AdjacentNodes) {
		        AdjacentIslands.Add(AdjacentNode.A.IslandId);
		        AdjacentIslands.Add(AdjacentNode.B.IslandId);
	        }
	        AdjacentIslands.Remove(IslandId);
	        IslandNodePriorityPredicate SortPredicate = new IslandNodePriorityPredicate(IslandToChunkMap);
            int[] AdjacentIslandArray = new List<int>(AdjacentIslands).ToArray();
            System.Array.Sort(AdjacentIslandArray, SortPredicate);

	        foreach (int AdjacentIsland in AdjacentIslandArray) {
		        if (IslandVisited.Contains(AdjacentIsland)) {
			        continue;
		        }

		        // Find all the adjacent cells between these two islands
		        List<FloorIslandAdjacency> EdgeNodes = new List<FloorIslandAdjacency>();
		        foreach (FloorIslandAdjacency AdjacentNode in AdjacentNodes) {
			        if (AdjacentNode.A.IslandId == AdjacentIsland || AdjacentNode.B.IslandId == AdjacentIsland) {
				        EdgeNodes.Add(AdjacentNode);
			        }
		        }

		        // Connect a door in any one of the edge nodes
		        if (EdgeNodes.Count > 0) {
			        int Index = random.Next(0, EdgeNodes.Count);
			        FloorIslandAdjacency DoorEdge = EdgeNodes[Index];
			        // Create a door here
			        DoorManager.RegisterDoor(DoorEdge.A.Location, DoorEdge.B.Location);

			        // Move into this room now
			        ConnectIslandRecursive(AdjacentIsland, AdjacencyByIslands, IslandVisited, random, DoorManager, IslandToChunkMap);
		        }
	        }
        }
        

        void CreateDoors(int y)
        {
            // Tag all islands
	        // Create adjacency list
	        // Do a DFS on the tagged islands and connect the islands with doors

	        HashSet<IntVector> Visited = new HashSet<IntVector>();
	        List<FloorIslandNode> IslandNodes = new List<FloorIslandNode>();
	        int TotalIslands = 0;

	        // Tag islands with a flood fill.  This helps if custom volume split existing
	        // rooms into multiple parts and needs to be treated separately (islands)
	        {
		        int IslandId = 0;
		        for (int x = 0; x < floorPlanConfig.BuildingSize.x; x++) {
			        for (int z = 0; z < floorPlanConfig.BuildingSize.z; z++) {
				        IntVector Location = new IntVector(x, y, z);
				        if (!Visited.Contains(Location)) {
					        // Flood fill from here
					        Visited.Add(Location);
					        FloodFill(Location, IslandId, Visited, IslandNodes, ChunkDB);
					        IslandId++;
				        }
			        }
		        }
		        TotalIslands = IslandId;
	        }

	        // Create a node map for faster access
	        Dictionary<IntVector, FloorIslandNode> IslandNodeByLocation = new Dictionary<IntVector,FloorIslandNode>();
	        foreach (FloorIslandNode Node in IslandNodes) {
		        if (Node.IslandId == -1) continue;
		        IslandNodeByLocation.Add(Node.Location, Node);
	        }

	        // Create adjacency list for each island
	        List<FloorIslandAdjacency> AdjacencyList = new List<FloorIslandAdjacency>();
	        for (int x = 0; x < floorPlanConfig.BuildingSize.x; x++) {
		        for (int z = 0; z < floorPlanConfig.BuildingSize.z; z++) {
			        IntVector Loc00 = new IntVector(x, y, z);
			        if (!IslandNodeByLocation.ContainsKey(Loc00)) {
				        continue;
			        }
			        FloorIslandNode Node00 = IslandNodeByLocation[Loc00];
			        if (Node00.IslandId == -1) {
				        continue;
			        }
			
			        // Test along the left cell
			        {
				        IntVector Loc10 = new IntVector(x + 1, y, z);
				        if (IslandNodeByLocation.ContainsKey(Loc10)) {
					        FloorIslandNode Node10 = IslandNodeByLocation[Loc10];
					        if (Node10.IslandId != -1 && Node00.IslandId != Node10.IslandId) {
						        // Different adjacent nodes.  Add to the list
						        FloorIslandAdjacency Adjacency = new FloorIslandAdjacency();
						        Adjacency.A = Node00;
						        Adjacency.B = Node10;
						        AdjacencyList.Add(Adjacency);
					        }
				        }
			        }

			        // Test along the bottom cell
			        {
                        IntVector Loc01 = new IntVector(x, y, z + 1);
				        if (IslandNodeByLocation.ContainsKey(Loc01)) {
					        FloorIslandNode Node01 = IslandNodeByLocation[Loc01];
					        if (Node01.IslandId != -1 && Node00.IslandId != Node01.IslandId) {
						        // Different adjacent nodes.  Add to the list
						        FloorIslandAdjacency Adjacency = new FloorIslandAdjacency();
						        Adjacency.A = Node00;
						        Adjacency.B = Node01;
						        AdjacencyList.Add(Adjacency);
					        }
				        }
			        }
		        }
	        }

	        // Create another lookup for faster access
	        Dictionary<int, List<FloorIslandAdjacency>> AdjacencyByIsland = new Dictionary<int,List<FloorIslandAdjacency>>();
	        foreach (FloorIslandAdjacency Adjacency in AdjacencyList) {
		        int IslandA = Adjacency.A.IslandId;
		        int IslandB = Adjacency.B.IslandId;
		        if (!AdjacencyByIsland.ContainsKey(IslandA)) AdjacencyByIsland.Add(IslandA, new List<FloorIslandAdjacency>());
		        if (!AdjacencyByIsland.ContainsKey(IslandB)) AdjacencyByIsland.Add(IslandB, new List<FloorIslandAdjacency>());

		        AdjacencyByIsland[IslandA].Add(Adjacency);
		        AdjacencyByIsland[IslandB].Add(Adjacency);
	        }

	        Dictionary<int, FloorChunk> IslandToChunkMap = new Dictionary<int,FloorChunk>();
	        foreach (FloorIslandNode IslandNode in IslandNodes) {
		        if (IslandToChunkMap.ContainsKey(IslandNode.IslandId)) {
			        continue;
		        }
		        IslandToChunkMap.Add(IslandNode.IslandId, IslandNode.Chunk);
	        }

	        // Connect the islands to the main network with doors
	        HashSet<int> IslandVisited = new HashSet<int>();
	        for (int IslandId = 0; IslandId < TotalIslands; IslandId++) {
		        ConnectIslandRecursive(IslandId, AdjacencyByIsland, IslandVisited, random, DoorManager, IslandToChunkMap);
	        }
        }

        FloorChunk GetPriorityChunk(FloorChunk A, FloorChunk B)
        {
            if (A == null) return B;
            if (B == null) return A;
            return A.Priority > B.Priority ? A : B;
        }

        string GetDoorMarkerName(FloorChunk ChunkA, FloorChunk ChunkB)
        {
            if (ChunkA == null && ChunkB == null)
            {
                return FloorPlanMarkerNames.Door;
            }

            FloorChunk PreferedChunk;
            if (ChunkA == null)
            {
                PreferedChunk = ChunkB;
            }
            else if (ChunkB == null)
            {
                PreferedChunk = ChunkA;
            }
            else
            {
                PreferedChunk = (ChunkA.Priority > ChunkB.Priority) ? ChunkA : ChunkB;
            }

            return PreferedChunk.DoorMarker.Length > 0 ? PreferedChunk.DoorMarker : FloorPlanMarkerNames.Door;
        }

        /// <summary>
        /// Generate a layout and save it in the model
        /// </summary>
        void EmitBuildingMarkers()
        {
            floorPlanConfig = config as FloorPlanConfig;
            floorPlanModel = model as FloorPlanModel;

            ClearSockets();

	        //TArray<AFloorPlanDoorVolume*> DoorVolumes = UDungeonModelHelper::GetVolumes<AFloorPlanDoorVolume>(Dungeon)

	        ClearSockets();
	        int NumFloors = Mathf.RoundToInt(floorPlanConfig.BuildingSize.y);
	        Vector3 GridSize = floorPlanConfig.GridSize;
	        bool bBuildingWallLeft, bBuildingWallBottom;
	        for (int y = 0; y < NumFloors; y++) {
		        for (int x = -1; x < floorPlanConfig.BuildingSize.x; x++) {
			        bBuildingWallLeft = (x == -1 || x == floorPlanConfig.BuildingSize.x - 1);
			        for (int z = -1; z < floorPlanConfig.BuildingSize.z; z++) {
				        bBuildingWallBottom = (z == -1 || z == floorPlanConfig.BuildingSize.z - 1);
				        FloorChunk Chunk00 = ChunkDB.GetChunkAt(x, y, z);
				        FloorChunk Chunk10 = ChunkDB.GetChunkAt(x + 1, y, z);
				        FloorChunk Chunk01 = ChunkDB.GetChunkAt(x, y, z + 1);

				        string GroundMarkerName = FloorPlanMarkerNames.Ground;
				        string CeilingMarkerName = FloorPlanMarkerNames.Ceiling;

				        FloorChunk ChunkAbove = ChunkDB.GetChunkAt(x, y + 1, z);
				        FloorChunk ChunkBelow = ChunkDB.GetChunkAt(x, y - 1, z);
				        bool bEmitGroundMarker = (Chunk00 != ChunkBelow);
				        bool bEmitCeilingMarker = (Chunk00 != ChunkAbove);

				        // Emit the ground marker
				        if (Chunk00 != null && Chunk00.ChunkType != FloorChunkType.Outside) {
					        if (bEmitGroundMarker) {
						        Vector3 GridLocation = new Vector3(x + 0.5f, y, z + 0.5f);
						        Vector3 WorldLocation = Vector3.Scale(GridLocation, GridSize);
						        if (Chunk00.GroundMarker.Length > 0) {
							        GroundMarkerName = Chunk00.GroundMarker;
						        }
						        EmitMarkerAt(WorldLocation, GroundMarkerName, 0);
					        }
					        if (bEmitCeilingMarker) {
						        Vector3 GridLocation = new Vector3(x + 0.5f, y + 1, z + 0.5f);
						        Vector3 WorldLocation = Vector3.Scale(GridLocation, GridSize);
						        if (Chunk00.CeilingMarker.Length > 0) {
							        CeilingMarkerName = Chunk00.CeilingMarker;
						        }
						        EmitMarkerAt(WorldLocation, CeilingMarkerName, Quaternion.Euler(180, 0, 0));
					        }
				        }

				        int Chunk00Id = (Chunk00 != null ? Chunk00.Id : -1);
				        int Chunk10Id = (Chunk10 != null ? Chunk10.Id : -1);
				        int Chunk01Id = (Chunk01 != null ? Chunk01.Id : -1);

				        bool bEmitLeftWall = (Chunk00Id != Chunk10Id);
				        bool bEmitBottomWall = (Chunk00Id != Chunk01Id);
				        bool bLeftDoor = DoorManager.ContainsDoor(new IntVector(x, y, z), new IntVector(x + 1, y, z));
					        // || DoorManager.ContainsDoorVolume(Vector3.Scale(new Vector(x + 1, y, z + 0.5f) * GridSize), DoorVolumes);
				        bool bBottomDoor = DoorManager.ContainsDoor(new IntVector(x, y, z), new IntVector(x, y, z + 1));
					        // || DoorManager.ContainsDoorVolume(Vector3.Scale(new Vector(x + 0.5f, y, z + 1) * GridSize), DoorVolumes);

				        if (Chunk00 != null && Chunk10 != null && Chunk00.ChunkType == FloorChunkType.Hall && Chunk10.ChunkType == FloorChunkType.Hall) {
					        // Do not block the halls with a wall
					        bEmitLeftWall = false;
				        }
				        if (Chunk00 != null && Chunk01 != null && Chunk00.ChunkType == FloorChunkType.Hall && Chunk01.ChunkType == FloorChunkType.Hall) {
					        // Do not block the halls with a wall
					        bEmitBottomWall = false;
				        }

				        if (Chunk00 != null && Chunk10 != null && (!Chunk00.bEmitGroundMarker || !Chunk10.bEmitGroundMarker)) {
					        // We don't have ground in one of the adjacent chunks. Can't have doors
					        bLeftDoor = false;
				        }
				        if (Chunk00 != null && Chunk01 != null && (!Chunk00.bEmitGroundMarker || !Chunk01.bEmitGroundMarker)) {
					        // We don't have ground in one of the adjacent chunks. Can't have doors
					        bBottomDoor = false;
				        }

				        float wallAngleLeft = 0;
				        if (bEmitLeftWall) {
					        FloorChunk PriorityChunk = GetPriorityChunk(Chunk00, Chunk10);
					        bEmitLeftWall = PriorityChunk != null ? PriorityChunk.bCreateWalls : true;
					        wallAngleLeft = (PriorityChunk == Chunk10) ? 90 : -90;
				        }
				        
				        float wallAngleBottom = 0;
				        if (bEmitBottomWall) {
					        FloorChunk PriorityChunk = GetPriorityChunk(Chunk00, Chunk01);
					        bEmitBottomWall = PriorityChunk != null ? PriorityChunk.bCreateWalls : true;
					        wallAngleBottom = (PriorityChunk == Chunk01) ? 0 : 180;
				        }

				        if (bEmitLeftWall) {
					        Vector3 GridLocation = new Vector3(x + 1, y, z + 0.5f);
					        Vector3 WorldLocation = Vector3.Scale(GridLocation, GridSize);

					        string MarkerName;
					        if (bLeftDoor) {
						        MarkerName = GetDoorMarkerName(Chunk00, Chunk10);
					        } 
					        else {
						        MarkerName = FloorPlanMarkerNames.Wall;
						        if (bBuildingWallLeft) {
							        MarkerName = FloorPlanMarkerNames.BuildingWall;
						        }
						        else {
							        if (Chunk00 != null && Chunk10 != null) {
								        FloorChunk PriorityChunk = (Chunk00.Priority > Chunk10.Priority) ? Chunk00 : Chunk10;
								        if (PriorityChunk.WallMarker.Length > 0) {
									        MarkerName = PriorityChunk.WallMarker;
								        }
							        }
						        }
					        }

					        EmitMarkerAt(WorldLocation, MarkerName, wallAngleLeft);
				        }
				        if (bEmitBottomWall) {
					        Vector3 GridLocation = new Vector3(x + 0.5f, y, z + 1);
					        Vector3 WorldLocation = Vector3.Scale(GridLocation, GridSize);

					        string MarkerName;
					        if (bBottomDoor) {
						        MarkerName = GetDoorMarkerName(Chunk00, Chunk01);
					        }
					        else {
						        MarkerName = FloorPlanMarkerNames.Wall;
						        if (bBuildingWallBottom) {
							        MarkerName = FloorPlanMarkerNames.BuildingWall;
						        }
						        else {
							        if (Chunk00 != null && Chunk01 != null) {
								        FloorChunk PriorityChunk = (Chunk00.Priority > Chunk01.Priority) ? Chunk00 : Chunk01;
								        if (PriorityChunk.WallMarker.Length > 0) {
									        MarkerName = PriorityChunk.WallMarker;
								        }
							        }
						        }
					        }

					        EmitMarkerAt(WorldLocation, MarkerName, wallAngleBottom);
				        }
			        }
		        }
	        }


	        // Emit center marker if specified
	        List<FloorChunk> Chunks = new List<FloorChunk>();
	        ChunkDB.GetChunks(Chunks);
	        foreach (FloorChunk Chunk in Chunks) {
		        if (Chunk.bEmitGroundMarker && Chunk.CenterMarker.Length > 0) {
			        Vector3 ChunkSize = MathUtils.ToVector3(Chunk.Bounds.Size) / 2.0f;
			        ChunkSize.y = 0;
			        Vector3 GridLocation = MathUtils.ToVector3(Chunk.Bounds.Location) + ChunkSize;
			        Vector3 WorldLocation = Vector3.Scale(GridLocation, GridSize);
			        EmitMarkerAt(WorldLocation, Chunk.CenterMarker, 0);
		        }
	        }
        }
    }
}