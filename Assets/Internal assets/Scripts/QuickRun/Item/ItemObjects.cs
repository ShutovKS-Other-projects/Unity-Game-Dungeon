using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Default, Food, Helmet, Chest, Pants, Boots, Back, Bracers, Gloves, Ring, Weapon, Shield
}
public enum Attributes
{ 
    HP, MP, Damage, Protection
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Items/Item")]
public class ItemObject : ScriptableObject
{
    public Sprite uiDisplay;
    public GameObject characterDisplay;
    public bool stackable;
    public ItemType type;
    [TextArea(15, 20)] public string description;
    public Item data = new Item();
    public List<string> boneNames = new List<string>();

    public Item CreateItem()
    {
        Item newItem = new Item(this);
        return newItem;
    }

    private void OnValidate()
    {
        boneNames.Clear();
        if (characterDisplay == null)
            return;
        if (!characterDisplay.GetComponent<SkinnedMeshRenderer>())
            return;

        var renderer = characterDisplay.GetComponent<SkinnedMeshRenderer>();
        var bones = renderer.bones;

        foreach (var t in bones)
        {
            boneNames.Add(t.name);
        }

    }
}
