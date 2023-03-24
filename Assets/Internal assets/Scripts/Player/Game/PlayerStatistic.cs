using Interactable;
using Player.Game.Characteristics;
using UnityEngine;

namespace Player.Game
{
    public class PlayerStatistic : MonoBehaviour
    {
        #region Singleton

        [SerializeField] private PlayerData playerData;
        public CharacteristicHealth CharacteristicHealth;
        public CharacteristicBase CharacteristicStrength;
        public CharacteristicBase CharacteristicArmor;
        public CharacteristicBase CharacteristicAgility;
        public CharacteristicBase CharacteristicStrengthMagic;
        public CharacteristicBase CharacteristicCriticalAttack;
        public CharacteristicBase CharacteristicCriticalChance;

        #endregion

        #region Unity methods

        private void Start()
        {
            CharacteristicHealth = new CharacteristicHealth(playerData.healthMax);
            CharacteristicStrength = new CharacteristicBase(playerData.strength);
            CharacteristicArmor = new CharacteristicBase(playerData.armor);
            CharacteristicAgility = new CharacteristicBase(playerData.agility);
            CharacteristicStrengthMagic = new CharacteristicBase(playerData.magicStrength);
            CharacteristicCriticalAttack = new CharacteristicBase(playerData.criticalDamage);
            CharacteristicCriticalChance = new CharacteristicBase(playerData.criticalChance);
        }

        #endregion

        #region Parameters public

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