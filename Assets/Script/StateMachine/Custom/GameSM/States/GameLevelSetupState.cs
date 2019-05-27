using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UI;
using System;

namespace StateMachine.GameSM
{
    public class GameLevelSetupState : GameSMStateBase
    {
        UI_ManagerBase uiManager;
        public override void Enter()
        {
            UIMenu_TutorialPanel.OnTutorialEnded += HandleOnTutorialEnded;
            context.gameManager.GetUIManager().ToggleMenu(MenuType.Tutorial);
        }

        private void HandleOnTutorialEnded()
        {
            context.gameManager.GetUIManager().ToggleMenu(MenuType.Loading);
            SceneManager.LoadScene(context.gameManager.GetLevelsManager().GetSelectedLevel().SceneName);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene _scene, LoadSceneMode _mode)
        {
            uiManager = context.gameManager.FindUIManager();
            LevelManager levelManager = FindObjectOfType<LevelManager>();
            levelManager.Init(uiManager, context.gameManager.speedrunMode);
            uiManager.ToggleMenu(MenuType.None);
            context.OnLevelSetupCallback();
        }

        public override void Exit()
        {
            UIMenu_TutorialPanel.OnTutorialEnded -= HandleOnTutorialEnded;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
