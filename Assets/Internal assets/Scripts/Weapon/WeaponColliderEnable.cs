using UnityEngine;

namespace Weapon
{
    public class WeaponColliderEnable : MonoBehaviour
    {
        private Collider _collider;

        private void Start()
        {
            _collider = GetComponent<Collider>();
            _collider.enabled = false;
            _collider.isTrigger = true;
        }

        public void EnableCollider(bool value)
        {
            _collider.enabled = value;
        }
    }
}