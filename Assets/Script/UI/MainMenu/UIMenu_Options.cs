using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UIMenu_Options : UIMenu_Base
    {
        /// <summary>
        /// Funzione che gestisce il pulsante back del main menu
        /// </summary>
        public void BackMenuButton()
        {
            uiManager.ToggleMenu(MenuType.MainMenu);
        }

        /// <summary>
        /// Funzione che gestisce il pulsante back del menù di pausa
        /// </summary>
        public void BackPauseButton()
        {
            uiManager.ToggleMenu(MenuType.Pause);
        }
    }
}
