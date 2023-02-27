using System;
using UnityEngine;
using TMPro;

namespace UI.Game
{
    public class Dialog : MonoBehaviour
    {
        //Add event system

        TextMeshProUGUI _dialogText;

        void OnEnable()
        {
            _dialogText = transform.Find("DialogText").GetComponent<TextMeshProUGUI>();
        }
        
        void UpdateTextDialog(string text) => _dialogText.text = text;
    }
}
