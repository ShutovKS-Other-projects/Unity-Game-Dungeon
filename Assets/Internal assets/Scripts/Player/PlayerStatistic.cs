using System;
using Interactable;
using Level;
using UnityEngine;

namespace Player
{
    public class PlayerStatistic : MonoBehaviour
    {
        [SerializeField] private PlayerData playerData;
        private PlayerAttribute _playerAttribute;
        public LevelSystem LevelSystem;

        private void Awake()
        {
            _playerAttribute = GetComponent<PlayerAttribute>();
            LevelSystem = new LevelSystem();
        }

        private void Start()
        {
            _health = playerData.healthMax;
            _stamina = playerData.staminaMax;
            _mana = playerData.manaMax;
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.C) && LevelSystem.Level < 10)
            {
                LevelSystem.AddExperience(100);
            }
        }

        #region Parameters private

        private float _health;
        private float _mana;
        private float _stamina;

        #endregion

        #region Parameters public

        public string ClassName => playerData.className;
        public int Level => LevelSystem.Level;

        public float Health
        {
            get { return _health; }
            set { _health = value; }
        }

        public float HealthMax => playerData.healthMax + _playerAttribute.Health;
        public float HealthRecoverySpeed => playerData.healthRecoverySpeed + _playerAttribute.HealthRecoverySpeed;

        public float Mana
        {
            get { return playerData.manaMax; }
            set { _mana = value; }
        }

        public float ManaMax => playerData.manaMax + _playerAttribute.Mana;
        public float ManaRecoverySpeed => playerData.manaRecoverySpeed + _playerAttribute.ManaRecoverySpeed;


        public float Stamina
        {
            get { return _stamina; }
            set { _stamina = value; }
        }

        public float StaminaMax => playerData.staminaMax + _playerAttribute.Stamina;
        public float StaminaRecoverySpeed => playerData.staminaRecoverySpeed + _playerAttribute.StaminaRecoverySpeed;
        public float StaminaRecoverySpeedIsFatigue => playerData.staminaRecoverySpeedIsFatigue;
        public float StaminaDecreaseRate => playerData.staminaDecreaseRate;

        public float Strength => playerData.strength + _playerAttribute.Strength;
        public float Armor => playerData.armor + _playerAttribute.Armor;
        public float Agility => playerData.agility + _playerAttribute.Agility;

        public float CriticalDamage => playerData.criticalDamage + _playerAttribute.CriticalDamage;
        public float CriticalChance => playerData.criticalChance + _playerAttribute.CriticalChance;

        public float MovementForce => playerData.movementForce;
        public float JumpSpeed => playerData.jumpSpeed;
        public float MovementSpeedMax => playerData.movementSpeedMax + _playerAttribute.MoveSpeed / 100f;
        public float RunMovementSpeedMax => playerData.runMovementSpeedMax + _playerAttribute.MoveSpeed / 100f;
        public float InAirMovementSpeedMax => playerData.inAirMovementSpeedMax + _playerAttribute.MoveSpeed / 100f;
        public float CrouchMovementSpeedMax => playerData.crouchMovementSpeedMax + _playerAttribute.MoveSpeed / 100f;
        public float BlockMovementSpeedMax => playerData.blockMovementSpeedMax + _playerAttribute.MoveSpeed / 100f;

        public float CrouchColliderCenter => playerData.crouchColliderCenter;
        public float CrouchColliderHeight => playerData.crouchColliderHeight;
        public float StandColliderCenter => playerData.standColliderCenter;
        public float StandColliderHeight => playerData.standColliderHeight;

        public float GroundCheckRadius => playerData.groundCheckRadius;
        public float InterCheckDistance => playerData.interCheckDistance;
        public float InterCheckSphereRadius => playerData.interCheckSphereRadius;

        public InteractionData interactionData = null;

        [NonSerialized] public bool IsAbility;
        [NonSerialized] public bool IsFatigue;

        #endregion
    }
}