using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using UI;

public abstract class UI_ManagerBase : MonoBehaviour
{
    private GameManager gm;
    private List<UIMenu_Base> menus;
    protected EventSystem eventSystem;

    public void Setup(GameManager _gm)
    {
        InputChecker.OnInputChanged += HandleOnInputChanged;
        eventSystem = EventSystem.current;

        gm = _gm;

        menus = new List<UIMenu_Base>();
        menus = GetComponentsInChildren<UIMenu_Base>(true).ToList();

        foreach (UIMenu_Base menu in menus)
            menu.Setup(this);

        StartSetup();
    }

    public abstract void StartSetup();

    /// <summary>
    /// Funzione che abilita il menù passato come parametro
    /// </summary>
    /// <param name="_menu"></param>
    public abstract void ToggleMenu(MenuType _menu);

    /// <summary>
    /// Funzione che disabilita tutti i menu
    /// </summary>
    public void DisableAllMenus()
    {
        foreach (UIMenu_Base menu in menus)
            menu.Disable();
    }

    /// <summary>
    /// Funzione che ritorna l'ui gameplay manager
    /// </summary>
    /// <returns></returns>
    public virtual UI_GameplayManager GetGameplayManager()
    {
        Debug.LogError("UI Gameplay Manager non presente");
        return null;
    }

    /// <summary>
    /// Funzione che ritorna l'ui menù manager
    /// </summary>
    /// <returns></returns>
    public virtual UI_MenuManager GetMenuManager()
    {
        Debug.LogError("UI Menu Manager non presente");
        return null;
    }

    /// <summary>
    /// Funzione che gestisce l'evento InputChecker.OnInputChanged
    /// </summary>
    /// <param name="_currentInput"></param>
    protected virtual void HandleOnInputChanged(InputType _currentInput)
    {
        switch (_currentInput)
        {
            case InputType.Joystick:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                storedSelected = null;
                if (eventSystemFix == null)
                {
                    eventSystemFix = FixEventSystemCoroutine();
                    StartCoroutine(eventSystemFix);
                }
                break;
            case InputType.Keyboard:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                StopFixEventSystemCoroutine();
                eventSystem.SetSelectedGameObject(null);
                break;
        }
    }

    /// <summary>
    /// Funzione che controlla il current input e esegue le funzioni di conseguenza
    /// </summary>
    protected virtual void CheckEventSystemInput()
    {
        HandleOnInputChanged(InputChecker.GetCurrentInputType());
    }

    #region FixEventSystem
    protected IEnumerator eventSystemFix;
    GameObject storedSelected;

    /// <summary>
    /// Funzione che stoppa la coroutine FixEventSystem
    /// </summary>
    protected void StopFixEventSystemCoroutine()
    {
        if (eventSystemFix != null)
        {
            StopCoroutine(eventSystemFix);
            eventSystemFix = null;
            storedSelected = null;
        }
    }

    /// <summary>
    /// Coroutine che reimposta il bottone selezionato se si perde la referenza
    /// </summary>
    /// <returns></returns>
    private IEnumerator FixEventSystemCoroutine()
    {
        while (true)
        {
            if ((eventSystem.currentSelectedGameObject == null && storedSelected == null) || (eventSystem.currentSelectedGameObject != storedSelected))
            {
                if (storedSelected == null)
                {
                    storedSelected = eventSystem.firstSelectedGameObject;
                    StartCoroutine(FixSelectionHiglightCoroutine(storedSelected));
                }
                else if (eventSystem.currentSelectedGameObject == null)
                    StartCoroutine(FixSelectionHiglightCoroutine(storedSelected));
                else
                    storedSelected = eventSystem.currentSelectedGameObject;
            }
            yield return null;
        }
    }

    /// <summary>
    /// Coroutine che imposta il bottone selezionato con quello passato come parametro
    /// </summary>
    /// <param name="_gobjectToSelect"></param>
    /// <returns></returns>
    private IEnumerator FixSelectionHiglightCoroutine(GameObject _gobjectToSelect)
    {
        eventSystem.SetSelectedGameObject(null);
        yield return null;
        eventSystem.SetSelectedGameObject(_gobjectToSelect);
    }
    #endregion

    private void OnDisable()
    {
        StopAllCoroutines();
        InputChecker.OnInputChanged -= HandleOnInputChanged;
    }
}

public enum MenuType
{
    None,
    MainMenu,
    Options,
    Leaderboard,
    LevelSelection,
    Loading,
    Countdown,
    Game,
    Pause,
    EndGame,
}

