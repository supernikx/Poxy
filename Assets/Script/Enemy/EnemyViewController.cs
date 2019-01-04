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

    /*public bool FindPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, viewRadius, playerLayer);

        for (int i = 0; i < hits.Length; i++)
        {
            Transform target = hits[i].transform;
            Vector3 dirToTarget = (transform.position - target.position).normalized;
            int direction = enemy.GetDirection();

            if (Vector3.Angle(direction * transform.right, dirToTarget) < viewAngle / 2)
            {
                float distance = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, target.position, distance, obstaclesLayer))
                {
                    return true;
                }
            }
        }

        return false;
    }*/

    public Transform GetPlayerInRadius()
    {
        Collider[] hits3 = Physics.OverlapSphere(transform.position, viewRadius);
        foreach (var item in hits3)
        {
            Debug.Log(item);
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, viewRadius, playerLayer);

        if (hits.Length > 0)
            return hits[0].transform;
        return null;
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
