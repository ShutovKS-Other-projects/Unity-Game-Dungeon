using UnityEngine;

namespace Old.UI.PlayerInfo
{
    public class Main : MonoBehaviour
    {
        [SerializeField] private GameObject uiInventory;
        [SerializeField] private GameObject uiSkills;

        private void OnDisable()
        {
            OnInventory();
        }

        public void OnInventory()
        {
            uiInventory.SetActive(true);
            uiSkills.SetActive(false);
        }
        
        public void OnSkills()
        {
            uiInventory.SetActive(false);
            uiSkills.SetActive(true);
        }
    }
}
