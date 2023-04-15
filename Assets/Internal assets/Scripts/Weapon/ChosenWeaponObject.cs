using System;
using UnityEngine;

namespace Weapon
{
    [CreateAssetMenu(fileName = "ChosenWeaponObject", menuName = "Data/Weapon/ChosenWeaponObject Data", order = 0)]
    public class ChosenWeaponObject : ScriptableObject
    {
        public WeaponType weaponType;

        private void OnDisable()
        {
            weaponType = WeaponType.Other;
        }
    }
}
