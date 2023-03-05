using UI;
using UnityEngine;

namespace Interactable.Interactable
{
    public class InteractableChest : InteractableBase
    {
        private UIController _uiController;

        public override void OnInteract()
        {
            _uiController = GameObject.Find("Canvas").GetComponent<UIController>();
            GetComponent<Chest.Chest>().enabled = true;
            _uiController.OnPlayerInfo();
        }
    }
}