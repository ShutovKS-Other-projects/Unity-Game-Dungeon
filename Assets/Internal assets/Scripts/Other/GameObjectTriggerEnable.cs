using UnityEngine;
namespace Other
{
    public class GameObjectTriggerEnable : MonoBehaviour
    {
        private Collider _collider;

        void Start()
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
