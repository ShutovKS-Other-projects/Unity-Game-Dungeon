using UnityEngine;
using Weapon;
using XR;

namespace Interactive.Interactive
{
    public class InteractiveWeapon : MonoBehaviour, IInteractive
    {
        [SerializeField] private InteractiveObject interactiveObject;
        InteractiveObject IInteractive.InteractiveObject => interactiveObject;

        public void OnGrab()
        {
            GrabsController.GrabRight(transform);
        }
        
        public void OnGrabXR(SideType sideType)
        {
            switch (sideType)
            {
                case SideType.Left:
                {
                    GrabsController.GrabLeft(transform);
                    break;
                }
                case SideType.Right:
                {
                    GrabsController.GrabRight(transform);
                    break;
                }
            }
        }
    }
}