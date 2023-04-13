using UnityEngine;
using Weapon;

namespace Interactive.Interactive
{
    public class InteractiveWeapon : MonoBehaviour, IInteractive
    {
        [SerializeField] private WeaponType weaponType;
        [SerializeField] private InteractiveObject interactiveObject;
        InteractiveObject IInteractive.InteractiveObject => interactiveObject;

        public void OnInteract()
        {
            WeaponController.ChooseWeapon(weaponType);
        }

        public void OnTake()
        {
        }
    }
}