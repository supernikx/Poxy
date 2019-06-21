using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.GameSM
{
    public class GameGameplayState : GameSMStateBase
    {
        UI_ManagerBase ui;
        LevelManager levelManager;

        public override void Enter()
        {
            levelManager = FindObjectOfType<LevelManager>();
            ui = context.gameManager.GetUIManager();
            if (context.gameManager.GetLevelsManager().GetMode())
            {
                PlayerInputManager.SetCanReadInput(false);
                levelManager.GetPlayer().GetHealthController().SetCanLoseHealth(false);

                ui.GetGameplayManager().GetCountdownPanel().OnCountdownEnd += HandleOnCountdownEnd;
                ui.ToggleMenu(MenuType.Countdown);
            }
            else
            {
                PlayerInputManager.SetCanReadInput(true);
                ui.ToggleMenu(MenuType.Game);
            }
        }

        public override void Exit()
        {
            ui.GetGameplayManager().GetCountdownPanel().OnCountdownEnd -= HandleOnCountdownEnd;
            context.gameManager.GetUIManager().ToggleMenu(MenuType.None);
        }

        private void HandleOnCountdownEnd()
        {
            PlayerInputManager.SetCanReadInput(true);
            levelManager.GetPlayer().GetHealthController().SetCanLoseHealth(true);

            ui.ToggleMenu(MenuType.Game);

            if (SpeedrunManager.StartTimer != null)
                SpeedrunManager.StartTimer();
        }
    }
}
