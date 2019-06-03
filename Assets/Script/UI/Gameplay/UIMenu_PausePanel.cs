using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UIMenu_PausePanel : UIMenu_Base
    {
        public void ResumeButton()
        {
            if (LevelManager.OnGameUnPause != null)
                LevelManager.OnGameUnPause();
        }

        public void RestartButton()
        {
            LevelManager.instance.RestartGame();
        }

        public void Options()
        {
            uiManager.ToggleMenu(MenuType.Options);
        }

        public void MenuButton()
        {
            LevelManager.instance.BackToMenu();
        }
    }
}
