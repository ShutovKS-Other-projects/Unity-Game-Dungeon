//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using System.Linq;
using DungeonArchitect.Flow.Exec;
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Editors.Flow
{
    public class FlowExecNodeHandlerInspectorBase : DAInspectorBase
    {

        public virtual void HandleInspectorGUI()
        {
            var attribute = FlowExecNodeInfoAttribute.GetHandlerAttribute(target.GetType());
            var title = "Node Settings";
            if (attribute != null)
            {
                title = attribute.Title;
            }
            
            // Draw the header
            GUILayout.Box(title, InspectorStyles.TitleStyle);
            EditorGUILayout.Space();
            
        }

        protected virtual void DrawMiscProperties()
        {
            DrawProperty("description");
        }

        public override void OnInspectorGUI()
        {
            sobject.Update();

            HandleInspectorGUI();
            
            DrawHeader("Misc");
            {
                EditorGUI.indentLevel++;
                DrawMiscProperties();
                EditorGUI.indentLevel--;
            }

            InspectorNotify.Dispatch(sobject, target);
        }
    }
    
}
