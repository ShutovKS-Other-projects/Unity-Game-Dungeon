using UnityEngine;
namespace Internal_assets.Scripts.QuickRun.Enemy
{
    public class EnemyStatistic
    {
        private readonly EnemyData _data;

        #region Parameters private

        private float _health;

        #endregion

        #region Parameters public

        public string raceName { get { return _data.raceName; } }
        public float maxHealth { get { return _data.maxHealth; } }
        public float health { get { return _health; } set { _health = value; } }
        public float movementSpeed { get { return _data.movementSpeed; } }
        public float attackDamage { get { return Random.Range(_data.attackDamage[0], _data.attackDamage[1]); } }
        public float attackRetryTime { get { return Random.Range(_data.attackRetryTime[0], _data.attackRetryTime[1]); } }
        public float attackDistance;
        public float playerCheckDistance { get { return _data.playerCheckDistance; } }
        public bool isDead { get { return health <= 0; } }
        #endregion

        #region Constructor
        public EnemyStatistic(EnemyData data)
        {
            _data = data;
            _health = data.maxHealth;
        }
        #endregion
    }
}
