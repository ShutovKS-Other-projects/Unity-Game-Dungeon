//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Editors.Flow.DomainEditors.Layout3D;
using DungeonArchitect.Flow.Impl.SnapGridFlow;

namespace DungeonArchitect.Editors.Flow.Impl
{
    public enum SnapGridFlowEditorLayoutMode
    {
        Vertical,
        Horizontal
    }
    
    public class SnapGridFlowEditorConfig : FlowEditorConfig, ILayout3DGraphDomainSettings
    {
        public SnapGridFlowModuleDatabase moduleDatabase;
        public bool autoFocusViewport = true;
        public SnapGridFlowEditorLayoutMode layoutMode = SnapGridFlowEditorLayoutMode.Vertical;
        
        public override DungeonBuilder FlowBuilder
        {
            get => null;
        }

        public bool AutoFocusViewport { get => autoFocusViewport; }
    } 
    
}