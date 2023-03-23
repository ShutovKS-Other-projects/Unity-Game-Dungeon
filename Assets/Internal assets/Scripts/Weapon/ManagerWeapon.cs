using System;
using UnityEngine;

namespace Weapon
{
    public class ManagerWeapon : MonoBehaviour
    {
        public static ManagerWeapon Instance;
        public void Awake() => Instance = this;
        
        private WeaponType _weaponType;
        
        public void ChooseWeapon(WeaponType weaponType)
        {
            _weaponType = weaponType;
            Debug.Log($"You chose {weaponType}");
        }
    }
}