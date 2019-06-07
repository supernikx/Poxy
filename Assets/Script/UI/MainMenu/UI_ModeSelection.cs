using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ModeSelection : MonoBehaviour
{
    [SerializeField]
    private GameObject defaultButton;

    private UI_SelectLevelButton uI_SelectLevelButton;

    public void Init(UI_SelectLevelButton _uI_SelectLevelButton)
    {
        uI_SelectLevelButton = _uI_SelectLevelButton;
    }

    public void EnablePanel(bool _enable)
    {
        gameObject.SetActive(_enable);
        if (_enable)
            FocusOnPanel();
    }

    public void DeselectLevel()
    {
        uI_SelectLevelButton.DeselectLevel();
    }

    public void SelectMode(bool _select)
    {
        uI_SelectLevelButton.LevelModePressed(_select);
    }

    public GameObject GetDefaultButton()
    {
        return defaultButton;
    }

    public void FocusOnPanel()
    {
        if (InputChecker.GetCurrentInputType() == InputType.Joystick)
            EventSystem.current.SetSelectedGameObject(defaultButton);
        else
            EventSystem.current.SetSelectedGameObject(null);
    }
}
