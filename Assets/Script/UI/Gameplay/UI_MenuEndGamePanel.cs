using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UI_MenuEndGamePanel : UIMenu_Base
    {
        public void BackMenuButton()
        {
            Time.timeScale = 1f;
            LevelManager.instance.BackToMenu();
        }
    }
}
