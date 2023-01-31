using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    [SerializeField] private GameObject[] _itemPrefabs;

    public GameObject GetItemPrefab(int numeber)
    {
        return _itemPrefabs[numeber];
    }

    public GameObject GetRandomItemPrefab()
    {
        return _itemPrefabs[Random.Range(0, _itemPrefabs.Length)];
    }

    public int ItemPrefabLength()
    {
        return _itemPrefabs.Length;
    }


}
