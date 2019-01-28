using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Animator))]
public class Walker : EnemyBase
{
    bool CanShot;

    public override void Init(EnemyManager _enemyMng)
    {
        base.Init(_enemyMng);
        CanShot = true;
    }

    private IEnumerator FiringRateCoroutine()
    {
        CanShot = false;
        yield return new WaitForSeconds(1 / enemyShotSettings.firingRate);
        CanShot = true;
    }

    #region API
    IEnumerator moveRoaming;
    public override void MoveRoaming(bool _enabled)
    {
        if (_enabled)
        {
            movementSpeed = roamingMovementSpeed;
            moveRoaming = MoveRoamingCoroutine();
            StartCoroutine(moveRoaming);
        }
        else
        {
            StopCoroutine(moveRoaming);
            transform.DOKill();
        }
    }
    private IEnumerator MoveRoamingCoroutine()
    {
        while (true)
        {
            Vector3 nextWaypoint = GetNextWaypointPosition();
            Vector3 targetPosition = new Vector3(nextWaypoint.x, transform.position.y, nextWaypoint.z);

            float rotationY;
            Vector3 direction = (targetPosition - transform.position).normalized;
            if (direction != transform.right)
            {
                rotationY = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                yield return transform.DORotateQuaternion(Quaternion.Euler(0.0f, rotationY, 0.0f), turnSpeed).SetEase(Ease.Linear).OnUpdate(() => movementCtrl.MovementCheck()).WaitForCompletion();
            }
            yield return transform.DOMoveX(nextWaypoint.x, movementSpeed)
                .SetSpeedBased()
                .SetEase(Ease.Linear)
                .OnUpdate(() => movementCtrl.MovementCheck())
                .OnPause(() => StartCoroutine(MoveRoamingStickyChecker(true)))
                .OnPlay(() => StartCoroutine(MoveRoamingStickyChecker(false)))
                .WaitForCompletion();
            yield return new WaitForFixedUpdate();
        }
    }
    private IEnumerator MoveRoamingStickyChecker(bool _pause)
    {        
        if (_pause)
        {
            while (collisionCtrl.collisions.StickyCollision())
            {
                yield return null;
            }
            transform.DOPlay();
        }
        else
        {
            while (!collisionCtrl.collisions.StickyCollision())
            {
                yield return null;
            }
            transform.DOPause();
        }
    }

    IEnumerator alertCoroutine;
    public override void AlertActions(bool _enable)
    {
        if (_enable)
        {
            movementSpeed = alertMovementSpeed;
            alertCoroutine = AlertActionCoroutine();
            StartCoroutine(alertCoroutine);
        }
        else
        {
            StopCoroutine(alertCoroutine);
            transform.DOKill();
        }

    }
    private IEnumerator AlertActionCoroutine()
    {
        Transform target = viewCtrl.FindPlayer();
        float rotationY;
        Vector3 targetPosition = new Vector3(target.position.x, transform.position.y, transform.position.z);
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != transform.right)
        {
            rotationY = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            yield return transform.DORotateQuaternion(Quaternion.Euler(0.0f, rotationY, 0.0f), 0.1f).SetEase(Ease.Linear).OnUpdate(() => movementCtrl.MovementCheck()).WaitForCompletion();
        }

        while (true)
        {
            target = viewCtrl.FindPlayer();
            Vector3 movementVector = Vector3.zero;

            if (target == null)
                yield return new WaitForFixedUpdate();

            IBullet bullet = PoolManager.instance.GetPooledObject(enemyShotSettings.bulletType, gameObject).GetComponent<IBullet>();
            if ((bullet as ParabolicBullet).CheckShotRange(target.position, shotPosition, enemyShotSettings.shotSpeed))
            {
                if (CanShot)
                {
                    bullet.Shot(enemyShotSettings.damage, enemyShotSettings.shotSpeed, 5f, shotPosition, target);
                    StartCoroutine(FiringRateCoroutine());
                }
                movementCtrl.MovementCheck(movementVector);
            }
            else
            {
                //Rotazione Nemico
                Vector3 targetRotation = new Vector3(target.position.x, transform.position.y, transform.position.z);
                direction = (targetRotation - transform.position).normalized;
                if (direction.x != 0)
                {
                    rotationY = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0.0f, rotationY, 0.0f);
                }

                //Movimento Nemico                
                movementVector.x = movementSpeed;
                movementCtrl.MovementCheck(movementVector);
            }
            yield return new WaitForFixedUpdate();
        }
    }
    #endregion
}
