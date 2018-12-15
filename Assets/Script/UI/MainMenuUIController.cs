using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MainMenuUIController : MonoBehaviour
    {
        [SerializeField]
        Button startButton;

        /// <summary>
        /// Riferimento all'ui manager
        /// </summary>
        UIManager uiManager;

        /// <summary>
        /// Funzione che inizializza il controller
        /// </summary>
        /// <param name="_uiManager"></param>
        public void Init(UIManager _uiManager)
        {
            uiManager = _uiManager;
            startButton.onClick.AddListener(HandleStartButton);
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
        /// Handle per lo start button
        /// </summary>
        private void HandleStartButton()
        {
            GameManager.singleton.StartGame();
        }
        #endregion
    }
}
