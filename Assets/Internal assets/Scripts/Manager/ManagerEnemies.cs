using System;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public class ManagerEnemies : MonoBehaviour
    {
        public static ManagerEnemies Instance { get; private set; }

        public Action AllEnemiesAreDead;
        private readonly Dictionary<int, GameObject> _enemies = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }

            AllEnemiesAreDead += () => Debug.Log("AllEnemiesAreDead");
            // ManagerScene.Instance.OnNewSceneLoaded += TODO: Generation Enemies
        }

        public void AddEnemy(GameObject enemy)
        {
            _enemies.Add(enemy.GetInstanceID(), enemy);
        }

        public void RemoveEnemy(GameObject enemy)
        {
            _enemies.Remove(enemy.GetInstanceID());

            if (_enemies.Count == 0)
                AllEnemiesAreDead?.Invoke();
        }
    }
}