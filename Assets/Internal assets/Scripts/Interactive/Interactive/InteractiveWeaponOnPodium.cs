using System;
using Unity.VisualScripting;
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
        public WeaponTransformObject WeaponTransformObject => weaponTransformObject;
        [SerializeField] private WeaponTransformObject weaponTransformObject;

        private static event Action SwitchWeapon;

        private void Start()
        {
            SwitchWeapon += EnableWeapon;
        }

        public void OnGrab()
        {
            if (WeaponController.WeaponType == weaponType) return;

            GrabsController.RemoveChildrens();
            WeaponController.ChooseWeapon(weaponType);

            SwitchWeapon?.Invoke();

            foreach (var variable in GameObject.FindGameObjectsWithTag("ObjectDamaging")) Destroy(variable);
            var weapon = Instantiate(Resources.Load<GameObject>($"Weapon/{weaponType}")).transform;

            GrabsController.GrabRight(weapon, weaponTransformObject);
            weapon.AddComponent<ObjectDamage>();
        }

        public void OnGrabXR(SideType sideType)
        {
            if (WeaponController.WeaponType == weaponType) return;

            GrabsController.RemoveChildrens();
            WeaponController.ChooseWeapon(weaponType);
            SwitchWeapon?.Invoke();

            foreach (var variable in GameObject.FindGameObjectsWithTag("ObjectDamaging")) Destroy(variable);
            var weapon = Instantiate(Resources.Load<GameObject>($"Weapon/{weaponType}")).transform;
            switch (sideType)
            {
                case SideType.Left:
                {
                    GrabsController.GrabLeft(weapon, weaponTransformObject);
                    break;
                }
                case SideType.Right:
                {
                    GrabsController.GrabRight(weapon, weaponTransformObject);
                    break;
                }
            }

            weapon.AddComponent<ObjectDamage>();
        }

        private void EnableWeapon() =>
            transform.GetChild(0).gameObject.SetActive(weaponType != WeaponController.WeaponType);
    }
}