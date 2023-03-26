using Interactable;
using UnityEngine;

namespace Player.Home
{
    public class PlayerStatistic : MonoBehaviour
    {
        [SerializeField] private PlayerData playerData;

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

        public InteractionData interactionData = null;
    }
}