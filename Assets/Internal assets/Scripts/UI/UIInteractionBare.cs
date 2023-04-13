using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIInteractionBare : MonoBehaviour
    {
        public static UIInteractionBare Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

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

        public void SetTooltipText([CanBeNull] string text)
        {
            _tooltipText.text = text == null ? "" : text;
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