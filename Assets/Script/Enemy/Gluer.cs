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

    private bool isRotating = false;
    protected override Vector3 MoveRoamingUpdate(Vector3? movementVector = null)
    {
        // Vertical movement
        Vector3 movementTranslation = movementCtrl.MovementCheck(movementVector);
        animCtrl.MovementAnimation(movementTranslation);
        IBullet bullet = PoolManager.instance.GetPooledObject(enemyShotSettings.bulletType, gameObject).GetComponent<IBullet>();
        if (CanShot && !isRotating)
        {
            bullet.Shot(enemyShotSettings.damage, enemyShotSettings.shotSpeed, 5f, shotPosition.position, transform.right);
            animCtrl.ShotAnimation();
            StartCoroutine(FiringRateCoroutine(roamingFiringRate));
        }

        return movementTranslation;
    }

    protected override IEnumerator AlertActionCoroutine()
    {
        Transform target = viewCtrl.FindPlayer();
        if (target == null)
            yield break; 

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
            Vector3 movementVelocity = movementVector;
            target = viewCtrl.FindPlayer();
            if (target == null)
                yield break;

            IBullet bullet = PoolManager.instance.GetPooledObject(enemyShotSettings.bulletType, gameObject).GetComponent<IBullet>();
            float _distance = Vector3.Distance(targetPosition, shotPosition.position);
            if (_distance <= shotRadius)
            {
                if (CanShot)
                {
                    Vector3 _direction = (target.position - shotPosition.position);
                    bullet.Shot(enemyShotSettings.damage, enemyShotSettings.shotSpeed, shotRadius, shotPosition.position, _direction);
                    animCtrl.ShotAnimation();
                    StartCoroutine(FiringRateCoroutine(enemyShotSettings.firingRate));
                }
                movementVelocity = movementCtrl.MovementCheck(movementVector);
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
                movementVelocity = movementCtrl.MovementCheck(movementVector);
            }
            animCtrl.MovementAnimation(movementVelocity);
            yield return new WaitForFixedUpdate();
        }
    }
    #endregion
}
