using UnityEngine;

namespace Manager
{
    public class ManagerManagers : MonoBehaviour
    {
        public static ManagerManagers Instance { get; private set; }
        
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
            
            DontDestroyOnLoad(this.gameObject);
        }
    }
}