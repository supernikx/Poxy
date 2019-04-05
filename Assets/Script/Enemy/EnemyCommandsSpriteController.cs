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
    CommandButtonSprites parasiteSprite;

    public void Init()
    {
        parasiteSprite.joystickSprite.SetActive(false);
        parasiteSprite.keyboardSprite.SetActive(false);
    }

    /// <summary>
    /// FUnzione che attiva disattiva il parasite button
    /// </summary>
    /// <param name="_toggle"></param>
    public void ToggleParasiteButton(bool _toggle)
    {
        switch (InputChecker.GetCurrentInputType())
        {
            case InputType.None:
                parasiteSprite.joystickSprite.SetActive(false);
                parasiteSprite.keyboardSprite.SetActive(false);
                break;
            case InputType.Joystick:
                parasiteSprite.joystickSprite.SetActive(_toggle);
                parasiteSprite.keyboardSprite.SetActive(false);
                break;
            case InputType.Keyboard:
                parasiteSprite.keyboardSprite.SetActive(_toggle);
                parasiteSprite.joystickSprite.SetActive(false);
                break;
        }
    }
}
