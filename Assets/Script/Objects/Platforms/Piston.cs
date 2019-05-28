using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piston : PlatformBase, IActivable
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
    private PistonDirection direction;
    [SerializeField]
    private PistonOrientation horizontalOrientation;
    [SerializeField]
    private LayerMask playerLayer;
    [SerializeField]
    private Transform waypoint;
    [SerializeField]
    private Transform bottomLeft;
    [SerializeField]
    private Transform bottomRight;
    [SerializeField]
    private bool canMoveAtStart = true;

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
        Setup();
    }

    private Vector3 movementVelocity = Vector3.zero;
    public override void MoveBehaviour()
    {
        if (canMoveAtStart)
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

            if (currentState == PistonState.Forward)
            {
                if (direction == PistonDirection.Vertical && Vector3.up == transform.up && transform.position.y <= targetPosition)
                {
                    stompVFX.Play();
                    currentState = PistonState.Backward;
                }
                else if (direction == PistonDirection.Vertical && Vector3.up != transform.up && transform.position.y >= targetPosition)
                {
                    stompVFX.Play();
                    currentState = PistonState.Backward;
                }
                else if (direction == PistonDirection.Horizontal && Vector3.left == transform.up && transform.position.x >= targetPosition)
                {
                    stompVFX.Play();
                    currentState = PistonState.Backward;
                }
                else if (direction == PistonDirection.Horizontal && Vector3.left != transform.up && transform.position.x <= targetPosition)
                {
                    stompVFX.Play();
                    currentState = PistonState.Backward;
                }
                else if (direction == PistonDirection.Platform)
                {
                    if ((Vector3.forward == transform.up && transform.position.z <= targetPosition) || (Vector3.forward != transform.up && transform.position.z >= targetPosition))
                    {
                        currentState = PistonState.Backward;
                    }
                }
            }

            if (currentState == PistonState.Backward)
            {
                if (direction == PistonDirection.Vertical && Vector3.up == transform.up && transform.position.y >= initialPosition)
                {
                    currentState = PistonState.Forward;
                }
                else if (direction == PistonDirection.Vertical && Vector3.up != transform.up && transform.position.y <= initialPosition)
                {
                    currentState = PistonState.Forward;
                }
                else if (direction == PistonDirection.Horizontal && Vector3.left == transform.up && transform.position.x <= initialPosition)
                {
                    currentState = PistonState.Forward;
                }
                else if (direction == PistonDirection.Horizontal && Vector3.left != transform.up && transform.position.x >= initialPosition)
                {
                    currentState = PistonState.Forward;
                }
                else if (direction == PistonDirection.Platform)
                {
                    if ((Vector3.forward == transform.up && transform.position.z >= initialPosition) ||
                        (Vector3.forward != transform.up && transform.position.z <= initialPosition))
                    {
                        currentState = PistonState.Forward;
                    }
                }
            } 
        }
    }
    #endregion

    #region Activable
    public void Setup()
    {
        currentState = PistonState.Forward;
        colliderToCheck = GetComponent<Collider>();
        CalculateRaySpacing();

        if (direction == PistonDirection.Vertical)
        {
            initialPosition = transform.position.y;
            targetPosition = waypoint.position.y;
        }
        else if (direction == PistonDirection.Horizontal)
        {
            initialPosition = transform.position.x;
            targetPosition = waypoint.position.x;
        }
        else if (direction == PistonDirection.Platform)
        {
            initialPosition = transform.position.z;
            targetPosition = waypoint.position.z;
        }
    }

    public void Activate()
    {
        canMoveAtStart = true;
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

        float _distance = Vector3.Distance(bottomRight.position, bottomLeft.position);
        if (_distance < 0)
            _distance = _distance * -1;
        verticalRaySpacing = _distance / (verticalRayCount - 1);

    }

    /// <summary>
    /// Funzione che calcola i 2 punti principali da cui partono i raycast
    /// AltoDestra, AltoSinistra, BassoDestra, BassoSinistra
    /// </summary>
    private void UpdateRaycastOrigins()
    {
        Bounds bounds = colliderToCheck.bounds;
        bounds.Expand(collisionOffset * -2);

        raycastStartPoints.bottomLeft = bottomLeft.position;
        raycastStartPoints.bottomRight = bottomRight.position;

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
            rayOrigin += transform.right * (verticalRaySpacing * i);

            //Crea il ray
            Ray ray = new Ray(rayOrigin, transform.up * directionY);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayLenght, playerLayer))
            {
                //Se colpisco qualcosa sul layer di collisione azzero la velocity
                rayLenght = hit.distance;

                Player _player;
                if (hit.transform.gameObject.tag == "Player")
                    _player = hit.transform.gameObject.GetComponent<Player>();
                else
                    _player = hit.transform.gameObject.GetComponentInParent<Player>();

                CollisionInfo collisions = _player.GetCollisionController().GetCollisionInfo();
                if (_player != null &&
                    ((direction == PistonDirection.Horizontal &&
                    ((horizontalOrientation == PistonOrientation.LeftToRight && (collisions.right || collisions.rightStickyCollision)) ||
                    (horizontalOrientation == PistonOrientation.RightToLeft && (collisions.left || collisions.leftStickyCollision)))) ||
                    (direction == PistonDirection.Vertical && _player.GetCollisionController().GetCollisionInfo().below)))
                {
                    _player.StartDeathCoroutine();
                }
            }
            Debug.DrawRay(rayOrigin, transform.up * directionY * rayLenght, Color.red);
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

    public enum PistonDirection
    {
        Vertical,
        Horizontal,
        Platform
    }

    public enum PistonOrientation
    {
        LeftToRight,
        RightToLeft
    }
}