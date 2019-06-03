using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UIMenu_CreditsPanel : UIMenu_Base
    {
        public void BackButton()
        {
            uiManager.ToggleMenu(MenuType.MainMenu);
        }
    }
}
