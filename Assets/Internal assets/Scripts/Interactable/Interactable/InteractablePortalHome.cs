using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Interactable.Interactable
{
    public class InteractablePortalHome : InteractableBase
    {
        public override void OnInteract()
        {
            ManagerScene.Instance.SwitchScene(SceneType.StartGame);
        }
    }
}