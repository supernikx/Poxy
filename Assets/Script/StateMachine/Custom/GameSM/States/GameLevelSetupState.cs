using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StateMachine.GameSM
{
    public class GameLevelSetupState : GameSMStateBase
    {
        UI_ManagerBase uiManager;
        public override void Enter()
        {
            context.gameManager.GetUIManager().ToggleMenu(MenuType.Loading);
            SceneManager.LoadScene("Level1"); //TODO: mettere il livello scelto
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene _scene, LoadSceneMode _mode)
        {
            uiManager = context.gameManager.FindUIManager();
            LevelManager levelManager = FindObjectOfType<LevelManager>();
            levelManager.Init();
            uiManager.ToggleMenu(MenuType.None);
            context.OnLevelSetupCallback();
        }
    }
}
