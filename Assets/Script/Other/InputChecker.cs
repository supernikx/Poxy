using System;
using System.Collections;
using UnityEngine;
using XInputDotNetPure;

public class InputChecker : MonoBehaviour
{
    #region Delegates
    public static Action<InputType> OnInputChanged;
    #endregion

    public static InputChecker instance;

    private bool playerIndexSet = false;
    private PlayerIndex joystickPlayerIndex;
    private InputType currentinputType;
    private GamePadState joystickState;
    private GamePadState joystickPrevState;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            StartCoroutine(CheckInputTypeCoroutine());
        }
    }

    /// <summary>
    /// Coroutine che in loop controlla il tipo di input in uso
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckInputTypeCoroutine()
    {
        while (true)
        {
            CheckInputType();
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }

    /// <summary>
    /// Funzione che controlla e ritorna se si deve utilizzare i comandi da tastiera o da joystick
    /// </summary>
    /// <returns></returns>
    private void CheckInputType()
    {
        if (!playerIndexSet || !joystickPrevState.IsConnected)
        {
            playerIndexSet = false;
            for (int i = 0; i < 4; ++i)
            {
                PlayerIndex testPlayerIndex = (PlayerIndex)i;
                GamePadState testState = GamePad.GetState(testPlayerIndex);
                if (testState.IsConnected)
                {
                    Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
                    joystickPlayerIndex = testPlayerIndex;
                    playerIndexSet = true;
                    break;
                }

            }
        }

        joystickPrevState = joystickState;
        joystickState = GamePad.GetState(joystickPlayerIndex);

        if ((playerIndexSet && currentinputType == InputType.None) || (playerIndexSet && currentinputType == InputType.Keyboard))
        {
            currentinputType = InputType.Joystick;
            if (OnInputChanged != null)
                OnInputChanged(currentinputType);
        }
        else if ((!playerIndexSet && currentinputType == InputType.None) || (!playerIndexSet && currentinputType == InputType.Joystick))
        {
            currentinputType = InputType.Keyboard;
            if (OnInputChanged != null)
                OnInputChanged(currentinputType);
        }
    }

    #region Getter
    /// <summary>
    /// Funzione che ritorna il tipo di input attuale
    /// </summary>
    /// <returns></returns>
    public static InputType GetCurrentInputType()
    {
        return instance.currentinputType;
    }

    /// <summary>
    /// Funzione che ritorna lo stato del joystcik collegato
    /// </summary>
    /// <returns></returns>
    public static GamePadState GetCurrentGamePadState()
    {
        return GamePad.GetState(instance.joystickPlayerIndex);
    }

    /// <summary>
    /// Funzione che ritorna il player index del joystick attivo
    /// </summary>
    /// <returns></returns>
    public static PlayerIndex GetJoystickPlayerIndex()
    {
        return instance.joystickPlayerIndex;
    }
    #endregion

}

public enum InputType
{
    None,
    Joystick,
    Keyboard,
}
