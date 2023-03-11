//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;


namespace DungeonArchitect.Grammar
{
    [System.Serializable]
    public enum GrammarExecRuleRunMode
    {
        RunOnce,
        RunWithProbability,
        Iterate,
        IterateRange
    }

    public class GrammarExecRuleNode : GrammarExecNodeBase
    {
        [SerializeField]
        [HideInInspector]
        public GrammarProductionRule rule;

        [SerializeField]
        public GrammarExecRuleRunMode runMode;

        public float runProbability = 1.0f;

        public int iterateCount = 3;

        public int minIterateCount = 3;

        public int maxIterateCount = 5;

    }
}
