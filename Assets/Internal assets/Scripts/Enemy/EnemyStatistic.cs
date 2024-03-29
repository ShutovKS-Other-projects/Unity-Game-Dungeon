﻿using System;
using Enemy.FiniteStateMachine;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy
{
    public class EnemyStatistic : MonoBehaviour
    {
        private readonly EnemyData _data;
        private readonly EnemyStateController _stateController;

        #region Parameters private

        private float _health;

        #endregion

        #region Parameters public

        public string RaceName => _data.raceName;

        public float MaxHealth => _data.maxHealth;

        public float Health
        {
            get => _health;
            set
            {
                _health = value;
                // _stateController.StrengthAttackFloat!();
            }
        }

        public float MovementSpeed => _data.movementSpeed;
        public float AttackDamage => Random.Range(_data.attackDamage[0], _data.attackDamage[1]);
        public float AttackRetryTime => Random.Range(_data.attackRetryTime[0], _data.attackRetryTime[1]);
        public float AttackDistance => _data.attackDistance;
        public float PlayerCheckDistance => _data.playerCheckDistance;

        public bool IsDead => Health <= 0;
        [NonSerialized] public bool isVisiblePlayer;

        // public AnimatorController AnimatorController => _data.animatorController;

        #endregion

        #region Constructor

        public EnemyStatistic(EnemyData data, EnemyStateController stateController)
        {
            _data = data;
            _stateController = stateController;
            Health = data.maxHealth;
        }

        #endregion
    }
}