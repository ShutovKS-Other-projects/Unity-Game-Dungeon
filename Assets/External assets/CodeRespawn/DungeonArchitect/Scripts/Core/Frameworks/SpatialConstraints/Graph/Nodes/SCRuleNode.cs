//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;
using DungeonArchitect.SpatialConstraints;

namespace DungeonArchitect.Graphs.SpatialConstraints
{
    public enum SCRuleNodeDomain
    {
        Tile,
        Edge,
        Corner,
        Invalid,
    }

    public enum SCRuleNodeEvaluationMode
    {
        AllRulesMustPass,
        AtleastOneRuleShouldPass
    }

    public class SCRuleNode : SCBaseDomainNode
    {
        [SerializeField]
        public ConstraintRule[] constraints = new ConstraintRule[0];

        [SerializeField]
        public SCRuleNodeEvaluationMode constraintEvaluationMode = SCRuleNodeEvaluationMode.AllRulesMustPass;

        [SerializeField]
        public float exclusionRuleSearchRadius = 0.1f;

        [SerializeField]
        public string[] exclusionRuleMarkersToRemove = new string[0];
        
        public override void Initialize(string id, Graph graph)
        {
            base.Initialize(id, graph);
            
        }

        public override Color GetColor()
        {
            return Color.black;
        }


        public override void CopyFrom(GraphNode node)
        {
            base.CopyFrom(node);

            if (node is SCRuleNode)
            {
                var otherNode = node as SCRuleNode;
                var constraintList = new List<ConstraintRule>();

                foreach (var otherConstraint in otherNode.constraints)
                {
                    var constraint = Object.Instantiate(otherConstraint) as ConstraintRule;
                    constraintList.Add(constraint);
                }

                constraints = constraintList.ToArray();
                constraintEvaluationMode = otherNode.constraintEvaluationMode;
                exclusionRuleSearchRadius = otherNode.exclusionRuleSearchRadius;
                System.Array.Copy(otherNode.exclusionRuleMarkersToRemove, exclusionRuleMarkersToRemove, otherNode.exclusionRuleMarkersToRemove.Length);
            }
        }
    }
}
