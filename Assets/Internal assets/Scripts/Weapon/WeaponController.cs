using System;
using Player;
using Scene;
using UnityEngine;

namespace Weapon
{
    public class WeaponController : MonoBehaviour
    {
        private static Transform _rWeapon;
        private static Transform _lWeapon;

        public static WeaponType WeaponType =>
            Resources.Load<ChosenWeaponObject>($"ScriptableObject/Weapon/ChosenWeaponData").weaponType;

        public delegate void SwitchCollider(bool value);

        public static event SwitchCollider OnSwitchCollider;

        private void Start()
        {
            _rWeapon = PlayerController.playerTransform!.Find("Weapons").Find("L_Weapon");
            _lWeapon = PlayerController.playerTransform!.Find("Weapons").Find("R_Weapon");
            if (_rWeapon == null && _lWeapon == null) Debug.LogError("RWeapon or LWeapon not found");
            InstantiateWeapon();
            SceneController.OnNewSceneLoaded += InstantiateWeapon;
        }

        public static void OnSwitchColliderWeapon(bool value) => OnSwitchCollider?.Invoke(value);

        public static void ChooseWeapon(WeaponType weaponType)
        {
            Resources.Load<ChosenWeaponObject>($"ScriptableObject/Weapon/ChosenWeaponData").weaponType = weaponType;
        }

        private static void InstantiateWeapon()
        {
            Debug.Log("InstantiateWeapon");

            for (var i = 0; i < _rWeapon.childCount; i++)
                Destroy(_rWeapon.GetChild(i).gameObject);
            for (var i = 0; i < _lWeapon.childCount; i++)
                Destroy(_lWeapon.GetChild(i).gameObject);

            if (SceneController.currentSceneType == SceneType.Home) return;

            GameObject lWeapon = null;
            GameObject rWeapon = null;

            switch (WeaponType)
            {
                case WeaponType.Spear:
                    Debug.Log("No weapon settings");
                    break;
                case WeaponType.BowAndArrow:
                    Debug.Log("No weapon settings");
                    break;
                case WeaponType.SwordAndShield:
                    Debug.Log("No weapon settings");
                    break;
                case WeaponType.Hammer:
                    Debug.Log("No weapon settings");
                    break;
                case WeaponType.TwoSwords:
                    lWeapon = Instantiate(Resources.Load<GameObject>($"Weapon/Sword"), _lWeapon);
                    rWeapon = Instantiate(Resources.Load<GameObject>($"Weapon/Sword"), _rWeapon);
                    break;
                case WeaponType.Other:
                    Debug.Log("No weapon settings");

                    break;
                default:
                    Debug.Log("No weapon settings");
                    throw new ArgumentOutOfRangeException();
            }

            if (lWeapon != null)
            {
                lWeapon.transform.localPosition = Resources
                    .Load<WeaponTransformObject>($"ScriptableObject/Weapon/L_Weapon_{WeaponType}").position;
                lWeapon.transform.localRotation = Resources
                    .Load<WeaponTransformObject>($"ScriptableObject/Weapon/L_Weapon_{WeaponType}").rotation;
                lWeapon.transform.localScale = Resources
                    .Load<WeaponTransformObject>($"ScriptableObject/Weapon/L_Weapon_{WeaponType}").scale;
            }

            if (rWeapon != null)
            {
                rWeapon.transform.localPosition = Resources
                    .Load<WeaponTransformObject>($"ScriptableObject/Weapon/R_Weapon_{WeaponType}").position;
                rWeapon.transform.localRotation = Resources
                    .Load<WeaponTransformObject>($"ScriptableObject/Weapon/R_Weapon_{WeaponType}").rotation;
                rWeapon.transform.localScale = Resources
                    .Load<WeaponTransformObject>($"ScriptableObject/Weapon/R_Weapon_{WeaponType}").scale;
            }
        }
    }
}