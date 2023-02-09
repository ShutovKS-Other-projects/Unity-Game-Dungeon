//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DungeonArchitect.Builders.Snap.SideScroller
{
    public class SnapSideScrollerBuilder : SnapBuilder
    {
        protected override Matrix4x4[] FindAttachmentTransforms(ref Matrix4x4 ParentModuleTransform, ref Matrix4x4 IncomingDoorTransform, ref Matrix4x4 AttachmentDoorTransform)
        {
            var result = new List<Matrix4x4>();

            // Calculate the translation
            {
                Vector3 DesiredOffset;
                Vector3 IncomingDoorPosition = Matrix.GetTranslation(ref IncomingDoorTransform);
                IncomingDoorPosition = ParentModuleTransform.MultiplyPoint3x4(IncomingDoorPosition);
                Vector3 ClampTarget = IncomingDoorPosition;

                Vector3 LocalDoorPosition = Matrix.GetTranslation(ref AttachmentDoorTransform);
                DesiredOffset = ClampTarget - LocalDoorPosition;
                result.Add(Matrix4x4.TRS(DesiredOffset, Quaternion.identity, Vector3.one));
            }

            // Calculate the translation
            {
                Vector3 DesiredOffset;
                Vector3 IncomingDoorPosition = Matrix.GetTranslation(ref IncomingDoorTransform);
                IncomingDoorPosition = ParentModuleTransform.MultiplyPoint3x4(IncomingDoorPosition);
                Vector3 ClampTarget = IncomingDoorPosition;

                Vector3 LocalDoorPosition = Matrix.GetTranslation(ref AttachmentDoorTransform);
                LocalDoorPosition.x *= -1;
                DesiredOffset = ClampTarget - LocalDoorPosition;
                result.Add(Matrix4x4.TRS(DesiredOffset, Quaternion.identity, new Vector3(-1, 1, 1)));
            }

            return result.ToArray();
        }

        protected override Bounds GetBounds(GameObject target)
        {
            var tilemap = target.GetComponentInChildren<Tilemap>();
            var grid = target.GetComponentInChildren<UnityEngine.Grid>();
            if (tilemap != null && grid != null)
            {
                var cellSize = grid.cellSize;
                var worldOrigin = Vector3.Scale(cellSize,tilemap.origin);
                
                var worldSize = Vector3.Scale(cellSize,tilemap.size);
                worldSize.z = 1;
                
                var worldCenter = worldOrigin + worldSize * 0.5f;
                worldCenter.z = 0;
                return new Bounds(worldCenter, worldSize);
            }
            return base.GetBounds(target);
        }
    }
}
