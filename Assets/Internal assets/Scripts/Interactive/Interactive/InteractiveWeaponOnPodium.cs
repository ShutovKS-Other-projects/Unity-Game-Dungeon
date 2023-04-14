using System;
using UnityEngine;
using Weapon;

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
            GrabsController.RemoveChildrenRight();
            WeaponController.ChooseWeapon(weaponType);

            SwitchWeapon?.Invoke();

            var weapon = Instantiate(Resources.Load<GameObject>($"Weapon/{weaponType}")).transform;
            GrabsController.GrabRight(weapon);
        }

        private void EnableWeapon() =>
            transform.GetChild(0).gameObject.SetActive(weaponType != WeaponController.WeaponType);
    }
}