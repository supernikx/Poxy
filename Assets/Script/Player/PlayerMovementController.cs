using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerCollisionController))]
public class PlayerMovementController : MonoBehaviour
{
    #region Delegates
    public delegate void MovementDelegate(Vector3 _movementVelocity);
    public static MovementDelegate OnMovement;
    #endregion

    [Header("Movement Settings")]
    [Header("Ground Settings")]
    [SerializeField]
    /// <summary>
    /// Velocità di movimento
    /// </summary>
    private float MovementSpeed;
    [SerializeField]
    /// <summary>
    /// Velocità di accelerazione e decelerazione se si è a terra
    /// più è bassa più veloce è l'accelerazione
    /// </summary>
    private float AccelerationTimeOnGround;

    [Header("Jump Settings")]
    [SerializeField]
    /// <summary>
    /// Altezza massima raggiungibile in unity unit
    /// </summary>
    private float maxJumpHeight;
    [SerializeField]
    /// <summary>
    /// Altezza minima raggiungibile in unity unit
    /// </summary>
    private float minJumpHeight;
    [SerializeField]
    /// <summary>
    /// Tempo in secondi che ci vuole per raggiungere l'altezza massima del salto
    /// </summary>
    private float JumpTimeToReachTop;
    [SerializeField]
    /// <summary>
    /// Velocità di accelerazione e decelerazione se si è in aria
    /// più è bassa più veloce è l'accelerazione
    /// </summary>
    private float AccelerationTimeOnAir;
    /// <summary>
    /// Variabile che definisce la gravità applicata overtime
    /// se non si è in collisione sopra/sotto
    /// </summary>
    private float gravity;

    [Header("Eject Settings")]
    /// <summary>
    /// Variabile che definisce la forze di eject dal nemico
    /// </summary>
    [SerializeField]
    private float enemyEjectForce;
    /// <summary>
    /// Variabile che definisce la forze di eject dalle piattaforme
    /// </summary>
    [SerializeField]
    private float platformEjectForce;

    /// <summary>
    /// Riferimento al collision controller
    /// </summary>
    private PlayerCollisionController collisionCtrl;
    /// <summary>
    /// Vettore che contiene gli input sull'asse orizzontale e verticale
    /// </summary>
    private Vector2 input;
    /// <summary>
    /// Boolean che definisce se mi posso muovere o meno
    /// </summary>
    bool canMove;

    private void Update()
    {
        if (canMove)
        {
            //Leggo input orrizontali e verticali
            input = PlayerInputManager.GetMovementVector();
            CalculateVelocity();

            Move();

            if (collisionCtrl.GetCollisionInfo().below || collisionCtrl.GetCollisionInfo().above)
            {
                //Se sono in collisione con qualcosa sopra/sotto evito di accumulare gravità
                movementVelocity.y = 0;
            }
        }
    }

    private float velocityXSmoothing;
    private Vector3 movementVelocity;
    /// <summary>
    /// Funzione che esegue i calcoli necessari per far muovere il player
    /// </summary>
    private void Move()
    {
        //Mi muovo
        Vector3 movementVelocityCollision = collisionCtrl.CheckMovementCollisions(movementVelocity * Time.deltaTime);
        transform.Translate(movementVelocityCollision);

        if (OnMovement != null)
            OnMovement(movementVelocityCollision);
    }

    /// <summary>
    /// Funzione che calcola la velocity iniziale del player
    /// </summary>
    private void CalculateVelocity()
    {
        //Calcolo di quanto dovrò traslare
        float targetTranslation = input.x * MovementSpeed;

        //Se sto mirando nelle direzioni diagonali mi muovo al 75% della mia velocità
        if (targetTranslation != 0 && input.y != 0)
            targetTranslation = targetTranslation * 0.75f;

        //Eseguo una breve transizione dalla mia velocity attuale a quella successiva
        movementVelocity.x = Mathf.SmoothDamp(movementVelocity.x, targetTranslation, ref velocityXSmoothing, (collisionCtrl.GetCollisionInfo().below ? AccelerationTimeOnGround : AccelerationTimeOnAir));

        //Aggiungo gravità al player se non sono incollato
        if (!collisionCtrl.GetCollisionInfo().StickyCollision())
            movementVelocity.y += gravity * Time.deltaTime;
    }

