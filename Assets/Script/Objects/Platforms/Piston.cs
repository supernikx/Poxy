using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piston : PlatformBase
{
    [Header("Graphic Settings")]
    [SerializeField]
    private ParticleSystem stompVFX;

    [Header("Piston Settings")]
    [SerializeField]
    private float downMovementSpd;
    [SerializeField]
    private float upMovementSpd;
    [SerializeField]
    private LayerMask playerLayer;
    [SerializeField]
    private Transform waypoint;

    [Header("Collisions")]
    [SerializeField]
    /// <summary>
    /// Numero di raycast orrizontali
    /// </summary>
    private int verticalRayCount = 4;
    /// <summary>
    /// Spazio tra un raycast verticale e l'altro
    /// </summary>
    private float verticalRaySpacing;
    /// <summary>
    /// Offset del bound del collider
    /// </summary>
    private float collisionOffset = 0.015f;
    /// <summary>
    /// Referenza agli start point
    /// </summary>
    private RaycastStartPoints raycastStartPoints;

    private PistonState currentState;
    private Collider colliderToCheck;

    private float initialPosition;
    private float targetPosition;


    private void CheckMovementCollisions(Vector3 _movementVelocity)
    {
        //Aggiorna le posizioni da cui partiranno i raycast
        UpdateRaycastOrigins();

        if (currentState == PistonState.Forward)
            VerticalCollisions();
    }

    #region API
    public override void Init()
    {
        currentState = PistonState.Forward;
        colliderToCheck = GetComponent<Collider>();
        CalculateRaySpacing();

        initialPosition = transform.position.y;
        targetPosition = waypoint.position.y;
    }

    private Vector3 movementVelocity = Vector3.zero;
    public override void MoveBehaviour()
    {
        if (currentState == PistonState.Forward)
        {
            movementVelocity = new Vector3(0, -downMovementSpd, 0);
        }
        else if (currentState == PistonState.Backward)
        {
            movementVelocity = new Vector3(0, upMovementSpd, 0);
        }

        CheckMovementCollisions(movementVelocity * Time.deltaTime);

        transform.Translate(movementVelocity * Time.deltaTime);

        if (currentState == PistonState.Forward && transform.position.y <= targetPosition)
        {
            stompVFX.Play();
            currentState = PistonState.Backward;
        }

        if (currentState == PistonState.Backward && transform.position.y >= initialPosition)
            currentState = PistonState.Forward;
    }
    #endregion

    #region Collision
    /// <summary>
    /// Funzione che calcola lo spazio tra i raycast sia verticali che orrizontali
    /// </summary>
    private void CalculateRaySpacing()
    {
        Bounds bounds = colliderToCheck.bounds;
        bounds.Expand(collisionOffset * -2);

        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    /// <summary>
    /// Funzione che calcola i 2 punti principali da cui partono i raycast
    /// AltoDestra, AltoSinistra, BassoDestra, BassoSinistra
    /// </summary>
    private void UpdateRaycastOrigins()
    {
        Bounds bounds = colliderToCheck.bounds;
        bounds.Expand(collisionOffset * -2);

        raycastStartPoints.bottomLeft = new Vector3(bounds.min.x, bounds.min.y, transform.position.z);
        raycastStartPoints.bottomRight = new Vector3(bounds.max.x, bounds.min.y, transform.position.z);
    }

    /// <summary>
    /// Funzione che controlla se avviene una collisione sugli assi verticali
    /// </summary>
    private void VerticalCollisions()
    {
        //Rileva la direzione in cui si sta andando
        float directionY = -1;
        //Determina la lunghezza del raycast
        float rayLenght = 0.3f;

        //Cicla tutti i punti da cui deve partire un raycast sull'asse verticale
        for (int i = 0; i < verticalRayCount; i++)
        {
            //Determina il punto da cui deve partire il ray
            Vector3 rayOrigin = raycastStartPoints.bottomLeft;
            rayOrigin += Vector3.right * (verticalRaySpacing * i);

            //Crea il ray
            Ray ray = new Ray(rayOrigin, Vector3.up * directionY);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayLenght, playerLayer))
            {
                //Se colpisco qualcosa sul layer di collisione azzero la velocity
                rayLenght = hit.distance;

                if (hit.transform.gameObject.tag == "Player")
                {
                    Player _player = hit.transform.gameObject.GetComponent<Player>();
                    if (_player.GetCollisionController().GetCollisionInfo().below)
                    {
                        _player.StartDeathCoroutine();
                    }
                }
            }
            Debug.DrawRay(rayOrigin, Vector3.up * directionY * rayLenght, Color.red);
        }
    }
    #endregion

    private struct RaycastStartPoints
    {
        public Vector3 bottomLeft;
        public Vector3 bottomRight;
    }

    public enum PistonState
    {
        Forward,
        Backward,
    }
}
