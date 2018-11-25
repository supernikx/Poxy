using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
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

    private float gravity;

    /// <summary>
    /// Referenza al player controller
    /// </summary>
    private PlayerController controller;
    /// <summary>
    /// Vettore che contiene gli input sull'asse orizzontale e verticale
    /// </summary>
    private Vector2 input;


    private void Start()
    {
        //Prendo le referenze ai component e li inizializzo
        controller = GetComponent<PlayerController>();
        controller.Init();

        //Calcolo la gravità
        gravity = -(2 * JumpUnitHeight) / Mathf.Pow(JumpTimeToReachTop, 2);
        //Calcolo la velocità del salto
        jumpVelocity = Mathf.Abs(gravity * JumpTimeToReachTop);
    }

    private void Update()
    {
        if (controller.collisions.above || controller.collisions.below)
        {
            //Se sono in collisione con qualcosa sopra/sotto evito di accumulare gravità
            movementVelocity.y = 0;
        }

        //Leggo input orrizontali e verticali
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        //Controllo se è stato premuto il tasto di salto e se sono a terra
        if (Input.GetButtonDown("Jump") && controller.collisions.below)
        {
            Jump();            
        }

        PrepareForMove();
    }
    
    private float velocityXSmoothing;
    private Vector3 movementVelocity;
    /// <summary>
    /// Funzione che esegue i calcoli necessari prima di muovere il player
    /// </summary>
    private void PrepareForMove()
    {
        //Eseguo una breve transizione dalla mia velocity attuale a quella successiva
        movementVelocity.x = Mathf.SmoothDamp(movementVelocity.x, (input.x * MovementSpeed), ref velocityXSmoothing, (controller.collisions.below ? AccelerationTimeOnGround : AccelerationTimeOnAir));
        
        //Aggiungo gravità al player
        movementVelocity.y += gravity * Time.deltaTime;
        
        //Mi muovo
        controller.Move(movementVelocity * Time.deltaTime);
    }

    private float jumpVelocity;
    /// <summary>
    /// Funzione che imposta la velocity dell'asse verticale per saltare
    /// </summary>
    private void Jump()
    {
        movementVelocity.y = jumpVelocity;
    }
}
