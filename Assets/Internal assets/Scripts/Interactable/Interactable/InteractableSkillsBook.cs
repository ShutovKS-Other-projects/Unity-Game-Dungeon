using Manager;
using Scene;
using UI.Home_Scene;
using UnityEngine;

namespace Interactable.Interactable
{
    public class InteractableSkillsBook : InteractableBase
    {
        public override void OnInteract()
        {
            UIHomeSceneController.Instance.SwitchSkillsBookUI(true);
            SceneController.SwitchCursor(true);
        }
    }
}