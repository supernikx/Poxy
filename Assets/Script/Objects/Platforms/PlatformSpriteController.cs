using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSpriteController : MonoBehaviour
{
    [System.Serializable]
    class CommandButtonSprites
    {
        public GameObject joystickSprite;
        public GameObject keyboardSprite;
    }
    [Header("Command Sprites")]
    [SerializeField]
    CommandButtonSprites buttonSprite;

    #region API
    public void Init()
    {
        buttonSprite.joystickSprite.SetActive(false);
        buttonSprite.keyboardSprite.SetActive(false);
    }
    #endregion

    #region Buttons
    public bool IsButtonActive()
    {
        switch (InputChecker.GetCurrentInputType())
        {
            case InputType.Joystick:
                return buttonSprite.joystickSprite.activeSelf;
            case InputType.Keyboard:
                return buttonSprite.keyboardSprite.activeSelf;
        }

        return false;
    }

    /// <summary>
    /// FUnzione che attiva disattiva il parasite button
    /// </summary>
    /// <param name="_toggle"></param>
    public void ToggleButton(bool _toggle)
    {
        if (_toggle)
            InputChecker.OnInputChanged += HandleOnInputChange;
        else
            InputChecker.OnInputChanged -= HandleOnInputChange;

        switch (InputChecker.GetCurrentInputType())
        {
            case InputType.None:
                buttonSprite.joystickSprite.SetActive(false);
                buttonSprite.keyboardSprite.SetActive(false);
                break;
            case InputType.Joystick:
                buttonSprite.joystickSprite.SetActive(_toggle);
                buttonSprite.keyboardSprite.SetActive(false);
                break;
            case InputType.Keyboard:
                buttonSprite.keyboardSprite.SetActive(_toggle);
                buttonSprite.joystickSprite.SetActive(false);
                break;
        }
    }

    private void HandleOnInputChange(InputType _newType)
    {
        switch (_newType)
        {
            case InputType.None:
                buttonSprite.joystickSprite.SetActive(false);
                buttonSprite.keyboardSprite.SetActive(false);
                break;
            case InputType.Joystick:
                buttonSprite.joystickSprite.SetActive(true);
                buttonSprite.keyboardSprite.SetActive(false);
                break;
            case InputType.Keyboard:
                buttonSprite.keyboardSprite.SetActive(true);
                buttonSprite.joystickSprite.SetActive(false);
                break;
        }
    }
    #endregion

    private void OnDisable()
    {
        InputChecker.OnInputChanged -= HandleOnInputChange;
    }
}
