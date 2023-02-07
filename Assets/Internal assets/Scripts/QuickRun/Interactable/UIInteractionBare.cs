using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInteractionBare : MonoBehaviour
{
    [SerializeField] private Image _progresBar;
    [SerializeField] private Text _toltipText;

    private void Start()
    {
        _progresBar = GetComponentInChildren<Image>();
        _toltipText = GetComponentInChildren<Text>();
    }

    public void SetProgress(float progress)
    {
        _progresBar.fillAmount = progress;
    }

    public void SetTooltipText(string text)
    {
        _toltipText.text = text;
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public void Reset()
    {
        SetProgress(0);
        SetTooltipText("");
    }
}
