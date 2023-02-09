//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace DungeonArchitect.Builders.FloorPlan
{
    public class FloorPlanModel : DungeonModel
    {
        [HideInInspector]
        public FloorPlanConfig Config;

        [HideInInspector]
        public FloorChunk[] Chunks;
    }

    public enum FloorChunkType
    {
	    Room,
	    Hall,
	    Outside	// Outside the floorplan
    };


    public class FloorChunk {
	    public FloorChunk()
	    {
            Id = -1;
		    ChunkType = FloorChunkType.Outside;
		    bReachable = false;
		    bConnectDoors = true;
		    bCreateWalls = true;
		    Priority = 0;
		    bEmitGroundMarker = true;
            bEmitCeilingMarker = true;
        }

	    public int Id;
	    public FloorChunkType ChunkType;
	    public Rectangle Bounds = new Rectangle();
	    public List<IntVector> BoundCells = new List<IntVector>();

	    public bool bReachable;
	    public float Priority;

	    public bool bEmitGroundMarker;
	    public bool bEmitCeilingMarker;
	    public bool bConnectDoors;
	    public bool bCreateWalls;

	    public string WallMarker = "";
        public string GroundMarker = "";
        public string CeilingMarker = "";
        public string DoorMarker = "";
        public string CenterMarker = "";

	    public int Area() { return Bounds.Size.x * Bounds.Size.z; }
	    /** Gets the dimension of the largest side */
	    public int GetLength() { return Bounds.Size.x > Bounds.Size.z ? Bounds.Size.x : Bounds.Size.z; }
	    public int GetWidth() { return Bounds.Size.x < Bounds.Size.z ? Bounds.Size.x : Bounds.Size.z; }

	    /** Sets the dimensions of the largest side */
	    public void SetLength(int Length) {
            IntVector Size = Bounds.Size;
		    if (Bounds.Size.x > Bounds.Size.z) {
			    Size.x = Length;
		    }
		    else {
			    Size.z = Length;
		    }
            Bounds.Size = Size;
	    }

	    public void OffsetAlongLength(int Offset) {
            IntVector Location = Bounds.Location;
		    if (Bounds.Size.x > Bounds.Size.z) {
			    Location.x += Offset;
		    }
		    else {
			    Location.z += Offset;
		    }
            Bounds.Location = Location;
	    }
    };



    public class FloorChunkDB
    {
    
	    public FloorChunkDB() {
            IdCounter = 0;
        }
        public FloorChunk Create()
        {
            var Chunk = new FloorChunk();
            Chunk.Id = ++IdCounter;
            return Chunk;
        }

        public void Register(FloorChunk Chunk)
        {
            if (!Chunks.ContainsKey(Chunk.Id))
            {
                Chunks.Add(Chunk.Id, Chunk);
            }
        }

        public void GetChunks(List<FloorChunk> OutChunks)
        {
            OutChunks.Clear();
            OutChunks.AddRange(Chunks.Values);
        }
        public void GetChunks(List<FloorChunk> OutChunks, FloorChunkType ChunkType)
        {
            var AllChunks = new List<FloorChunk>();
            GetChunks(AllChunks);

            OutChunks.Clear();
            foreach (var Chunk in Chunks.Values)
            {
                if (Chunk.ChunkType == ChunkType)
                {
                    OutChunks.Add(Chunk);
                }
            }
        }

        public FloorChunk GetChunk(int Id)
        {
            return Chunks.ContainsKey(Id) ? Chunks[Id] : null;
        }


        public FloorChunk GetChunkAt(int x, int y, int z)
        {
            int Hash = HASH(x, y, z);
            if (CachePositionToChunk.ContainsKey(Hash))
            {
                int ChunkId = CachePositionToChunk[Hash].ChunkId;
                return GetChunk(ChunkId);
            }
            else
            {
                return null;
            }
        }

        public FloorChunk GetChunkAt(IntVector Location)
        {
            return GetChunkAt(Location.x, Location.y, Location.z);
        }

	    /** Create the cache for faster access */
        public void CacheChunkPositions()
        {
            
	        List<FloorChunk> Chunks = new List<FloorChunk>();
	        GetChunks(Chunks);

	        foreach (FloorChunk Chunk in Chunks) {
		        int X0 = Chunk.Bounds.Location.x;
		        int Z0 = Chunk.Bounds.Location.z;
		        int X1 = X0 + Chunk.Bounds.Size.x;
		        int Z1 = Z0 + Chunk.Bounds.Size.z;
		        int y = Chunk.Bounds.Location.y;
		        List<IntVector> BoundCells = Chunk.BoundCells;
		        if (BoundCells.Count == 0) {
			        for (int x = X0; x < X1; x++) {
				        for (int z = Z0; z < Z1; z++) {
					        BoundCells.Add(new IntVector(x, y, z));
				        }
			        }
		        }

		        foreach (IntVector Cell in BoundCells) {
			        int Hash = HASH(Cell.x, Cell.y, Cell.z);
			        if (!CachePositionToChunk.ContainsKey(Hash)) {
				        CachePositionToChunk.Add(Hash, new FChunkCacheNode(Chunk.Id, Chunk.Priority));
			        }
			        else {
				        // Entry already exists.  Override if the priority is higher
				        FChunkCacheNode Node = CachePositionToChunk[Hash];
				        if (Node.Priority < Chunk.Priority) {
					        Node.ChunkId = Chunk.Id;
					        Node.Priority = Chunk.Priority;
				        }
			        }
		        }
	        }
        }

    
	    public int HASH(int x, int y, int z) {
		    return new IntVector(x, y, z).GetHashCode();
		    //return  (x + 16384) << 16 | (y + 16384); 
	    }

	    class FChunkCacheNode {
		    public FChunkCacheNode() {
                ChunkId = -1;
                Priority = 0;
            }
            public FChunkCacheNode(int InChunkId, float InPriority)
            {
                ChunkId = InChunkId;
                Priority = InPriority;
            }

		    public int ChunkId;
            public float Priority;
	    };

        public FloorChunk[] GetAllChunks()
        {
            return Chunks.Values.ToArray();
        }

        private Dictionary<int, FloorChunk> Chunks = new Dictionary<int,FloorChunk>();
        private Dictionary<int, FChunkCacheNode> CachePositionToChunk = new Dictionary<int,FChunkCacheNode>();
        private int IdCounter;
    }


    public class FloorDoorManager
    {

        public void RegisterDoor(IntVector A, IntVector B)
        {
            if (!DoorMap.ContainsKey(A))
            {
                DoorMap.Add(A, new HashSet<IntVector>());
            }
            DoorMap[A].Add(B);

            if (!DoorMap.ContainsKey(B))
            {
                DoorMap.Add(B, new HashSet<IntVector>());
            }
            DoorMap[B].Add(A);
        }

        public bool ContainsDoorVolume(Vector3 WorldLocation, List<DungeonArchitect.Volume> DoorVolumes)
        {
            // TODO: Implement me
            return false;
        }

	    public bool ContainsDoor(IntVector A, IntVector B) {
		    if (!DoorMap.ContainsKey(A)) {
			    return false;
		    }
		    return DoorMap[A].Contains(B);
	    }

	    public void Clear() {
		    DoorMap.Clear();
	    }

        private Dictionary<IntVector, HashSet<IntVector>> DoorMap = new Dictionary<IntVector,HashSet<IntVector>>();
    }

}