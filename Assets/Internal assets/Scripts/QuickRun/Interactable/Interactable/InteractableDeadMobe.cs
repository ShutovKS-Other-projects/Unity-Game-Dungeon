using UnityEngine;

public class InteractableDeadMobe : InteractableBase
{


    public override void OnInteract()
    {
        DropItem(transform.position);
        Destroy(gameObject);
        GameObject.FindWithTag("Player").GetComponent<PlayerController>().statistic.CollectCrystal++;
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
