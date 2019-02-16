using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piston : Platform
{

    [Header("Piston Settings")]
    [SerializeField]
    private float movementSpd;
    [SerializeField]
    private LayerMask obstacleLayer;
    [SerializeField]
    private LayerMask playerLayer;
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
    private bool collisionBelow;
    private bool playerHit;
    private Vector3 initialPosition;

    private Vector3 CheckMovementCollisions(Vector3 _movementVelocity)
    {
        //Aggiorna le posizioni da cui partiranno i raycast
        UpdateRaycastOrigins();
        //Reset delle collisioni attuali
        collisionBelow = false;
        playerHit = false;
        
        if (currentState == PistonState.Forward)
            VerticalCollisions(ref _movementVelocity);

        return _movementVelocity;
    }

    #region API
    public override void Init()
    {
        currentState = PistonState.Forward;
        colliderToCheck = GetComponent<Collider>();
        initialPosition = transform.position;

        CalculateRaySpacing();

        StartCoroutine(VerticalBehaviour());
    }
    #endregion

    #region Coroutines
    private IEnumerator VerticalBehaviour()
    {
        Vector3 movementVelocity = Vector3.zero;

        while (true)
        {
            if (currentState == PistonState.Forward)
            {
                movementVelocity = new Vector3(0, -movementSpd, 0);
            }
            else if (currentState == PistonState.Backward)
            {
                movementVelocity = new Vector3(0, movementSpd, 0);
            }

            movementVelocity = CheckMovementCollisions(movementVelocity);

            transform.Translate(movementVelocity);

            if (currentState == PistonState.Forward && collisionBelow && !playerHit)
                currentState = PistonState.Backward;

            if (currentState == PistonState.Backward && transform.position.y >= initialPosition.y)
                currentState = PistonState.Forward;

            yield return null;
        }
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
    /// <param name="_movementVelocity"></param>
    private void VerticalCollisions(ref Vector3 _movementVelocity)
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
            
            //Eseguo il raycast
            if (Physics.Raycast(ray, out hit, rayLenght, obstacleLayer))
            {
                //Se colpisco qualcosa sul layer di collisione azzero la velocity
                _movementVelocity.y = (hit.distance - collisionOffset) * directionY;
                rayLenght = hit.distance;

                collisionBelow = true;
            }

            if (Physics.Raycast(ray, out hit, rayLenght, playerLayer))
            {
                //Se colpisco qualcosa sul layer di collisione azzero la velocity
                _movementVelocity.y = (hit.distance - collisionOffset) * directionY;
                rayLenght = hit.distance;

                if (hit.transform.gameObject.tag == "Player")
                {
                    Player _player = hit.transform.gameObject.GetComponent<Player>();
                    if (_player.GetCollisionController().GetCollisionInfo().below)
                    {
                        _player.StartDeathCoroutine();
                        playerHit = true;
                        collisionBelow = true;
                    }
                }
            }
            Debug.DrawRay(rayOrigin, Vector3.up * directionY * rayLenght, Color.red);
        }
    }
    #endregion

    /*
    private void OnDrawGizmos()
    {
        collisionOffset = 0.015f;

        Bounds bounds = GetComponent<Collider>().bounds;
        bounds.Expand(collisionOffset * -2);

        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
        raycastStartPoints.bottomLeft = new Vector3(bounds.min.x, bounds.min.y, transform.position.z);

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector3 rayOrigin = raycastStartPoints.bottomLeft;
            rayOrigin += Vector3.right * (verticalRaySpacing * i);
            Debug.DrawRay(rayOrigin, Vector3.up * -1 * -collisionOffset, Color.yellow);
        }
    }*/

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
