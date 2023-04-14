using Enemy;
using Scene;
using UnityEngine;
using XR;

namespace Interactive.Interactive
{
    public class InteractiveExitLevel : MonoBehaviour, IInteractive
    {
        [SerializeField] private InteractiveObject interactiveObject;
        InteractiveObject IInteractive.InteractiveObject => interactiveObject;
        
        private void Start()
        {
            // EnemiesController.AllEnemiesAreDead += () => _isInteractable = true;
        }

        public void OnInteract()
        {
            SceneController.SwitchScene(SceneType.Game);
        }
        public void OnInteractXR(SideType sideType)
        {
            SceneController.SwitchScene(SceneType.Game);
        }

    }
}