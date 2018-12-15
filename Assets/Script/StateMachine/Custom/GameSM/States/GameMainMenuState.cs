using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;

namespace StateMachine.GameSM
{
    public class GameMainMenuState : GameSMStateBase
    {
        UIManager uiManager;

        public override void Enter()
        {
            uiManager = context.gameManager.GetUIManager();
            uiManager.ActiveMenu(Menu.MainMenu);
        }

        public override void Exit()
        {
            uiManager.DisableAllMenu();
        }
    }
}
