using Manager;
using UnityEngine;
using Weapon;

namespace Interactable.Interactable
{
    public class InteractableChoosingWeapon : InteractableBase
    {
        [SerializeField] private WeaponType weaponType;

        public override void OnInteract()
        {
            base.OnInteract();
            WeaponController.ChooseWeapon(weaponType);
        }
    }
}