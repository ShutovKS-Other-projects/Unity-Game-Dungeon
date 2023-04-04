using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager
{
    public class ManagerScene : MonoBehaviour
    {
        public static ManagerScene Instance { get; private set; }
        public static SceneType currentSceneType;
        public Action OnNewSceneLoaded;

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
        }

        private void Start()
        {
            SwitchCursor(false);
        }

        public static void SwitchCursor(bool isLocked)
        {
            Cursor.visible = isLocked;
            Cursor.lockState = isLocked ? CursorLockMode.None : CursorLockMode.Locked;
        }

        public void SwitchScene(SceneType sceneType)
        {
            switch (sceneType)
            {
                case SceneType.Home:
                    currentSceneType = SceneType.Home;
                    SceneManager.LoadScene($"HomeScene");
                    break;
                case SceneType.StartGame:
                    currentSceneType = SceneType.StartGame;
                    SceneManager.LoadScene($"InitialScene");
                    break;
                case SceneType.Game:
                    currentSceneType = SceneType.Game;
                    break;
                case SceneType.Boss:
                    currentSceneType = SceneType.Boss;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sceneType), sceneType, null);
            }

            OnNewSceneLoaded.Invoke();
        }
    }
}