using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerInteractionObject : MonoBehaviour
{
    private static System.Random random = new System.Random();
    private PlayerInventory playerInventory;
    private Text interactionLabel;
    private void Start()
    {
        playerInventory = gameObject.GetComponent<PlayerInventory>();
        interactionLabel = GameObject.Find("InteractionLabel").GetComponent<Text>();
    }

    private void Update()
    {
        switch (GettingVisibility()?.name)
        {
            case "Mobe":
                if (GettingVisibility().GetComponent<MobeController>().statistics.isDead)
                {
                    interactionLabel.text = "Соберать душу F";
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        gameObject.GetComponent<PlayerController>().statistics.killCount++;
                        DropItem(GettingVisibility());
                        Destroy(GettingVisibility());
                    }
                }
                break;

            case "Item":
                interactionLabel.text = "Подобрать предмет F";
                if (Input.GetKeyDown(KeyCode.F))
                {
                    playerInventory.AddItem(GettingVisibility());
                    Destroy(GettingVisibility());
                }
                break;

            case "Chest":
                //interactionLabel.text = "Открыть сундук F";
                //if (Input.GetKeyDown(KeyCode.F))
                //{
                //    Chest chest = gettingVisibility.GetComponent<Chest>();
                //    if (chest != null)
                //    {
                //        chest.Loot();
                //    }
                //    Destroy(GettingVisibility();
                //}
                break;

            case "End":
                interactionLabel.text = "Закончить лабиринт F";
                if (Input.GetKeyDown(KeyCode.F))
                {
                    Cursor.lockState = CursorLockMode.None;
                    SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
                }
                break;

            default:
                interactionLabel.text = "No interaction";
                break;
        }
    }

    private GameObject GettingVisibility()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        Physics.Raycast(ray, out RaycastHit hits, 5f);
        Debug.DrawRay(ray.origin, ray.direction, Color.red, 5f);
        if (hits.collider != null)
        {
            return hits.collider.gameObject;
        }
        else
        {
            return null;
        }
    }

    private void DropItem(GameObject mobe)
    {
        if (random.Next(0, 100) < 50)
        {
            GameObject obj = Instantiate(Resources.Load("ItemPrefab")) as GameObject;
            obj.transform.position = mobe.transform.position;
            obj.AddComponent<Rigidbody>();
            obj.AddComponent<BoxCollider>();
            obj.tag = "Item";
        }
    }
}