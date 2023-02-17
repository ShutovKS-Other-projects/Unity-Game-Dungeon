using UnityEngine;
namespace Internal_assets.Scripts.QuickRun.Enemy
{
    [CreateAssetMenu(fileName = "newEnemyData", menuName = "Data/Enemy Data/Data Base")]
    public class EnemyData : ScriptableObject
    {
        [Header("Enemy Name")]
        public string raceName;

        [Header("Enemy Health")]
        public static float maxHealth;
        public float health = maxHealth;
    
        [Header("Enemy Movement")]
        public float movementSpeed;

        [Header("Enemy Attack")]
        public float[] attackDamage = new float[2];
        public float[] attackRetryTime = new float[2];
        public float attackDistance;

        [Header("Player Check")]
        public float playerCheckDistance;

        public bool isDead { get { return health <= 0; } }
        
        public RuntimeAnimatorController AnimatorController;
    }
}