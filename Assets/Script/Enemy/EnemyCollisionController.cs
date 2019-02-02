using UnityEngine;
using System.Collections;

public class EnemyCollisionController : MonoBehaviour, ISticky
{

    [Header("Collision Settings")]
    [SerializeField]
    private LayerMask collisionLayer;

    [SerializeField]
    /// <summary>
    /// Numero di raycast orrizontali
    /// </summary>
    private int HorizontalRayCount = 4;
    /// <summary>
    /// Spazio tra un raycast orrizontale e l'altro
    /// </summary>
    private float horizontalRaySpacing;
    [SerializeField]
    /// <summary>
    /// Numero di raycast orrizontali
    /// </summary>
    private int VerticalRayCount = 4;
    /// <summary>
    /// Spazio tra un raycast verticale e l'altro
    /// </summary>
    private float verticalRaySpacing;
    [SerializeField]
    /// <summary>
    /// Angolo massimo che si può scalare
    /// </summar
    private float maxClimbAngle;
    [SerializeField]
    /// <summary>
    /// Angolo massimo che si può scendere
    /// </summar
    private float maxDecendingAngle;
    /// <summary>
    /// Offset del bound del collider
    /// </summary>
    private float collisionOffset = 0.015f;

    private Collider enemyCollider;
    private RaycastStartPoints raycastStartPoints;
    private CollisionInfo collisions;

    #region API
    /// <summary>
    /// Funzione che inizializza lo script
    /// </summary>
    public void Init()
    {
        //Prendo le referenze ai component
        enemyCollider = GetComponent<Collider>();

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

        //Se sto scendendo
        if (_movementVelocity.y < 0)
        {
            //Controllo se posso scendere
            DescendSlope(ref _movementVelocity);
        }

        if (collisions.HorizontalStickyCollision())
        {
            _movementVelocity.x = 0f;
            _movementVelocity.y = 0f;
        }
        else if (_movementVelocity.x != 0)
        {
            //Se mi sto muovendo sull'asse X controllo se entro in collisione con qualcosa
            HorizontalCollisions(ref _movementVelocity);
        }

        if (collisions.VerticalStickyCollision())
        {
            _movementVelocity.y = 0f;
            _movementVelocity.x = 0f;
        }
        else if (_movementVelocity.y != 0)
        {
            //Se mi sto muovendo sull'asse Y controllo se entro in collisione con qualcosa
            VerticalCollisions(ref _movementVelocity);
        }

        return _movementVelocity;
    }
    #endregion

