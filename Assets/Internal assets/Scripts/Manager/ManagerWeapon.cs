using System;
using Scene;
using Unity.VisualScripting;
using UnityEngine;
using Weapon;

namespace Manager
{
    public class ManagerWeapon : MonoBehaviour
    {
        public static ManagerWeapon Instance { get; private set; }

        private Transform RWeapon;
        private Transform LWeapon;

        public static WeaponType WeaponType =>
            Resources.Load<ChosenWeaponObject>($"ScriptableObject/Weapon/ChosenWeaponData").weaponType;

        public delegate void SwitchCollider(bool value);

        public event SwitchCollider OnSwitchCollider;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            RWeapon = ManagerPlayer.Instance.playerTransform!.Find("Weapons").Find("L_Weapon");
            LWeapon = ManagerPlayer.Instance.playerTransform!.Find("Weapons").Find("R_Weapon");
            if (RWeapon == null && LWeapon == null) Debug.LogError("RWeapon or LWeapon not found");
            InstantiateWeapon();
            SceneController.OnNewSceneLoaded += InstantiateWeapon;
        }

        public void OnSwitchColliderWeapon(bool value) => OnSwitchCollider?.Invoke(value);

        public static void ChooseWeapon(WeaponType weaponType)
        {
            Resources.Load<ChosenWeaponObject>($"ScriptableObject/Weapon/ChosenWeaponData").weaponType = weaponType;
        }

        private void InstantiateWeapon()
        {
            Debug.Log("InstantiateWeapon");
            
            for (var i = 0; i < RWeapon.childCount; i++)
                Destroy(RWeapon.GetChild(i).gameObject);
            for (var i = 0; i < LWeapon.childCount; i++)
                Destroy(LWeapon.GetChild(i).gameObject);

            if (SceneController.currentSceneType == SceneType.Home) return;
 
            GameObject lWeapon = null;
            GameObject rWeapon = null;

            switch (WeaponType)
            {
                case WeaponType.Spear:
                    break;
                case WeaponType.BowAndArrow:
                    break;
                case WeaponType.SwordAndShield:
                    break;
                case WeaponType.Hammer:
                    break;
                case WeaponType.TwoSwords:
                    lWeapon = Instantiate(Resources.Load<GameObject>($"Weapon/Sword"), LWeapon);
                    rWeapon = Instantiate(Resources.Load<GameObject>($"Weapon/Sword"), RWeapon);
                    break;
                case WeaponType.Other:
                    break;
                default:
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