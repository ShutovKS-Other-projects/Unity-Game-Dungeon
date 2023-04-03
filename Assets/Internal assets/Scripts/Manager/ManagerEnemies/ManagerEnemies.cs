using System;
using System.Collections.Generic;
using UnityEngine;

namespace ManagerEnemies
{
    public class ManagerEnemies : MonoBehaviour
    {
        public static ManagerEnemies Instance { get; private set; }

        public Action AllEnemiesAreDead;
        private readonly Dictionary<int, GameObject> _enemies = new();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            AllEnemiesAreDead += () => Debug.Log("AllEnemiesAreDead");
        }

        public void AddEnemy(GameObject enemy)
        {
            _enemies.Add(enemy.GetInstanceID(), enemy);
        }

        public void RemoveEnemy(GameObject enemy)
        {
            _enemies.Remove(enemy.GetInstanceID());
            
            if(_enemies.Count == 0)
                AllEnemiesAreDead?.Invoke();
        }
    }
}