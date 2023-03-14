//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Flow.Domains.Layout.Tooling.Graph3D
{
    [ExecuteInEditMode]
    public abstract class FlowLayoutCamAlignerBase : MonoBehaviour
    {
        void OnDrawGizmos()
        {
            AlignToCam();
        }
        
        void OnDrawGizmosSelected()
        {
            AlignToCam();
        }

        public void AlignToCam()
        {
            var cameraPosition = GetCameraPosition();
            AlignToCamImpl(cameraPosition);
        }

        void Update()
        {
            if (Application.isPlaying)
            {
                AlignToCam();
            }
        }

        protected Vector3 GetCameraPosition()
        {
            
            if (Application.isPlaying)
            {
                if (Camera.main != null)
                {
                    return Camera.main.transform.position;
                }
            }
            else
            {
#if UNITY_EDITOR
                var sceneView = UnityEditor.SceneView.lastActiveSceneView;
                if (sceneView != null && sceneView.camera != null)
                {
                    return sceneView.camera.transform.position;
                }
#endif // UNITY_EDITOR
            }

            return Vector3.zero;
        }
        
        protected abstract void AlignToCamImpl(Vector3 cameraPosition);
    }
}