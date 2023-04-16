using UnityEngine;
using Weapon;
using XR;

namespace Interactive.Interactive
{
    public class InteractiveWeapon : MonoBehaviour, IInteractive
    {
        [SerializeField] private InteractiveObject interactiveObject;
        InteractiveObject IInteractive.InteractiveObject => interactiveObject;
        public WeaponTransformObject WeaponTransformObject => weaponTransformObject;
        [SerializeField] private WeaponTransformObject weaponTransformObject;

        public void OnGrab()
        {
            GrabsController.GrabRight(transform, weaponTransformObject);
        }

        public void OnGrabXR(SideType sideType)
        {
            switch (sideType)
            {
                case SideType.Left:
                {
                    GrabsController.GrabLeft(transform, weaponTransformObject);
                    break;
                }
                case SideType.Right:
                {
                    GrabsController.GrabRight(transform, weaponTransformObject);
                    break;
                }
            }
        }
    }
}