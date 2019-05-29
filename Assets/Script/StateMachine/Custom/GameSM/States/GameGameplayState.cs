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
            if (context.gameManager.speedrunMode)
                ui.ToggleMenu(MenuType.Countdown);
            else
                ui.ToggleMenu(MenuType.Game);
        }

        public override void Exit()
        {
            context.gameManager.GetUIManager().ToggleMenu(MenuType.None);
        }
    }
}
