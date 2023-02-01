using UnityEngine;

[CreateAssetMenu(fileName = "InteractionInputData", menuName = "InteractionSystem/InputData", order = 0)]
public class InteractionInputData : ScriptableObject
{
    private bool _interactedClicked;
    private bool _interactedReleased;

    public bool InteractedClicked
    {
        get => _interactedClicked;
        set => _interactedClicked = value;
    }

    public bool InteractedReleased
    {
        get => _interactedReleased;
        set => _interactedReleased = value;
    }

    public void ResetInput()
    {
        _interactedClicked = false;
        _interactedReleased = false;
    }
}
