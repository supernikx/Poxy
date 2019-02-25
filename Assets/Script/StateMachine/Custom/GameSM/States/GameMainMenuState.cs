using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;

namespace StateMachine.GameSM
{
    public class GameMainMenuState : GameSMStateBase
    {
        UI_ManagerBase uiManager;

        public override void Enter()
        {
            uiManager = context.gameManager.FindUIManager();
        }

        public override void Exit()
        {
            uiManager.ToggleMenu(MenuType.None);
        }
    }
}
