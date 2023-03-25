using UnityEngine;
using UnityEngine.SceneManagement;

namespace Interactable.Interactable
{
    public class PortalHome : InteractableBase
    {
        public override void OnInteract()
        {
            SceneManager.LoadScene($"InitialScene");
        }
    }
}