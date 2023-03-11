//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Frameworks.Snap
{
    public class SnapUtils
    {
        public static Bounds GetSnapModuleBounds(GameObject target)
        {
            var bounds = new Bounds();
            if (target == null)
            {
                return bounds;
            }

            var stack = new Stack<GameObject>();
            stack.Push(target);
            
            //var renderers = target.GetComponentsInChildren<Renderer>();
            var renderers = new List<Renderer>();
            while (stack.Count > 0)
            {
                var top = stack.Pop();
                if (top == null) continue;

                var connectionComponent = top.GetComponent<SnapConnection>();
                if (connectionComponent != null)
                {
                    // Ignore this for the bounds calculation
                    continue;
                }

                var rendererComponent = top.GetComponent<Renderer>();
                if (rendererComponent != null)
                {
                    renderers.Add(rendererComponent);
                }

                for (int i = 0; i < top.transform.childCount; i++)
                {
                    stack.Push(top.transform.GetChild(i).gameObject);
                }
            }
            
            bool firstEntry = true;
            foreach (var renderer in renderers)
            {
                if (!renderer.enabled)
                {
                    continue;
                }
                
                if (firstEntry)
                {
                    firstEntry = false;
                    bounds = renderer.bounds;
                }
                else
                {
                    var renderBounds = renderer.bounds;
                    if (renderBounds.extents != Vector3.zero)
                    {
                        bounds.Encapsulate(renderer.bounds);
                    }
                }
            }

            return bounds;
        }
        
        public static void FindConnectionTransforms(GameObject moduleGameObject, out Matrix4x4[] outTransforms, out string[] outCategories)
        {
            if (moduleGameObject == null)
            {
                outTransforms = new Matrix4x4[0];
                outCategories = new string[0];
                return;
            }

            var connections = moduleGameObject.GetComponentsInChildren<SnapConnection>();
            var transforms = new List<Matrix4x4>();
            var categories = new List<string>();
            foreach (var connection in connections)
            {
                var transform = connection.transform;
                var worldTransform = Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);

                transforms.Add(worldTransform);
                categories.Add(connection.category);
            }

            outTransforms = transforms.ToArray();
            outCategories = categories.ToArray();
        }
        
        
        public static Matrix4x4[] FindAttachmentTransforms(ref Matrix4x4 ParentModuleTransform, ref Matrix4x4 IncomingDoorTransform, ref Matrix4x4 AttachmentDoorTransform) 
        {
            Matrix4x4 DesiredDoorTransform = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 180, 0), Vector3.one) * IncomingDoorTransform * ParentModuleTransform;
            
            // Calculate the rotation
            Quaternion DesiredRotation;
            {
                Vector3 TargetVector = Matrix.GetRotation(ref DesiredDoorTransform) * new Vector3(1, 0, 0);
                Vector3 SourceVector = Matrix.GetRotation(ref AttachmentDoorTransform) * new Vector3(1, 0, 0);

                Quaternion TargetRot = Quaternion.LookRotation(TargetVector, Vector3.up);
                Quaternion SourceRot = Quaternion.LookRotation(SourceVector, Vector3.up);
                DesiredRotation = TargetRot * Quaternion.Inverse(SourceRot);
            }

            // Calculate the translation
            Vector3 DesiredOffset;
            {
                Vector3 IncomingDoorPosition = Matrix.GetTranslation(ref IncomingDoorTransform);
                IncomingDoorPosition = ParentModuleTransform.MultiplyPoint3x4(IncomingDoorPosition);
                Vector3 ClampTarget = IncomingDoorPosition;

                Vector3 LocalDoorPosition = DesiredRotation * Matrix.GetTranslation(ref AttachmentDoorTransform);
                DesiredOffset = ClampTarget - LocalDoorPosition;
            }
            
            var ModuleTransform = Matrix4x4.TRS(DesiredOffset, DesiredRotation, Vector3.one);
            return new Matrix4x4[] { ModuleTransform };
        }

    }
}