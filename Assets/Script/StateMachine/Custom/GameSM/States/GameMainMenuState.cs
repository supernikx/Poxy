using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UI;

namespace StateMachine.GameSM
{
    public class GameMainMenuState : GameSMStateBase
    {
        UI_ManagerBase uiManager;

        public override void Enter()
        {
            UIMenu_LevelSelection.OnLevelSelected += HandleLevelSelected;

            if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                uiManager = context.gameManager.FindUIManager();
                uiManager.ToggleMenu(MenuType.MainMenu);
                context.gameManager.GetSoundManager().PlayMainMenuMnusic();
            }
            else
            {
                context.gameManager.GetUIManager().ToggleMenu(MenuType.Loading);
                SceneManager.LoadScene("MainMenu");
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
        }

        private void OnSceneLoaded(Scene _scene, LoadSceneMode _mode)
        {
            uiManager = context.gameManager.FindUIManager();
            uiManager.ToggleMenu(MenuType.MainMenu);
            context.gameManager.GetSoundManager().PlayMainMenuMnusic();
        }

        private void HandleLevelSelected(LevelScriptable _selectedLevel, bool _speedRun)
        {
            context.gameManager.GetLevelsManager().SetSelectedLevel(_selectedLevel);
            context.gameManager.GetLevelsManager().SetMode(_speedRun);
            GameManager.StartGame();
        }

        public override void Exit()
        {
            UIMenu_LevelSelection.OnLevelSelected -= HandleLevelSelected;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            uiManager.ToggleMenu(MenuType.None);
            context.gameManager.GetSoundManager().StopMusic();
        }
    }
}
