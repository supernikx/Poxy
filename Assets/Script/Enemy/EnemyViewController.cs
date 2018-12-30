using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyViewController : MonoBehaviour {

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

    public bool FindPlayer()
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

                if (!Physics.Raycast(transform.position,target.position, distance, obstaclesLayer))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public Vector3 GetPlayerPosition()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, viewRadius, playerLayer);

        for (int i = 0; i < hits.Length; i++)
        {
            Transform target = hits[i].transform;
            float distance = Vector3.Distance(transform.position, target.position);

            if (!Physics.Raycast(transform.position, target.position, distance, obstaclesLayer))
            {
                return target.position;
            }
        }
        
        return new Vector3(0,0,0);
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, viewRadius);
    }

}
