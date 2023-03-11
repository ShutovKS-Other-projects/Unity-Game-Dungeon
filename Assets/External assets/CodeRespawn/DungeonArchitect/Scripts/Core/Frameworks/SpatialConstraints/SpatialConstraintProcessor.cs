//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using DungeonArchitect.Graphs.SpatialConstraints;
using DungeonArchitect.Utils;

namespace DungeonArchitect.SpatialConstraints
{
    public class SpatialConstraintProcessorUtils
    {

        public static Vector2 GetPosition2D(Vector3 position)
        {
            return new Vector2(position.x, position.z);
        }

        public static Vector3 RotateVector(Vector3 position, Matrix4x4 matrix)
        {
            var rotation = Matrix.GetRotation(ref matrix);
            return rotation * position;
        }

        public static Vector3 GetRuleNodeWorldPosition(SCRuleNode ruleNode, SCReferenceNode referenceNode, PropSocket marker, Vector3 gridSize, ref Matrix4x4 rotationFrame)
        {
            var scGraph = ruleNode.Graph as SpatialConstraintGraph;
            var relativeToMarkerRotation = scGraph.asset.checkRelativeToMarkerRotation;

            var markerTransform = marker.Transform;

            Vector2 offset2D = (ruleNode.Bounds.center - referenceNode.Bounds.center) / SCBaseDomainNode.TileSize;
            Vector3 offset = new Vector3(offset2D.x, 0, offset2D.y);
            if (relativeToMarkerRotation)
            {
                offset = RotateVector(offset, markerTransform);
            }
            offset = RotateVector(offset, rotationFrame);
            
            // Find the world position of the marker and the location to search (relative to the spatial constraint node from the SC reference node)
            var baseMarkerPosition = Matrix.GetTranslation(ref marker.Transform);
            var ruleNodePosition = baseMarkerPosition + Vector3.Scale(offset, gridSize);
            return ruleNodePosition;
       }

    }

    public class SpatialConstraintProcessorContext
    {
        public SpatialConstraintAsset constraintAsset;
        public PropSocket marker;
        public DungeonModel model;
        public DungeonConfig config;
        public DungeonBuilder builder;
        public LevelMarkerList levelMarkers;
    }

    public abstract class SpatialConstraintProcessor : MonoBehaviour
    {
        public virtual SpatialConstraintRuleDomain GetDomain(SpatialConstraintProcessorContext context)
        {
            var domain = new SpatialConstraintRuleDomain();
            var nodes = context.constraintAsset.Graph.Nodes;
            domain.referenceNode = nodes.Where(node => node is SCReferenceNode).FirstOrDefault() as SCReferenceNode;
            return domain;
        }

        bool ProcessSpatialConstraintFrame(SpatialConstraintProcessorContext context, SpatialConstraintRuleDomain domain, Matrix4x4 rotationFrame, out PropSocket[] outMarkersToRemove)
        {
            var nodes = context.constraintAsset.Graph.Nodes.Where(node => node is SCRuleNode);
            outMarkersToRemove = new PropSocket[0];

            if (nodes.Count() == 0)
            {
                // No rules specified.  Return true by default
                return true;
            }

            var nodeWorldPositions = new Dictionary<SCRuleNode, Vector3>();

            foreach (var node in nodes)
            {
                var ruleNode = node as SCRuleNode;

                var nodePosition = SpatialConstraintProcessorUtils.GetRuleNodeWorldPosition(ruleNode, domain.referenceNode,
                        context.marker, domain.gridSize, ref rotationFrame);
                nodeWorldPositions.Add(ruleNode, nodePosition);

                var constraints = ruleNode.constraints.Where(c => c != null);
                if (constraints.Count() == 0) continue;

                bool allRulesPassed = true;
                bool atleastOneRulePassed = false;

                foreach (var constraint in constraints)
                {
                    var ruleContext = new ConstraintRuleContext();
                    ruleContext.processorContext = context;
                    ruleContext.domain = domain;
                    ruleContext.ruleNode = ruleNode;
                    ruleContext.rotationFrame = rotationFrame;
                    ruleContext.ruleNodeWorldPosition = nodePosition;


                    bool success = constraint.Process(ruleContext);
                    if (constraint.inverseRule)
                    {
                        success = !success;
                    }

                    allRulesPassed &= success;
                    atleastOneRulePassed |= success;
                }

                if (ruleNode.constraintEvaluationMode == SCRuleNodeEvaluationMode.AllRulesMustPass && !allRulesPassed)
                {
                    return false;
                }
                else if (ruleNode.constraintEvaluationMode == SCRuleNodeEvaluationMode.AtleastOneRuleShouldPass && !atleastOneRulePassed)
                {
                    return false;
                }
            }

            // The spatial constraint setup has passed
            // Process removal rules
            var markersToRemove = new List<PropSocket>();
            foreach (var node in nodes)
            {
                var ruleNode = node as SCRuleNode;
                if (ruleNode.exclusionRuleMarkersToRemove.Length == 0)
                {
                    continue;
                }

                var radius = ruleNode.exclusionRuleSearchRadius;
                float searchRadiusSq = radius * radius;
                var nodePosition3D = nodeWorldPositions[ruleNode];
                var searchPosition = SpatialConstraintProcessorUtils.GetPosition2D(nodePosition3D);
                foreach (var markerToRemove in ruleNode.exclusionRuleMarkersToRemove)
                {
                    var markerSearchSpace = context.levelMarkers.GetMarkersInSearchArea(searchPosition, radius);
                    foreach (var candidateMarker in markerSearchSpace)
                    {
                        var candidateMarkerName = candidateMarker.SocketType;
                        if (ruleNode.exclusionRuleMarkersToRemove.Contains(candidateMarkerName))
                        {
                            var candidateMarkerPosition = SpatialConstraintProcessorUtils.GetPosition2D(Matrix.GetTranslation(ref candidateMarker.Transform));
                            float distanceSq = (searchPosition - candidateMarkerPosition).sqrMagnitude;
                            if (distanceSq < searchRadiusSq)
                            {
                                markersToRemove.Add(candidateMarker);
                            }
                        }
                    }
                }
            }
            outMarkersToRemove = markersToRemove.ToArray();

            return true;
        }

        public bool ProcessSpatialConstraint(SpatialConstraintProcessorContext context, out Matrix4x4 outOffset, out PropSocket[] outMarkersToRemove)
        {
            outOffset = Matrix4x4.identity;
            var domain = GetDomain(context);
            if (context.constraintAsset != null && context.constraintAsset.Graph != null)
            {
                Matrix4x4[] rotationFrames;
                if (context.constraintAsset.rotateToFit)
                {
                    rotationFrames = new Matrix4x4[]
                    {
                        Matrix4x4.identity,
                        Matrix4x4.Rotate(Quaternion.Euler(0, 90, 0)),
                        Matrix4x4.Rotate(Quaternion.Euler(0, 180, 0)),
                        Matrix4x4.Rotate(Quaternion.Euler(0, 270, 0)),
                    };
                }
                else
                {
                    rotationFrames = new Matrix4x4[]
                    {
                        Matrix4x4.identity
                    };
                }
                
                foreach (var rotationFrame in rotationFrames)
                {
                    if (ProcessSpatialConstraintFrame(context, domain, rotationFrame, out outMarkersToRemove))
                    {
                        if (context.constraintAsset.applyFitRotation)
                        {
                            outOffset = rotationFrame;
                        }
                        return true;
                    }
                }
            }

            outMarkersToRemove = new PropSocket[0];
            return false;
        }
    }
}