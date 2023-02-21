using System;
using Interactable;
using UnityEngine;
namespace Player
{
    public class PlayerStatistic : MonoBehaviour
    {
        [SerializeField] private PlayerData _playerData;
        private PlayerAttribute _playerAttribute;

        void Awake()
        {
            _playerAttribute = GetComponent<PlayerAttribute>();
        }

        #region Parameters private

        private float _health;
        private float _stamina;
        private float _experience;

        #endregion

        #region Parameters public

        public string ClassName { get { return _playerData.className; } }

        public float Health { get { return _health; } set { _health = value; } }
        public float HealthMax { get { return _playerData.healthMax + _playerAttribute.Health; } }
        public float HealthRecoverySpeed { get { return _playerData.healthRecoverySpeed + _playerAttribute.HealthRecoverySpeed; } }

        public float Stamina { get { return _stamina; } set { _stamina = value; } }
        public float StaminaMax { get { return _playerData.staminaMax + _playerAttribute.Stamina; } }
        public float StaminaRecoverySpeed { get { return _playerData.staminaRecoverySpeed + _playerAttribute.StaminaRecoverySpeed; } }
        public float StaminaRecoverySpeedIsFatigue { get { return _playerData.staminaRecoverySpeedIsFatigue; } }

        public float Experience { get { return _experience; } set { _experience = value; } }

        public float Strength { get { return _playerData.strength + _playerAttribute.Strength; } }
        public float Armor { get { return _playerData.armor + _playerAttribute.Armor; } }

        public float MovementForce { get { return _playerData.movementForce; } }
        public float JumpSpeed { get { return _playerData.jumpSpeed; } }
        public float MovementSpeedMax { get { return _playerData.movementSpeedMax + _playerAttribute.MoveSpeed / 100f; } }
        public float RunMovementSpeedMax { get { return _playerData.runMovementSpeedMax + _playerAttribute.MoveSpeed / 100f; } }
        public float InAirMovementSpeedMax { get { return _playerData.inAirMovementSpeedMax + _playerAttribute.MoveSpeed / 100f; } }
        public float CrouchMovementSpeedMax { get { return _playerData.crouchMovementSpeedMax + _playerAttribute.MoveSpeed / 100f; } }
        public float BlockMovementSpeedMax { get { return _playerData.blockMovementSpeedMax + _playerAttribute.MoveSpeed / 100f; } }

        public float CrouchColliderCenter { get { return _playerData.crouchColliderCenter; } }
        public float CrouchColliderHeight { get { return _playerData.crouchColliderHeight; } }
        public float StandColliderCenter { get { return _playerData.standColliderCenter; } }
        public float StandColliderHeight { get { return _playerData.standColliderHeight; } }

        public float GroundCheckRadius { get { return _playerData.groundCheckRadius; } }
        public float InterCheckDistance { get { return _playerData.interCheckDistance; } }
        public float InterCheckSphereRadius { get { return _playerData.interCheckSphereRadius; } }
        public InteractionData InteractionData { get { return _playerData.interactionData; } set { _playerData.interactionData = value; } }

        [NonSerialized] public bool IsAbility;
        [NonSerialized] public bool IsFatigue;

        #endregion
    }
}
