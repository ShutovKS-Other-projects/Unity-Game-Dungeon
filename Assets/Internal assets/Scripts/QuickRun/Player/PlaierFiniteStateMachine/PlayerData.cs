using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data/Data Base")]
public class PlayerData : ScriptableObject
{
    public float movementVelocity = 6f;
    public float jumpVelocity = 10f;
    public float jumpVelocityHold = 2f;
    
    public float groundCheckRadius = 0.2f;
    public LayerMask whatIsGround;

    public float stamina = 100f;
    public bool isFatigue = false;
}
