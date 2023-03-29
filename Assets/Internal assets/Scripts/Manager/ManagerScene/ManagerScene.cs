using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace ManagerScene
{
    public class ManagerScene : MonoBehaviour
    {
        public SceneType currentSceneType;

        private void Start()
        {
            SwitchCursor(false);
        }
        
        public static void SwitchCursor(bool isLocked)
        {
            Cursor.visible = isLocked;
            Cursor.lockState = isLocked ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}