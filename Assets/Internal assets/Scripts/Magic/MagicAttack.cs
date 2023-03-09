using System.Collections;
using UnityEngine;

namespace Magic
{
    public class MagicAttack : MonoBehaviour
    {
        private void Start() => Destroy(gameObject, 2f);
        private void OnTriggerEnter(Collider other) => StartCoroutine(MagicDestroy());

        private IEnumerator MagicDestroy()
        {
            yield return null;
            Destroy(gameObject);
        }
    }
}