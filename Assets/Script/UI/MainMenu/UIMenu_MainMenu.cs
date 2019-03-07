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
        /// Riferimento all'ui manager
        /// </summary>
        UI_ManagerBase uiManager;

        /// <summary>
        /// Funzione che inizializza il controller
        /// </summary>
        /// <param name="_uiManager"></param>
        public override void Setup(UI_ManagerBase _uiManager)
        {
            uiManager = _uiManager;
        }


        #region API
        /// <summary>
        /// Funzione che attiva/disattiva il menu
        /// </summary>
        /// <param name="_switch"></param>
        public void ActiveMenu(bool _switch)
        {
            gameObject.SetActive(_switch);
        }

        #endregion

        #region ButtonHandler
        /// <summary>
        /// Funzione che fa startare il game
        /// </summary>
        public void StartButton()
        {
            GameManager.StartGame();
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
