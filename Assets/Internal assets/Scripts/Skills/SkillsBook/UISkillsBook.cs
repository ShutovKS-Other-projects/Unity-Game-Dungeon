using System;
using Manager;
using UI.Home_Scene;
using UnityEngine;

namespace Skills.SkillsBook
{
    public class UISkillsBook : MonoBehaviour
    {
        public void LateUpdate()
        {
            if (ManagerInput.Instance.GetPlayerMovementInput() != Vector2.zero)
            {
                ManagerScene.SwitchCursor(false);
                UIHomeSceneController.Instance.SwitchSkillsBookUI(false);
            }
            else if (ManagerInput.Instance.GetPlayerCrouchInput())
            {
                ManagerScene.SwitchCursor(false);
                UIHomeSceneController.Instance.SwitchSkillsBookUI(false);
            }
            else if (ManagerInput.Instance.GetPlayerJumpInput())
            {
                ManagerScene.SwitchCursor(false);
                UIHomeSceneController.Instance.SwitchSkillsBookUI(false);
            }
            else if (ManagerInput.Instance.GetPlayerInteractInput())
            {
                ManagerScene.SwitchCursor(false);
                UIHomeSceneController.Instance.SwitchSkillsBookUI(false);
            }
        }
    }
}