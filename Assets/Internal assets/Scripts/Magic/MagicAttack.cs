using UnityEngine;

namespace Magic
{
    public class MagicAttack : MonoBehaviour
    {
        private void Start()
        {
            Destroy(gameObject, 5f);
        }

        private void OnTriggerEnter(Collider other)
        {
            Destroy(gameObject, 0.1f);
        }
    }
}