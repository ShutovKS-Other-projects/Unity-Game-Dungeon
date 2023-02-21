using UnityEngine;
namespace Interactable
{
    [CreateAssetMenu(fileName = "Interaction Data", menuName = "InteractionSystem/InteractionData", order = 0)]
    public class InteractionData : ScriptableObject
    {
        private InteractableBase _interactable;

        public InteractableBase Interactable
        {
            get => _interactable;
            set => _interactable = value;
        }

        public void Interact()
        {
            if (_interactable == null)
                return;
            
            _interactable.OnInteract();
            ResetData();
        }

        public bool IsSameInteractable(InteractableBase newInteractable) => _interactable == newInteractable;
        public bool IsEmpy() => _interactable == null;
        public void ResetData() => _interactable = null;
    }
}
