using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Animator))]
public class Gluer : EnemyBase
{
    bool CanShot;

    [Header("Enemy Specifics Settings")]
    [SerializeField]
    private float roamingFiringRate;
    [SerializeField]
    private float shotRadius;

    private IEnumerator FiringRateCoroutine(float _firingRate)
    {
        CanShot = false;
        yield return new WaitForSeconds(1 / _firingRate);
        CanShot = true;
    }

    #region API
    public override void Init(EnemyManager _enemyMng)
    {
        base.Init(_enemyMng);
        CanShot = true;
    }

    #region Roaming Behaviour
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
        Vector3 movementVector = Vector3.zero;
        float pathLenght = GetPathLenght();
        float pathTraveled = 0f;
        bool movementBlocked = false;

        while (true)
        {
            if (pathTraveled >= pathLenght - 0.1f)
            {
                pathTraveled = 0f;

                Vector3 rotationVector = Vector3.zero;
                if (transform.rotation.y == 0)
                    rotationVector.y = 180f;
                yield return transform.DORotateQuaternion(Quaternion.Euler(rotationVector), turnSpeed)
                    .SetEase(Ease.Linear)
                    .OnUpdate(() => MoveRoamingUpdate())
                    .OnPause(() => StartCoroutine(MoveRoamingStickyChecker(true)))
                    .OnPlay(() => StartCoroutine(MoveRoamingStickyChecker(false)))
                    .WaitForCompletion();
            }

            //Movimento Nemico                
            movementVector.x = movementSpeed;
            Vector3 distanceTraveled = MoveRoamingUpdate(movementVector);

            if ((distanceTraveled - Vector3.zero).sqrMagnitude < 0.001f && movementBlocked == false)
                movementBlocked = true;
            else if ((distanceTraveled - Vector3.zero).sqrMagnitude < 0.001f && movementBlocked == true)
            {
                pathTraveled = pathLenght;
            }
            else if ((distanceTraveled - Vector3.zero).sqrMagnitude > 0.001f && movementBlocked == true)
                movementBlocked = false;

            pathTraveled += distanceTraveled.x;
            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator MoveRoamingStickyChecker(bool _pause)
    {
        if (_pause)
        {
            while (collisionCtrl.GetCollisionInfo().StickyCollision())
            {
                yield return null;
            }
            transform.DOPlay();
        }
        else
        {
            while (!collisionCtrl.GetCollisionInfo().StickyCollision())
            {
                yield return null;
            }
            transform.DOPause();
        }
    }

    private bool isRotating = false;
    private Vector3 MoveRoamingUpdate(Vector3? movementVector = null)
    {
        // Vertical movement
        Vector3 movementTranslation = movementCtrl.MovementCheck(movementVector);

        IBullet bullet = PoolManager.instance.GetPooledObject(enemyShotSettings.bulletType, gameObject).GetComponent<IBullet>();
        if (CanShot && !isRotating)
        {
            bullet.Shot(enemyShotSettings.damage, enemyShotSettings.shotSpeed, 5f, shotPosition, transform.right);
            StartCoroutine(FiringRateCoroutine(roamingFiringRate));
        }

        return movementTranslation;
    }
    #endregion

    #region Alert Behaviour
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
            Vector3 movementVector = Vector3.zero;

            target = viewCtrl.FindPlayer();
            if (target == null)
                yield return new WaitForFixedUpdate();

            IBullet bullet = PoolManager.instance.GetPooledObject(enemyShotSettings.bulletType, gameObject).GetComponent<IBullet>();
            float _distance = Vector3.Distance(targetPosition, shotPosition.position);
            if (_distance <= shotRadius)
            {
                if (CanShot)
                {
                    Vector3 _direction = (target.position - shotPosition.position);
                    bullet.Shot(enemyShotSettings.damage, enemyShotSettings.shotSpeed, shotRadius, shotPosition, _direction);
                    StartCoroutine(FiringRateCoroutine(enemyShotSettings.firingRate));
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
                movementVector.z = 0;
                movementVector.y = 0;
                movementVector.x = movementSpeed;
                movementCtrl.MovementCheck(movementVector);
            }

            yield return new WaitForFixedUpdate();
        }
    }
    #endregion
    #endregion
}
