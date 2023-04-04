using UnityEngine;
using Weapon;

namespace Manager
{
    public class ManagerWeapon : MonoBehaviour
    {
        public static ManagerWeapon Instance { get; private set; }

        public static WeaponType WeaponType =>
            Resources.Load<ChosenWeaponObject>($"ScriptableObject/Weapon/ChosenWeaponData").weaponType;

        public delegate void SwitchTriggerCollider(bool value);

        public event SwitchTriggerCollider OnSwitchTriggerCollider;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        public void OnSwitchTriggerColliderWeapon(bool value) => OnSwitchTriggerCollider?.Invoke(value);

        public static void ChooseWeapon(WeaponType weaponType)
        {
            Resources.Load<ChosenWeaponObject>($"ScriptableObject/Weapon/ChosenWeaponData").weaponType = weaponType;
        }
    }
}