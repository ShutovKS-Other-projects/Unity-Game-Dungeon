using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Inventory
{
    public static class ExtensionMethods
    {
        public static void UpdateSlotDisplay(this Dictionary<GameObject, InventorySlot> _slotsOnInterface)
        {
            foreach (KeyValuePair<GameObject, InventorySlot> _slot in _slotsOnInterface)
            {
                if (_slot.Value.item.Id >= 0)
                {
                    _slot.Key.transform.GetChild(0).GetComponent<Image>().sprite = _slot.Value.ItemObject.uiDisplay;
                    _slot.Key.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    _slot.Key.transform.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 0);
                    _slot.Key.transform.GetChild(2).GetComponent<Text>().text = _slot.Value.amount == 1 ? "" : _slot.Value.amount.ToString("n0");
                }
                else
                {
                    _slot.Key.transform.GetChild(0).GetComponent<Image>().sprite = null;
                    _slot.Key.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0);
                    _slot.Key.transform.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    _slot.Key.transform.GetChild(1).GetComponent<Text>().text = "";
                }
            }
        }
    }
}