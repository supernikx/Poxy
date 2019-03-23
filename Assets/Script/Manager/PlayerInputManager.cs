using System;
using UnityEngine;
using XInputDotNetPure;

public class PlayerInputManager : MonoBehaviour
{
    #region Delegates
    public static Action OnJumpPressed;
    public static Action OnJumpRelease;
    public static Action OnParasitePressed;
    public static Action OnPausePressed;
    #endregion

    public static PlayerInputManager instance;

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
    private bool isShooting;

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

        if (LX >= 0.5)
            movementVector.x = 1;
        else if (LX <= -0.5)
            movementVector.x = -1;
        else
            movementVector.x = 0;

        if (LY >= 0.5)
            movementVector.y = 1;
        else if (LY <= -0.5)
            movementVector.y = -1;
        else
            movementVector.y = 0;


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

        //Shoot
        isShooting = joystickState.Triggers.Right > 0;

        //Parasite
        if (joystickPrevState.Buttons.LeftShoulder == ButtonState.Released && joystickState.Buttons.LeftShoulder == ButtonState.Pressed ||
            joystickPrevState.Buttons.RightShoulder == ButtonState.Released && joystickState.Buttons.RightShoulder == ButtonState.Pressed)
        {
            if (OnParasitePressed != null)
                OnParasitePressed();
        }

        //Pause
        if (joystickPrevState.Buttons.Start == ButtonState.Released && joystickState.Buttons.Start == ButtonState.Pressed)
        {
            if (OnPausePressed != null)
                OnPausePressed();
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

        //Shoot
        isShooting = Input.GetMouseButton(0);

        //Parasite
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (OnParasitePressed != null)
                OnParasitePressed();
        }

        //Pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (OnPausePressed != null)
                OnPausePressed();
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
    public static bool IsJumping()
    {
        return instance.isJumping;
    }

    /// <summary>
    /// Funzione che ritorna se il tasto di sparo è premuto o no
    /// </summary>
    /// <returns></returns>
    public static bool IsShooting()
    {
        return instance.isShooting;
    }
    #endregion
}

public enum InputType
{
    Joystick,
    Keyboard,
}
