using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatistic
{
    #region Parameters private
    [Header("Health")]
    private float maxHealth;
    private float health;
    private float healthRecoverySpeed;

    [Header("Movement")]
    private float movementSpeed;

    [Header("Attack")]
    private float[] attackDamage = new float[2];
    private float[] attackRetryTime = new float[2];
    private float attackDistance;

    [Header("Player Check")]
    private float playerCheckDistance;
    #endregion

    #region Parameters public
    public float MaxHealth { get => maxHealth; }
    public float Health { get => health; set => health = value; }
    public float MovementSpeed { get => movementSpeed; }
    public float[] AttackDamage { get => attackDamage; }
    public float[] AttackRetryTime { get => attackRetryTime; }
    public float AttackDistance { get => attackDistance; }
    public float PlayerCheckDistance { get => playerCheckDistance; }

    public bool IsDead { get => health <= 0; }
    public bool IsVisiblePlayer { get; set; }
    #endregion

    #region Constructor
    public EnemyStatistic(EnemyData enemyData)
    {
        //maxHealth = enemyData.maxHealth;
        //health = maxHealth;
        movementSpeed = enemyData.movementSpeed;
        attackDamage = enemyData.attackDamage;
        attackRetryTime = enemyData.attackRetryTime;
        attackDistance = enemyData.attackDistance;
        playerCheckDistance = enemyData.playerCheckDistance;
    }
    #endregion
}
