using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerCollisionController))]
public class PlayerMovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Header("Ground Settings")]
    /// <summary>
    /// Velocità di movimento
    /// </summary>
    public float MovementSpeed;
    /// <summary>
    /// Velocità di accelerazione e decelerazione se si è a terra
    /// più è bassa più veloce è l'accelerazione
    /// </summary>
    public float AccelerationTimeOnGround;

    [Header("Jump Settings")]
    /// <summary>
    /// Altezza massima raggiungibile in unity unit
    /// </summary>
    public float JumpUnitHeight;
    /// <summary>
    /// Tempo in secondi che ci vuole per raggiungere l'altezza massima del salto
    /// </summary>
    public float JumpTimeToReachTop;
    /// <summary>
    /// Velocità di accelerazione e decelerazione se si è in aria
    /// più è bassa più veloce è l'accelerazione
    /// </summary>
    public float AccelerationTimeOnAir;
    /// <summary>
    /// Variabile che definisce la gravità applicata overtime
    /// se non si è in collisione sopra/sotto
    /// </summary>
    private float gravity;

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

    private bool isJumping;

    private float jumpTimer = 0;

    private void Update()
    {
        jumpTimer += Time.deltaTime;

        if (canMove)
        {
            if (collisionCtrl.GetCollisionInfo().below || collisionCtrl.GetCollisionInfo().above)
            {
                //Se sono in collisione con qualcosa sopra/sotto evito di accumulare gravità
                movementVelocity.y = 0;

                if (collisionCtrl.GetCollisionInfo().below)
                {                    
                    isJumping = false;
                }
            }

            //Leggo input orrizontali e verticali
            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            //Controllo se è stato premuto il tasto di salto e se sono a terra
            if (Input.GetButtonDown("Jump") && (collisionCtrl.GetCollisionInfo().below || collisionCtrl.GetCollisionInfo().HorizontalStickyCollision()))
            {
                Jump();
            }

            if (Input.GetButtonUp("Jump") && isJumping && jumpTimer <= JumpTimeToReachTop)
            {
                StopJump();
            }

            if (eject)
            {
                Eject();
            }

            if (!collisionCtrl.GetCollisionInfo().StickyCollision())
                AddGravity();

            Move();
        }
    }

    /// <summary>
    /// Funzione che aggiunge gravità al player
    /// </summary>
    private void AddGravity()
    {
        //Aggiungo gravità al player
        movementVelocity.y += gravity * Time.deltaTime;
    }

    private float velocityXSmoothing;
    private Vector3 movementVelocity;
    /// <summary>
    /// Funzione che esegue i calcoli necessari per far muovere il player
    /// </summary>
    private void Move()
    {
        //Eseguo una breve transizione dalla mia velocity attuale a quella successiva
        movementVelocity.x = Mathf.SmoothDamp(movementVelocity.x, (input.x * MovementSpeed), ref velocityXSmoothing, (collisionCtrl.GetCollisionInfo().below ? AccelerationTimeOnGround : AccelerationTimeOnAir));

        //Mi muovo
        transform.Translate(collisionCtrl.CheckMovementCollisions(movementVelocity * Time.deltaTime));
    }

    /// <summary>
    /// Funzione che imposta la velocity dell'asse verticale per saltare
    /// </summary>
    private float jumpVelocity;
    private void Jump()
    {
        movementVelocity.y = jumpVelocity;
        isJumping = true;
        jumpTimer = 0;
    }

    private void StopJump()
    {
        movementVelocity.y = 0;
        isJumping = false;
    }

    /// <summary>
    /// Funzione che tratta l'evento OnStickyCollision
    /// </summary>
    private void OnStickyCollision()
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
        collisionCtrl.OnStickyCollision += OnStickyCollision;

        //Calcolo la gravità
        gravity = -(2 * JumpUnitHeight) / Mathf.Pow(JumpTimeToReachTop, 2);
        //Calcolo la velocità del salto
        jumpVelocity = Mathf.Abs(gravity * JumpTimeToReachTop);

        canMove = false;
        isJumping = false;
    }

    /// <summary>
    /// Funzione che imposta la variabile can move
    /// con la variabile passata come parametro
    /// </summary>
    /// <param name="_canMove"></param>
    public void SetCanMove(bool _canMove)
    {
        canMove = _canMove;
    }

    /// <summary>
    /// Funzione che fa eseguire l'eject al player
    /// </summary>
    bool eject;
    public void Eject()
    {
        if (collisionCtrl.GetCollisionInfo().aboveStickyCollision)
            return;

        if (!eject)
            eject = true;
        else
        {
            movementVelocity.y += jumpVelocity * 1.3f;
            eject = false;
        }
    }
    #endregion
}
