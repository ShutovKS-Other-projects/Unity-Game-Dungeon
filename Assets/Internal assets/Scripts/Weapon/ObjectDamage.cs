using System;
using UnityEngine;

namespace Weapon
{
    public class ObjectDamage : MonoBehaviour
    {
        // public WeaponType WeaponType { get; protected set; }
        public float Damage { get; protected set; }
        private Collider Collider { get; set; }

        private void Awake()
        {
            Collider = GetComponent<Collider>();
        }

        private void Start()
        {
            ManagerWeapon.Instance.OnSwitchTriggerCollider += SwitchTriggerCollider;
        }

        private void SwitchTriggerCollider(bool value)
        {
            Collider.isTrigger = value;
        }

        public void SetDamageValue(object value)
        {
            Damage = Convert.ToSingle(value);
        }
    }
}