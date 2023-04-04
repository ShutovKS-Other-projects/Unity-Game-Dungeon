using UnityEngine;
using Weapon;

namespace Manager
{
    public class ManagerWeapon : MonoBehaviour
    {
        public static ManagerWeapon Instance { get; private set; }

        public static WeaponType WeaponType =>
            Resources.Load<ChosenWeaponObject>($"ScriptableObject/Weapon/ChosenWeaponData").weaponType;

        public delegate void SwitchCollider(bool value);

        public event SwitchCollider OnSwitchCollider;

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

        public void OnSwitchColliderWeapon(bool value) => OnSwitchCollider?.Invoke(value);

        public static void ChooseWeapon(WeaponType weaponType)
        {
            Resources.Load<ChosenWeaponObject>($"ScriptableObject/Weapon/ChosenWeaponData").weaponType = weaponType;
        }
    }
}