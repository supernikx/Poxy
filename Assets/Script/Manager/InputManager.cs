using System;
using UnityEngine;
using XInputDotNetPure;

public class InputManager : MonoBehaviour
{
    #region Delegates
    public static Action OnJumpPressed;
    public static Action OnJumpRelease;
    #endregion

    public static InputManager instance;

    [Header("Joystick Settings")]
    [SerializeField]
    private float LeftStickDeadZone;

    bool playerIndexSet = false;
    PlayerIndex joystickPlayerIndex;
    GamePadState joystickState;
    GamePadState joystickPrevState;

    //Input
    private Vector2 movementVector;
    private bool isJumping;

    private static bool ltWasDown;
    public static bool GetLTDown()
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

    private static bool rtWasDown;
    public static bool GetRTDown()
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

    public static bool GetRT()
    {
        if (Input.GetAxis("RT") > 0)
        {
            if (!rtWasDown)
            {
                rtWasDown = true;
            }
            return true;
        }
        else
        {
            rtWasDown = false;
            return false;
        }
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            DestroyImmediate(gameObject);
    }

    private void Update()
    {
        switch (CheckInputType())
        {
            case InputType.Joystick:
                joystickPrevState = joystickState;
                joystickState = GamePad.GetState(joystickPlayerIndex);                
                CheckJoystickInput();
                break;
            case InputType.Keyboard:
                CheckKeyboardInput();
                break;
        }
    }

    /// <summary>
    /// Funzione che controlla e ritorna se si deve utilizzare i comandi da tastiera o da joystick
    /// </summary>
    /// <returns></returns>
    private InputType CheckInputType()
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

        if (playerIndexSet)
            return InputType.Joystick;
        else
            return InputType.Keyboard;
    }

    /// <summary>
    /// Funzione che controlla gli input del joystick
    /// </summary>
    private void CheckJoystickInput()
    {
        //Movement
        float LX = joystickState.ThumbSticks.Left.X;
        float LY = joystickState.ThumbSticks.Left.Y;

        float magnitude = Mathf.Sqrt(LX * LX + LY * LY);

        if (magnitude > LeftStickDeadZone)
        {
            movementVector = new Vector2(LX, LY);
        }
        else
        {
            magnitude = 0.0f;
            movementVector = Vector2.zero;
        }

        //Jump
        if (joystickPrevState.Buttons.A == ButtonState.Released && joystickState.Buttons.A == ButtonState.Pressed)
        {
            isJumping = true;
            if (OnJumpPressed != null)
                OnJumpPressed();
        }
        else if (joystickPrevState.Buttons.A == ButtonState.Pressed && joystickState.Buttons.A == ButtonState.Released)
        {
            isJumping = false;
            if (OnJumpRelease != null)
                OnJumpRelease();
        }
    }

    /// <summary>
    /// Funzione che controlla gli input da tastiera
    /// </summary>
    private void CheckKeyboardInput()
    {
        //Movement
        movementVector = new Vector2(Input.GetAxisRaw("AD"), Input.GetAxisRaw("WS"));

        //Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isJumping = true;
            if (OnJumpPressed != null)
                OnJumpPressed();
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
            if (OnJumpRelease != null)
                OnJumpRelease();
        }
    }

    #region API
    /// <summary>
    /// Funzione che ritorna il vettore di movimento
    /// </summary>
    /// <returns></returns>
    public static Vector2 GetMovementVector()
    {
        return instance.movementVector;
    }

    /// <summary>
    /// Funzione che ritorna se il tasto di salto è premuto o no
    /// </summary>
    /// <returns></returns>
    public static bool JumpPressed()
    {
        return instance.isJumping;
    }
    #endregion
}

public enum InputType
{
    Joystick,
    Keyboard,
}
