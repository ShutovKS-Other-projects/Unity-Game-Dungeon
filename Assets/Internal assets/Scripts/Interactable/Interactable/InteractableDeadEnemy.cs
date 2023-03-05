using System;
using Item;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Interactable.Interactable
{
    public class InteractableDeadEnemy : InteractableBase
    {
        [SerializeField] private GameObject[] interactableObjects;

        public override void OnInteract()
        {
            DropItem(transform.position);
            Destroy(gameObject);
        }

        private void DropItem(Vector3 position)
        {
            var item = Instantiate(interactableObjects[Random.Range(0, interactableObjects.Length)], position,
                Quaternion.identity);
            item.layer = LayerMask.NameToLayer("Interactable");
            item.AddComponent<Rigidbody>();
            item.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            item.GetComponent<Rigidbody>().constraints =
                RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        }
    }
}