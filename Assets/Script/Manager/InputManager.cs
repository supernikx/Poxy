using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager singleton;

    #region Delegates
    public delegate void GeneralInputDelegates();
    public static GeneralInputDelegates OnShotButtonPressed;
    public static GeneralInputDelegates OnParasiteButtonPressed;
    public static GeneralInputDelegates OnChangeBulletPressed;
    public static GeneralInputDelegates OnJumpPressed;

    public delegate void AxisInputDelegates(Vector2 _axis);
    public static AxisInputDelegates OnKeyboardAxisPressed;
    public static AxisInputDelegates OnJoystickLeftStick;
    public static AxisInputDelegates OnJoystickRightStick;
    #endregion

    #region Input Settings
    [Header("Keyboard/Mouse Input Settings")]
    [SerializeField]
    private KeyCode KMParasiteButton;
    [SerializeField]
    private KeyCode KMJumpButton;
    [SerializeField]
    private KeyCode KMShotButton;
    [SerializeField]
    private KeyCode KMChangeBulletButton;

    [Header("Joystick Input Settings")]
    [SerializeField]
    private KeyCode JJumpButton;
    [SerializeField]
    private KeyCode JShotButton;
    #endregion

    /// <summary>
    /// Posizione precedente del mouse
    /// </summary>
    private Vector3 mousePreviewsPos;

    void Update()
    {
        if ((Input.GetKeyDown(KMParasiteButton) || CheckJoystickLTTrigger()) && OnParasiteButtonPressed != null)
        {
            OnParasiteButtonPressed();
        }

        if ((Input.GetKeyDown(KMChangeBulletButton) || CheckJoystickRTTrigger()) && OnChangeBulletPressed != null)
        {
            OnChangeBulletPressed();
        }

        Vector2 keyboardAxis = new Vector2(Input.GetAxisRaw("KeyboardADAxis"), Input.GetAxisRaw("KeyboardWSAxis"));
        if ((keyboardAxis.x != 0 || keyboardAxis.y != 0) && OnKeyboardAxisPressed != null)
        {
            OnKeyboardAxisPressed(keyboardAxis);
        }

        if ((Input.GetKeyDown(KMJumpButton) || Input.GetKeyDown(JJumpButton)) && OnJumpPressed != null)
        {
            OnJumpPressed();
        }
    }

    #region API
    /// <summary>
    /// Funzione che ritorna la posizione attuale del mouse
    /// </summary>
    /// <returns></returns>
    public static Vector3 GetMousePosition()
    {
        return Input.mousePosition;
    }

    /// <summary>
    /// Funzione che controlla se usare gli input del mouse o del controller
    /// </summary>
    /// <returns></returns>   
    /*public static bool UseMouseInput()
    {
        if (Mathf.Approximately(Input.mousePosition.x, mousePreviewsPos.x) && Mathf.Approximately(Input.mousePosition.y, mousePreviewsPos.y))
        {
            if (Input.GetJoystickNames().Where(j => j != "").FirstOrDefault() != null)
                return false;
            return true;
        }

        mousePreviewsPos = Input.mousePosition;
        return true;
    }*/
    #endregion

    #region Joystick Trigger
    protected bool ltWasDown;
    protected bool CheckJoystickLTTrigger()
    {
        if (Input.GetAxis("LT") > 0)
        {
            if (!ltWasDown)
            {
                ltWasDown = true;
                return true;
            }
            return false;
        }
        else
        {
            ltWasDown = false;
            return false;
        }
    }

    bool rtWasDown;
    protected bool CheckJoystickRTTrigger()
    {
        if (Input.GetAxis("RT") > 0)
        {
            if (!rtWasDown)
            {
                rtWasDown = true;
                return true;
            }
            return false;
        }
        else
        {
            rtWasDown = false;
            return false;
        }
    }
    #endregion
}

