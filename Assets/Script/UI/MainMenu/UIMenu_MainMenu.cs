using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIMenu_MainMenu : UIMenu_Base
    {
        #region ButtonHandler
        /// <summary>
        /// Funzione che fa startare il game
        /// </summary>
        public void StartButton()
        {
            GameManager.SelectLevel();
        }

        /// <summary>
        /// Funzione che apre il menù delle opzioni
        /// </summary>
        public void OptionsButton()
        {
            uiManager.ToggleMenu(MenuType.Options);
        }

        /// <summary>
        /// Funzione che apre il menù delle opzioni
        /// </summary>
        public void LeaderboardButton()
        {
            uiManager.ToggleMenu(MenuType.Leaderboard);
        }

        /// <summary>
        /// Funzione che chiude l'applicazione
        /// </summary>
        public void QuitGame()
        {
            GameManager.QuitGame();
        }
        #endregion
    }
}
