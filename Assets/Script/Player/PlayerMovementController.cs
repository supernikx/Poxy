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
            input = InputManager.GetMovementVector();
            CalculateVelocity();

            Move();

            if (collisionCtrl.GetCollisionInfo().below || collisionCtrl.GetCollisionInfo().above)
            {
                //Se sono in collisione con qualcosa sopra/sotto evito di accumulare gravità
                movementVelocity.y = 0;
            }

            if (eject)
            {
                Eject();
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
        //Eseguo una breve transizione dalla mia velocity attuale a quella successiva
        movementVelocity.x = Mathf.SmoothDamp(movementVelocity.x, (input.x * MovementSpeed), ref velocityXSmoothing, (collisionCtrl.GetCollisionInfo().below ? AccelerationTimeOnGround : AccelerationTimeOnAir));

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
        if (!eject && (collisionCtrl.GetCollisionInfo().below || collisionCtrl.GetCollisionInfo().HorizontalStickyCollision()))
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

        InputManager.OnJumpPressed += HanldeOnJumpPressed;
        InputManager.OnJumpRelease += HanldeOnJumpReleased;

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

    #region Eject
    /// <summary>
    /// Funzione che fa eseguire l'eject al player
    /// </summary>
    bool eject;
    float ejectMultiplyer;
    public void SetEject(ControllableType _controllable)
    {
        switch (_controllable)
        {
            case ControllableType.Enemy:
                ejectMultiplyer = enemyEjectForce;
                break;
            case ControllableType.Platform:
                ejectMultiplyer = platformEjectForce;
                break;
            default:
                break;
        }
        eject = true;
    }

    /// <summary>
    /// Funzione che esegue l'eject con l'eject multiplyer settato
    /// </summary>
    public void Eject()
    {
        movementVelocity.y = 0f;
        movementVelocity.y += maxJumpVelocity * ejectMultiplyer;
        eject = false;
    }
    #endregion
    #endregion

    private void OnDisable()
    {
        collisionCtrl.OnStickyCollision -= HandleOnStickyCollision;
        InputManager.OnJumpPressed -= HanldeOnJumpPressed;
        InputManager.OnJumpRelease -= HanldeOnJumpReleased;
    }
}
