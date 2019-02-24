using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UI;

public abstract class UI_ManagerBase : MonoBehaviour
{
    private GameManager gm;
    private List<UIMenu_Base> menus;

    public void Setup(GameManager _gm)
    {
        gm = _gm;

        menus = new List<UIMenu_Base>();
        menus = GetComponentsInChildren<UIMenu_Base>(true).ToList();

        foreach (UIMenu_Base menu in menus)
            menu.Setup(this);

        StartSetup();
    }

    public abstract void StartSetup();

    public abstract void ToggleMenu(MenuType _menu);

    public void DisableAllMenus()
    {
        foreach (UIMenu_Base menu in menus)
            menu.Disable();
    }
}

public enum MenuType
{
    None,
    MainMenu,
    Loading,
}

