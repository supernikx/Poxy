using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StateMachine.GameSM
{
    public class GameLevelSetupState : GameSMStateBase
    {
        public override void Enter()
        {
            SceneManager.LoadScene("Level1"); //TODO: mettere il livello scelto
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene _scene, LoadSceneMode _mode)
        {
            LevelManager levelManager = FindObjectOfType<LevelManager>();
            levelManager.Init();
            context.OnLevelSetupCallback();
        }
    }
}
