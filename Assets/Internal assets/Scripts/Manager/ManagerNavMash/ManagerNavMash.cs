using Unity.AI.Navigation;
using UnityEngine;

namespace Manager
{
    public class ManagerNavMash : MonoBehaviour
    {
        public static ManagerNavMash Instance { get; private set; }
        public static NavMeshSurface NavMeshSurface { get; private set; }

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

            FindNavMeshSurface();
            ManagerScene.Instance.OnNewSceneLoaded += FindNavMeshSurface;
        }

        private static void FindNavMeshSurface()
        {
            NavMeshSurface = FindObjectOfType<NavMeshSurface>();
        }
    }
}