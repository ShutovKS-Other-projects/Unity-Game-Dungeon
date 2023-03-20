using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Interactable
{
    public class UIInteractionBare : MonoBehaviour
    {
        private Image _progressBar;
        private TextMeshProUGUI _tooltipText;

        private void Start()
        {
            _progressBar = GetComponentInChildren<Image>();
            _tooltipText = transform.Find("InteractionText").GetComponent<TextMeshProUGUI>();
        }

        public void SetProgress(float progress)
        {
            _progressBar.fillAmount = progress;
        }

        public void SetTooltipText(string text)
        {
            _tooltipText.text = text;
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
}
