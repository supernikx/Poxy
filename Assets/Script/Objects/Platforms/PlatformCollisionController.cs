using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlatformCollisionController : MonoBehaviour
{
    [Header("Collision Settings")]
    [SerializeField]
    LayerMask passengerLayer;
    [SerializeField]
    LayerMask alwaysFollowLayer;
    [SerializeField]
    /// <summary>
    /// Numero di raycast orrizontali
    /// </summary>
    private int horizontalRayCount = 4;
    [SerializeField]
    /// <summary>
    /// Numero di raycast orrizontali
    /// </summary>
    private int verticalRayCount = 4;
    /// <summary>
    /// Referenza al collider della piattaforma
    /// </summary>
    private List<ColliderSettings> collidersSettings;
    /// <summary>
    /// Offset del bound del collider
    /// </summary>
    private float collisionOffset = 0.02f;

    /// <summary>
    /// Funzione che calcola lo spazio tra i raycast sia verticali che orrizontali
    /// </summary>
    private void CalculateRaySpacing(ColliderSettings _settings)
    {
        Bounds bounds = _settings.platformCollider.bounds;
        bounds.Expand(collisionOffset * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        _settings.horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        _settings.verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    /// <summary>
    /// Funzione che calcola i 4 punti principali da cui partono i raycast
    /// AltoDestra, AltoSinistra, BassoDestra, BassoSinistra
    /// </summary>
    private void UpdateRaycastOrigins(ColliderSettings _settings)
    {
        RaycastStartPoints raycastPoint;
        Bounds bounds = _settings.platformCollider.bounds;
        bounds.Expand(collisionOffset * -2);

        raycastPoint.bottomLeft = new GameObject("BottomLeftPoint").transform;
        raycastPoint.bottomRight = new GameObject("BottomRightPoint").transform;
        raycastPoint.topLeft = new GameObject("TopLeftPoint").transform;
        raycastPoint.topRight = new GameObject("TopRightPoint").transform;

        raycastPoint.bottomLeft.SetParent(transform);
        raycastPoint.bottomRight.SetParent(transform);
        raycastPoint.topLeft.SetParent(transform);
        raycastPoint.topRight.SetParent(transform);

        raycastPoint.bottomLeft.position = new Vector3(bounds.min.x, bounds.min.y, transform.position.z);
        raycastPoint.bottomRight.position = new Vector3(bounds.max.x, bounds.min.y, transform.position.z);
        raycastPoint.topLeft.position = new Vector3(bounds.min.x, bounds.max.y, transform.position.z);
        raycastPoint.topRight.position = new Vector3(bounds.max.x, bounds.max.y, transform.position.z);

        _settings.raycastPoints = raycastPoint;
    }

    #region API
    public void Init()
    {
        collidersSettings = new List<ColliderSettings>();
        Collider[] platformColliders = GetComponentsInChildren<Collider>();
        foreach (Collider c in platformColliders)
        {
            ColliderSettings newCollider = new ColliderSettings();
            newCollider.platformCollider = c;
            CalculateRaySpacing(newCollider);
            UpdateRaycastOrigins(newCollider);

            collidersSettings.Add(newCollider);
        }
    }

    public void MovePassenger(Vector3 _velocity)
    {
        HashSet<Transform> movedPassengers = new HashSet<Transform>();
        float directionX = Mathf.Sign(_velocity.x);
        float directionY = Mathf.Sign(_velocity.y);

        for (int k = 0; k < collidersSettings.Count; k++)
        {
            //Vertical Moving platform
            if (_velocity.y != 0)
            {
                float rayLenght = Mathf.Abs(_velocity.y) + collisionOffset;

                for (int i = 0; i < verticalRayCount; i++)
                {
                    //Determina il punto da cui deve partire il ray
                    Vector3 rayOrigin = (directionY == -1) ? collidersSettings[k].raycastPoints.bottomLeft.position : collidersSettings[k].raycastPoints.topLeft.position;
                    rayOrigin += transform.right * (collidersSettings[k].verticalRaySpacing * i);

                    //Crea il ray
                    Ray ray = new Ray(rayOrigin, transform.up * directionY);
                    RaycastHit hit;

                    //Eseguo il raycast
                    if (Physics.Raycast(ray, out hit, rayLenght, passengerLayer))
                    {
                        if (!movedPassengers.Contains(hit.transform))
                        {
                            movedPassengers.Add(hit.transform);

                            float pushX = (directionY == 1) ? _velocity.x : 0;
                            float pushY = _velocity.y * directionY;

                            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player") || hit.transform.gameObject.layer == LayerMask.NameToLayer("PlayerImmunity"))
                            {
                                PlayerCollisionController collisionCtrl = hit.transform.GetComponentInParent<PlayerCollisionController>();
                                collisionCtrl.transform.Translate(new Vector3(pushX, pushY, 0));
                            }
                            else
                            {
                                hit.transform.Translate(new Vector3(pushX, pushY, 0));
                            }
                        }
                    }
                }

                if (_velocity.x == 0)
                {
                    for (int i = 0; i < horizontalRayCount; i++)
                    {
                        //Determina il punto da cui deve partire il ray
                        Vector3 rayOrigin = collidersSettings[k].raycastPoints.bottomLeft.position;
                        rayOrigin += transform.up * (collidersSettings[k].horizontalRaySpacing * i);
                        //Crea il ray
                        Ray ray = new Ray(rayOrigin, -transform.right);
                        RaycastHit hit;

                        //Eseguo il raycast
                        if (Physics.Raycast(ray, out hit, rayLenght, alwaysFollowLayer))
                        {
                            if (!movedPassengers.Contains(hit.transform))
                            {
                                movedPassengers.Add(hit.transform);

                                float pushX = _velocity.y;
                                float pushY = 0;

                                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player") || hit.transform.gameObject.layer == LayerMask.NameToLayer("PlayerImmunity"))
                                {
                                    PlayerCollisionController collisionCtrl = hit.transform.GetComponentInParent<PlayerCollisionController>();
                                    collisionCtrl.transform.Translate(new Vector3(pushX, pushY, 0));
                                }
                                else
                                {
                                    hit.transform.Translate(new Vector3(pushX, pushY, 0));
                                }
                            }
                        }

                        //Determina il punto da cui deve partire il ray
                        Vector3 oppositeRayOrigin = collidersSettings[k].raycastPoints.bottomRight.position;
                        oppositeRayOrigin += transform.up * (collidersSettings[k].horizontalRaySpacing * i);

                        //Crea il ray
                        Ray oppositeRay = new Ray(oppositeRayOrigin, transform.right);
                        RaycastHit oppositeHit;

                        //Eseguo il raycast
                        if (Physics.Raycast(oppositeRay, out oppositeHit, rayLenght, alwaysFollowLayer))
                        {
                            if (!movedPassengers.Contains(oppositeHit.transform))
                            {
                                movedPassengers.Add(oppositeHit.transform);

                                float pushX = _velocity.y;
                                float pushY = 0f;

                                if (oppositeHit.transform.gameObject.layer == LayerMask.NameToLayer("Player") || oppositeHit.transform.gameObject.layer == LayerMask.NameToLayer("PlayerImmunity"))
                                {
                                    PlayerCollisionController collisionCtrl = oppositeHit.transform.GetComponentInParent<PlayerCollisionController>();
                                    collisionCtrl.transform.Translate(new Vector3(pushX, pushY, 0));
                                }
                                else
                                {
                                    oppositeHit.transform.Translate(new Vector3(-pushX, pushY, 0));
                                }
                            }
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
                    Vector3 rayOrigin = (directionX == -1) ? collidersSettings[k].raycastPoints.bottomLeft.position : collidersSettings[k].raycastPoints.bottomRight.position;
                    rayOrigin += transform.up * (collidersSettings[k].horizontalRaySpacing * i);

                    //Crea il ray
                    Ray ray = new Ray(rayOrigin, transform.right * directionX);
                    RaycastHit hit;

                    //Eseguo il raycast
                    if (Physics.Raycast(ray, out hit, rayLenght, passengerLayer))
                    {
                        if (!movedPassengers.Contains(hit.transform))
                        {
                            movedPassengers.Add(hit.transform);

                            float pushX = _velocity.x - (hit.distance - collisionOffset) * directionX;
                            float pushY = 0;

                            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player") || hit.transform.gameObject.layer == LayerMask.NameToLayer("PlayerImmunity"))
                            {
                                PlayerCollisionController collisionCtrl = hit.transform.GetComponentInParent<PlayerCollisionController>();
                                collisionCtrl.transform.Translate(new Vector3(pushX, pushY, 0));
                            }
                            else
                            {
                                hit.transform.Translate(new Vector3(pushX, pushY, 0));
                            }
                        }
                    }
                }

                if (_velocity.y == 0)
                {
                    for (int i = 0; i < verticalRayCount; i++)
                    {
                        //Determina il punto da cui deve partire il ray
                        Vector3 rayOrigin = collidersSettings[k].raycastPoints.bottomLeft.position;
                        rayOrigin += transform.right * (collidersSettings[k].verticalRaySpacing * i);

                        //Crea il ray
                        Ray ray = new Ray(rayOrigin, -transform.up);
                        RaycastHit hit;

                        //Eseguo il raycast
                        if (Physics.Raycast(ray, out hit, rayLenght, alwaysFollowLayer))
                        {
                            if (!movedPassengers.Contains(hit.transform))
                            {
                                movedPassengers.Add(hit.transform);

                                float pushX = -_velocity.x;
                                float pushY = 0f;

                                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player") || hit.transform.gameObject.layer == LayerMask.NameToLayer("PlayerImmunity"))
                                {
                                    PlayerCollisionController collisionCtrl = hit.transform.GetComponentInParent<PlayerCollisionController>();
                                    collisionCtrl.transform.Translate(new Vector3(pushX, pushY, 0));
                                }
                                else
                                {
                                    hit.transform.Translate(new Vector3(pushX, pushY, 0));
                                }
                            }
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
                    Vector3 rayOrigin = collidersSettings[k].raycastPoints.topLeft.position + transform.right * (collidersSettings[k].verticalRaySpacing * i);

                    //Crea il ray
                    Ray ray = new Ray(rayOrigin, transform.up);
                    RaycastHit hit;

                    //Eseguo il raycast
                    if (Physics.Raycast(ray, out hit, rayLenght, passengerLayer))
                    {
                        if (!movedPassengers.Contains(hit.transform))
                        {
                            movedPassengers.Add(hit.transform);

                            float pushX = _velocity.x;
                            float pushY = _velocity.y;

                            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player") || hit.transform.gameObject.layer == LayerMask.NameToLayer("PlayerImmunity"))
                            {
                                PlayerCollisionController collisionCtrl = hit.transform.GetComponentInParent<PlayerCollisionController>();
                                collisionCtrl.transform.Translate(new Vector3(pushX, pushY, 0));
                            }
                            else
                            {
                                hit.transform.Translate(new Vector3(pushX, pushY, 0));
                            }
                        }
                    }
                }
            }
        }
    }

    public void MovePassengerRotating(Vector3 _velocity)
    {
        HashSet<Transform> movedPassengers = new HashSet<Transform>();

        for (int k = 0; k < collidersSettings.Count; k++)
        {

            //Vertical Moving platform
            if (_velocity.y != 0)
            {
                float rayLenght = Mathf.Abs(_velocity.y) + collisionOffset;

                for (int i = 0; i < verticalRayCount; i++)
                {
                    //Determina il punto da cui deve partire il ray
                    Vector3 rayOrigin = collidersSettings[k].raycastPoints.topLeft.position;
                    rayOrigin += transform.right * (collidersSettings[k].verticalRaySpacing * i);

                    //Crea il ray
                    Ray ray = new Ray(rayOrigin, transform.up);
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

                    //Determina il punto da cui deve partire il ray opposto
                    Vector3 rayOriginOpposite = collidersSettings[k].raycastPoints.bottomLeft.position;
                    rayOriginOpposite += transform.right * (collidersSettings[k].verticalRaySpacing * i);

                    //Crea il ray opposto
                    Ray rayOpposite = new Ray(rayOriginOpposite, -transform.up);
                    RaycastHit hitOpposite;

                    //Eseguo il raycast opposto
                    if (Physics.Raycast(rayOpposite, out hitOpposite, rayLenght, passengerLayer))
                    {
                        if (!movedPassengers.Contains(hitOpposite.transform))
                        {
                            movedPassengers.Add(hitOpposite.transform);

                            float pushX = _velocity.x;
                            float pushY = _velocity.y;

                            hitOpposite.transform.Translate(new Vector3(pushX, pushY, 0));
                        }
                    }

                    Debug.DrawRay(rayOrigin, transform.up * rayLenght, Color.red);
                    Debug.DrawRay(rayOriginOpposite, -transform.up * rayLenght, Color.blue);
                }
            }
        }
    }
    #endregion

    private class ColliderSettings
    {
        public Collider platformCollider;
        public RaycastStartPoints raycastPoints;

        /// <summary>
        /// Spazio tra un raycast orrizontale e l'altro
        /// </summary>
        public float horizontalRaySpacing;

        /// <summary>
        /// Spazio tra un raycast verticale e l'altro
        /// </summary>
        public float verticalRaySpacing;
    }

    /// <summary>
    /// Struttura che contiene le coordinate dei 4 punti principali da cui partono i ray
    /// che controllano le collisioni
    /// </summary>
    private struct RaycastStartPoints
    {
        public Transform topLeft;
        public Transform topRight;
        public Transform bottomLeft;
        public Transform bottomRight;
    }
}
