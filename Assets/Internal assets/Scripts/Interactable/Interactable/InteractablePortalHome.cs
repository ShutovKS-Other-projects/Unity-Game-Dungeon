using Manager;
using Scene;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Interactable.Interactable
{
    public class InteractablePortalHome : InteractableBase
    {
        public override void OnInteract()
        {
            SceneController.SwitchScene(SceneType.StartGame);
        }
    }
}