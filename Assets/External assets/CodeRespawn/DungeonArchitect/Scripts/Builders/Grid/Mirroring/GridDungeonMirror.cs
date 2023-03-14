//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Builders.Grid.Mirroring
{
    public class GridDungeonMirrorAxisX : GridDungeonMirror
    {
        public GridDungeonMirrorAxisX(Vector3 mirrorBasePosition, Vector3 gridSize) 
            : base(mirrorBasePosition.x, gridSize.x)
        {
        }

        protected override Vector3 GetMirrorTangent()
        {
            return new Vector3(1, 0, 0);
        }

        protected override float GetComponent(Vector3 value)
        {
            return value.x;
        }

        protected override int GetComponent(IntVector value)
        {
            return value.x;
        }

        protected override void SetComponent(ref Vector3 v, float value)
        {
            v.x = value;
        }

        protected override void SetComponent(ref IntVector v, int value)
        {
            v.x = value;
        }
        
        protected override Vector3 CreateVector(Vector3 template, float value)
        {
            var v = template;
            v.x = value;
            return v;
        }

        protected override IntVector CreateVector(IntVector template, int value)
        {
            var v = template;
            v.x = value;
            return v;
        }

    }
    
    public class GridDungeonMirrorAxisZ : GridDungeonMirror
    {
        public GridDungeonMirrorAxisZ(Vector3 mirrorBasePosition, Vector3 gridSize) 
            : base(mirrorBasePosition.z, gridSize.z)
        {
        }

        protected override Vector3 GetMirrorTangent()
        {
            return new Vector3(0, 0, 1);
        }

        protected override float GetComponent(Vector3 value)
        {
            return value.z;
        }

        protected override int GetComponent(IntVector value)
        {
            return value.z;
        }

        protected override void SetComponent(ref Vector3 v, float value)
        {
            v.z = value;
        }

        protected override void SetComponent(ref IntVector v, int value)
        {
            v.z = value;
        }

        protected override Vector3 CreateVector(Vector3 template, float value)
        {
            var v = template;
            v.z = value;
            return v;
        }

        protected override IntVector CreateVector(IntVector template, int value)
        {
            var v = template;
            v.z = value;
            return v;
        }
    }

    
    public abstract class GridDungeonMirror
    {
        private readonly float mirrorBasePosition;
        private readonly float gridSize;

        protected abstract Vector3 GetMirrorTangent();
        protected abstract float GetComponent(Vector3 value);
        protected abstract int GetComponent(IntVector value);
        protected abstract void SetComponent(ref Vector3 v, float value);
        protected abstract void SetComponent(ref IntVector v, int value);
        protected abstract Vector3 CreateVector(Vector3 template, float value);
        protected abstract IntVector CreateVector(IntVector template, int value);
        
        public static GridDungeonMirror Create(Vector3 mirrorBasePosition, Vector3 gridSize, MirrorVolumeDirection direction)
        {
            switch (direction)
            {
                case MirrorVolumeDirection.AxisX:
                    return new GridDungeonMirrorAxisX(mirrorBasePosition, gridSize);

                case MirrorVolumeDirection.AxisZ:
                default:
                    return new GridDungeonMirrorAxisZ(mirrorBasePosition, gridSize);
            }
        }
        
        protected GridDungeonMirror(float mirrorBasePosition, float gridSize)
        {
            this.mirrorBasePosition = mirrorBasePosition;
            this.gridSize = gridSize;
        }

        int GetCutoff()
        {
            return Mathf.RoundToInt(mirrorBasePosition / gridSize);
        }
        
        public bool CanMergeCells(Rectangle a, Rectangle b, ref Rectangle mergedBounds)
        {
            int a0 = GetComponent(a.Location);
            int a1 = a0 + GetComponent(a.Size);

            int b0 = GetComponent(b.Location);
            int b1 = b0 + GetComponent(b.Size);

            if (a1 == b0 || b1 == a0)
            {
                var start = Mathf.Min(a0, b0);
                var end = Mathf.Max(a1, b1);
                var location = a.Location;
                var size = a.Size;
                SetComponent(ref location, start);
                SetComponent(ref size, end - start);
                mergedBounds = new Rectangle(location, size);
                return true;
            }
            
            return false;
        }

        public Rectangle CalculateMirrorReflection(Rectangle bounds)
        {
            var start = GetComponent(bounds.Location);
            var end = GetComponent(bounds.Location) + GetComponent(bounds.Size);

            var cutoff = GetCutoff();
            var distanceToCutoff = cutoff - end;
            var newX0 = cutoff + distanceToCutoff;
            return new Rectangle(CreateVector(bounds.Location, newX0), bounds.Size);
        }

        public bool CanDiscardBounds(Rectangle bounds)
        {
            var start = GetComponent(bounds.Location);
            var end = GetComponent(bounds.Location) + GetComponent(bounds.Size);

            int cutoff = GetCutoff();
            return (start >= cutoff && end >= cutoff);
        }

        public bool CanCropBounds(Rectangle bounds)
        {
            var x0 = GetComponent(bounds.Location);
            var x1 = GetComponent(bounds.Location) + GetComponent(bounds.Size);

            int cutoffX = GetCutoff();
            return (x0 < cutoffX && x1 > cutoffX);
        }

        public void CropCell(Cell cell)
        {
            int cutoffX = GetCutoff();
            var newWidth = cutoffX - GetComponent(cell.Bounds.Location);
            if (newWidth > 0)
            {
                var bounds = cell.Bounds;
                var size = bounds.Size;
                SetComponent(ref size, newWidth);
                bounds.Size = size;
                cell.Bounds = bounds;
            }
        }

        public Quaternion CalculateMirrorReflection(Quaternion rotation)
        {
            var mirrorTangent = GetMirrorTangent();
            var forward = rotation * Vector3.forward;

            var dot = Vector3.Dot(mirrorTangent, forward);
            dot = Mathf.Abs(dot);

            if (Mathf.Approximately(dot, 1.0f))
            {
                // The lines are parallel, do not rotate
                return rotation;
            }

            // rotate by 180
            return Quaternion.Euler(0, 180, 0) * rotation;
        }

        public Vector3 CalculateMirrorReflection(Vector3 position)
        {
            var cutoffX = GetCutoff() * gridSize;
            if (GetComponent(position) >= cutoffX)
            {
                Debug.Log("Invalid mirror input");
                return Vector3.zero;
            }

            var distanceToCutoff = cutoffX - GetComponent(position);
            var mirroredDistance = cutoffX + distanceToCutoff;

            var mirroredPosition = position;
            SetComponent(ref mirroredPosition, mirroredDistance);
            return mirroredPosition;
        }

        public IntVector CalculateMirrorReflection(IntVector position)
        {
            var cutoffX = GetCutoff();
            var distanceToCutoff = cutoffX - GetComponent(position);
            var mirroredDistance = cutoffX + distanceToCutoff - 1;

            var mirroredPosition = position;
            SetComponent(ref mirroredPosition, mirroredDistance);
            return mirroredPosition;
        }
    }

}