using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class testButton : MonoBehaviour
{
    [SerializeField] GameObject panel;

    public void Enable() => panel.SetActive(true);
    public void Disable() => panel.SetActive(false);
}
