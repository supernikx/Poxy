using UnityEngine;
using System.Collections;

public class EnemyCollisionController : MonoBehaviour
{

    [Header("Collision Settings")]
    public LayerMask CollisionMask;

    /// <summary>
    /// Numero di raycast orrizontali
    /// </summary>
    public int HorizontalRayCount = 4;
    /// <summary>
    /// Spazio tra un raycast orrizontale e l'altro
    /// </summary>
    private float horizontalRaySpacing;

    /// <summary>
    /// Numero di raycast orrizontali
    /// </summary>
    public int VerticalRayCount = 4;
    /// <summary>
    /// Spazio tra un raycast verticale e l'altro
    /// </summary>
    private float verticalRaySpacing;

    /// <summary>
    /// Offset del bound del collider
    /// </summary>
    private float collisionOffset = 0.015f;

    private Collider colliderToCheck;
    private Collider enemyCollider;
    private RaycastStartPoints raycastStartPoints;
    public CollisionInfo collisions;

    #region API
    /// <summary>
    /// Funzione che inizializza lo script
    /// </summary>
    public void Init()
    {
        //Prendo le referenze ai component
        enemyCollider = GetComponent<Collider>();
        colliderToCheck = enemyCollider;

        //Calcolo lo spazio tra i raycast
        CalculateRaySpacing();
    }

    /// <summary>
    /// Funzione che controlla se il nemico va in collisione con qualcosa durante il movimento
    /// </summary>
    /// <param name="_movementVelocity"></param>
    public Vector3 CheckMovementCollisions(Vector3 _movementVelocity)
    {
        //Aggiorna le posizioni da cui partiranno i raycast
        UpdateRaycastOrigins();
        //Reset delle collisioni attuali
        collisions.Reset();
        
        if (_movementVelocity.x != 0)
        {
            //Se mi sto muovendo sull'asse X controllo se entro in collisione con qualcosa
            HorizontalCollisions(ref _movementVelocity);
        }
        
        if (_movementVelocity.y != 0)
        {
            //Se mi sto muovendo sull'asse Y controllo se entro in collisione con qualcosa
            VerticalCollisions(ref _movementVelocity);
        }

        return _movementVelocity;
    }

    /// <summary>
    /// Funzione che ricalcola le collisioni sul collider normale del nemico
    /// </summary>
    public void CalculateNormalCollision()
    {
        enemyCollider.enabled = true;
        colliderToCheck = enemyCollider;
        CalculateRaySpacing();
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

        HorizontalRayCount = Mathf.Clamp(HorizontalRayCount, 2, int.MaxValue);
        VerticalRayCount = Mathf.Clamp(VerticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (HorizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (VerticalRayCount - 1);
    }

    /// <summary>
    /// Funzione che calcola i 4 punti principali da cui partono i raycast
    /// AltoDestra, AltoSinistra, BassoDestra, BassoSinistra
    /// </summary>
    private void UpdateRaycastOrigins()
    {
        Bounds bounds = colliderToCheck.bounds;
        bounds.Expand(collisionOffset * -2);

        raycastStartPoints.bottomLeft = new Vector3(bounds.min.x, bounds.min.y, transform.position.z);
        raycastStartPoints.bottomRight = new Vector3(bounds.max.x, bounds.min.y, transform.position.z);
        raycastStartPoints.topLeft = new Vector3(bounds.min.x, bounds.max.y, transform.position.z);
        raycastStartPoints.topRight = new Vector3(bounds.max.x, bounds.max.y, transform.position.z);
    }

    /// <summary>
    /// Funzione che controlla se avviene una collisione sugli assi verticali
    /// </summary>
    /// <param name="_movementVelocity"></param>
    private void VerticalCollisions(ref Vector3 _movementVelocity)
    {
        //Rileva la direzione in cui si sta andando
        float directionY = Mathf.Sign(_movementVelocity.y);
        //Determina la lunghezza del raycast
        float rayLenght = Mathf.Abs(_movementVelocity.y) + collisionOffset;

        //Cicla tutti i punti da cui deve partire un raycast sull'asse verticale
        for (int i = 0; i < VerticalRayCount; i++)
        {
            //Determina il punto da cui deve partire il ray
            Vector3 rayOrigin = (directionY == -1) ? raycastStartPoints.bottomLeft : raycastStartPoints.topLeft;
            rayOrigin += Vector3.right * (verticalRaySpacing * i + _movementVelocity.x);

            //Crea il ray
            Ray ray = new Ray(rayOrigin, Vector3.up * directionY);
            RaycastHit hit;

            //Eseguo il raycast
            if (Physics.Raycast(ray, out hit, rayLenght, CollisionMask))
            {
                //Se colpisco qualcosa sul layer di collisione azzero la velocity
                _movementVelocity.y = (hit.distance - collisionOffset) * directionY;
                rayLenght = hit.distance;

                //Assegno la direzione della collisione al CollisionInfo
                collisions.above = directionY == 1;
                collisions.below = directionY == -1;
            }

            Debug.DrawRay(rayOrigin, Vector3.up * directionY * rayLenght, Color.yellow);
        }
    }

    /// <summary>
    /// Funzione che controlla se avviene una collisione sugli assi orizzontali
    /// </summary>
    /// <param name="_movementVelocity"></param>
    private void HorizontalCollisions(ref Vector3 _movementVelocity)
    {
        //Rileva la direzione in cui si sta andando
        float directionX = Mathf.Sign(_movementVelocity.x);
        //Determina la lunghezza del raycast
        float rayLenght = Mathf.Abs(_movementVelocity.x) + collisionOffset;

        //Cicla tutti i punti da cui deve partire un raycast sull'asse orizzontale
        for (int i = 0; i < HorizontalRayCount; i++)
        {
            //Determina il punto da cui deve partire il ray
            Vector3 rayOrigin = (directionX == -1) ? raycastStartPoints.bottomLeft : raycastStartPoints.bottomRight;
            rayOrigin += Vector3.up * (horizontalRaySpacing * i);

            //Crea il ray
            Ray ray = new Ray(rayOrigin, Vector3.right * directionX);
            RaycastHit hit;

            //Eseguo il raycast
            if (Physics.Raycast(ray, out hit, rayLenght, CollisionMask))
            {
                //Se colpisco qualcosa sul layer di collisione azzero la velocity
                _movementVelocity.x = (hit.distance - collisionOffset) * directionX;
                rayLenght = hit.distance;

                //Assegno la direzione della collisione al CollisionInfo
                collisions.left = directionX == -1;
                collisions.right = directionX == 1;
            }

            Debug.DrawRay(rayOrigin, Vector3.right * directionX * rayLenght, Color.red);
        }
    }

    /// <summary>
    /// Struttura che contiene le coordinate dei 4 punti principali da cui partono i ray
    /// che controllano le collisioni
    /// </summary>
    private struct RaycastStartPoints
    {
        public Vector3 topLeft;
        public Vector3 topRight;
        public Vector3 bottomLeft;
        public Vector3 bottomRight;
    }

    /// <summary>
    /// Struttura che contiene le informazioni sulla collisione attuale
    /// </summary>
    public struct CollisionInfo
    {
        public bool above;
        public bool below;
        public bool left;
        public bool right;

        public void Reset()
        {
            above = false;
            below = false;
            left = false;
            right = false;
        }
    }
    #endregion

}
