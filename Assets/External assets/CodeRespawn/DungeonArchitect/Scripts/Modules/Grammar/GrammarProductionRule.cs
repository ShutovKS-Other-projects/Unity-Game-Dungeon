//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Grammar
{
    [System.Serializable]
    public class GrammarProductionRule : ScriptableObject
    {
        [SerializeField]
        public string ruleName;

        [HideInInspector]
        [SerializeField]
        public GrammarGraph LHSGraph;

        [HideInInspector]
        [SerializeField]
        public List<WeightedGrammarGraph> RHSGraphs = new List<WeightedGrammarGraph>();
    }
}
