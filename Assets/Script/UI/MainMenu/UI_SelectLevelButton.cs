using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UI;
using System;
using UnityEngine.EventSystems;

public class UI_SelectLevelButton : MonoBehaviour
{
    [SerializeField]
    private LevelScriptable level;
    [SerializeField]
    private TextMeshProUGUI levelNameText;
    [SerializeField]
    private UI_ModeSelection selectModePanel;

    Button button;
    UIMenu_LevelSelection menuLevelSelection;

    public void Init(UIMenu_LevelSelection _menuLevelSelection)
    {
        button = GetComponent<Button>();
        menuLevelSelection = _menuLevelSelection;
        levelNameText.text = level.LevelName;
        selectModePanel.Init(this);
        SelectModePanel(false);
    }

    public void LevelButtonPressed()
    {
        if (menuLevelSelection.OnLevelButtonPressed != null)
            menuLevelSelection.OnLevelButtonPressed(this);
    }

    public void DeselectLevel()
    {
        FocusOnPanel();
        if (menuLevelSelection != null)
            menuLevelSelection.OnLevelButtonDeselected();
    }

    public void LevelModePressed(bool _speedRun)
    {
        if (menuLevelSelection != null)
            menuLevelSelection.OnModeButtonPressed(_speedRun);
    }

    public void EnableButton(bool _enable)
    {
        button.interactable = _enable;
    }

    public LevelScriptable GetLevelScriptable()
    {
        return level;
    }

    public void SelectModePanel(bool _enable)
    {
        selectModePanel.EnablePanel(_enable);
    }

    public UI_ModeSelection GetModeSelectionPanel()
    {
        return selectModePanel;
    }

    public void FocusOnPanel()
    {
        if (InputChecker.GetCurrentInputType() == InputType.Joystick)
            EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
