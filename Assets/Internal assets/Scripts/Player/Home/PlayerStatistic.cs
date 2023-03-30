using Interactable;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player.Home
{
    public class PlayerStatistic : MonoBehaviour
    {
        [SerializeField] public PlayerData playerData;

        public float MovementForce => playerData.movementForce;
        public float JumpSpeed => playerData.jumpSpeed;
        public float MovementSpeedMax => playerData.movementSpeedMax;
        public float RunMovementSpeedMax => playerData.runMovementSpeedMax;
        public float InAirMovementSpeedMax => playerData.inAirMovementSpeedMax;
        public float CrouchMovementSpeedMax => playerData.crouchMovementSpeedMax;

        public float CrouchColliderHeight => playerData.crouchColliderHeight;
        public float CrouchColliderCenter => playerData.crouchColliderCenter;
        public float StandColliderHeight => playerData.standColliderHeight;
        public float StandColliderCenter => playerData.standColliderCenter;

        public float GroundCheckRadius => playerData.groundCheckRadius;
        public float InterCheckDistance => playerData.interCheckDistance;
        public float InterCheckSphereRadius => playerData.interCheckSphereRadius;

        [FormerlySerializedAs("interactionData")] public InteractionObject interactionObject = null;
    }
}