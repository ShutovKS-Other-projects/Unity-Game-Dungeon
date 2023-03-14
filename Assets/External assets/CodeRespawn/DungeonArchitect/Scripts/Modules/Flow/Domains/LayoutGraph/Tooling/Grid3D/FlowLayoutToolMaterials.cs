//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.SxEngine;

namespace DungeonArchitect.Flow.Domains.Layout.Tooling.Graph3D
{
    public class SxFlowNodeMaterial : SxUnityResourceMaterial
    {
        public SxFlowNodeMaterial() : base("FlowNodeMaterial") {}
    }
    
    public class SxFlowSubNodeMaterial : SxUnityResourceMaterial
    {
        public SxFlowSubNodeMaterial() : base("FlowNodeMaterial")
        {
        }
    }
    
    public class SxFlowMergedNodeMaterial : SxUnityResourceMaterial
    {
        public SxFlowMergedNodeMaterial() : base("FlowMergedNodeMaterial")
        {
            DepthBias = -2;
        }
    }
    
    public class SxFlowMergedNodeMaterialZWrite : SxUnityResourceMaterial
    {
        public SxFlowMergedNodeMaterialZWrite() : base("FlowMergedNodeMaterial_ZWrite")
        {
            DepthBias = -2;
        }
    }
    
    public class SxFlowItemMaterial : SxUnityResourceMaterial
    {
        public SxFlowItemMaterial() : base("FlowItemMaterial")
        {
            DepthBias = -0.25f;
        }
    }

    public class SxFlowItemMaterialZWrite : SxUnityResourceMaterial
    {
        public SxFlowItemMaterialZWrite() : base("FlowItemMaterial_ZWrite")
        {
            DepthBias = -0.25f;
        }
    }
    public class SxFlowLinkMaterial : SxUnityResourceMaterial
    {
        public SxFlowLinkMaterial() : base("FlowLinkMaterial") {}
    }
    
    public class SxFlowLinkHeadMaterial : SxUnityResourceMaterial
    {
        public SxFlowLinkHeadMaterial() : base("FlowLinkHeadMaterial") {}
    }
    
    public class SxFlowLinkOneWayHeadMaterial : SxUnityResourceMaterial
    {
        public SxFlowLinkOneWayHeadMaterial() : base("FlowLinkOneWayMaterial") {}
    }
    
    public class SxGridMaterial : SxUnityResourceMaterial
    {
        public SxGridMaterial() : base("GridMaterial") {}
    }
}