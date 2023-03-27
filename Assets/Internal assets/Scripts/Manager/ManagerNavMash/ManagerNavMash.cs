using UnityEngine;

namespace ManagerNavMash
{
    public class ManagerNavMash : MonoBehaviour
    {
        public static ManagerNavMash Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}