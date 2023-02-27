using System;
using Interactable;
using UnityEngine;
using UnityEngine.Serialization;
namespace Player
{
    public class PlayerStatistic : MonoBehaviour
    {
        [SerializeField] PlayerData playerData;
        PlayerAttribute _playerAttribute;

        void Awake()
        {
            _playerAttribute = GetComponent<PlayerAttribute>();

            _health = playerData.healthMax;
            _stamina = playerData.staminaMax;
            _mana = playerData.manaMax;
        }

        #region Parameters private

        float _health;
        float _mana;
        float _stamina;
        float _experience;

        #endregion

        #region Parameters public

        public string ClassName { get { return playerData.className; } }

        public float Health { get { return _health; } set { _health = value; } }
        public float HealthMax { get { return playerData.healthMax + _playerAttribute.Health; } }
        public float HealthRecoverySpeed { get { return playerData.healthRecoverySpeed + _playerAttribute.HealthRecoverySpeed; } }

        public float Mana { get { return playerData.manaMax; } set { _mana = value; } }
        public float ManaMax { get { return playerData.manaMax + _playerAttribute.Mana; } }
        public float ManaRecoverySpeed { get { return playerData.manaRecoverySpeed + _playerAttribute.ManaRecoverySpeed; } }


        public float Stamina { get { return _stamina; } set { _stamina = value; } }
        public float StaminaMax { get { return playerData.staminaMax + _playerAttribute.Stamina; } }
        public float StaminaRecoverySpeed { get { return playerData.staminaRecoverySpeed + _playerAttribute.StaminaRecoverySpeed; } }
        public float StaminaRecoverySpeedIsFatigue { get { return playerData.staminaRecoverySpeedIsFatigue; } }
        public float StaminaDecreaseRate { get { return playerData.staminaDecreaseRate; } }

        public float Experience { get { return _experience; } set { _experience = value; } }

        public float Strength { get { return playerData.strength + _playerAttribute.Strength; } }
        public float Armor { get { return playerData.armor + _playerAttribute.Armor; } }
        public float Agility { get { return playerData.agility + _playerAttribute.Agility; } }

        public float CriticalDamage { get { return playerData.criticalDamage + _playerAttribute.CriticalDamage; } }
        public float CriticalChance { get { return playerData.criticalChance + _playerAttribute.CriticalChance; } }

        public float MovementForce { get { return playerData.movementForce; } }
        public float JumpSpeed { get { return playerData.jumpSpeed; } }
        public float MovementSpeedMax { get { return playerData.movementSpeedMax + _playerAttribute.MoveSpeed / 100f; } }
        public float RunMovementSpeedMax { get { return playerData.runMovementSpeedMax + _playerAttribute.MoveSpeed / 100f; } }
        public float InAirMovementSpeedMax { get { return playerData.inAirMovementSpeedMax + _playerAttribute.MoveSpeed / 100f; } }
        public float CrouchMovementSpeedMax { get { return playerData.crouchMovementSpeedMax + _playerAttribute.MoveSpeed / 100f; } }
        public float BlockMovementSpeedMax { get { return playerData.blockMovementSpeedMax + _playerAttribute.MoveSpeed / 100f; } }

        public float CrouchColliderCenter { get { return playerData.crouchColliderCenter; } }
        public float CrouchColliderHeight { get { return playerData.crouchColliderHeight; } }
        public float StandColliderCenter { get { return playerData.standColliderCenter; } }
        public float StandColliderHeight { get { return playerData.standColliderHeight; } }

        public float GroundCheckRadius { get { return playerData.groundCheckRadius; } }
        public float InterCheckDistance { get { return playerData.interCheckDistance; } }
        public float InterCheckSphereRadius { get { return playerData.interCheckSphereRadius; } }

        public InteractionData interactionData = null;

        [NonSerialized] public bool IsAbility;
        [NonSerialized] public bool IsFatigue;

        #endregion

    }
}
