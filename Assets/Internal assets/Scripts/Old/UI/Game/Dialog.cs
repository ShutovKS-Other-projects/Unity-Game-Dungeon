using TMPro;
using UnityEngine;

namespace Old.UI.Game
{
    public class Dialog : MonoBehaviour
    {
        //Add event system

        private TextMeshProUGUI _dialogText;

        private void OnEnable()
        {
            _dialogText = transform.Find("DialogText").GetComponent<TextMeshProUGUI>();
        }

        private void UpdateTextDialog(string text) => _dialogText.text = text;
    }
}