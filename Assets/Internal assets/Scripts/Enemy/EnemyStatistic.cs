using UnityEngine;

namespace Enemy
{
    public class EnemyStatistic
    {
        private readonly EnemyData _data;

        #region Parameters public

        public string RaceName => _data.raceName;

        public int Level { get; }
        public int Experience => Level * 100;

        public float MaxHealth => _data.maxHealth;

        public float Health { get; set; }

        public float MovementSpeed => _data.movementSpeed;
        public float AttackDamage => Random.Range(_data.attackDamage[0], _data.attackDamage[1]);
        public float AttackRetryTime => Random.Range(_data.attackRetryTime[0], _data.attackRetryTime[1]);
        public float AttackDistance => _data.attackDistance;
        public float PlayerCheckDistance => _data.playerCheckDistance;
        public bool IsDead => Health <= 0;

        #endregion

        #region Constructor

        public EnemyStatistic(EnemyData data)
        {
            _data = data;
            Health = data.maxHealth;
            
            var playerLevelStatistic = GameObject.FindWithTag("Player").GetComponent<Player.PlayerStatistic>();
            var playerLevel = playerLevelStatistic.Level;
            Level = playerLevel < 3 ? playerLevel + Random.Range(0, 2) : playerLevel + Random.Range(-2, 3);
        }

        #endregion
    }
}