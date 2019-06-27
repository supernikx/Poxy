using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIMenu_PausePanel : UIMenu_Base
    {
        [Header("Pause Settings")]
        [SerializeField]
        private Image backgroundImage;
        [SerializeField]
        private Sprite backgroundKeyboard;
        [SerializeField]
        private Sprite backgroundJoystick;

        public void ResumeButton()
        {
            if (LevelManager.OnGameUnPause != null)
                LevelManager.OnGameUnPause();
        }

        public void RestartButton()
        {
            uiManager.GetGameplayManager().GetLoadingPanel().SetLivesText(5);
            LevelManager.instance.RestartGame();
        }

        public void Options()
        {
            uiManager.ToggleMenu(MenuType.Options);
        }

        public void MenuButton()
        {
            uiManager.GetGameplayManager().GetLoadingPanel().ToggleLivesText(false);
            LevelManager.instance.BackToMenu();
        }

        public void UpdatedImage(InputType currentInput)
        {
            switch (currentInput)
            {
                case InputType.Joystick:
                    backgroundImage.sprite = backgroundJoystick;
                    break;
                case InputType.Keyboard:
                    backgroundImage.sprite = backgroundKeyboard;
                    break;
            }
        }
    }
}
