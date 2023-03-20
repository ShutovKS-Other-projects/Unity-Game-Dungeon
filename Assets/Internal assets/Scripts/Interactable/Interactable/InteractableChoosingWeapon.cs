using UnityEngine;

namespace Interactable.Interactable
{
    public class InteractableChoosingWeapon : InteractableBase
    {
        public override void OnInteract()
        {
            base.OnInteract();
            Debug.Log("InteractableChoosingWeapon");
        }
        
        
    }
}
