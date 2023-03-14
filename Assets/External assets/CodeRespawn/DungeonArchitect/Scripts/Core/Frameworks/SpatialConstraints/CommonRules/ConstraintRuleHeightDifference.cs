//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect.SpatialConstraints;
using DungeonArchitect.Utils;

namespace DungeonArchitect.Builders.Grid.SpatialConstraints
{
    public enum ConstraintRuleHeightDifferenceType
    {
        IsSameHeight,
        IsAboveReferenceNode,
        IsBelowReferenceNode
    }

    [RuleMeta(name = "Common/Height Difference")]
    public class ConstraintRuleHeightDifference : ConstraintRule
    {
        [SerializeField]
        public string markerName;

        [SerializeField]
        public float markerSearchRadius = 0.1f;

        [SerializeField]
        public float heightCheckTollerance = 0.5f;

        [SerializeField]
        public ConstraintRuleHeightDifferenceType heightFunction;

        public override string ToString()
        {
            return string.Format("{0}: {1}", base.ToString(), markerName);
        }

        public override bool Process(ConstraintRuleContext context)
        {
            var searchResult = GetMarkerSearchResult(context);
            if (searchResult == null)
            {
                return false;
            }

            var source = Matrix.GetTranslation(ref context.processorContext.marker.Transform);
            var check = Matrix.GetTranslation(ref searchResult.Transform);

            var heightDiff = check.y - source.y;
            if (heightFunction == ConstraintRuleHeightDifferenceType.IsSameHeight)
            {
                return Mathf.Abs(heightDiff) < heightCheckTollerance;
            }
            else if (heightFunction == ConstraintRuleHeightDifferenceType.IsAboveReferenceNode)
            {
                return heightDiff > heightCheckTollerance;
            }
            else if (heightFunction == ConstraintRuleHeightDifferenceType.IsBelowReferenceNode)
            {
                return heightDiff < -heightCheckTollerance;
            }
            else
            {
                return false;
            }

        }

        PropSocket GetMarkerSearchResult(ConstraintRuleContext context)
        {
            var searchPosition = SpatialConstraintProcessorUtils.GetPosition2D(context.ruleNodeWorldPosition);

            float searchRadiusSq = markerSearchRadius * markerSearchRadius;
            // Check if we have a marker with the specified name here
            var markerSearchSpace = context.processorContext.levelMarkers.GetMarkersInSearchArea(searchPosition, markerSearchRadius);
            foreach (var marker in markerSearchSpace)
            {
                if (marker.markForDeletion)
                {
                    continue;
                }

                if (marker.SocketType == markerName)
                {
                    // Check if the distance is within the range
                    var candidateMarkerPosition = SpatialConstraintProcessorUtils.GetPosition2D(Matrix.GetTranslation(ref marker.Transform));

                    float distanceSq = (searchPosition - candidateMarkerPosition).sqrMagnitude;
                    if (distanceSq < searchRadiusSq)
                    {
                        return marker;
                    }
                }
            }

            return null;
        }
    }
}
