using UnityEngine;
public class PlayerInventory : MonoBehaviour
{
    public Cell[] inventory;

    private void Awake()
    {
        inventory = new Cell[20];
        CreateInvertory();
    }

    private void CreateInvertory()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            inventory[i] = new Cell(i);
        }
    }

    public void AddItem(GameObject gameObject)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].state == false)
            {
                inventory[i].state = true;
                inventory[i].gameObject = gameObject;
                inventory[i].itemIcon = Sprite.Create(Texture2D.blackTexture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
                break;
            }
        }
    }

    public void RemoveItem(int nume)
    {
        if (inventory[nume].state == true)
        {
            inventory[nume].state = false;
            DropItem(inventory[nume].gameObject);
            inventory[nume].gameObject = null;
            inventory[nume].itemIcon = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
        }

    }

    public void RemoveAllItems()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            RemoveItem(i);
        }
    }

    public void DropItem(GameObject gameObject)
    {
        Instantiate(gameObject, transform.position + new Vector3(0.1f, 0.1f, 0.1f), Quaternion.identity);
    }



    public class Cell
    {
        PlayerInventory playerInventory = new PlayerInventory();
        public int id;
        public Sprite itemIcon;
        public bool state = false;
        public GameObject gameObject = null;

        public Cell(int nume)
        {
            this.id = nume;
            this.itemIcon = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
        }
    }
}

