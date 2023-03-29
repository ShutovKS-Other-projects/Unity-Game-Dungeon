using UI.Home_Scene;
using UnityEngine;

namespace Interactable.Interactable
{
    public class InteractableSkillsBook : InteractableBase
    {
        public override void OnInteract()
        {
            ManagerUI.Instance.SwitchSkillsBookUI(true);
            ManagerScene.ManagerScene.SwitchCursor(true);
        }
    }
}