using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerInteractionObject : MonoBehaviour
{
    private static System.Random random = new System.Random();
    [NonSerialized] public string interactionText;
    [SerializeField] private GameObject sword;
    public InventoryObject inventory;
    private Collider gettingVisibility;

    private void Update()
    {
        interactionText = " ";
        GettingVisibility(out gettingVisibility);
        if (gettingVisibility != null)
        {
            Interaction(gettingVisibility);
        }
    }

    private void Interaction(Collider collider)
    {
        switch (collider.tag)
        {
            case "Item":
                interactionText = "Нажмите F чтобы забрать предмет";
                if (Input.GetKeyDown(KeyCode.F))
                {
                    var item = collider.gameObject.GetComponent<GroundItem>();
                    if (item)
                    {
                        Item _item = new Item(item.item);
                        inventory.AddItem(_item, 1);
                        Destroy(collider.gameObject);
                    }
                }
                break;
            case "Mobe":
                if (gettingVisibility.GetComponent<MobeController>().statistics.isDead)
                {
                    interactionText = "Нажмите F чтобы забрать душу врага";
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        DropItem(gettingVisibility.transform.position);
                        Destroy(gettingVisibility.gameObject);
                        gameObject.GetComponent<PlayerController>().statistics.KillCount++;
                    }
                }
                break;


            case "Chest":
                interactionText = "Нажмите F чтобы открыть сундук";
                if (Input.GetKeyDown(KeyCode.F))
                {
                    DropItem(gettingVisibility.transform.position);
                    Destroy(gettingVisibility.gameObject);
                }
                break;
        }
        switch (collider.name)
        {
            case "End":
                interactionText = "Нажмите F чтобы выйти из лабиринта";
                if (Input.GetKeyDown(KeyCode.F))
                {
                    Cursor.lockState = CursorLockMode.None;
                    SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
                }
                break;
        }
    }

    private void GettingVisibility(out Collider collider)
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        Physics.Raycast(ray, out RaycastHit hits, 5f);
        Debug.DrawRay(ray.origin, ray.direction, Color.red, 5f);
        collider = hits.collider;
    }

    private void DropItem(Vector3 position)
    {
        GameObject item = Instantiate(sword, position, Quaternion.identity);
    }
}