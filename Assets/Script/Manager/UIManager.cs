using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager uiManager;
        /// <summary>
        /// Referenza al controller del menu principale
        /// </summary>
        [SerializeField]
        MainMenuUIController mainMenuController;

        public void Init()
        {
            mainMenuController.Init(this);

            // Singleton
            if (uiManager == null)
            {
                uiManager = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                DestroyImmediate(gameObject);
        }

        #region API
        /// <summary>
        /// Funzione che attiva il menu passato come parametro
        /// </summary>
        /// <param name="_menu"></param>
        public void ActiveMenu(Menu _menu)
        {
            DisableAllMenu();

            switch (_menu)
            {
                case Menu.MainMenu:
                    mainMenuController.ActiveMenu(true);
                    break;
            }
        }

        /// <summary>
        /// Funzione che disabilità tutti menu
        /// </summary>
        public void DisableAllMenu()
        {
            mainMenuController.ActiveMenu(false);
        }
        #endregion
    }

    public enum Menu
    {
        MainMenu,
    }
}
