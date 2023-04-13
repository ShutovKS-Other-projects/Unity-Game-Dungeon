using UnityEngine;

namespace Interactive
{
    [CreateAssetMenu(fileName = "new Interaction", menuName = "Data/Interactive/InteractionObject Data", order = 0)]
    public class InteractionObject : ScriptableObject
    {
        public IInteractive Interactive { get; set; }

        public void Interact()
        {
            if (Interactive == null)
                return;

            Interactive.OnInteract();
            ResetData();
        }
        
        public void Take()
        {
            if (Interactive == null)
                return;

            Interactive.OnTake();
            ResetData();
        }

        public bool IsSameInteractable(IInteractive newInteractable) => Interactive == newInteractable;
        public bool IsEmpty() => Interactive == null;
        public void ResetData() => Interactive = null;
    }
}