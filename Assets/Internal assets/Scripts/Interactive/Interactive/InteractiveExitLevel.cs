using Enemy;
using Scene;
using UnityEngine;

namespace Interactive.Interactive
{
    public class InteractiveExitLevel : MonoBehaviour, IInteractive
    {
        private bool _isInteractable;
        [SerializeField] private InteractiveObject interactiveObject;
        InteractiveObject IInteractive.InteractiveObject => interactiveObject;

        private void Start()
        {
            _isInteractable = false;
            EnemiesController.AllEnemiesAreDead += () => _isInteractable = true;
        }


        public void OnInteract()
        {
            SceneController.SwitchScene(SceneType.Game);
        }

        public void OnTake()
        {
            throw new System.NotImplementedException();
        }
    }
}