using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.GameSM
{
    public class GameGameplayState : GameSMStateBase
    {
        UI_ManagerBase ui;

        public override void Enter()
        {
            ui = context.gameManager.GetUIManager();
            ui.ToggleMenu(MenuType.Game);
        }
    }
}
