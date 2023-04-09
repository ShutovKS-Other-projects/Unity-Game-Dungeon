using System;
using Scene;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [NonSerialized] public static GameObject player;
        [NonSerialized] public static Transform playerTransform;
        public static Vector3 PlayerPosition => playerTransform!.position;

        private void Awake()
        {
            FindPlayer();
            SceneController.OnNewSceneLoaded += () => { playerTransform!.position = new Vector3(0, 0, 0); };
        }

        private static void FindPlayer()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerTransform = player!.transform;
        }
    }
}