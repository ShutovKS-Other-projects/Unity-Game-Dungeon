using UnityEngine;
namespace Internal_assets.Scripts.QuickRun.Item
{
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
}
