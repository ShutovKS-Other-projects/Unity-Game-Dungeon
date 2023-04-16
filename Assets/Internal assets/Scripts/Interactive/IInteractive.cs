using JetBrains.Annotations;
using UnityEngine;
using Weapon;
using XR;

namespace Interactive
{
    public interface IInteractive
    {
        protected InteractiveObject InteractiveObject { get; }
        public WeaponTransformObject WeaponTransformObject { get; }
        public bool IsInteract => InteractiveObject.isInteract;
        public bool IsGrab => InteractiveObject.isGrab;

        [CanBeNull] public string TooltipTextInteract => InteractiveObject.tooltipTextInteract;
        [CanBeNull] public string TooltipTextTake => InteractiveObject.tooltipTextTake;

        public void OnInteract()
        {
            Debug.Log("No interact");
        }

        public void OnInteractXR(SideType sideType)
        {
            Debug.Log("No interact XR");
        }

        public void OnGrab()
        {
            Debug.Log("No grab");
        }

        public void OnGrabXR(SideType sideType)
        {
            Debug.Log("No grab XR");
        }
    }
}