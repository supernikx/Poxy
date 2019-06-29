using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyViewController : MonoBehaviour
{

    [Header("View Cone Settings")]
    [SerializeField]
    private float viewRadius;
    [SerializeField]
    private float viewAngle;

    [Header("Target Layers")]
    [SerializeField]
    private LayerMask playerLayer;
    [SerializeField]
    private LayerMask obstaclesLayer;

    private IEnemy enemy;

    #region API
    public void Init()
    {
        enemy = GetComponent<IEnemy>();
    }

    /// <summary>
    /// Funzione che controlla se il player è nel range del nemico
    /// </summary>
    Collider[] hits = new Collider[1];
    public Transform FindPlayer(float _radius = 0)
    {
        float radiusToUse = viewRadius;
        if (_radius != 0)
            radiusToUse = _radius;

        if (Physics.OverlapSphereNonAlloc(transform.position, radiusToUse, hits, playerLayer) == 0)
            return null;
        return hits[0].transform;
    }

    /// <summary>
    /// Funzione che controlla se il player è nel mio ViewAngle senza ostacoli davanti
    /// </summary>
    /// <param name="_playerPosition"></param>
    /// <returns></returns>
    public bool CanSeePlayer(Vector3 _playerPosition)
    {
        Vector3 dirToPlayer = (_playerPosition - transform.position).normalized;
        float angleBetweenMeAndPlayer = Vector3.Angle(transform.right, dirToPlayer);
        if (angleBetweenMeAndPlayer < viewAngle / 2f)
            if (!Physics.Linecast(transform.position, _playerPosition, obstaclesLayer))
                return true;

        return false;
    }

    /// <summary>
    /// Funzione che controlla la distanza del player dalla posizione del nemico
    /// ritorna true se è inferiore al raggio di visione del nemico, altirmenti ritorna false
    /// </summary>
    /// <returns></returns>
    public bool CheckPlayerDistance(Vector3 _playerPosition)
    {
        if (Vector3.Distance(transform.position, _playerPosition) < viewRadius)
            return true;
        return false;
    }

    #region Getter
    /// <summary>
    /// Funzione che ritorna il raggio di visione del nemico
    /// </summary>
    /// <returns></returns>
    public float GetViewRadius()
    {
        return viewRadius;
    }

    /// <summary>
    /// Funzione che ritorna l'angolo di visione del nemico
    /// </summary>
    /// <returns></returns>
    public float GetViewAngle()
    {
        return viewAngle;
    }
    #endregion
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, viewRadius);
    }
}
