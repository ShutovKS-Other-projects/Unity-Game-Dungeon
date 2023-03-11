//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System;
using System.Collections.Generic;
using DungeonArchitect.Editors.SnapFlow;
using DungeonArchitect.Flow.Exec;
using DungeonArchitect.Grammar;
using UnityEditor;

namespace DungeonArchitect.Editors
{
    public class InspectorNotify
    {
        // Delegates
        public delegate void OnFlowTaskPropertyChanged(FlowExecTask task);
        public delegate void OnSnapPropertyChanged(Object obj);

        // Events
        public static event OnFlowTaskPropertyChanged FlowTaskPropertyChanged;
        public static event OnSnapPropertyChanged SnapPropertyChanged;

        private static readonly HashSet<System.Type> SnapEditorTypes;

        static InspectorNotify()
        {
            SnapEditorTypes = new HashSet<Type>()
            {
                typeof(GrammarExecRuleNode),
                typeof(GrammarGraph),
                typeof(GrammarTaskNode),
                typeof(SnapEdResultGraphEditorConfig),
                typeof(GrammarProductionRule),
                typeof(GrammarNodeType),
                typeof(WeightedGrammarGraph)
            };
        }
        
        public static void Dispatch(SerializedObject sobject, Object target)
        {
            if (sobject == null || target == null) return;
            var modified = sobject.ApplyModifiedProperties();
            if (modified)
            {
                if (target is FlowExecTask)
                {
                    if (FlowTaskPropertyChanged != null)
                    {
                        FlowTaskPropertyChanged.Invoke((FlowExecTask)target);
                    }
                }
                else if (SnapEditorTypes.Contains(target.GetType()))
                {
                    if (SnapPropertyChanged != null)
                    {
                        SnapPropertyChanged.Invoke(target);
                    }
                }
            }
        }
    }
}