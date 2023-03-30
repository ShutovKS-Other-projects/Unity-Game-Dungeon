using UnityEngine;

namespace Weapon
{
    public class ManagerWeapon : MonoBehaviour
    {
        public static ManagerWeapon Instance;
        public void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public static void ChooseWeapon(WeaponType weaponType)
        {
            Resources.Load<ChosenWeaponObject>($"ScriptableObject/Weapon/ChosenWeaponData").weaponType = weaponType;
        }
    }
}