//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonArchitect.Utils;

namespace DungeonArchitect
{
    public class LevelMarkerList : IEnumerable<PropSocket>
    {
        protected List<PropSocket> markers = new List<PropSocket>();
        protected int _SocketIdCounter = 0;

        public IEnumerator<PropSocket> GetEnumerator()
        {
            return markers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return markers.GetEnumerator();
        }

        public virtual void Add(PropSocket marker)
        {
            markers.Add(marker);
        }

        public virtual void AddRange(PropSocket[] markerList)
        {
            markers.AddRange(markerList);
        }

        public virtual void Remove(PropSocket marker)
        {
            markers.Remove(marker);
        }

        public virtual void Clear()
        {
            _SocketIdCounter = 0;
            markers.Clear();
        }

        public int GetNextSocketId()
        {
            return ++_SocketIdCounter;
        }

        public PropSocket this[int index]
        {
            get
            {
                return markers[index];
            }
        }

        public int Count
        {
            get
            {
                return markers.Count;
            }
        }

        public virtual IEnumerable<PropSocket> GetMarkersInSearchArea(Vector2 center, float radius)
        {
            return markers;
        }

        public PropSocket EmitMarker(string SocketType, Matrix4x4 transform, IntVector gridPosition, int cellId)
        {
            return EmitMarker(SocketType, transform, gridPosition, cellId, null);
        }

        public PropSocket EmitMarker(string SocketType, Matrix4x4 transform, IntVector gridPosition, int cellId, object metadata)
        {
            PropSocket socket = new PropSocket();
            socket.Id = GetNextSocketId();
            socket.SocketType = SocketType;
            socket.Transform = transform;
            socket.gridPosition = gridPosition;
            socket.cellId = cellId;
            socket.metadata = metadata;
            Add(socket);
            return socket;
        }

        public void EmitMarker(string SocketType, Matrix4x4 _transform, int count, Vector3 InterOffset, IntVector gridPosition, int cellId, Vector3 LogicalToWorldScale)
        {
            EmitMarker(SocketType, _transform, count, InterOffset, gridPosition, cellId, LogicalToWorldScale, null);
        }

        public void EmitMarker(string SocketType, Matrix4x4 _transform, int count, Vector3 InterOffset, IntVector gridPosition, int cellId, Vector3 LogicalToWorldScale, object metadata)
        {
            var iposition = new IntVector(gridPosition.x, gridPosition.y, gridPosition.z);
            var ioffset = new IntVector(
                Mathf.RoundToInt(InterOffset.x / LogicalToWorldScale.x),
                Mathf.RoundToInt(InterOffset.y / LogicalToWorldScale.y),
                Mathf.RoundToInt(InterOffset.z / LogicalToWorldScale.z)
            );
            Matrix4x4 transform = Matrix.Copy(_transform);
            var position = Matrix.GetTranslation(ref transform);

            for (int i = 0; i < count; i++)
            {
                EmitMarker(SocketType, transform, iposition, cellId, metadata);
                position += InterOffset;
                iposition += ioffset;
                transform = Matrix.Copy(transform);
                Matrix.SetTranslation(ref transform, position);
            }
        }

    }

    public class SpatialPartionedLevelMarkerList : LevelMarkerList
    {
        private float partitionCellSize = 4.0f;
        private Dictionary<IntVector2, List<PropSocket>> buckets = new Dictionary<IntVector2, List<PropSocket>>();

        public SpatialPartionedLevelMarkerList(float partitionCellSize)
        {
            this.partitionCellSize = partitionCellSize;
        }

        IntVector2 GetBucketCoord(PropSocket marker)
        {
            var position = Matrix.GetTranslation(ref marker.Transform);
            return GetBucketCoord(position.x, position.z);
        }

        IntVector2 GetBucketCoord(Vector2 position)
        {
            return GetBucketCoord(position.x, position.y);
        }

        IntVector2 GetBucketCoord(float x, float z)
        {
            int ix = Mathf.FloorToInt(x / partitionCellSize);
            int iy = Mathf.FloorToInt(z / partitionCellSize);
            return new IntVector2(ix, iy);
        }

        public override void Add(PropSocket marker)
        {
            base.Add(marker);

            var partitionCoord = GetBucketCoord(marker);
            if (!buckets.ContainsKey(partitionCoord))
            {
                buckets.Add(partitionCoord, new List<PropSocket>());
            }
            buckets[partitionCoord].Add(marker);
        }

        public override void Remove(PropSocket marker)
        {
            base.Remove(marker);

            var partitionCoord = GetBucketCoord(marker);
            if (buckets.ContainsKey(partitionCoord))
            {
                buckets[partitionCoord].Remove(marker);
            }
        }

        public override IEnumerable<PropSocket> GetMarkersInSearchArea(Vector2 center, float radius)
        {
            var extent = new Vector2(radius, radius);
            var start = GetBucketCoord(center - extent);
            var end = GetBucketCoord(center + extent);

            var searchSpace = new List<PropSocket>();
            for (int x = start.x; x <= end.x; x++)
            {
                for (int y = start.y; y <= end.y; y++)
                {
                    var key = new IntVector2(x, y);
                    if (buckets.ContainsKey(key))
                    {
                        searchSpace.AddRange(buckets[key]);
                    }
                }
            }
            return searchSpace;
        }

        public override void Clear()
        {
            base.Clear();
            buckets.Clear();
        }
    }
}
