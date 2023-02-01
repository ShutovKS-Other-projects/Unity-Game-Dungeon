using UnityEngine;

public class InteractableBase : MonoBehaviour, IInteractable
{
#region Variables
    [Header("Interectable Settings")]
    [SerializeField] private float _holdDuration = 0f;
    
    [Space]
    [SerializeField] private bool _holdInteract = false;
    [SerializeField] private bool _multipleUse = false;
    [SerializeField] private bool _isInteractable = true;

    [Space]
    [TextArea(3, 10)]
    [SerializeField] private string _tooltipText = "";
#endregion

#region Properties
    public float HoldDuration => _holdDuration;
    public bool HoldInteract => _holdInteract;
    public bool MultipleUse => _multipleUse;
    public bool IsInteractable => _isInteractable;
    public string TooltipText => _tooltipText;
#endregion

#region Methods
    public virtual void OnInteract()
    {
        Debug.Log("Interacted with " + gameObject.name);
    }
#endregion
}
