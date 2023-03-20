using UnityEngine;

namespace Interactable.Interactable
{
    public class PortalHome : InteractableBase
    {
        public override void OnInteract()
        {
            Debug.Log("Interacted with " + gameObject.name);
        }
    }
}