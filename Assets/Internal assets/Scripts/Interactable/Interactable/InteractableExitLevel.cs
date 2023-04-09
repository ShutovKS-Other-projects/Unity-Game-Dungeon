using Enemy;
using Manager;
using Scene;

namespace Interactable.Interactable
{
    public class InteractableExitLevel : InteractableBase
    {
        private void Start()
        {
            isInteractable = false;
            EnemiesController.AllEnemiesAreDead += () => isInteractable = true;
        }

        public override void OnInteract()
        {
            base.OnInteract();
            
            SceneController.SwitchScene(SceneType.Game);
        }
    }
}
