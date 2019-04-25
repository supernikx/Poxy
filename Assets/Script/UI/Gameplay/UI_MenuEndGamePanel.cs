using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UI_MenuEndGamePanel : UIMenu_Base
    {
        public override void Setup(UI_ManagerBase uiManager)
        {

        }

        public override void Enable()
        {
            base.Enable();
            PlayerInputManager.OnConfirmPressed += HandleOnConfirmPressed;
            Time.timeScale = 0f;
        }

        private void HandleOnConfirmPressed()
        {
            LevelManager.instance.BackToMenu();
        }

        public override void Disable()
        {
            PlayerInputManager.OnConfirmPressed -= HandleOnConfirmPressed;
            Time.timeScale = 1f;
            base.Disable();
        }
    }
}
