//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect.Graphs;
using DungeonArchitect.Utils;

namespace DungeonArchitect.Grammar
{
    public class GrammarGraph : Graph
    {
        public bool useProceduralScript = false;
        public string generatorScriptClass;

        public KeyValueData editorData = new KeyValueData();

        public override void OnEnable()
        {
            base.OnEnable();

            hideFlags = HideFlags.HideInHierarchy;
        }
    }
}
