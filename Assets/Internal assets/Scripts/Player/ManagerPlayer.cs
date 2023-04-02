﻿using System;
using UnityEngine;

namespace Player
{
    public class ManagerPlayer : MonoBehaviour
    {
        public static ManagerPlayer Instance { get; private set; }

        [NonSerialized] public GameObject player;
        [NonSerialized] public Transform playerTransform;
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