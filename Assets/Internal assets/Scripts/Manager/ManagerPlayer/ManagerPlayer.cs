using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace Manager
{
    public class ManagerPlayer : MonoBehaviour
    {
        public static ManagerPlayer Instance { get; private set; }

        [NonSerialized] [CanBeNull] public GameObject player;
        [NonSerialized] [CanBeNull] public Transform playerTransform;
        public Vector3 PlayerPosition => playerTransform!.position;

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

            FindPlayer();
            ManagerScene.Instance.OnNewSceneLoaded += () => { playerTransform!.position = new Vector3(0, 0, 0); };
        }

        private void FindPlayer()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerTransform = player!.transform;
        }
    }
}