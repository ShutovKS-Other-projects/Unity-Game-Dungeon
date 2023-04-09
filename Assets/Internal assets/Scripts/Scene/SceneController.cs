using System;
using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scene
{
    public class SceneController : MonoBehaviour
    {
        public static SceneType currentSceneType;
        public static Action OnNewSceneLoaded;

        private void Start()
        {
            SwitchCursor(false);
        }

        public static void SwitchCursor(bool isLocked)
        {
            Cursor.visible = isLocked;
            Cursor.lockState = isLocked ? CursorLockMode.None : CursorLockMode.Locked;
        }

        public static void SwitchScene(SceneType sceneType)
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
                    SceneManager.LoadScene($"InitialScene");
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