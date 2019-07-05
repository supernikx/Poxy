using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.GameSM
{
    public class GameGameplayState : GameSMStateBase
    {
        UI_ManagerBase ui;
        LevelManager levelManager;
        StreamVideo videoStream;

        public override void Enter()
        {
            levelManager = FindObjectOfType<LevelManager>();
            ui = context.gameManager.GetUIManager();

            videoStream = levelManager.GetVideoStream();
            if (videoStream != null)
            {
                videoStream.OnVideoEnd += SetupStartGame;
                videoStream.PlayVideo();
            }
            else
                SetupStartGame();
        }

        public override void Exit()
        {
            if (videoStream != null)
                videoStream.OnVideoEnd -= SetupStartGame;

            ui.GetGameplayManager().GetCountdownPanel().OnCountdownEnd -= HandleOnCountdownEnd;
            context.gameManager.GetUIManager().ToggleMenu(MenuType.None);
            context.gameManager.GetSoundManager().StopMusic();
        }

        private void HandleOnCountdownEnd()
        {
            PlayerInputManager.SetCanReadInput(true);
            levelManager.GetPlayer().GetHealthController().SetCanLoseHealth(true);

            ui.ToggleMenu(MenuType.Game);

            SpeedrunManager.StartTimer?.Invoke();
        }

        private void SetupStartGame()
        {
            context.gameManager.GetSoundManager().PlayGameMusic();

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
    }
}
