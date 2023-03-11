//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.SxEngine.Utils
{
    public struct SxTransform
    {
        private Vector3 _position;
        private Quaternion _rotation;
        private Vector3 _scale;
        private Matrix4x4 _matrix;
        private bool dirty;

        public static readonly SxTransform identity = new SxTransform()
        {
            _position = Vector3.zero,
            _rotation = Quaternion.identity,
            _scale = Vector3.one,
            _matrix = Matrix4x4.identity,
            dirty = false
        };

        public SxTransform(Vector3 position)
        {
            _position = position;
            _rotation = Quaternion.identity;
            _scale = Vector3.one;
            _matrix = Matrix4x4.identity;
            dirty = true;
        }
        
        public SxTransform(Quaternion rotation)
        {
            _position = Vector3.zero;
            _rotation = rotation;
            _scale = Vector3.one;
            _matrix = Matrix4x4.identity;
            dirty = true;
        }
        
        public SxTransform(Vector3 position, Quaternion rotation)
        {
            _position = position;
            _rotation = rotation;
            _scale = Vector3.one;
            _matrix = Matrix4x4.identity;
            dirty = true;
        }

        public SxTransform(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            _position = position;
            _rotation = rotation;
            _scale = scale;
            _matrix = Matrix4x4.identity;
            dirty = true;
        }
        
        public Vector3 Positon
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;
                    dirty = true;
                }
            }
        }
        
        public Quaternion Rotation
        {
            get => _rotation;
            set
            {
                if (_rotation != value)
                {
                    _rotation = value;
                    dirty = true;
                }
            }
        }
        
        public Vector3 Scale
        {
            get => _scale;
            set
            {
                if (_scale != value)
                {
                    _scale = value;
                    dirty = true;
                }
            }
        }

        public Matrix4x4 Matrix
        {
            get
            {
                if (dirty)
                {
                    _matrix = Matrix4x4.TRS(_position, _rotation, _scale);
                    dirty = false;
                }

                return _matrix;
            }
        }
    }
}