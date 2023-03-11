//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Flow.Domains.Layout.Tooling.Graph3D
{
    public class FlowLayoutNodeCamAligner : FlowLayoutCamAlignerBase
    {
        protected override void AlignToCamImpl(Vector3 cameraPosition)
        {
            var direction = (cameraPosition - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
    }
}