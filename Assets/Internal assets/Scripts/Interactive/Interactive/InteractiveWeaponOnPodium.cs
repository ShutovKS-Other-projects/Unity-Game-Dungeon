using System;
using UnityEngine;
using Weapon;
using XR;

namespace Interactive.Interactive
{
    public class InteractiveWeaponOnPodium : MonoBehaviour, IInteractive
    {
        [SerializeField] private WeaponType weaponType;

        [SerializeField] private InteractiveObject interactiveObject;
        InteractiveObject IInteractive.InteractiveObject => interactiveObject;

        private static event Action SwitchWeapon;

        private void Start()
        {
            SwitchWeapon += EnableWeapon;
        }

        public void OnGrab()
        {
            GrabsController.RemoveChildrens();
            WeaponController.ChooseWeapon(weaponType);

            SwitchWeapon?.Invoke();

            var weapon = Instantiate(Resources.Load<GameObject>($"Weapon/{weaponType}")).transform;
            GrabsController.GrabRight(weapon);
        }
        
        public void OnGrabXR(SideType sideType)
        {
            switch (sideType)
            {
                case SideType.Left:
                {
                    GrabsController.RemoveChildrens();
                    WeaponController.ChooseWeapon(weaponType);

                    SwitchWeapon?.Invoke();

                    var weapon = Instantiate(Resources.Load<GameObject>($"Weapon/{weaponType}")).transform;
                    GrabsController.GrabLeft(weapon);
                    break;
                }
                case SideType.Right:
                {
                    GrabsController.RemoveChildrens();
                    WeaponController.ChooseWeapon(weaponType);

                    SwitchWeapon?.Invoke();

                    var weapon = Instantiate(Resources.Load<GameObject>($"Weapon/{weaponType}")).transform;
                    GrabsController.GrabRight(weapon);
                    break;
                }
            }
        }

        private void EnableWeapon() =>
            transform.GetChild(0).gameObject.SetActive(weaponType != WeaponController.WeaponType);
    }
}