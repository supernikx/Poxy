using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UIMenu_Options : UIMenu_Base
    {
        /// <summary>
        /// Funzione che gestisce il pulsante back
        /// </summary>
        public void BackButton()
        {
            uiManager.ToggleMenu(MenuType.MainMenu);
        }
    }
}
