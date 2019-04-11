using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UIMenu_PausePanel : UIMenu_Base
    {
        UI_ManagerBase uiManager;

        public override void Setup(UI_ManagerBase _uiManager)
        {
            uiManager = _uiManager;
        }

        public void ResumeButton()
        {
            if (LevelManager.OnGameUnPause != null)
                LevelManager.OnGameUnPause();
        }

        public void RestartButton()
        {
            LevelManager.instance.RestartGame();
        }

        public void MenuButton()
        {
            LevelManager.instance.BackToMenu();
        }

        public void QuitButton()
        {
            LevelManager.instance.QuitGame();
        }
    }
}
