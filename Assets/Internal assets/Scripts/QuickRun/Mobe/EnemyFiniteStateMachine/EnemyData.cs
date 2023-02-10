using UnityEngine;

[CreateAssetMenu(fileName = "newEnemyData", menuName = "Data/Enemy Data/Data Base")]
public class EnemyData : ScriptableObject
{
    [Header("Enemy Stats Now")]
    public string name = "Enemy";
    public float health = 100f;
    public float healthRecoverySpeed = 10f;
    public float damage = 10f;
    public float movementSpeed = 10f;

    [Header("Enemy Stats Max")]
    public float maxHealth = 100f;
    public float maxStamina = 100f;

    [Header("IsStates")]
    public bool isAbility = false;
    public bool isVisiblePlyer = false;

    [Header("Ground Check")]
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer = LayerMask.GetMask("Ground");

    [Header("Player Check")]
    public GameObject playerGameObject;
    public float playerCheckDistance = 3f;
    public float playerCheckSphereRadius = 0.5f;
}