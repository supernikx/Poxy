using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UI
{
    public class UIMenu_Leaderboard : UIMenu_Base
    {
        [SerializeField]
        private TextMeshProUGUI loadingText;
        [SerializeField]
        private GameObject leaderboardPanel;
        [SerializeField]
        private UI_LeaderboardField leaderboardFieldPrefab;

        dreamloLeaderBoard lbMgn;
        List<UI_LeaderboardField> leaderboardFields = new List<UI_LeaderboardField>();

        public override void Setup(UI_ManagerBase _uiManager)
        {
            base.Setup(_uiManager);
            if (GameManager.Exist())
                lbMgn = GameManager.instance.GetLeaderboard();
        }

        public override void Enable()
        {
            base.Enable();

            loadingText.text = "Loading...";
            loadingText.gameObject.SetActive(true);

            if (lbMgn != null)
                lbMgn.LoadScores(SocresLoadCallback);
            else
                loadingText.text = "Can't Load Leabderboard";
        }

        private void SocresLoadCallback(bool _success)
        {
            if (_success)
                LoadLeaderboard();
            else
                loadingText.text = "Can't Load Leabderboard";
        }

        private void LoadLeaderboard()
        {
            List<dreamloLeaderBoard.Score> scores = lbMgn.ToListLowToHigh();

            for (int i = 0; i < scores.Count; i++)
            {
                if (i == 5)
                    break;
                UI_LeaderboardField instantiatedField = Instantiate(leaderboardFieldPrefab, leaderboardPanel.transform);
                instantiatedField.Setup(scores[i].playerName, scores[i].seconds.ToString("0.00"));
                leaderboardFields.Add(instantiatedField);
            }

            loadingText.gameObject.SetActive(false);
        }

        private void DestroyLeaderboard()
        {
            for (int i = 0; i < leaderboardFields.Count; i++)
            {
                Destroy(leaderboardFields[i].gameObject);
                leaderboardFields.RemoveAt(i);
                i--;
            }
        }

        /// <summary>
        /// Funzione che gestisce il pulsante back
        /// </summary>
        public void BackButtonMainMenu()
        {
            uiManager.ToggleMenu(MenuType.MainMenu);
        }

        /// <summary>
        /// Funzione che gestisce il pulsante back
        /// </summary>
        public void BackButtonEndGame()
        {
            uiManager.ToggleMenu(MenuType.EndGame);
        }

        public override void Disable()
        {
            base.Disable();
            DestroyLeaderboard();
        }
    }
}
