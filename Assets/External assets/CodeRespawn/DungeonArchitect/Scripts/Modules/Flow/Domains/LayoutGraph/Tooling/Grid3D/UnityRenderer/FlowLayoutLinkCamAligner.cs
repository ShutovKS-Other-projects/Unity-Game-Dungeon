//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Flow.Domains.Layout.Tooling.Graph3D
{
    [ExecuteInEditMode]
    public class FlowLayoutLinkCamAligner : FlowLayoutCamAlignerBase
    {
        public Transform startTransform;
        public Transform endTransform;

        public float thickness;
        public float startRadius;
        public float endRadius;
        public bool oneWay;
        

        protected override void AlignToCamImpl(Vector3 cameraPosition)
        {
            if (startTransform == null || endTransform == null) return;
            OrientLinkToNodes();

            var axisX = (endTransform.position - startTransform.position).normalized;
            var axisZ = (cameraPosition - transform.position).normalized;
            var axisY = Vector3.Cross(axisZ, axisX);
            axisZ = Vector3.Cross(axisX, axisY);
            
            var rotationMatrix = new Matrix4x4(axisX, axisY, axisZ, new Vector4(0, 0, 0, 1));
            transform.rotation = rotationMatrix.rotation;
        }
        
        void OrientLinkToNodes()
        {
            var headThickness = thickness * FlowLayout3DConstants.LinkHeadThicknessMultiplier;
            float headLength = headThickness;
            float headWidth = headThickness;
            if (oneWay)
            {
                headLength *= 2;
            }

            var start = startTransform.position;
            var end = endTransform.position;

            var direction = (end - start).normalized;
            start = start + direction * startRadius;
            end = end - direction * (endRadius + headLength);

            var length = (end - start).magnitude;
            
            transform.rotation = Quaternion.FromToRotation(new Vector3(1, 0, 0), direction);
            transform.position = start + (end - start) * 0.5f;
            var scale = new Vector3(length / 2.0f, thickness / 2.0f, 1);
            transform.localScale = scale;

            if (transform.childCount > 0)
            {
                var headTransform = transform.GetChild(0);
                var headScale = Vector3.Scale(Vector3.one / 2.0f, new Vector3(headLength, headWidth, 1));
                headScale = MathUtils.Divide(headScale, scale);
                headTransform.localScale = headScale;
                headTransform.localPosition = MathUtils.Divide(new Vector3(length * 0.5f + headLength * 0.5f, 0, 0), scale);
            }
        }
    }
}