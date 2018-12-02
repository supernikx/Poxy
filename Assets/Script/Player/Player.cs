using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerCollisionController))]
public class Player : MonoBehaviour
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

    [Header("Other Settings")]
    [SerializeField]
    GameObject graphics;

    private float gravity;

    /// <summary>
    /// Referenza al player controller
    /// </summary>
    private PlayerCollisionController collisionCtrl;
    /// <summary>
    /// Referenza al shoot controller
    /// </summary>
    private ShootController shootCtrl;
    /// <summary>
    /// Vettore che contiene gli input sull'asse orizzontale e verticale
    /// </summary>
    private Vector2 input;

    /// <summary>
    /// Funzione di inizializzazione del player
    /// </summary>
    public void Init()
    {
        //Prendo le referenze ai component e li inizializzo
        collisionCtrl = GetComponent<PlayerCollisionController>();
        collisionCtrl.Init();

        shootCtrl = GetComponent<ShootController>();
        shootCtrl.Init();

        //Calcolo la gravità
        gravity = -(2 * JumpUnitHeight) / Mathf.Pow(JumpTimeToReachTop, 2);
        //Calcolo la velocità del salto
        jumpVelocity = Mathf.Abs(gravity * JumpTimeToReachTop);
    }

    private void Update()
    {
        if (collisionCtrl.collisions.above || collisionCtrl.collisions.below)
        {
            //Se sono in collisione con qualcosa sopra/sotto evito di accumulare gravità
            movementVelocity.y = 0;
        }

        //Leggo input orrizontali e verticali
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        //Controllo se è stato premuto il tasto di salto e se sono a terra
        if (Input.GetButtonDown("Jump") && collisionCtrl.collisions.below)
        {
            Jump();
        }

        AddGravity();

        Move();
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
    private int previewDirection = 1;
    /// <summary>
    /// Funzione che esegue i calcoli necessari per far muovere il player
    /// </summary>
    private void Move()
    {
        //Eseguo una breve transizione dalla mia velocity attuale a quella successiva
        movementVelocity.x = Mathf.SmoothDamp(movementVelocity.x, (input.x * MovementSpeed), ref velocityXSmoothing, (collisionCtrl.collisions.below ? AccelerationTimeOnGround : AccelerationTimeOnAir));

        //Giro la grafica
        Flip();

        //Mi muovo
        transform.Translate(collisionCtrl.CheckMovementCollisions(movementVelocity * Time.deltaTime));
    }

    private float jumpVelocity;
    /// <summary>
    /// Funzione che imposta la velocity dell'asse verticale per saltare
    /// </summary>
    private void Jump()
    {
        movementVelocity.y = jumpVelocity;
    }

    private void Flip()
    {
        if (Mathf.Sign(movementVelocity.x) != previewDirection)
        {
            graphics.transform.Rotate(new Vector3(0f, 180f, 0f));
        }

        previewDirection = (int)Mathf.Sign(movementVelocity.x);
    }
}
