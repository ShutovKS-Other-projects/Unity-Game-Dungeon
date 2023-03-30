using UnityEngine;

namespace Weapon
{
    [CreateAssetMenu(fileName = "ChosenWeaponObject", menuName = "Data/ChosenWeaponObject Data", order = 0)]
    public class ChosenWeaponObject : ScriptableObject
    {
        public WeaponType weaponType;
    }
}
