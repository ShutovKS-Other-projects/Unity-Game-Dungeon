using System;
using Manager;
using UnityEngine;

namespace Weapon
{
    public class ObjectDamage : MonoBehaviour
    {
        // public WeaponType WeaponType { get; protected set; }
        public float Damage { get; private set; } = 40f;
        private Collider Collider { get; set; }

        private void Awake()
        {
            Collider = GetComponent<Collider>();
            Collider = GetComponent<Collider>();
            Collider.isTrigger = true;
        }

        private void Start()
        {
            WeaponController.OnSwitchCollider += SwitchCollider;
        }

        private void SwitchCollider(bool value)
        {
            Collider.enabled = value;
        }

        public void SetDamageValue(object value)
        {
            Damage = Convert.ToSingle(value);
        }
    }
}