using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIMenu_MainMenu : UIMenu_Base
    {
        [SerializeField]
        Button startButton;

        /// <summary>
        /// Funzione che inizializza il controller
        /// </summary>
        /// <param name="_uiManager"></param>
        public override void Setup(UI_ManagerBase _uiManager)
        {
            base.Setup(_uiManager);
        }

        #region ButtonHandler
        /// <summary>
        /// Funzione che fa startare il game
        /// </summary>
        public void StartButton()
        {
            GameManager.SelectLevel();
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
