//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Grammar
{
    public class GrammarNodeType : ScriptableObject
    {
        [SerializeField]
        public string nodeName;

        [SerializeField]
        public string description;

        [SerializeField]
        public Color nodeColor = new Color(0.2f, 0.3f, 0.3f);

        [SerializeField]
        [HideInInspector]
        public bool wildcard = false;
    }
}