    #region Collision
    /// <summary>
    /// Funzione che calcola lo spazio tra i raycast sia verticali che orrizontali
    /// </summary>
    private void CalculateRaySpacing()
    {
        Bounds bounds = enemyCollider.bounds;
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
        Bounds bounds = enemyCollider.bounds;
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
            if (Physics.Raycast(ray, out hit, rayLenght, collisionLayer))
            {
                //Se colpisco qualcosa sul layer di collisione azzero la velocity
                _movementVelocity.y = (hit.distance - collisionOffset) * directionY;
                rayLenght = hit.distance;

                //Se sto scalando qualcosa
                if (collisions.climbingSlope)
                {
                    _movementVelocity.x = _movementVelocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(_movementVelocity.x);
                }

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
        float directionX = (transform.right == Vector3.right) ? 1 : -1;
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
            if (Physics.Raycast(ray, out hit, rayLenght, collisionLayer))
            {
                //Calcolo e controllo l'angolo e se lo posso scalare
                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                if (i == 0 && slopeAngle <= maxClimbAngle)
                {
                    //Controllo se è un nuovo angolo
                    float distanceSlopeStart = 0f;
                    if (slopeAngle != collisions.oldSlopeAngle)
                    {
                        //Faccio avvicinare il player il più possibile alla salita
                        distanceSlopeStart = hit.distance - collisionOffset;
                        _movementVelocity.x -= distanceSlopeStart * directionX;
                    }

                    //Se è minore dell'angolo massimo che posso scalare lo scalo
                    ClimbSlope(ref _movementVelocity, slopeAngle);
                    _movementVelocity.x += distanceSlopeStart * directionX;
                }

                //Se non sto scalando nulla o se l'angolo sa scalare è troppo grande
                if (!collisions.climbingSlope || collisions.slopeAngle > maxClimbAngle)
                {
                    //Se colpisco qualcosa sul layer di collisione azzero la velocity
                    _movementVelocity.x = (hit.distance - collisionOffset) * directionX;
                    rayLenght = hit.distance;

                    //Se sto scalando qualcosa
                    if (collisions.climbingSlope)
                    {
                        _movementVelocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad * Mathf.Abs(_movementVelocity.x));
                    }

                    //Assegno la direzione della collisione al CollisionInfo
                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }

            Debug.DrawRay(rayOrigin, Vector3.right * directionX * rayLenght, Color.red);
        }

        #region Fix Freez frame tra un cambio di pendenza e l'altro
        if (collisions.climbingSlope)
        {
            directionX = Mathf.Sign(_movementVelocity.x);
            rayLenght = Mathf.Abs(_movementVelocity.x) + collisionOffset;
            Vector3 rayOrigin = ((directionX == -1) ? raycastStartPoints.bottomLeft : raycastStartPoints.bottomRight) + Vector3.up * _movementVelocity.y;
            Ray ray = new Ray(rayOrigin, Vector3.right * directionX);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, rayLenght, collisionLayer))
            {
                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                if (slopeAngle != collisions.slopeAngle)
                {
                    _movementVelocity.x = (hit.distance - collisionOffset) * directionX;
                    collisions.slopeAngle = slopeAngle;
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// Funzione che imposta la velocity per scalare l'angolo passato come parametro
    /// </summary>
    /// <param name="_movementVelocity"></param>
    /// <param name="slopeAngle"></param>
    private void ClimbSlope(ref Vector3 _movementVelocity, float slopeAngle)
    {
        //Calcolo la distanza e la velocity sulla Y
        float moveDistance = Mathf.Abs(_movementVelocity.x);
        float newVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        //Se la mia velocity è minore (non sto saltando)
        if (_movementVelocity.y <= newVelocityY)
        {
            //Imposto la velocity Y con quella calcolata e calcolo la veloctity X
            _movementVelocity.y = newVelocityY;
            _movementVelocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(_movementVelocity.x);

            //Imposto le informazioni sulla collisione
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }
    }

    /// <summary>
    /// Funzione che imposta la velocity per scendere una discesa se non è troppo ripida
    /// </summary>
    /// <param name="_movementVelocity"></param>
    private void DescendSlope(ref Vector3 _movementVelocity)
    {
        //Calcolo la direzione
        float directionX = Mathf.Sign(_movementVelocity.x);
        //Calcolo la lunghezza del ray
        float rayLenght = Mathf.Abs(_movementVelocity.y) + collisionOffset;
        //Calcolo il punto di inizio del ray
        Vector3 rayOrigin = (directionX == -1) ? raycastStartPoints.bottomRight : raycastStartPoints.bottomLeft;
        //Creo il ray (che punta verso il basso)
        Ray ray = new Ray(rayOrigin, -Vector3.up);
        RaycastHit hit;
        //Faccio paritre il ray
        if (Physics.Raycast(ray, out hit, rayLenght, collisionLayer))
        {
            //Calcolo l'angolazione se colpisco qualcosa
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            //Se l'angolo è diverso da 0 e minore dell'angolo minimo da scendere
            if (slopeAngle != 0 && slopeAngle <= maxDecendingAngle)
            {
                //Se la direzione dell'oggetto colpito è uguale a quella del player
                if (Mathf.Sign(hit.normal.x) == directionX)
                {
                    //Se la distanza tra il player e il punto colpito è minore della tangente tra l'angolo e la mia velocità sulla X
                    if (hit.distance - collisionOffset <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(_movementVelocity.x))
                    {
                        //Calcolo la distanza, la velocità sulla Y, la veloictà sulla X e le applico
                        float moveDistance = Mathf.Abs(_movementVelocity.x);
                        float newVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        _movementVelocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(_movementVelocity.x);

                        //Sottraggo alla movementVelocity la velocity appena calcolata
                        _movementVelocity.y -= newVelocityY;

                        //Imposto le informazioni sulla collisione
                        collisions.slopeAngle = slopeAngle;
                        collisions.descendingSlope = true;
                        collisions.below = true;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Funzione chiamata quando si entra in collisione con un oggetto sticky
    /// </summary>
    /// <param name="_direction"></param>
    public void OnSticky(Direction _direction)
    {
        collisions.ResetStickyCollision();
        switch (_direction)
        {
            case Direction.Left:
                collisions.leftStickyCollision = true;
                break;
            case Direction.Right:
                collisions.rightStickyCollision = true;
                break;
            case Direction.Above:
                collisions.aboveStickyCollision = true;
                break;
            case Direction.Below:
                collisions.belowStickyCollision = true;
                break;
            case Direction.None:
                Debug.LogWarning("Direzione di collisione non prevista");
                break;
        }
    }

    /// <summary>
    /// Funzione chiamata quando non si è più in collisione con un oggetto sticky
    /// </summary>
    public void OnStickyEnd()
    {
        collisions.ResetStickyCollision();
    }

    #region Getter
    /// <summary>
    /// Funzione che ritorna le informazioni sulle collisioni
    /// </summary>
    /// <returns></returns>
    public CollisionInfo GetCollisionInfo()
    {
        return collisions;
    }
    #endregion

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

        public bool climbingSlope;
        public bool descendingSlope;
        public float oldSlopeAngle;
        public float slopeAngle;

        public bool leftStickyCollision;
        public bool rightStickyCollision;
        public bool aboveStickyCollision;
        public bool belowStickyCollision;

        public void Reset()
        {
            above = false;
            below = false;
            left = false;
            right = false;
            climbingSlope = false;
            descendingSlope = false;
            oldSlopeAngle = slopeAngle;
            slopeAngle = 0f;
        }

        public bool StickyCollision()
        {
            if (leftStickyCollision || rightStickyCollision || aboveStickyCollision || belowStickyCollision)
                return true;
            return false;
        }

        public bool HorizontalStickyCollision()
        {
            if (leftStickyCollision || rightStickyCollision)
                return true;
            return false;
        }

        public bool VerticalStickyCollision()
        {
            if (aboveStickyCollision || belowStickyCollision)
                return true;
            return false;
        }

        public void ResetStickyCollision()
        {
            aboveStickyCollision = false;
            belowStickyCollision = false;
            leftStickyCollision = false;
            rightStickyCollision = false;
        }
    }
    #endregion

}
