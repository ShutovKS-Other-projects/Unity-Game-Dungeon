using Item;
using UnityEngine;
namespace Interactable.Interactable
{
    public class InteractableDeadMobe : InteractableBase
    {
        public override void OnInteract()
        {
            DropItem(transform.position);
            Destroy(gameObject);
        }

        private void DropItem(Vector3 position)
        {
            GameObject item = Instantiate(GameObject.Find("ItemDatabase").GetComponent<ItemDatabase>().GetRandomItemPrefab(), position, Quaternion.identity);
            item.layer = LayerMask.NameToLayer("Interactable");
            item.AddComponent<Rigidbody>();
            item.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            item.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        }
    }
}
