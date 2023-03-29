using System;
using Input;
using UI.Home_Scene;
using UnityEngine;

namespace Skills.SkillsBook
{
    public class UISkillsBook : MonoBehaviour
    {
        public void LateUpdate()
        {
            if (InputManagerHomeScene.Instance.GetPlayerMovementInput() != Vector2.zero)
            {
                ManagerScene.ManagerScene.SwitchCursor(false);
                ManagerUI.Instance.SwitchSkillsBookUI(false);
            }
            else if (InputManagerHomeScene.Instance.GetPlayerCrouchInput())
            {
                ManagerScene.ManagerScene.SwitchCursor(false);
                ManagerUI.Instance.SwitchSkillsBookUI(false);
            }
            else if (InputManagerHomeScene.Instance.GetPlayerJumpInput())
            {
                ManagerScene.ManagerScene.SwitchCursor(false);
                ManagerUI.Instance.SwitchSkillsBookUI(false);
            }
            else if (InputManagerHomeScene.Instance.GetPlayerInteractInput())
            {
                ManagerScene.ManagerScene.SwitchCursor(false);
                ManagerUI.Instance.SwitchSkillsBookUI(false);
            }
        }
    }
}