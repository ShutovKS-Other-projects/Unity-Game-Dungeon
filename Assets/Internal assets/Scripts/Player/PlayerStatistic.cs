using Interactive;
using Player.Characteristics;
using UnityEngine;

namespace Player
{
    public class PlayerStatistic : MonoBehaviour
    {
        public static PlayerStatistic Instance { get; private set; }

        #region Singleton

        [SerializeField] public PlayerData playerData;
        public CharacteristicHealth CharacteristicHealth;
        public CharacteristicBase CharacteristicStrength;
        public CharacteristicBase CharacteristicArmor;
        public CharacteristicBase CharacteristicAgility;
        public CharacteristicBase CharacteristicStrengthMagic;
        public CharacteristicBase CharacteristicCriticalAttack;
        public CharacteristicBase CharacteristicCriticalChance;

        #endregion

        #region Unity methods

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

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
        public float MovementSpeedMax => playerData.movementSpeedMax;
        public float RunMovementSpeedMax => playerData.runMovementSpeedMax;

        public float GroundCheckRadius => playerData.groundCheckRadius;
        public float InterCheckDistance => playerData.interCheckDistance;
        public float InterCheckSphereRadius => playerData.interCheckSphereRadius;

        public InteractionObject interactionObject = new InteractionObject();
        public Transform interactionTransform = null;

        #endregion
        
    }
}