using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine;
using UI;

public class UI_MenuManager : UI_ManagerBase
{
    [Header("Panels")]
    [SerializeField]
    private UIMenu_MainMenu mainMenuPanel;
    [SerializeField]
    private UIMenu_Options optionsPanel;
    [SerializeField]
    private UIMenu_Leaderboard leaderboardPanel;
    [SerializeField]
    private UIMenu_CreditsPanel creditsPanel;
    [SerializeField]
    private UIMenu_LevelSelection levelSelctionPanel;
    [SerializeField]
    private UIMenu_LoadingPanel loadingPanel;

    /// <summary>
    /// Riferimento al menù attualmente attivo
    /// </summary>
    private MenuType currentMenu;

    #region Getter
    public override UI_MenuManager GetMenuManager()
    {
        return this;
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
        currentMenu = _menu;

        switch (_menu)
        {
            case MenuType.None:
                break;
            case MenuType.MainMenu:
                mainMenuPanel.Enable();
                break;
            case MenuType.Options:
                optionsPanel.Enable();
                break;
            case MenuType.Leaderboard:
                leaderboardPanel.Enable();
                break;
            case MenuType.Credits:
                creditsPanel.Enable();
                break;
            case MenuType.LevelSelection:
                levelSelctionPanel.Enable();
                break;
            case MenuType.Loading:
                loadingPanel.Enable();
                break;
            default:
                Debug.LogError(_menu + " non presente in questo manager");
                break;
        }

        CheckEventSystemInput();
    }

    /// <summary>
    /// Funzione che gestisce l'evento InputChecker.OnInputChanged
    /// </summary>
    /// <param name="_currentInput"></param>
    protected override void HandleOnInputChanged(InputType _currentInput)
    {
        switch (currentMenu)
        {
            case MenuType.None:
                eventSystem.SetSelectedGameObject(null);
                StopFixEventSystemCoroutine();
                break;
            case MenuType.MainMenu:
                eventSystem.firstSelectedGameObject = mainMenuPanel.GetPanelDefaultSelection();
                base.HandleOnInputChanged(_currentInput);
                break;
            case MenuType.Options:
                eventSystem.firstSelectedGameObject = optionsPanel.GetPanelDefaultSelection();
                base.HandleOnInputChanged(_currentInput);
                break;
            case MenuType.Leaderboard:
                eventSystem.firstSelectedGameObject = leaderboardPanel.GetPanelDefaultSelection();
                base.HandleOnInputChanged(_currentInput);
                break;
            case MenuType.Credits:
                eventSystem.firstSelectedGameObject = creditsPanel.GetPanelDefaultSelection();
                base.HandleOnInputChanged(_currentInput);
                break;
            case MenuType.LevelSelection:
                eventSystem.firstSelectedGameObject = levelSelctionPanel.GetButtonToSelect();
                base.HandleOnInputChanged(_currentInput);
                break;
            case MenuType.Loading:
                eventSystem.SetSelectedGameObject(null);
                StopFixEventSystemCoroutine();
                break;
        }
    }
}
