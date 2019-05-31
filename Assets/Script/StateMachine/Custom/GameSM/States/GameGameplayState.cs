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
                ui.GetGameplayManager().GetCountdownPanel().OnCountdown += HandleOnCountdown;
                ui.GetGameplayManager().GetCountdownPanel().OnCountdownEnd += HandleOnCountdownEnd;
                ui.ToggleMenu(MenuType.Countdown);
            }
            else
                ui.ToggleMenu(MenuType.Game);
        }

        public override void Exit()
        {
            ui.GetGameplayManager().GetCountdownPanel().OnCountdown -= HandleOnCountdown;
            ui.GetGameplayManager().GetCountdownPanel().OnCountdownEnd -= HandleOnCountdownEnd;
            context.gameManager.GetUIManager().ToggleMenu(MenuType.None);
        }

        private void HandleOnCountdown()
        {
            PlayerInputManager.SetCanReadGameplayInput(false);
            levelManager.GetPlayer().GetHealthController().SetCanLoseHealth(false);
        }

        private void HandleOnCountdownEnd()
        {
            PlayerInputManager.SetCanReadGameplayInput(true);
            levelManager.GetPlayer().GetHealthController().SetCanLoseHealth(true);

            ui.ToggleMenu(MenuType.Game);

            if (SpeedrunManager.StartTimer != null)
                SpeedrunManager.StartTimer();
        }
    }
}
