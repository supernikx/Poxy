using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UI
{
    public class UI_MenuEndGamePanel : UIMenu_Base
    {
        [Header("End Game References")]
        [SerializeField]
        private TextMeshProUGUI nameText;
        [SerializeField]
        private GameObject speedRunPanel;
        [SerializeField]
        private TextMeshProUGUI speedRunTimerText;
        [SerializeField]
        private GameObject normalPanel;
        [SerializeField]
        private TextMeshProUGUI tokenText;

        public override void Enable()
        {
            if (GameManager.Exist())
                nameText.text = GameManager.instance.GetOptionsManager().GetUserName();

            SpeedrunManager speedrunMng = LevelManager.instance.GetSpeedrunManager();
            if (speedrunMng.GetIsActive())
            {
                speedRunPanel.SetActive(true);
                normalPanel.SetActive(false);
                speedRunTimerText.text = "Time: " + speedrunMng.GetTimer().ToString("0.00");
            }
            else
            {
                normalPanel.SetActive(true);
                speedRunPanel.SetActive(false);
                tokenText.text = "Tokens: " + LevelManager.instance.GetTokenManager().GetTokensCount().ToString();
            }

            base.Enable();
        }

        public void RetryButton()
        {
            LevelManager.instance.RestartGame();
        }

        public void NextLevelButton()
        {
            if (GameManager.Exist())
                GameManager.StartNextLevel(LevelManager.instance);
        }

        public void BackMenuButton()
        {
            uiManager.GetGameplayManager().GetLoadingPanel().ToggleLivesText(false);
            LevelManager.instance.BackToMenu();
        }

        public void LeaderboardButton()
        {

        }
    }
}
