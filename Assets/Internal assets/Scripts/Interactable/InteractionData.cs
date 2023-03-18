using UnityEngine;
namespace Interactable
{
    [CreateAssetMenu(fileName = "Interaction Data", menuName = "InteractionSystem/InteractionData", order = 0)]
    public class InteractionData : ScriptableObject
    {
        public InteractableBase Interactable { get; set; }

        public void Interact()
        {
            if (Interactable == null)
                return;
            
            Interactable.OnInteract();
            ResetData();
        }

        public bool IsSameInteractable(InteractableBase newInteractable) => Interactable == newInteractable;
        public bool IsEmpty() => Interactable == null;
        public void ResetData() => Interactable = null;
    }
}
