using UnityEngine;

[CreateAssetMenu(fileName = "newEnemyData", menuName = "Data/Enemy Data/Data Base")]
public class EnemyData : ScriptableObject
{
    [Header("Enemy Name")]
    public string raceName = "Enemy";

    [Header("Enemy Health")]
    public float health = 100f;
    public float healthRecoverySpeed = 10f;
    
    [Header("Enemy Movement")]
    public float movementSpeed = 10f;


    [Header("Enemy Attack")]
    public float[] attackDamage = new float[2] { 7.5f, 15f };
    public float[] attackRetryTime = new float[2] { 2f, 3f };
    public float attackDistance = 1f;

    [Header("IsStates")]
    public bool isVisiblePlyer = false;
    public bool isPlayer = false;

    [Header("Player Check")]
    public LayerMask playerLayer = LayerMask.GetMask("Player");
    public GameObject playerGameObject;
    public float playerCheckDistance = 3f;
    public float playerCheckSphereRadius = 0.5f;
}