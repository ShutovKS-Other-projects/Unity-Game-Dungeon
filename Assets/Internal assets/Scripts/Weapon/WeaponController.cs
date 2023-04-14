using System;
using Player;
using Scene;
using UnityEngine;

namespace Weapon
{
    public class WeaponController : MonoBehaviour
    {
        public static WeaponType WeaponType =>
            Resources.Load<ChosenWeaponObject>($"ScriptableObject/Weapon/ChosenWeaponData").weaponType;

        public delegate void SwitchCollider(bool value);

        public static event SwitchCollider OnSwitchCollider;
        
        public static void OnSwitchColliderWeapon(bool value) => OnSwitchCollider?.Invoke(value);

        public static void ChooseWeapon(WeaponType weaponType)
        {
            Resources.Load<ChosenWeaponObject>($"ScriptableObject/Weapon/ChosenWeaponData").weaponType = weaponType;
        }
    }
}