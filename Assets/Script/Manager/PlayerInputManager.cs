using System;
using System.Collections;
using UnityEngine;
using XInputDotNetPure;

public class PlayerInputManager : MonoBehaviour
{
    #region Delegates
    public static Action OnJumpPressed;
    public static Action OnJumpRelease;
    public static Action OnParasitePressed;
    public static Action OnShotPressed;
    public static Action OnPausePressed;
    public static Action OnConfirmPressed;
    #endregion

    public static PlayerInputManager instance;

    Player player;
    InputType currentInputType;
    GamePadState joystickState;
    GamePadState joystickPrevState;

    //Input
    private Vector2 movementVector;
    private Vector2 aimVector;
    private bool isJumping;
    private bool isShooting;
    private bool canPressParasite = true;
    private bool canReadGameplayInput = true;
    private bool canReadInpit = true;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            player = GetComponent<Player>();
            canReadGameplayInput = true;
            canReadInpit = true;
        }
    }

    private void Update()
    {
        if (!canReadInpit)
            return;

        currentInputType = InputChecker.GetCurrentInputType();
        switch (currentInputType)
        {
            case InputType.Joystick:
                joystickPrevState = joystickState;
                joystickState = InputChecker.GetCurrentGamePadState();
                CheckJoystickInput();
                break;
            case InputType.Keyboard:
                CheckKeyboardInput();
                break;
        }
    }

    /// <summary>
    /// Funzione che controlla gli input del joystick
    /// </summary>
    private void CheckJoystickInput()
    {
        if (canReadGameplayInput)
        {
            //Movement Stick
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

            //Movement DPad
            if (movementVector == Vector2.zero)
            {
                if (joystickState.DPad.Right == ButtonState.Pressed)
                    movementVector.x = 1;
                else if (joystickState.DPad.Left == ButtonState.Pressed)
                    movementVector.x = -1;
                else
                    movementVector.x = 0;

                if (joystickState.DPad.Up == ButtonState.Pressed)
                    movementVector.y = 1;
                else if (joystickState.DPad.Down == ButtonState.Pressed)
                    movementVector.y = -1;
                else
                    movementVector.y = 0;
            }

            //Aim
            aimVector.x = joystickState.ThumbSticks.Right.X;
            aimVector.y = joystickState.ThumbSticks.Right.Y;

            //Jump
            if ((joystickPrevState.Buttons.LeftShoulder == ButtonState.Released && joystickState.Buttons.LeftShoulder == ButtonState.Pressed) ||
                (joystickPrevState.Buttons.A == ButtonState.Released && joystickState.Buttons.A == ButtonState.Pressed))
            {
                isJumping = true;
                if (OnJumpPressed != null)
                    OnJumpPressed();
            }
            else if ((joystickPrevState.Buttons.LeftShoulder == ButtonState.Pressed && joystickState.Buttons.LeftShoulder == ButtonState.Released) ||
                (joystickPrevState.Buttons.A == ButtonState.Pressed && joystickState.Buttons.A == ButtonState.Released))
            {
                isJumping = false;
                if (OnJumpRelease != null)
                    OnJumpRelease();
            }

            //Shoot
            isShooting = joystickState.Triggers.Right > 0;
            if (joystickState.Triggers.Right > 0 && joystickPrevState.Triggers.Right == 0)
            {
                if (OnShotPressed != null)
                    OnShotPressed();
            }

            //Parasite
            if (canPressParasite && (joystickPrevState.Buttons.X == ButtonState.Released && joystickState.Buttons.X == ButtonState.Pressed) ||
                (joystickPrevState.Buttons.RightShoulder == ButtonState.Released && joystickState.Buttons.RightShoulder == ButtonState.Pressed))
            {
                if (OnParasitePressed != null)
                    OnParasitePressed();
            }
        }

        //Pause
        if (joystickPrevState.Buttons.Start == ButtonState.Released && joystickState.Buttons.Start == ButtonState.Pressed)
        {
            if (OnPausePressed != null)
                OnPausePressed();
        }

        //Confirm Button
        if (joystickPrevState.Buttons.A == ButtonState.Released && joystickState.Buttons.A == ButtonState.Pressed)
        {
            if (OnConfirmPressed != null)
                OnConfirmPressed();
        }
    }

    /// <summary>
    /// Funzione che controlla gli input da tastiera
    /// </summary>
    private void CheckKeyboardInput()
    {
        if (canReadGameplayInput)
        {
            //Movement       
            movementVector = new Vector2(Input.GetAxisRaw("AD"), Input.GetAxisRaw("WS"));

            //Aim
            Vector3 playerScreen = Camera.main.WorldToScreenPoint(player.transform.position);
            aimVector = (Input.mousePosition - playerScreen).normalized;

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
            if (Input.GetMouseButtonDown(0))
            {
                if (OnShotPressed != null)
                    OnShotPressed();
            }

            //Parasite
            if (canPressParasite && Input.GetKeyDown(KeyCode.Q))
            {
                if (OnParasitePressed != null)
                    OnParasitePressed();
            }
        }

        //Pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (OnPausePressed != null)
                OnPausePressed();
        }

        //Confirm Button
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (OnConfirmPressed != null)
                OnConfirmPressed();
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
    /// Funzione che ritorna il vettore di mira
    /// </summary>
    /// <returns></returns>
    public static Vector2 GetAimVector()
    {
        return instance.aimVector;
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

    /// <summary>
    /// Funzione che fa vibrare il controller
    /// </summary>
    public static void Rumble(float _leftMotorIntensity, float _rightMotorIntensity, float _duration)
    {
        if (instance.currentInputType == InputType.Joystick)
            instance.StartCoroutine(instance.RumbleCoroutine(_leftMotorIntensity, _rightMotorIntensity, _duration));
    }

    /// <summary>
    /// Funzione che imposta la variabile canReadGameplayInput con il valore passato come parametro
    /// </summary>
    /// <param name="_switch"></param>
    public static void SetCanReadGameplayInput(bool _switch)
    {
        instance.canReadGameplayInput = _switch;
        if (!instance.canReadGameplayInput)        
            instance.movementVector = Vector2.zero;        
    }

    /// <summary>
    /// Funzione che imposta la variabile canReadInput con il valore passato come parametro
    /// </summary>
    /// <param name="_switch"></param>
    public static void SetCanReadInput(bool _switch)
    {
        instance.canReadInpit = _switch;
    }

    /// <summary>
    /// Funzione che ritarda del tempo che si passa come parametro la pressio del tasto parassita
    /// </summary>
    /// <param name="_time"></param>
    public static void DelayParasiteButtonPress(float _time)
    {
        instance.StartCoroutine(instance.ParasiteDelay(_time));
    }
    #endregion

    /// <summary>
    /// Coroutine che fa virbrare il joystick per la potenza e il tempo indicato
    /// </summary>
    /// <param name="_leftMotorIntensity"></param>
    /// <param name="_rightMotorIntensity"></param>
    /// <param name="_duration"></param>
    /// <returns></returns>
    private IEnumerator RumbleCoroutine(float _leftMotorIntensity, float _rightMotorIntensity, float _duration)
    {
        GamePad.SetVibration(InputChecker.GetJoystickPlayerIndex(), _leftMotorIntensity, _rightMotorIntensity);
        yield return new WaitForSecondsRealtime(_duration);
        GamePad.SetVibration(InputChecker.GetJoystickPlayerIndex(), 0f, 0f);
    }

    /// <summary>
    /// Coroutine che disattiva/riattiva la possibilità di premere il tasto parasite
    /// </summary>
    /// <returns></returns>
    private IEnumerator ParasiteDelay(float _time)
    {
        canPressParasite = false;
        yield return new WaitForSeconds(_time);
        canPressParasite = true;
    }
}
