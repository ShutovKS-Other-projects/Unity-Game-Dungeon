using Scene;
using UnityEngine;

namespace Interactive.Interactive
{
    public class InteractivePortalHome : MonoBehaviour, IInteractive
    {
        [SerializeField] private InteractiveObject interactiveObject;
        InteractiveObject IInteractive.InteractiveObject => interactiveObject;

        public void OnInteract()
        {
            SceneController.SwitchScene(SceneType.StartGame);
        }

        public void OnTake()
        {
            throw new System.NotImplementedException();
        }
    }
}