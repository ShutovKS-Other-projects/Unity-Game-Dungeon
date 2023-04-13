using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data")]
    public class PlayerData : ScriptableObject
    {
        [Header("Health")] public int healthMax = 100;

        [Header("Attributes")] public int agility = 10;
        public int strength = 10;
        public int armor = 10;

        [Header("CriticalDamage")] public int criticalDamage = 25;
        public int criticalChance = 15;

        [Header("Magic")] public int magicStrength = 10;

        [Header("Movement Speed")] public float jumpSpeed = 5f;
        public float movementForce = 10f;
        public float movementSpeedMax = 4f;
        public float runMovementSpeedMax = 5f;

        [Header("Ground Check")] public float groundCheckRadius = 0.2f;

        [Header("Inter Check")] public float interCheckDistance = 3f;
        public float interCheckSphereRadius = 0.5f;
    }
}