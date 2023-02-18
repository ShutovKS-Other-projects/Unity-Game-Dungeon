using System.Collections;
using System.Collections.Generic;
using Internal_assets.Scripts.QuickRun.Interactable;
using Internal_assets.Scripts.QuickRun.Player;
using UnityEngine;

public class PlayerStatistic
{
    private readonly PlayerData _playerData;
    
    #region Parameters private
    private float _health;
    private float _stamina;
    private float _experience;
    #endregion
    
    #region Parameters public
    public string ClassName { get { return _playerData.className; } }
    
    public float Health { get { return _health; } set { _health = value; } }
    public float MaxHealth { get { return _playerData.maxHealth; } }
    public float HealthRecoverySpeed { get { return _playerData.healthRecoverySpeed; } }
    
    public float Stamina { get { return _stamina; } set { _stamina = value; } }
    public float MaxStamina { get { return _playerData.maxStamina; } }
    public float StaminaRecoverySpeed { get { return _playerData.staminaRecoverySpeed; } }
    public float StaminaRecoverySpeedIsFatigue { get { return _playerData.staminaRecoverySpeedIsFatigue; } }
    
    public float Experience { get { return _experience; } set { _experience = value; } }
    
    public float Damage { get { return _playerData.damage; } }
    
    public float MovementForce { get { return _playerData.movementForce; } }
    public float JumpSpeed { get { return _playerData.jumpSpeed; } }
    public float MovementSpeedMax { get { return _playerData.movementSpeedMax; } }
    public float RunMovementSpeedMax { get { return _playerData.runMovementSpeedMax; } }
    public float InAirMovementSpeedMax { get { return _playerData.inAirMovementSpeedMax; } }
    public float CrouchMovementSpeedMax { get { return _playerData.crouchMovementSpeedMax; } }
    public float BlockMovementSpeedMax { get { return _playerData.blockMovementSpeedMax; } }
    
    public float CrouchColliderCenter { get { return _playerData.crouchColliderCenter; } }
    public float CrouchColliderHeight { get { return _playerData.crouchColliderHeight; } }
    public float StandColliderCenter { get { return _playerData.standColliderCenter; } }
    public float StandColliderHeight { get { return _playerData.standColliderHeight; } }
    
    public float GroundCheckRadius { get { return _playerData.groundCheckRadius; } }
    public float InterCheckDistance { get { return _playerData.interCheckDistance; } }
    public float InterCheckSphereRadius { get { return _playerData.interCheckSphereRadius; } }
    
    public bool IsAbility;
    public bool IsFatigue;
    
    public InteractionData InteractionData { get { return _playerData.interactionData; } set { _playerData.interactionData = value; } }
    #endregion
    
    public PlayerStatistic(PlayerData playerData)
    {
        _playerData = playerData;
        _health = _playerData.maxHealth;
        _stamina = _playerData.maxStamina;
        _experience = 0f;
    }
}
