using Internal_assets.Scripts.QuickRun.Interactable;
using UnityEngine;
namespace Internal_assets.Scripts.QuickRun.Player
{
    [CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data/Data Base")]
    public class PlayerData : ScriptableObject
    {
        [Header("Player Stats Now")]
        public string className = "No one";
        public float health = 100f;
        public float healthRecoverySpeed = 10f;
        public float stamina = 100f;
        public float staminaRecoverySpeed = 10f;
        public float staminaRecoverySpeedIsFatigue = 7.5f;
        public float experience = 0f;
        public float damage = 10f;

        [Header("Player Stats Max")]
        public float maxHealth = 100f;
        public float maxStamina = 100f;

        [Header("Movement Speed")]
        public float jumpSpeed = 5f;
        public float movementSpeed = 10f;
        public float inAirMovementSpeed = 8f;
        public float crouchMovementSpeed = 8f;
        public float blockMovementSpeed = 4f;

        [Header("IsStates")]
        public bool isAbility = false;
        public bool isFatigue = false;

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
        public InteractionData interactionData = null;
    }
}
