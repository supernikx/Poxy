using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UIMenu_GameOverPanel : UIMenu_Base
    {

        public void MainMenuButton()
        {
            uiManager.GetGameplayManager().GetLoadingPanel().ToggleLivesText(false);
            LevelManager.instance.BackToMenu();
        }

        public void RestartButton()
        {
            uiManager.GetGameplayManager().GetLoadingPanel().SetLivesText(5);
            LevelManager.instance.RestartGame();
        }
    }
}
