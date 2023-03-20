using Old.UI;
using UnityEngine;

namespace Interactable.Interactable
{
    public class InteractableChest : InteractableBase
    {
        private UIController _uiController;

        public override void OnInteract()
        {
            _uiController = GameObject.Find("Canvas").GetComponent<UIController>();
            GetComponent<Old.Chest.Chest>().enabled = true;
            _uiController.OnPlayerInfo();
        }
    }
}