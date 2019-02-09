using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformCollisionController : MonoBehaviour
{
    [Header("Collision Settings")]
    [SerializeField]
    LayerMask passengerLayer;
    [SerializeField]
    /// <summary>
    /// Numero di raycast orrizontali
    /// </summary>
    private int horizontalRayCount = 4;
    /// <summary>
    /// Spazio tra un raycast orrizontale e l'altro
    /// </summary>
    private float horizontalRaySpacing;
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
    /// Referenza al collider della piattaforma
    /// </summary>
    private Collider platformCollider;
    /// <summary>
    /// Offset del bound del collider
    /// </summary>
    private float collisionOffset = 0.015f;
    /// <summary>
    /// Referenza agli start point
    /// </summary>
    private RaycastStartPoints raycastStartPoints;

    /// <summary>
    /// Funzione che calcola lo spazio tra i raycast sia verticali che orrizontali
    /// </summary>
    private void CalculateRaySpacing()
    {
        Bounds bounds = platformCollider.bounds;
        bounds.Expand(collisionOffset * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    /// <summary>
    /// Funzione che calcola i 4 punti principali da cui partono i raycast
    /// AltoDestra, AltoSinistra, BassoDestra, BassoSinistra
    /// </summary>
    private void UpdateRaycastOrigins()
    {
        Bounds bounds = platformCollider.bounds;
        bounds.Expand(collisionOffset * -2);

        raycastStartPoints.bottomLeft = new Vector3(bounds.min.x, bounds.min.y, transform.position.z);
        raycastStartPoints.bottomRight = new Vector3(bounds.max.x, bounds.min.y, transform.position.z);
        raycastStartPoints.topLeft = new Vector3(bounds.min.x, bounds.max.y, transform.position.z);
        raycastStartPoints.topRight = new Vector3(bounds.max.x, bounds.max.y, transform.position.z);
    }

    #region API
    public void Init()
    {
        platformCollider = GetComponent<Collider>();
        CalculateRaySpacing();
    }

    public void MovePassenger(Vector3 _velocity)
    {
        HashSet<Transform> movedPassengers = new HashSet<Transform>();

        float directionX = Mathf.Sign(_velocity.x);
        float directionY = Mathf.Sign(_velocity.y);

        //Vertical Moving platform
        if (_velocity.y != 0)
        {
            float rayLenght = Mathf.Abs(_velocity.y) + collisionOffset;

            for (int i = 0; i < verticalRayCount; i++)
            {
                //Determina il punto da cui deve partire il ray
                Vector3 rayOrigin = (directionY == -1) ? raycastStartPoints.bottomLeft : raycastStartPoints.topLeft;
                rayOrigin += Vector3.right * (verticalRaySpacing * i);

                //Crea il ray
                Ray ray = new Ray(rayOrigin, Vector3.up * directionY);
                RaycastHit hit;

                //Eseguo il raycast
                if (Physics.Raycast(ray, out hit, rayLenght, passengerLayer))
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);

                        float pushX = (directionY == 1) ? _velocity.x : 0;
                        float pushY = _velocity.y - (hit.distance - collisionOffset) * directionY;

                        hit.transform.Translate(new Vector3(pushX, pushY, 0));
                    }
                }
            }
        }

        //Horizontal Moving platform
        if (_velocity.x != 0)
        {
            float rayLenght = Mathf.Abs(_velocity.x) + collisionOffset;

            //Cicla tutti i punti da cui deve partire un raycast sull'asse orizzontale
            for (int i = 0; i < horizontalRayCount; i++)
            {
                //Determina il punto da cui deve partire il ray
                Vector3 rayOrigin = (directionX == -1) ? raycastStartPoints.bottomLeft : raycastStartPoints.bottomRight;
                rayOrigin += Vector3.up * (horizontalRaySpacing * i);

                //Crea il ray
                Ray ray = new Ray(rayOrigin, Vector3.right * directionX);
                RaycastHit hit;

                //Eseguo il raycast
                if (Physics.Raycast(ray, out hit, rayLenght, passengerLayer))
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);

                        float pushX = _velocity.x - (hit.distance - collisionOffset) * directionX;
                        float pushY = 0;

                        hit.transform.Translate(new Vector3(pushX, pushY, 0));
                    }
                }
            }
        }

        //Passenger Fix horzizontal top e piattaforma che scende
        if (directionY == -1 || _velocity.y == 0 && _velocity.x != 0)
        {
            float rayLenght = collisionOffset * 2f;

            for (int i = 0; i < verticalRayCount; i++)
            {
                //Determina il punto da cui deve partire il ray
                Vector3 rayOrigin = raycastStartPoints.topLeft + Vector3.right * (verticalRaySpacing * i);

                //Crea il ray
                Ray ray = new Ray(rayOrigin, Vector3.up);
                RaycastHit hit;

                //Eseguo il raycast
                if (Physics.Raycast(ray, out hit, rayLenght, passengerLayer))
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);

                        float pushX = _velocity.x;
                        float pushY = _velocity.y;

                        hit.transform.Translate(new Vector3(pushX, pushY, 0));
                    }
                }
            }
        }
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
}
