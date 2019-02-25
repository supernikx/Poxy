﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;

public class UI_MenuManager : UI_ManagerBase
{
    [Header("Panels")]
    [SerializeField]
    private UIMenu_MainMenu mainMenuPanel;
    [SerializeField]
    private UIMenu_LoadingPanel loadingPanel;

    #region Getter
    public override UI_MenuManager GetMenuManager()
    {
        return this;
    }

    public UIMenu_MainMenu GetMainMenuPanel()
    {
        return mainMenuPanel;
    }

    public UIMenu_LoadingPanel GetLoadingPanel()
    {
        return loadingPanel;
    }
    #endregion

    /// <summary>
    /// Funzione che imposta i settaggi come devono essere alla conclusione del Setup
    /// </summary>
    public override void StartSetup()
    {
        ToggleMenu(MenuType.MainMenu);
    }

    /// <summary>
    /// Funzione che attiva il menu passato come parametro
    /// </summary>
    /// <param name="_menu"></param>
    public override void ToggleMenu(MenuType _menu)
    {
        DisableAllMenus();
        switch (_menu)
        {
            case MenuType.None:
                break;
            case MenuType.MainMenu:
                mainMenuPanel.Enable();
                break;
            case MenuType.Loading:
                loadingPanel.Enable();
                break;
            default:
                Debug.LogError(_menu + " non presente in questo manager");
                break;
        }
    }
}
