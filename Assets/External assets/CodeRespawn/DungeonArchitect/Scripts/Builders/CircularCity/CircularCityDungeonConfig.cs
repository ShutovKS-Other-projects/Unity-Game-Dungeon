//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Builders.CircularCity
{
    public class CircularCityDungeonConfig : DungeonConfig
    {
        public int numRings = 5;
        public int numRays = 7;

        public float startRadius = 50;
        public int endRadius = 200;

        public float mainRoadStrength = 4.0f;
        public float sideRoadStrength = 2.0f;

        public float mainRoadRemovalProbability = 0.1f;
        public float sideRoadRemovalProbability = 0.2f;

        public float randomSideLaneOffsetAngle = 3;

        public float interNodeDistance = 6.0f;

        public float buildingSize = 4.0f;

        public MeshFilter roadMesh;
    }
}

