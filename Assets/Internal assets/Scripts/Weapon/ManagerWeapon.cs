using System;
using UnityEngine;

namespace Weapon
{
    public class ManagerWeapon : MonoBehaviour
    {
        public static ManagerWeapon Instance;

        public delegate void SwitchTriggerCollider(bool value);

        public event SwitchTriggerCollider OnSwitchTriggerCollider;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {

        }

        public void OnSwitchTriggerColliderWeapon(bool value) => OnSwitchTriggerCollider.Invoke(value);

        public static void ChooseWeapon(WeaponType weaponType)
        {
            Resources.Load<ChosenWeaponObject>($"ScriptableObject/Weapon/ChosenWeaponData").weaponType = weaponType;
        }
    }
}