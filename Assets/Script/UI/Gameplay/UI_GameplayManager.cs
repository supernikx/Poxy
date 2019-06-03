using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;

public class UI_GameplayManager : UI_ManagerBase
{
    [Header("Panels")]
    [SerializeField]
    private UIMenu_LoadingPanel loadingPanel;
    [SerializeField]
    private UIMenu_GamePanel gamePanel;
    [SerializeField]
    private UIMenu_PausePanel pausePanel;
    [SerializeField]
    private UIMenu_Options optionsPanel;
    [SerializeField]
    private UI_MenuEndGamePanel endGamePanel;
    [SerializeField]
    private UIMenu_CountdownPanel countdownPanel;
    /// <summary>
    /// Riferimento al menù attualmente attivo
    /// </summary>
    private MenuType currentMenu;

    #region Getter
    /// <summary>
    /// Funzione che ritorna l'uigameplay manager
    /// </summary>
    /// <returns></returns>
    public override UI_GameplayManager GetGameplayManager()
    {
        return this;
    }

    /// <summary>
    /// Funzione che ritorna il loading panel
    /// </summary>
    /// <returns></returns>
    public UIMenu_LoadingPanel GetLoadingPanel()
    {
        return loadingPanel;
    }

    /// <summary>
    /// Funzione che ritorna il game panel
    /// </summary>
    /// <returns></returns>
    public UIMenu_GamePanel GetGamePanel()
    {
        return gamePanel;
    }

    /// <summary>
    /// Funzione che ritorna il pause panel
    /// </summary>
    /// <returns></returns>
    public UIMenu_PausePanel GetPausePanel()
    {
        return pausePanel;
    }

    /// <summary>
    /// Funzione che ritorna il countdown panel
    /// </summary>
    /// <returns></returns>
    public UIMenu_CountdownPanel GetCountdownPanel()
    {
        return countdownPanel;
    }
    #endregion

    #region Pause
    /// <summary>
    /// Funzione che si occupa dell'evento LevelManager.OnGamePause 
    /// </summary>
    private void OnGamePause()
    {
        ToggleMenu(MenuType.Pause);
    }

    /// <summary>
    /// Funzione che si occupa dell'evento LevelManager.OnGameUnPause 
    /// </summary>
    private void OnGameUnPause()
    {
        ToggleMenu(MenuType.Game);
    }
    #endregion

    /// <summary>
    /// Funzione che imposta i settaggi come devono essere alla conclusione del Setup
    /// </summary>
    public override void StartSetup()
    {
        LevelManager.OnGamePause += OnGamePause;
        LevelManager.OnGameUnPause += OnGameUnPause;
        ToggleMenu(MenuType.Loading);
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
            case MenuType.Loading:
                loadingPanel.Enable();
                break;
            case MenuType.Countdown:
                countdownPanel.Enable();
                break;
            case MenuType.Game:
                gamePanel.Enable();
                break;
            case MenuType.Pause:
                pausePanel.Enable();                
                break;
            case MenuType.Options:
                optionsPanel.Enable();
                break;
            case MenuType.EndGame:
                endGamePanel.Enable();
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
            case MenuType.Loading:
                eventSystem.SetSelectedGameObject(null);
                StopFixEventSystemCoroutine();
                break;
            case MenuType.Countdown:
                eventSystem.SetSelectedGameObject(null);
                StopFixEventSystemCoroutine();
                break;
            case MenuType.Game:
                eventSystem.SetSelectedGameObject(null);
                StopFixEventSystemCoroutine();
                break;
            case MenuType.Pause:
                eventSystem.firstSelectedGameObject = pausePanel.GetPanelDefaultSelection();
                base.HandleOnInputChanged(_currentInput);
                break;
            case MenuType.Options:
                eventSystem.firstSelectedGameObject = optionsPanel.GetPanelDefaultSelection();
                base.HandleOnInputChanged(_currentInput);
                break;
            case MenuType.EndGame:
                eventSystem.firstSelectedGameObject = endGamePanel.GetPanelDefaultSelection();
                base.HandleOnInputChanged(_currentInput);
                break;
        }
    }

    private void OnDisable()
    {
        LevelManager.OnGamePause -= OnGamePause;
        LevelManager.OnGameUnPause -= OnGameUnPause;
    }
}
