using System;
using Manager;
using Scene;
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
                SceneController.SwitchCursor(false);
                UIHomeSceneController.Instance.SwitchSkillsBookUI(false);
            }
            else if (ManagerInput.Instance.GetPlayerCrouchInput())
            {
                SceneController.SwitchCursor(false);
                UIHomeSceneController.Instance.SwitchSkillsBookUI(false);
            }
            else if (ManagerInput.Instance.GetPlayerJumpInput())
            {
                SceneController.SwitchCursor(false);
                UIHomeSceneController.Instance.SwitchSkillsBookUI(false);
            }
            else if (ManagerInput.Instance.GetPlayerInteractInput())
            {
                SceneController.SwitchCursor(false);
                UIHomeSceneController.Instance.SwitchSkillsBookUI(false);
            }
        }
    }
}