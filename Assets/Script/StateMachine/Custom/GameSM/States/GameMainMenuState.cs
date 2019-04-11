using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StateMachine.GameSM
{
    public class GameMainMenuState : GameSMStateBase
    {
        UI_ManagerBase uiManager;

        public override void Enter()
        {            
            if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                uiManager = context.gameManager.FindUIManager();
                uiManager.ToggleMenu(MenuType.MainMenu);
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
        }

        public override void Exit()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            uiManager.ToggleMenu(MenuType.None);
        }
    }
}
