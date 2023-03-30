using UnityEngine;

namespace Interactable
{
    [CreateAssetMenu(fileName = "new Interaction", menuName = "Data/Interaction Data", order = 0)]
    public class InteractionObject : ScriptableObject
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