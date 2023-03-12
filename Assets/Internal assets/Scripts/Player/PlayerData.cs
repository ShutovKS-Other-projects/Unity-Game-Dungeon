using Magic.SubMagic;
using Magic.SuperMagic;
using Magic.Type;
using Skill.Enum;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    [CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data/Data Base")]
    public class PlayerData : ScriptableObject
    {
        [Header("Player Stats")]
        public string className = "No one";
        public float experience = 0f;
        
        [Header("Health")]
        public float healthMax = 100f;
        public float healthRecoverySpeed = 10f;
        
        public float manaMax = 100f;
        public float manaRecoverySpeed = 10f;

        [Header("Stamina")]
        public float staminaMax = 100f;
        public float staminaRecoverySpeed = 10f;
        public float staminaRecoverySpeedIsFatigue = 7.5f;
        public float staminaDecreaseRate = 3f;
        
        [Header("Attributes")]
        public float agility = 10f;
        public float strength = 10f;
        public float armor = 10f;

        [Header("CriticalDamage")]
        public float criticalDamage = 25f;
        public float criticalChance = 15f;
        
        [Header("Magic")]
        public float magicStrength = 10f;
        public MagicAttackType magicAttackType = MagicAttackType.None;

        [Header("Movement Speed")]
        public float jumpSpeed = 5f;
        public float movementForce = 10f;
        public float movementSpeedMax = 4f;
        public float runMovementSpeedMax = 5f;
        public float inAirMovementSpeedMax = 4f;
        public float crouchMovementSpeedMax = 3f;
        public float blockMovementSpeedMax = 3f;

        [Header("Collider Size")]
        public float crouchColliderCenter = 0.6f;
        public float crouchColliderHeight = 1.2f;
        public float standColliderCenter = 0.9f;
        public float standColliderHeight = 1.8f;

        [Header("Ground Check")]
        public float groundCheckRadius = 0.2f;

        [Header("Inter Check")]
        public float interCheckDistance = 3f;
        public float interCheckSphereRadius = 0.5f;
    }
}
