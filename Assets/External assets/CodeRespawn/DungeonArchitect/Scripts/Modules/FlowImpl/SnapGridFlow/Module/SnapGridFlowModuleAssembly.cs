//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Domains.Layout.Pathing;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Flow.Impl.SnapGridFlow
{
    
    // Snap Grid Flow Module Assembly Structures
    [System.Serializable]
    public struct SgfModuleAssemblySideCell
    {
        public SgfModuleAssemblySideCell(int connectionIdx)
        {
            this.connectionIdx = connectionIdx;
            this.nodeId = DungeonUID.Empty;
            this.linkedNodeId = DungeonUID.Empty;
            this.linkId = DungeonUID.Empty;
        }

        public static readonly SgfModuleAssemblySideCell Empty = new SgfModuleAssemblySideCell()
        {
            connectionIdx = -1,
            nodeId = DungeonUID.Empty,
            linkedNodeId = DungeonUID.Empty,
            linkId = DungeonUID.Empty
        };
        
        public int connectionIdx;
        public DungeonUID nodeId;
        public DungeonUID linkedNodeId;
        public DungeonUID linkId;

        public bool HasConnection() { return connectionIdx != -1; }
    }
    
    [System.Serializable]
    public class SgfModuleAssemblySide {
        [SerializeField]
        public int width = 0;
        
        [SerializeField]
        public int height = 0;
        
        [SerializeField]
        public SgfModuleAssemblySideCell[] connectionIndices;

        public void Init(int width, int height)
        {
            this.width = width;
            this.height = height;
            
            this.connectionIndices = new SgfModuleAssemblySideCell[width * height];
            for (var i = 0; i < connectionIndices.Length; i++)
            {
                connectionIndices[i] = SgfModuleAssemblySideCell.Empty;
            }
        }

        public void SetZeroValues()
        {
            for (var i = 0; i < connectionIndices.Length; i++)
            {
                connectionIndices[i] = SgfModuleAssemblySideCell.Empty;
            }
        }
        
        public SgfModuleAssemblySide Clone()
        {
            var clone = new SgfModuleAssemblySide();
            clone.width = width;
            clone.height = height;
            clone.connectionIndices = new SgfModuleAssemblySideCell[width * height];
            for (var i = 0; i < connectionIndices.Length; i++)
            {
                clone.connectionIndices[i] = connectionIndices[i];
            }

            return clone;
        }
        
        public SgfModuleAssemblySideCell Get(int x, int y) {
            Debug.Assert(IsCoordValid(x, y));
            return connectionIndices[y * width + x];
        }

        public bool Set(int x, int y, SgfModuleAssemblySideCell cell)
        {
            if (!IsCoordValid(x, y)) return false;
            
            connectionIndices[y * width + x] = cell;
            return true;
        }

        public bool IsCoordValid(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }

        public SgfModuleAssemblySide Rotate90Cw()
        {
            var rotatedSide = new SgfModuleAssemblySide();
            rotatedSide.Init(height, width);
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    var cell = Get(x, y);
                    int xx = height - 1 - y;
                    int yy = x;
                    rotatedSide.Set(xx, yy, cell);
                }
            }
    
            return rotatedSide;
        }
        
        public static readonly int IndexValidUnknown = -2;
    };

    [System.Serializable]
    public class SgfModuleAssembly
    {
        public Vector3Int numChunks;
        public SgfModuleAssemblySide front = new SgfModuleAssemblySide();
        public SgfModuleAssemblySide left = new SgfModuleAssemblySide();
        public SgfModuleAssemblySide back = new SgfModuleAssemblySide();
        public SgfModuleAssemblySide right = new SgfModuleAssemblySide();
        public SgfModuleAssemblySide top = new SgfModuleAssemblySide();
        public SgfModuleAssemblySide down = new SgfModuleAssemblySide();

        public void Initialize(Vector3Int numChunks)
        {
            this.numChunks = numChunks;
            front.Init(this.numChunks.x, this.numChunks.y);
            left.Init(this.numChunks.z, this.numChunks.y);
            back.Init(this.numChunks.x, this.numChunks.y);
            right.Init(this.numChunks.z, this.numChunks.y);

            top.Init(this.numChunks.x, this.numChunks.z);
            down.Init(this.numChunks.x, this.numChunks.z);
        }

        public void SetZeroValues()
        {
            front.SetZeroValues();
            left.SetZeroValues();
            back.SetZeroValues();
            right.SetZeroValues();

            top.SetZeroValues();
            down.SetZeroValues();
        }
        

        public bool CanFit(SgfModuleAssembly assemblyToFit, out SgfModuleAssemblySideCell[] outDoorIndices)
        {
            if (numChunks != assemblyToFit.numChunks)
            {
                outDoorIndices = new SgfModuleAssemblySideCell[0];
                return false;
            }
            
            var hostSides = new SgfModuleAssemblySide[] { front, left, back, right, top, down };
            var targetSides = new SgfModuleAssemblySide[] { assemblyToFit.front, assemblyToFit.left, assemblyToFit.back, assemblyToFit.right, assemblyToFit.top, assemblyToFit.down };

            var doorIndices = new List<SgfModuleAssemblySideCell>();
            for (int s = 0; s < 6; s++) {
                var hostSide = hostSides[s];
                var targetSide = targetSides[s];
                Debug.Assert(hostSide.width == targetSide.width && hostSide.height == targetSide.height);
                var numEntries = hostSide.connectionIndices.Length;
                for (int i = 0; i < numEntries; i++) {
                    bool bTargetRequiresConnection = targetSide.connectionIndices[i].HasConnection();
                    if (bTargetRequiresConnection) {
                        bool bHostContainsConnection = hostSide.connectionIndices[i].HasConnection();
                        if (bHostContainsConnection) {
                            var doorCell = new SgfModuleAssemblySideCell();
                            doorCell.connectionIdx = hostSide.connectionIndices[i].connectionIdx;
                            doorCell.nodeId = targetSide.connectionIndices[i].nodeId;
                            doorCell.linkedNodeId = targetSide.connectionIndices[i].linkedNodeId;
                            doorIndices.Add(doorCell);
                        }
                        else {
                            outDoorIndices = new SgfModuleAssemblySideCell[0];
                            return false;
                        }
                    }
                }
            }

            outDoorIndices = doorIndices.ToArray();
            return true;
        }
    }
    
    
    public class SGFModuleAssemblyBuilder {
        
        enum EAssemblySide  { Unknown, Front, Left, Back, Right, Down, Top };

        class AssemblyDoorPositions
        {
            public EAssemblySide Side;
            public Vector3 LocalPosition;
            public IntVector2 Coord;

            public AssemblyDoorPositions(EAssemblySide side, Vector3 localPosition, IntVector2 coord)
            {
                this.Side = side;
                this.LocalPosition = localPosition;
                this.Coord = coord;
            }
        }
        
        public static void Build(SnapGridFlowModuleBounds moduleBoundsAsset, SgfModuleDatabaseItem moduleInfo, out SgfModuleAssembly outAssembly)
        {
            var chunkSize = moduleBoundsAsset.chunkSize;
            var numChunks = moduleInfo.NumChunks;
            outAssembly = new SgfModuleAssembly();
            outAssembly.Initialize(numChunks);
            
            Vector3 baseOffset = -moduleInfo.ModuleBounds.min;
            for (int connectionIdx = 0; connectionIdx < moduleInfo.Connections.Length; connectionIdx++) {
                var connectionInfo = moduleInfo.Connections[connectionIdx];
                Vector3 connectionLocation = Matrix.GetTranslation(ref connectionInfo.Transform) + baseOffset;
                
                var doorPositions = GetDoorPositions(moduleBoundsAsset.chunkSize, moduleBoundsAsset.doorOffsetY, moduleInfo.NumChunks);

                AssemblyDoorPositions bestDoor = null;
                float bestDistance = float.MaxValue;
                foreach (var doorInfo in doorPositions) {
                    // Check if the connection is aligned with this door's direction
                    
                    
                    float distance = (doorInfo.LocalPosition - connectionLocation).magnitude;
                    if (distance < bestDistance) {
                        bestDistance = distance;
                        bestDoor = doorInfo;
                    }
                }

                var connectionCell = new SgfModuleAssemblySideCell(connectionIdx);
                if (bestDoor != null)
                {
                    var cx = bestDoor.Coord.x;
                    var cy = bestDoor.Coord.y;
                    var bestSide = bestDoor.Side;
                    if (bestSide == EAssemblySide.Front)
                    {
                        if (!outAssembly.front.Set(cx, cy, connectionCell))
                        {
                            Debug.LogError("Failed to register connection on module side: FRONT");
                        }
                        //Debug.Log("FRONT: " + string.Format("Location ({0}, {1})", cx, cy));
                    }
                    else if (bestSide == EAssemblySide.Left)
                    {
                        if (!outAssembly.left.Set(cx, cy, connectionCell))
                        {
                            Debug.LogError("Failed to register connection on module side: LEFT");
                        }
                        //Debug.Log("LEFT: " + string.Format("Location ({0}, {1})", cx, cy));
                    }
                    else if (bestSide == EAssemblySide.Back)
                    {
                        if (!outAssembly.back.Set(cx, cy, connectionCell))
                        {
                            Debug.LogError("Failed to register connection on module side: BACK");
                        }
                        //Debug.Log("BACK: " + string.Format("Location ({0}, {1})", cx, cy));
                    }
                    else if (bestSide == EAssemblySide.Right)
                    {
                        if (!outAssembly.right.Set(cx, cy, connectionCell))
                        {
                            Debug.LogError("Failed to register connection on module side: RIGHT");
                        }
                        //Debug.Log("RIGHT: " + string.Format("Location ({0}, {1})", cx, cy));
                    }
                    else if (bestSide == EAssemblySide.Down)
                    {
                        if (!outAssembly.down.Set(cx, cy, connectionCell))
                        {
                            Debug.LogError("Failed to register connection on module side: DOWN");
                        }
                        //Debug.Log("DOWN: " + string.Format("Location ({0}, {1})", cx, cy));
                    }
                    else if (bestSide == EAssemblySide.Top)
                    {
                        if (!outAssembly.top.Set(cx, cy, connectionCell))
                        {
                            Debug.LogError("Failed to register connection on module side: TOP");
                        }
                        //Debug.Log("TOP: " + string.Format("Location ({0}, {1})", cx, cy));
                    }
                }
            }
        }

        static AssemblyDoorPositions[] GetDoorPositions(Vector3 chunkSize, float offsetY, Vector3Int numChunks)
        {
            var doorPositions = new List<AssemblyDoorPositions>();
            
            // Process along the X-axis
            {
                for (int x = 0; x < numChunks.x; x++)
                {
                    for (int y = 0; y < numChunks.y; y++)
                    {
                        var coordFront = new Vector3(x + 0.5f, y, 0);               // Front
                        var coordBack = new Vector3(x + 0.5f, y, numChunks.z);        // Back
                        var rotation = Quaternion.identity;

                        var doorPosFront = Vector3.Scale(coordFront, chunkSize);
                        var doorPosBack = Vector3.Scale(coordBack, chunkSize);

                        doorPosFront.y += offsetY;
                        doorPosBack.y += offsetY;

                        var icoordFront = new IntVector2(x, y);
                        var icoordBack = new IntVector2(numChunks.x - 1 - x, y);
                        
                        doorPositions.Add(new AssemblyDoorPositions(EAssemblySide.Front, doorPosFront, icoordFront));
                        doorPositions.Add(new AssemblyDoorPositions(EAssemblySide.Back, doorPosBack, icoordBack));
                    }
                }
            }

            // Process along the Z-axis
            {
                for (int z = 0; z < numChunks.z; z++)
                {
                    for (int y = 0; y < numChunks.y; y++)
                    {
                        var coordRight = new Vector3(0, y, z + 0.5f);           // Right
                        var coordLeft = new Vector3(numChunks.x, y, z + 0.5f);    // Left
                        var rotation = Quaternion.identity;

                        var doorPosRight = Vector3.Scale(coordRight, chunkSize);
                        var doorPosLeft = Vector3.Scale(coordLeft, chunkSize);

                        doorPosRight.y += offsetY;
                        doorPosLeft.y += offsetY;

                        var icoordRight = new IntVector2(numChunks.z - 1 - z, y);
                        var icoordLeft = new IntVector2(z, y);
                        
                        doorPositions.Add(new AssemblyDoorPositions(EAssemblySide.Right, doorPosRight, icoordRight));
                        doorPositions.Add(new AssemblyDoorPositions(EAssemblySide.Left, doorPosLeft, icoordLeft));
                    }
                }
            }

            // Process along the Y-axis
            {
                for (int x = 0; x < numChunks.x; x++)
                {
                    for (int z = 0; z < numChunks.z; z++)
                    {
                        var coordDown = new Vector3(x + 0.5f, 0, z + 0.5f);            // Down
                        var coordTop = new Vector3(x + 0.5f, numChunks.y, z + 0.5f);    // Top
                        var rotation = Quaternion.identity;

                        var doorPosDown = Vector3.Scale(coordDown, chunkSize);
                        var doorPosTop = Vector3.Scale(coordTop, chunkSize);

                        var icoordDown = new IntVector2(x, z);
                        var icoordTop = new IntVector2(x, z);
                        
                        doorPositions.Add(new AssemblyDoorPositions(EAssemblySide.Down, doorPosDown, icoordDown));
                        doorPositions.Add(new AssemblyDoorPositions(EAssemblySide.Top, doorPosTop, icoordTop));
                    }
                }
            }

            return doorPositions.ToArray();
        }

        private static readonly Dictionary<Vector3Int, SgfModuleAssembly> AssemblyInstanceBySize = new Dictionary<Vector3Int, SgfModuleAssembly>();
        private static SgfModuleAssemblySideCell _cellCache = new SgfModuleAssemblySideCell();
        
        public static void Build(FlowLayoutGraphQuery graphQuery, FlowLayoutPathNodeGroup group, FFAGConstraintsLink[] incomingNodes, out SgfModuleAssembly outAssembly)
        {
            var minCoordF = new Vector3();
            var maxCoordF = new Vector3();
            
            for (int i = 0; i < group.GroupNodes.Count; i++) {
                var nodeId = group.GroupNodes[i];
                var node = graphQuery.GetNode(nodeId);
                if (node == null) {
                    node = graphQuery.GetSubNode(nodeId);
                }
                Debug.Assert(node != null);
                
                if (i == 0) {
                    minCoordF = maxCoordF = node.coord; 
                }
                else {
                    minCoordF = MathUtils.ComponentMin(minCoordF, node.coord);
                    maxCoordF = MathUtils.ComponentMax(maxCoordF, node.coord);
                }
            }

            var minCoord = MathUtils.RoundToVector3Int(minCoordF);
            var maxCoord = MathUtils.RoundToVector3Int(maxCoordF);

            var numChunks = maxCoord - minCoord + new Vector3Int(1, 1, 1);
            
            if (AssemblyInstanceBySize.ContainsKey(numChunks))
            {
                outAssembly = AssemblyInstanceBySize[numChunks];
                outAssembly.SetZeroValues();
            }
            else
            {
                outAssembly = new SgfModuleAssembly();
                outAssembly.Initialize(numChunks);
                AssemblyInstanceBySize.Add(numChunks, outAssembly);
            }

            void SetAssemblyCell(SgfModuleAssemblySide side, int x, int y, SgfModuleAssemblySideCell cell, string sideText)
            {
                if (!side.Set(x, y, cell))
                {
                    Debug.LogError("Failed to register connection on module side: " + sideText);
                }
            }
            
            foreach (var link in incomingNodes) {
                if (link.IncomingNode == null) continue;
                var c = MathUtils.RoundToVector3Int(link.Node.coord) - minCoord;
                var ic = MathUtils.RoundToVector3Int(link.IncomingNode.coord) - minCoord;
                
                //var cellCache = new SgfModuleAssemblySideCell();     // TODO: Do not create a new object, instead overwrite it
                _cellCache.connectionIdx = SgfModuleAssemblySide.IndexValidUnknown;
                _cellCache.nodeId = (link.Node != null) ? link.Node.nodeId : DungeonUID.Empty;
                _cellCache.linkedNodeId = (link.IncomingNode != null) ? link.IncomingNode.nodeId : DungeonUID.Empty;
                _cellCache.linkId = link.Link.linkId;

                if (c.z > ic.z) {
                    // Front
                    SetAssemblyCell(outAssembly.front, c.x, c.y, _cellCache, "FRONT");
                    /*
                    if (!outAssembly.front.Set(c.x, c.y, _cellCache))
                    {
                        Debug.LogError("Failed to register connection on module side: FRONT");
                    }
                    */
                }
                else if (ic.x > c.x) {
                    // Left
                    SetAssemblyCell(outAssembly.left, c.z, c.y, _cellCache, "LEFT");
                    /*
                    if (!outAssembly.left.Set(c.z, c.y, _cellCache))
                    {
                        Debug.LogError("Failed to register connection on module side: LEFT");
                    }
                    */
                }
                else if (ic.z > c.z) {
                    // Back
                    SetAssemblyCell(outAssembly.back, numChunks.x - 1 - c.x, c.y, _cellCache, "BACK");
                    /*
                    if (!outAssembly.back.Set(numChunks.x - 1 - c.x, c.y, _cellCache))
                    {
                        Debug.LogError("Failed to register connection on module side: BACK");
                    }
                    */
                }
                else if (c.x > ic.x) {
                    // Right
                    SetAssemblyCell(outAssembly.right, numChunks.z - 1 - c.z, c.y, _cellCache, "RIGHT");
                    /*
                    if (!outAssembly.right.Set(numChunks.z - 1 - c.z, c.y, _cellCache))
                    {
                        Debug.LogError("Failed to register connection on module side: RIGHT");
                    }
                    */
                }
                else if (c.y > ic.y) {
                    // Down
                    SetAssemblyCell(outAssembly.down, c.x, c.z, _cellCache, "DOWN");
                    /*
                    if (!outAssembly.down.Set(c.x, c.z, _cellCache))
                    {
                        Debug.LogError("Failed to register connection on module side: DOWN");
                    }
                    */
                }
                else if (ic.y > c.y) {
                    // Top
                    SetAssemblyCell(outAssembly.top, c.x, c.z, _cellCache, "TOP");
                    /*
                    if (!outAssembly.top.Set(c.x, c.z, _cellCache))
                    {
                        Debug.LogError("Failed to register connection on module side: TOP");
                    }
                    */
                }
            }
        }

        public static void Rotate90Cw(SgfModuleAssembly inAssembly, out SgfModuleAssembly outAssembly)
        {
            outAssembly = new SgfModuleAssembly();
            outAssembly.numChunks = new Vector3Int(inAssembly.numChunks.z, inAssembly.numChunks.y, inAssembly.numChunks.x);
            outAssembly.left = inAssembly.front.Clone();
            outAssembly.back = inAssembly.left.Clone();
            outAssembly.right = inAssembly.back.Clone();
            outAssembly.front = inAssembly.right.Clone();
            outAssembly.top = inAssembly.top.Rotate90Cw();
            outAssembly.down = inAssembly.down.Rotate90Cw();
        }
    }

}