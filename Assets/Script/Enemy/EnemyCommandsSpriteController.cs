using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCommandsSpriteController : MonoBehaviour
{
    [System.Serializable]
    class CommandButtonSprites
    {
        public GameObject joystickSprite;
        public GameObject keyboardSprite;
    }
    [SerializeField]
    CommandButtonSprites buttonSprite;

    public void Init()
    {
        buttonSprite.joystickSprite.SetActive(false);
        buttonSprite.keyboardSprite.SetActive(false);
    }

    /// <summary>
    /// FUnzione che attiva disattiva il parasite button
    /// </summary>
    /// <param name="_toggle"></param>
    public void ToggleButton(bool _toggle)
    {
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
}
