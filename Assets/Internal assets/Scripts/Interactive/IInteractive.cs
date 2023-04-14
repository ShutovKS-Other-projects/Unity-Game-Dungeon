using JetBrains.Annotations;
using UnityEngine;

namespace Interactive
{
    public interface IInteractive
    {
        protected InteractiveObject InteractiveObject { get; }

        public bool IsInteract => InteractiveObject.isInteract;
        public bool IsGrab => InteractiveObject.isGrab;
        
        [CanBeNull] public string TooltipTextInteract => InteractiveObject.tooltipTextInteract;
        [CanBeNull] public string TooltipTextTake => InteractiveObject.tooltipTextTake;

        public void OnInteract()
        {
            Debug.Log("No interact");
        }

        public void OnGrab()
        {
            Debug.Log("No grab");
        }
    }
}