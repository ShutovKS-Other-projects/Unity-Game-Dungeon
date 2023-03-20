using UnityEngine;

namespace Old.UI.PlayerInfo
{
    public class Skills : MonoBehaviour
    {
        [SerializeField] private GameObject magic;
        [SerializeField] private GameObject characteristic;

        private void OnDisable()
        {
            OnMagic();
        }
        
        public void OnMagic()
        {
            magic.SetActive(true);
            characteristic.SetActive(false);
        }
        
        public void OnCharacteristic()
        {
            magic.SetActive(false);
            characteristic.SetActive(true);
        }
    }
}
