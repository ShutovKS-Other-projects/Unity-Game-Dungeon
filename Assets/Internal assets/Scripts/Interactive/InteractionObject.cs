using UnityEngine;

namespace Interactive
{
    [CreateAssetMenu(fileName = "new Interaction", menuName = "Data/Interactive/InteractionObject Data", order = 0)]
    public class InteractionObject : ScriptableObject
    {
        public IInteractive Interactive { get; set; }

        public bool TryInteract()
        {
            if (Interactive == null)
                return false;

            Interactive.OnInteract();
            ResetData();
            return true;
        }

        public bool TryTake()
        {
            if (Interactive == null)
                return false;

            Interactive.OnGrab();
            ResetData();
            return true;
        }

        public bool IsSameInteractable(IInteractive newInteractable) => Interactive == newInteractable;
        public bool IsEmpty() => Interactive == null;
        public void ResetData() => Interactive = null;
    }
}