using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data/Data Base")]
public class PlayerData : ScriptableObject
{
    [Header("Movement")]
    public float movementSpeed = 10f;
    public float inAirMovementSpeed = 8f;
    public float jumpSpeed = 5f;
    
    [Header("Ground Check")]
    public float groundCheckRadius = 0.2f;
    public LayerMask whatIsGround;

    [Header("Stamina")]
    public float stamina = 100f;
    public bool isFatigue = false;

    [Header("Crouch")]
    public float crouchMovementSpeed = 8f;
    public float crouchColliderHeight = 1.2f;
    public float standColliderHeight = 1.8f;
    public float crouchColliderCenter = 0.9f;
    public float standColliderCenter = 0.6f;

    [Header("Block")]
    public float blockMovementSpeed = 4f;
}
