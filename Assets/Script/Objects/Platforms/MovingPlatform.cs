using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class MovingPlatform : Platform
{
    [Header("Movement Options")]
    [SerializeField]
    private float movingSpd;
    [SerializeField]
    private Transform reachPoint;
    Vector3 reachPosition;
    private Vector3 startPosition;
    private Vector3 direction;

    private PlatformCollisionController collisionCtrl;

    #region API
    public override void Init()
    {
        collisionCtrl = GetComponent<PlatformCollisionController>();
        if (collisionCtrl != null)
            collisionCtrl.Init();

        startPosition = transform.position;
        reachPosition = reachPoint.position;
        direction = (reachPosition - startPosition).normalized;
        pointReached = false;
        StartCoroutine(Move());
    }
    #endregion
    bool pointReached;
    private IEnumerator Move()
    {
        Vector3 moveVelocity;
        while (true)
        {
            if (!pointReached && Vector3.Distance(transform.position, reachPosition) > 0.1f)
            {
                moveVelocity = direction * movingSpd * Time.deltaTime;
                collisionCtrl.MovePassenger(moveVelocity);
                transform.Translate(moveVelocity);
            }
            else
            {
                if (!pointReached)
                    pointReached = true;

                moveVelocity = -direction * movingSpd * Time.deltaTime;
                collisionCtrl.MovePassenger(moveVelocity);
                transform.Translate(moveVelocity);

                if (Vector3.Distance(transform.position, startPosition) < 0.1f)
                {
                    pointReached = false;
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }
}