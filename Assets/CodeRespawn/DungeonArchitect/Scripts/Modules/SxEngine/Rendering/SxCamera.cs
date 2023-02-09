//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.SxEngine
{
    public class SxCamera
    {
        public Vector3 Location
        {
            get => location;
            set
            {
                if (location != value)
                {
                    location = value;
                    dirty = true;
                }
            } 
        }
        public Quaternion Rotation
        {
            get => rotation;
            set
            {
                if (rotation != value)
                {
                    rotation = value;
                    dirty = true;
                }
            } 
        }

        public Vector3 GetRightVector()
        {
            var axisZ = Rotation * Vector3.forward;
            return Vector3.Cross(Vector3.up, axisZ);
        }

        public Matrix4x4 ViewMatrix
        {
            get
            {
                if (dirty)
                {
                    BuildViewMatrix();
                }

                return viewMatrix;
            }
        }

        private Vector3 location = Vector3.zero;
        private Quaternion rotation = Quaternion.identity;
        private Matrix4x4 viewMatrix = Matrix4x4.identity;
        private bool dirty = true;

        public void LookAt(Vector3 target)
        {
            Rotation = Quaternion.LookRotation((location - target).normalized);
        }

        void BuildViewMatrix()
        {
            var baseTransform = new Matrix4x4(
                new Vector4(-1, 0, 0, 0),
                new Vector4(0, 1, 0, 0),
                new Vector4(0, 0, 1, 0),
                new Vector4(0, 0, 0, 1));
            var camTransform = Matrix4x4.TRS(location, rotation, Vector3.one) * baseTransform;
            viewMatrix = camTransform.inverse;
            dirty = false;
        }
    }
}