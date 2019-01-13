using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Animator))]
public class Gluer : EnemyBase
{
    bool CanShot;

    Transform player;

    [Header("Enemy Specifics Settings")]
    [SerializeField]
    private float roamingFiringRate;

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
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
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

        while (true)
        {
            Vector3 nextWaypoint = GetNextWaypointPosition();
            Vector3 targetPosition = new Vector3(nextWaypoint.x, transform.position.y, nextWaypoint.z);

            float rotationY;
            Vector3 direction = (targetPosition - transform.position).normalized;
            if (direction != transform.right)
            {
                rotationY = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                yield return transform.DORotateQuaternion(Quaternion.Euler(0.0f, rotationY, 0.0f), turnSpeed).SetEase(Ease.Linear).OnUpdate(() => MoveRoamingUpdate()).WaitForCompletion();
            }

            yield return transform.DOMoveX(nextWaypoint.x, movementSpeed).SetSpeedBased().SetEase(Ease.Linear).OnUpdate(() => MoveRoamingUpdate()).WaitForCompletion();
            yield return new WaitForFixedUpdate();
        }
    }

    private void MoveRoamingUpdate()
    {
        // Vertical movement
        movementCtrl.MovementCheck();
        
        IBullet bullet = PoolManager.instance.GetPooledObject(enemyShotSettings.bulletType, gameObject).GetComponent<IBullet>();
        if (CanShot)
        {
            Debug.Log("Shot");
            bullet.Shot(enemyShotSettings.damage, enemyShotSettings.shotSpeed, 5f, shotPosition, player.position);
            StartCoroutine(FiringRateCoroutine(roamingFiringRate));
        }
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
            target = viewCtrl.FindPlayer();
            if (target == null)
                yield return null;
            
            IBullet bullet = PoolManager.instance.GetPooledObject(enemyShotSettings.bulletType, gameObject).GetComponent<IBullet>();
            if (CanShot)
            {
                Vector3 _direction = (target.position - shotPosition.position);
                bullet.Shot(enemyShotSettings.damage, enemyShotSettings.shotSpeed, 5f, shotPosition, _direction);
                StartCoroutine(FiringRateCoroutine(enemyShotSettings.firingRate));
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
                Vector3 movementVector = new Vector3();
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
