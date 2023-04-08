using UnityEngine;

namespace Manager
{
    public class ManagerController : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}