using UnityEngine;
using UnityEngine.SceneManagement;

namespace Interactable.Interactable
{
    public class InteractablePortalHome : InteractableBase
    {
        public override void OnInteract()
        {
            SceneManager.LoadScene($"InitialScene");
        }
    }
}