    #region Jump    
    /// <summary>
    /// Massima Jump velocity
    /// </summary>
    private float maxJumpVelocity;
    /// <summary>
    /// Minima Jump velocity
    /// </summary>
    private float minJumpVelocity;

    /// <summary>
    /// Funzione chiamata alla pressione del tasto di salto
    /// </summary>
    private void HanldeOnJumpPressed()
    {
        //Controllo se è stato premuto il tasto di salto e se sono a terra
        if (collisionCtrl.GetCollisionInfo().below || collisionCtrl.GetCollisionInfo().HorizontalStickyCollision())
            movementVelocity.y = maxJumpVelocity;
    }

    /// <summary>
    /// Funzione chiamata al rilascio del tasto di salto
    /// </summary>
    private void HanldeOnJumpReleased()
    {
        if (movementVelocity.y > minJumpVelocity)
            movementVelocity.y = minJumpVelocity;
    }
    #endregion

    /// <summary>
    /// Funzione che tratta l'evento OnStickyCollision
    /// </summary>
    private void HandleOnStickyCollision()
    {
        movementVelocity.y = 0;
    }

    #region API
    /// <summary>
    /// Funzione di inizializzazione del player
    /// </summary>
    public void Init(PlayerCollisionController _collisionCtrl)
    {
        collisionCtrl = _collisionCtrl;
        collisionCtrl.OnStickyCollision += HandleOnStickyCollision;

        PlayerInputManager.OnJumpPressed += HanldeOnJumpPressed;
        PlayerInputManager.OnJumpRelease += HanldeOnJumpReleased;

        //Calcolo la gravità
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(JumpTimeToReachTop, 2);

        //Calcolo la velocità del salto
        maxJumpVelocity = Mathf.Abs(gravity) * JumpTimeToReachTop;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

        canMove = false;
    }

    /// <summary>
    /// Funzione che imposta la variabile can move
    /// con la variabile passata come parametro
    /// </summary>
    /// <param name="_canMove"></param>
    public void SetCanMove(bool _canMove)
    {
        canMove = _canMove;
        if (!canMove)
        {
            movementVelocity = Vector3.zero;
        }
    }

    /// <summary>
    /// Funzione che esegue l'eject con l'eject multiplyer settato
    /// </summary>
    public void Eject(ControllableType _controllable)
    {
        float ejectMultiplyer = 0f;
        switch (_controllable)
        {
            case ControllableType.Enemy:
                ejectMultiplyer = enemyEjectForce;
                movementVelocity.y = maxJumpVelocity * ejectMultiplyer;
                break;
            case ControllableType.Platform:
                ejectMultiplyer = platformEjectForce;
                movementVelocity = CalculateEjectVelocity(ejectMultiplyer);
                break;
        }
    }

    private Vector3 CalculateEjectVelocity(float _ejectMult)
    {
        Vector3 _velocity = new Vector3(0, 0, 0);

        if (input.x == 0)
        {
            // verticale in basso o alto
            _velocity.y = maxJumpVelocity * _ejectMult * Math.Sign(input.y);
            Debug.Log(_velocity.x);
        }
        else if (input.y == 0)
        {
            // orizzontale destra o sinistra
            _velocity.x = maxJumpVelocity * _ejectMult * Math.Sign(input.x);
        }

        return _velocity;
    }
    #endregion

    private void OnDisable()
    {
        collisionCtrl.OnStickyCollision -= HandleOnStickyCollision;
        PlayerInputManager.OnJumpPressed -= HanldeOnJumpPressed;
        PlayerInputManager.OnJumpRelease -= HanldeOnJumpReleased;
    }
}
