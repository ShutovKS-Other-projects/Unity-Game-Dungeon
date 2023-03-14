using System;
using Interactable;
using Level;
using Magic.SuperMagic;
using Magic.Type;
using Player.Delegate;
using Skill;
using Skill.Enum;
using UnityEngine;

namespace Player
{
    public class PlayerStatistic : MonoBehaviour
    {
        #region Singleton

        [SerializeField] private PlayerData playerData;
        public LevelSystem LevelSystem { get; private set; }
        private PlayerAttribute PlayerAttribute { get; set; }
        private PlayerSkillCharacteristic PlayerSkillCharacteristic { get; set; }
        private PlayerSkillMagicAttack PlayerSkillMagicAttack { get; set; }
        #endregion

        public event ObtainingExperience ObtainingExperienceEvent;

        #region Unity methods

        private void Awake()
        {
            PlayerAttribute = GetComponent<PlayerAttribute>();
            LevelSystem = new LevelSystem();
            PlayerSkillCharacteristic = GetComponent<PlayerSkillCharacteristic>();
            PlayerSkillMagicAttack = GetComponent<PlayerSkillMagicAttack>();

            ObtainingExperienceEvent += LevelSystem.AddExperience;
        }

        private void Start()
        {
            _health = playerData.healthMax;
            _stamina = playerData.staminaMax;
            _mana = playerData.manaMax;
            _magicType = playerData.magicType;
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.C) && LevelSystem.Level < 10)
            {
                LevelSystem.AddExperience(100);
                PlayerSkillCharacteristic.AddSkillPont();
                PlayerSkillMagicAttack.AddSkillPont();
            }
        }

        #endregion

        #region Parameters private

        private float _health;
        private float _mana;
        private float _stamina;
        private MagicType _magicType;

        #endregion

        #region Parameters public

        public string ClassName => playerData.className;
        public int Level => LevelSystem.Level;

        public float Health
        {
            get { return _health; }
            set { _health = value; }
        }

        public float HealthMax => playerData.healthMax + PlayerAttribute.Health + PlayerSkillCharacteristic.Health!;
        public float HealthRecoverySpeed => playerData.healthRecoverySpeed + PlayerAttribute.HealthRecoverySpeed;

        public float Mana
        {
            get { return playerData.manaMax; }
            set { _mana = value; }
        }

        public float ManaMax => playerData.manaMax + PlayerAttribute.Mana + PlayerSkillCharacteristic.Mana!;
        public float ManaRecoverySpeed => playerData.manaRecoverySpeed + PlayerAttribute.ManaRecoverySpeed;
        public float MagicAttackDamage => playerData.magicStrength + PlayerAttribute.MagicAttackDamage;

        public MagicType MagicType
        {
            get { return _magicType; }
            set { _magicType = value; }
        }

        public float StaminaMax => playerData.staminaMax + PlayerAttribute.Stamina + PlayerSkillCharacteristic.Stamina!;
        public float StaminaRecoverySpeed => playerData.staminaRecoverySpeed + PlayerAttribute.StaminaRecoverySpeed;
        public float StaminaRecoverySpeedIsFatigue => playerData.staminaRecoverySpeedIsFatigue;
        public float StaminaDecreaseRate => playerData.staminaDecreaseRate;

        public float Stamina
        {
            get { return _stamina; }
            set { _stamina = value; }
        }
        
        public float Strength => playerData.strength + PlayerAttribute.Strength + PlayerSkillCharacteristic.Strength!;
        public float Armor => playerData.armor + PlayerAttribute.Armor + PlayerSkillCharacteristic.Armor!;
        public float Agility => playerData.agility + PlayerAttribute.Agility;

        public float CriticalDamage => playerData.criticalDamage + PlayerAttribute.CriticalDamage;
        public float CriticalChance => playerData.criticalChance + PlayerAttribute.CriticalChance;


        public float MovementForce => playerData.movementForce;
        public float JumpSpeed => playerData.jumpSpeed;
        public float MovementSpeedMax => playerData.movementSpeedMax + PlayerAttribute.MoveSpeed / 100f;
        public float RunMovementSpeedMax => playerData.runMovementSpeedMax + PlayerAttribute.MoveSpeed / 100f;
        public float InAirMovementSpeedMax => playerData.inAirMovementSpeedMax + PlayerAttribute.MoveSpeed / 100f;
        public float CrouchMovementSpeedMax => playerData.crouchMovementSpeedMax + PlayerAttribute.MoveSpeed / 100f;
        public float BlockMovementSpeedMax => playerData.blockMovementSpeedMax + PlayerAttribute.MoveSpeed / 100f;

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