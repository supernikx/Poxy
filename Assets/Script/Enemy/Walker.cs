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
    protected override Vector3 MoveRoamingUpdate(Vector3? movementVector = null)
    {
        Vector3 movementVelocity = movementCtrl.MovementCheck(movementVector);
        animCtrl.MovementAnimation(movementVelocity);
        return movementVelocity;
    }

    protected override IEnumerator AlertActionCoroutine()
    {
        Transform target = viewCtrl.FindPlayer();
        if (target == null)
            yield return new WaitForFixedUpdate();

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
            Vector3 movementVelocity = movementVector;

            if (target == null)
                yield return new WaitForFixedUpdate();

            IBullet bullet = PoolManager.instance.GetPooledObject(enemyShotSettings.bulletType, gameObject).GetComponent<IBullet>();
            if ((bullet as ParabolicBullet).CheckShotRange(target.position, shotPosition.position, enemyShotSettings.shotSpeed))
            {
                if (CanShot)
                {
                    animCtrl.ShotAnimation();
                    bullet.Shot(enemyShotSettings.damage, enemyShotSettings.shotSpeed, 5f, shotPosition.position, target);
                    StartCoroutine(FiringRateCoroutine());
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
                movementVector.x = movementSpeed;
                movementVelocity = movementCtrl.MovementCheck(movementVector);
            }

            animCtrl.MovementAnimation(movementVelocity);
            yield return new WaitForFixedUpdate();
        }
    }
    #endregion
}
