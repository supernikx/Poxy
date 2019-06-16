using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpriteController : MonoBehaviour
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

    [Header("Exclamation Marks")]
    [SerializeField]
    private GameObject exclamationSprite;
    [SerializeField]
    private float exclamationTriggerToleranceTotal;
    [SerializeField]
    private bool exclamationMarkActive = true;

    private bool enemyParasiteState = false;

    #region API
    public void Init()
    {
        buttonSprite.joystickSprite.SetActive(false);
        buttonSprite.keyboardSprite.SetActive(false);
        if (exclamationSprite != null)
            exclamationSprite.SetActive(false);

        // Exclamation Events
        EnemyToleranceController.OnToleranceChange += HandleOnToleranceChange;
    }

    public void SetParasiteState(bool _val)
    {
        enemyParasiteState = _val;
        if (!enemyParasiteState)
            exclamationSprite.SetActive(false);
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

    #region Exclamation
    private void HandleOnToleranceChange(float _tolerance)
    {
        if (exclamationMarkActive)
        {
            if (enemyParasiteState && _tolerance >= exclamationTriggerToleranceTotal)
            {
                exclamationSprite.SetActive(true);
            }
            else if (enemyParasiteState && _tolerance < exclamationTriggerToleranceTotal)
            {
                exclamationSprite.SetActive(false);
            }

            if (enemyParasiteState && _tolerance <= 0)
                exclamationSprite.SetActive(false); 
        }
    }
    #endregion

    private void OnDisable()
    {
        InputChecker.OnInputChanged -= HandleOnInputChange;
        EnemyToleranceController.OnToleranceChange -= HandleOnToleranceChange;
    }
}
