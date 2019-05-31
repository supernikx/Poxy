using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UI;

public class UI_SelectLevelButton : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI levelNameText;
    [SerializeField]
    private GameObject selectModePanel;
    private LevelScriptable level;

    UIMenu_LevelSelection menuLevelSelection;

    public void Init(LevelScriptable _level, UIMenu_LevelSelection _menuLevelSelection)
    {
        level = _level;
        menuLevelSelection = _menuLevelSelection;
        levelNameText.text = level.LevelName;
        SelectMode(false);
    }

    public void LevelButtonPressed()
    {
        if (menuLevelSelection.OnLevelButtonPressed != null)
            menuLevelSelection.OnLevelButtonPressed(this);
    }

    public void LevelModePressed(bool _speedRun)
    {
        if (UIMenu_LevelSelection.OnModeSelected != null)
            UIMenu_LevelSelection.OnModeSelected(_speedRun);
    }

    public LevelScriptable GetLevelScriptable()
    {
        return level;
    }

    public void SelectMode(bool _enable)
    {
        selectModePanel.SetActive(_enable);
    }
}
