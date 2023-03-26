using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player.Game
{
    public class ManagerPlayer : MonoBehaviour
    {
        public static ManagerPlayer Instance { get; private set; }

        public GameObject player;
        public Transform playerTransform;
        public Vector3 PlayerPosition => playerTransform.position;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            player = GameObject.FindGameObjectWithTag("Player");
            playerTransform = player.transform;
        }
    }
}