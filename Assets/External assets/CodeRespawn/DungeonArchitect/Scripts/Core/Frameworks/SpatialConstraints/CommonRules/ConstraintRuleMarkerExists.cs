//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect.SpatialConstraints;
using DungeonArchitect.Utils;

namespace DungeonArchitect.Builders.Grid.SpatialConstraints
{
    [RuleMeta(name = "Common/Marker Exists")]
    public class ConstraintRuleMarkerExists : ConstraintRule
    {
        [SerializeField]
        public string markerName;

        [SerializeField]
        public float searchRadius = 0.1f;
        
        public override string ToString()
        {
            return string.Format("{0}: {1}", base.ToString(), markerName);
        }

        public override bool Process(ConstraintRuleContext context)
        {
            var searchPosition = SpatialConstraintProcessorUtils.GetPosition2D(context.ruleNodeWorldPosition);

            float searchRadiusSq = searchRadius * searchRadius;
            // Check if we have a marker with the specified name here
            var markerSearchSpace = context.processorContext.levelMarkers.GetMarkersInSearchArea(searchPosition, searchRadius);
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
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
