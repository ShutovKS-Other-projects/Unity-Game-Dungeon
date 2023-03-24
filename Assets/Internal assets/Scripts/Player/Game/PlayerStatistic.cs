using Interactable;
using UnityEngine;

namespace Player.Game
{
    public class PlayerStatistic : MonoBehaviour
    {
        #region Singleton

        [SerializeField] private PlayerData playerData;

        #endregion

        #region Unity methods

        private void Start()
        {
            _health = playerData.healthMax;
        }

        #endregion

        #region Parameters private

        private float _health;

        #endregion

        #region Parameters public
        
        public float Health
        {
            get => _health;
            set => _health = value;
        }

        public float HealthMax => playerData.healthMax;
        public float HealthRecoverySpeed => playerData.healthRecoverySpeed;

        public float MagicAttackDamage => playerData.magicStrength;

        public float Strength => playerData.strength;
        public float Armor => playerData.armor;
        public float Agility => playerData.agility;

        public float CriticalDamage => playerData.criticalDamage;
        public float CriticalChance => playerData.criticalChance;


        public float MovementForce => playerData.movementForce;
        public float JumpSpeed => playerData.jumpSpeed;
        public float MovementSpeedMax => playerData.movementSpeedMax;
        public float RunMovementSpeedMax => playerData.runMovementSpeedMax;
        public float InAirMovementSpeedMax => playerData.inAirMovementSpeedMax;
        public float CrouchMovementSpeedMax => playerData.crouchMovementSpeedMax;
        public float BlockMovementSpeedMax => playerData.blockMovementSpeedMax;

        public float CrouchColliderCenter => playerData.crouchColliderCenter;
        public float CrouchColliderHeight => playerData.crouchColliderHeight;
        public float StandColliderCenter => playerData.standColliderCenter;
        public float StandColliderHeight => playerData.standColliderHeight;

        public float GroundCheckRadius => playerData.groundCheckRadius;
        public float InterCheckDistance => playerData.interCheckDistance;
        public float InterCheckSphereRadius => playerData.interCheckSphereRadius;

        public InteractionData interactionData = null;

        #endregion
    }
}