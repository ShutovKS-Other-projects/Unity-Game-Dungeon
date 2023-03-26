using UnityEngine;

namespace Interactable
{
    public class InteractableBase : MonoBehaviour, IInteractable
    {
        #region Variables

        [Header("Interactable Settings")] [SerializeField]
        private float holdDuration = 0f;

        [Space] [SerializeField] private bool holdInteract = false;
        [SerializeField] private bool multipleUse = false;
        [SerializeField] private bool isInteractable = true;

        [Space] [TextArea(3, 10)] [SerializeField]
        private string tooltipText = "";

        #endregion

        #region Properties

        public float HoldDuration => holdDuration;
        public bool HoldInteract => holdInteract;
        public bool MultipleUse => multipleUse;
        public bool IsInteractable => isInteractable;
        public string TooltipText => tooltipText;

        #endregion

        #region Methods

        public virtual void OnInteract()
        {
            Debug.Log("Interacted with " + gameObject.name);
        }

        #endregion
    }
}