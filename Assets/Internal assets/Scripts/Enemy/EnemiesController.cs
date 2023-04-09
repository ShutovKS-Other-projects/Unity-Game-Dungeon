using System;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemiesController : MonoBehaviour
    {
        public static Action AllEnemiesAreDead;
        private static Dictionary<int, GameObject> _enemies = new();

        private void Awake()
        {
            AllEnemiesAreDead += () => Debug.Log("AllEnemiesAreDead");
            // ManagerScene.Instance.OnNewSceneLoaded += TODO: Generation Enemies
        }

        public static void AddEnemy(GameObject enemy)
        {
            _enemies.Add(enemy.GetInstanceID(), enemy);
        }

        public static void RemoveEnemy(GameObject enemy)
        {
            _enemies.Remove(enemy.GetInstanceID());

            if (_enemies.Count == 0)
                AllEnemiesAreDead?.Invoke();
        }
    }
}