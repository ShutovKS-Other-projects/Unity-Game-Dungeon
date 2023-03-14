//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Flow.Domains;
using DungeonArchitect.Flow.Exec;
using DungeonArchitect.UI;
using DungeonArchitect.UI.Widgets;
using UnityEngine;

namespace DungeonArchitect.Editors.Flow.DomainEditors
{
    public abstract class FlowDomainEditor : ScriptableObject
    {        
        public virtual void Init(IFlowDomain domain, FlowEditorConfig editorConfig, UISystem uiSystem)
        {
            FlowDomain = domain;
            EditorConfig = editorConfig;
        }

        public bool IsInitialized()
        {
            return Content != null;
        }
        
        public FlowEditorConfig EditorConfig { get; private set; }
        public IFlowDomain FlowDomain { get; private set; }
        public abstract IWidget Content { get; protected set; }
        public abstract bool StateValid { get; }
        public abstract void UpdateNodePreview(FlowExecTaskState taskState);
        public virtual void Update() {}
        public virtual void Destroy() {}
        public virtual void Invalidate() {}
        public virtual void SetReadOnly(bool readOnly) {}
        public virtual bool RequiresRepaint() { return false; }
        public virtual void HandleInput(UISystem uiSystem) {}
    }

}