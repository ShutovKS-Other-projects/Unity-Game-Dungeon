using UnityEngine;

namespace Interactable
{
    public class InteractableBase : MonoBehaviour, IInteractable
    {
        #region Variables

        [Header("Interactable Settings")] [SerializeField]
        protected float holdDuration = 0f;

        [Space] [SerializeField] protected bool holdInteract = false;
        [SerializeField] protected bool multipleUse = false;
        [SerializeField] protected bool isInteractable = true;

        [Space] [TextArea(3, 10)] [SerializeField]
        protected string tooltipText = "";

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