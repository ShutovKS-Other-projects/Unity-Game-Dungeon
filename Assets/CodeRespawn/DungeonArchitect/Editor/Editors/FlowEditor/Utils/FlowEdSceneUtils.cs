//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using UnityEditor;
using DungeonArchitect.Flow.Items;

namespace DungeonArchitect.Editors.Flow
{
    public abstract class KeyLockLinkVisualizerBase : Editor
    {
        protected void DrawLine(Component a, Component b)
        {
            if (a == null || b == null) return;
            var rendererA = a.gameObject.GetComponent<Renderer>();
            var rendererB = b.gameObject.GetComponent<Renderer>();
            var posA = rendererA != null ? rendererA.bounds.center : a.gameObject.transform.position;
            var posB = rendererB != null ? rendererB.bounds.center : b.gameObject.transform.position;

            Handles.color = Color.red;
            Handles.DrawLine(posA, posB);
        }
    }

    [CustomEditor(typeof(FlowDoorKeyComponent))]
    public class GridFlowDoorKeyComponentEditor : KeyLockLinkVisualizerBase
    {
        void OnSceneGUI()
        {
            var key = target as FlowDoorKeyComponent;
            if (key != null && key.lockRefs != null)
            {
                foreach (var lockComponent in key.lockRefs) {
                    DrawLine(key, lockComponent);
                }
            }
        }
    }

    [CustomEditor(typeof(FlowDoorLockComponent))]
    public class GridFlowDoorLockComponentEditor : KeyLockLinkVisualizerBase
    {
        void OnSceneGUI()
        {
            var lockComponent = target as FlowDoorLockComponent;
            if (lockComponent != null && lockComponent.validKeyRefs != null)
            {
                foreach (var key in lockComponent.validKeyRefs)
                {
                    DrawLine(key, lockComponent);
                }
            }
        }
    }
}
